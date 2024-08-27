using Dispo.Shared.Core.Domain.Entities;
using Dispo.Shared.Core.Domain.Enums;
using Dispo.Shared.Filter.Model;
using Dispo.Shared.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text.Json;

namespace Dispo.Shared.Filter.Services
{
    public class FilterService : IFilterService
    {
        private const string UnsupportedSearchTypeMessage = "Unsupported search type for property of type:";
        private readonly DispoContext _dispoContext;

        public FilterService(DispoContext dispoContext)
        {
            _dispoContext = dispoContext;
        }

        public object Get<T>(FilterModel filterModel) where T : EntityBase
        {
            if (filterModel is null)
            {
                throw new ArgumentNullException(nameof(filterModel));
            }

            var buildExpression = BuildExpression<T>(filterModel);

            var recordCount = _dispoContext.Set<T>()
                                .AsNoTracking()
                                .Where(buildExpression)
                                .Count();

            var records = _dispoContext.Set<T>()
                                .AsNoTracking()
                                .Where(buildExpression)
                                .Skip((filterModel.PaginationConfig.PageNumber - 1) * filterModel.PaginationConfig.PageSize)
                                .Take(filterModel.PaginationConfig.PageSize)
                                .ToList();

            var obj = new
            {
                RecordCount = recordCount,
                Records = records
            };

            return obj;
        }

        private Func<T, bool> BuildExpression<T>(FilterModel filterModel)
        {
            var parameter = Expression.Parameter(typeof(T), filterModel.Entity);
            Expression filterExpression = null;
            foreach (var property in filterModel.Properties)
            {
                JsonElement jsonElementValue = property.Value;

                var valueType = jsonElementValue.ValueKind;
                Expression comparison;
                var memberExpression = Expression.Property(parameter, property.Name);

                if (valueType == JsonValueKind.String) // String
                {
                    var valor = jsonElementValue.GetString();
                    var constant = Expression.Constant(valor);

                    comparison = Expression.Call(memberExpression, property.SearchType.ToString(), Type.EmptyTypes, constant);
                }
                else if (IsNumericType(memberExpression.Type, property.Value, out ConstantExpression convertedConstant) && valueType == JsonValueKind.Number) // Numerico
                {
                    comparison = GetGenericComparisonExpression(memberExpression, property, convertedConstant);
                }
                else if (valueType == JsonValueKind.Object) // Enum
                {
                    var valor = jsonElementValue.GetProperty("value").GetInt64();
                    var convertedValue = Enum.Parse(memberExpression.Type, valor.ToString());
                    var constant = Expression.Constant(convertedValue);
                    comparison = Expression.Equal(memberExpression, constant);

                }
                else if (IsDateTimeType(memberExpression.Type, property.Value, out DateTime convertedDateTime))
                {
                    comparison = GetGenericComparisonExpression(memberExpression, property, Expression.Constant(convertedDateTime));
                }
                else
                {
                    throw new NotSupportedException($"{UnsupportedSearchTypeMessage} {property.SearchType}");
                }

                filterExpression = filterExpression == null ? comparison : Expression.And(filterExpression, comparison);
            }

            return Expression.Lambda<Func<T, bool>>(filterExpression ?? Expression.Constant(true), parameter).Compile();
        }

        private Expression GetStringComparisonExpression(MemberExpression memberExpression, PropertyModel property)
        {
            string convertedValue = Convert.ToString(property.Value);
            var constant = Expression.Constant(convertedValue);

            // Ao enviar o SearchType.Equals está disparando uma exceção.
            return Expression.Call(memberExpression, property.SearchType.ToString(), Type.EmptyTypes, constant);
        }

        private Expression GetGenericComparisonExpression(MemberExpression memberExpression, PropertyModel property, ConstantExpression convertedConstant)
        {
            switch (property.SearchType)
            {
                case SearchType.Equals:
                    return Expression.Equal(memberExpression, convertedConstant);
                case SearchType.GreaterThan:
                    return Expression.GreaterThan(memberExpression, convertedConstant);
                case SearchType.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(memberExpression, convertedConstant);
                case SearchType.LessThan:
                    return Expression.LessThan(memberExpression, convertedConstant);
                case SearchType.LessThanOrEqual:
                    return Expression.LessThanOrEqual(memberExpression, convertedConstant);
                default:
                    throw new NotSupportedException($"{UnsupportedSearchTypeMessage} {property.SearchType}");
            }
        }

        private bool IsNumericType(Type type, object value, out ConstantExpression convertedConstant)
        {
            convertedConstant = null;
            if (NumericTypes.Contains(type))
            {
                try
                {
                    var convertedValue = Convert.ChangeType(Convert.ToString(value), type);
                    convertedConstant = Expression.Constant(convertedValue);
                    return true;
                }
                catch (InvalidCastException)
                {
                    return false;
                }
            }

            return false;
        }

        private bool IsDateTimeType(Type type, object value, out DateTime convertedDateTime)
        {
            convertedDateTime = DateTime.MinValue;
            return type == typeof(DateTime) && DateTime.TryParse(Convert.ToString(value), out convertedDateTime);
        }

        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int), typeof(double), typeof(float), typeof(decimal),
            typeof(long), typeof(short), typeof(byte), typeof(uint),
            typeof(ulong), typeof(ushort), typeof(sbyte)
        };
    }
}

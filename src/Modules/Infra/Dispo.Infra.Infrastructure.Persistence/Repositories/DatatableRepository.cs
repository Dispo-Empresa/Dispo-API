using Dispo.Shared.Core.Domain.DTOs;
using Dispo.Shared.Core.Domain.Entities;
using Dispo.Shared.Core.Domain.Interfaces;
using Dispo.Shared.Infrastructure.Persistence;
using Dispo.Shared.Infrastructure.Persistence.Context;
using Dispo.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Infra.Infrastructure.Persistence.Repositories
{
    public class DatatableRepository : BaseRepository<Manufacturer>, IDatatableRepository
    {
        private readonly DispoContext _dispoContext;

        public DatatableRepository(DispoContext dispoContext) : base(dispoContext)
        {
            _dispoContext = dispoContext;
        }

        public int GetTotalRecords<T>() where T : EntityBase
            => _dispoContext.Set<T>()
                            .AsNoTracking()
                            .Count();

        //public IEnumerable<ManufacturerInfoDto> GetToDatatable(int pageNumber, int pageSize)
        //    => _dispoContext.Manufacturers.Skip((pageNumber - 1) * pageSize)
        //                                  .Take(pageSize)
        //                                  .Select(s => new ManufacturerInfoDto()
        //                                  {
        //                                      Id = s.Id,
        //                                      Name = s.Name,
        //                                  })
        //                                  .ToList();

        public IEnumerable<ProductDatatableDto> GetToDatatableProduct(int pageNumber, int pageSize)
            => _dispoContext.Set<Product>().Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .Select(product => new ProductDatatableDto
                                           {
                                               Id = product.Id,
                                               Name = product.Name,
                                               PurchasePrice = product.PurchasePrice.ConvertToCurrency(),
                                               SalePrice = product.SalePrice.ConvertToCurrency(),
                                               Category = EnumExtension.ConvertToString(product.Category),
                                               UnitOfMeasurement = EnumExtension.ConvertToString(product.UnitOfMeasurement),
                                           })
                                           .ToList();

        public IEnumerable<ManufacturerDatatableDto> GetToDatatableManufacturer(int pageNumber, int pageSize)
            => _dispoContext.Set<Manufacturer>().Skip((pageNumber - 1) * pageSize)
                                                .Take(pageSize)
                                                .Select(manufacturer => new ManufacturerDatatableDto
                                                {
                                                    Id = manufacturer.Id,
                                                    Name = manufacturer.Name,
                                                })
                                                .ToList();

        public IEnumerable<SupplierDatatableDto> GetToDatatableSupplier(int pageNumber, int pageSize)
            => _dispoContext.Set<Supplier>().Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .Select(supplier => new SupplierDatatableDto
                                           {
                                               Id = supplier.Id,
                                               Name = supplier.Name,
                                               ContactName = supplier.ContactName,
                                               Cnpj = supplier.Cnpj,
                                               Email = supplier.Email,
                                               Phone = supplier.Phone,
                                           })
                                           .ToList();



        public IEnumerable<EntityDatatableDto> GetToDatatable<TEntity>(int pageNumber, int pageSize) where TEntity : EntityBase
            => _dispoContext.Set<TEntity>().Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .Select(ConvertToDatabaseDto<TEntity>)
                                           .ToList();

        private EntityDatatableDto ConvertToDatabaseDto<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            if (typeof(TEntity) == typeof(Product))
            {
                var product = entity as Product;
                return new ProductDatatableDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    PurchasePrice = product.PurchasePrice.ConvertToCurrency(),
                    SalePrice = product.SalePrice.ConvertToCurrency(),
                    Category = EnumExtension.ConvertToString(product.Category),
                    UnitOfMeasurement = EnumExtension.ConvertToString(product.UnitOfMeasurement),
                };
            }
            else if (typeof(TEntity) == typeof(Manufacturer))
            {
                var manufacturer = entity as Manufacturer;
                return new ManufacturerDatatableDto
                {
                    Id = manufacturer.Id,
                    Name = manufacturer.Name,
                };
            }
            else if (typeof(TEntity) == typeof(Supplier))
            {
                var supplier = entity as Supplier;
                return new SupplierDatatableDto
                {
                    Id = supplier.Id,
                    Name = supplier.Name,
                    ContactName = supplier.ContactName,
                    Cnpj = supplier.Cnpj,
                    Email = supplier.Email,
                    Phone = supplier.Phone,
                };
            }

            throw new NotImplementedException($"Entidade não encontrada {typeof(TEntity)}");
        }
    }
}

using Dispo.Shared.Core.Domain.Entities;
using Dispo.Shared.Filter.Model;

namespace Dispo.Shared.Filter.Services
{
    public interface IFilterService
    {
        object Get<T>(FilterModel filterModel) where T : EntityBase;
    }
}

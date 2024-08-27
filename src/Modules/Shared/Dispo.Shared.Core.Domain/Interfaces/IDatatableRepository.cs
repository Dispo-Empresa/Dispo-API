using Dispo.Shared.Core.Domain.DTOs;
using Dispo.Shared.Core.Domain.Entities;

namespace Dispo.Shared.Core.Domain.Interfaces
{
    public interface IDatatableRepository
    {
        int GetTotalRecords<T>() where T : EntityBase;
        IEnumerable<EntityDatatableDto> GetToDatatable<TEntity>(int pageNumber, int pageSize) where TEntity : EntityBase;
        IEnumerable<ProductDatatableDto> GetToDatatableProduct(int pageNumber, int pageSize);
        IEnumerable<ManufacturerDatatableDto> GetToDatatableManufacturer(int pageNumber, int pageSize);
        IEnumerable<SupplierDatatableDto> GetToDatatableSupplier(int pageNumber, int pageSize);
    }
}

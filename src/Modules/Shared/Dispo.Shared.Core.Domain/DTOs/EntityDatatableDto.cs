namespace Dispo.Shared.Core.Domain.DTOs
{
    public class EntityDatatableDto
    {
        public long Id { get; set; }
    }

    public class ProductDatatableDto : EntityDatatableDto
    {
        public string Name { get; set; }
        public string PurchasePrice { get; set; }
        public string SalePrice { get; set; }
        public string UnitOfMeasurement { get; set; }
        public string Category { get; set; }
    }

    public class ManufacturerDatatableDto : EntityDatatableDto
    {
        public string Name { get; set; }
    }

    public class SupplierDatatableDto : EntityDatatableDto
    {
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string Cnpj { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}

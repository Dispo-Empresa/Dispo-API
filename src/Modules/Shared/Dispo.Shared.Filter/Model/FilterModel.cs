namespace Dispo.Shared.Filter.Model
{
    public class FilterModel
    {
        public required string Entity { get; set; }
        public required List<PropertyModel> Properties { get; set; }
        public required PaginationFilter PaginationConfig { get; set; }
    }

    public class PaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

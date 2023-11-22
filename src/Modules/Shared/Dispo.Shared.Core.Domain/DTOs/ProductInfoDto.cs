﻿namespace Dispo.Shared.Core.Domain.DTOs
{
    public class ProductInfoDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PurchasePrice { get; set; }
        public string SalePrice { get; set; }
        public string UnitOfMeasurement { get; set; }
        public string Category { get; set; }
    }
}
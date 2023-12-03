﻿using Dispo.Shared.Core.Domain.Enums;

namespace Dispo.Shared.Core.Domain.Entities
{
    public class Product : EntityBase
    {
        public bool Ativo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[]? Image { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public eProductCategory Category { get; set; }
        public eUnitOfMeasurement UnitOfMeasurement { get; set; }
        public long? ProductDimensionId { get; set; }

        public ProductDimension ProductDimension { get; set; }
        public IList<ProductManufacturer> ProductManufacturers { get; set; }
        public IList<Order> Orders { get; set; }
    }
}
﻿using Dispo.Shared.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dispo.Shared.Infrastructure.Persistence.Mappings
{
    public class MovementMapping : IEntityTypeConfiguration<Movement>
    {
        public void Configure(EntityTypeBuilder<Movement> builder)
        {
            builder.ToTable("Movements");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .UseIdentityColumn()
                   .HasColumnType("BIGINT")
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.Date)
                   .IsRequired()
                   .HasColumnName("Date")
                   .HasColumnType("datetime2");

            builder.Property(x => x.Type)
                   .IsRequired()
                   .HasColumnName("Type")
                   .HasColumnType("SMALLINT")
                   .HasMaxLength(120);

            builder.Property(x => x.Quantity)
                   .IsRequired()
                   .HasColumnName("Quantity")
                   .HasColumnType("INTEGER")
                   .HasMaxLength(9999);

            builder.HasOne(a => a.Warehouse)
                   .WithMany(b => b.Movements)
                   .HasForeignKey(c => c.WarehouseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Account)
                   .WithMany(b => b.Movements)
                   .HasForeignKey(c => c.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShoppingBackend.Models;

public partial class ShoplistDbContext : DbContext
{
    public ShoplistDbContext()
    {
    }

    public ShoplistDbContext(DbContextOptions<ShoplistDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Shoplist> Shoplist { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Finnish_Swedish_CI_AI");

        modelBuilder.Entity<Shoplist>(entity =>
        {
            entity.ToTable("Shoplist");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Item)
                .HasMaxLength(50)
                .HasColumnName("item");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

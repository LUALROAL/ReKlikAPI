using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ReKlik.MODEL.Entities;

namespace ReKlik.DAL.DBContext;

public partial class ReKlikDbContext : DbContext
{
    public ReKlikDbContext()
    {
    }

    public ReKlikDbContext(DbContextOptions<ReKlikDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCode> ProductCodes { get; set; }

    public virtual DbSet<Reward> Rewards { get; set; }

    public virtual DbSet<ScanLog> ScanLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwProductTraceability> VwProductTraceabilities { get; set; }

    public virtual DbSet<VwRecyclingStatsByMaterial> VwRecyclingStatsByMaterials { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__companie__3213E83F6B9DC9C1");

            entity.ToTable("companies");

            entity.HasIndex(e => e.Email, "UQ__companie__AB6E6164CA45576C").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.ContactPerson)
                .HasMaxLength(100)
                .HasColumnName("contact_person");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__products__3213E83F519B810D");

            entity.ToTable("products");

            entity.HasIndex(e => e.CompanyId, "idx_products_company");

            entity.HasIndex(e => e.MaterialType, "idx_products_material");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Brand)
                .HasMaxLength(100)
                .HasColumnName("brand");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.MaterialType)
                .HasMaxLength(20)
                .HasColumnName("material_type");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Recyclable)
                .HasDefaultValue(true)
                .HasColumnName("recyclable");
            entity.Property(e => e.RecyclingInstructions).HasColumnName("recycling_instructions");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");
            entity.Property(e => e.Weight)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("weight");

            entity.HasOne(d => d.Company).WithMany(p => p.Products)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK__products__compan__59063A47");
        });

        modelBuilder.Entity<ProductCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__product___3213E83F1F25C236");

            entity.ToTable("product_codes");

            entity.HasIndex(e => e.UuidCode, "UQ__product___75BAE2C33F1FA0B2").IsUnique();

            entity.HasIndex(e => e.UuidCode, "idx_product_codes_uuid");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BatchNumber)
                .HasMaxLength(50)
                .HasColumnName("batch_number");
            entity.Property(e => e.GeneratedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("generated_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UuidCode)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("uuid_code");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductCodes)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__product_c__produ__5FB337D6");
        });

        modelBuilder.Entity<Reward>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__rewards__3213E83F903EE684");

            entity.ToTable("rewards");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AwardedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("awarded_at");
            entity.Property(e => e.PointsEarned).HasColumnName("points_earned");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .HasColumnName("reason");
            entity.Property(e => e.ScanLogId).HasColumnName("scan_log_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ScanLog).WithMany(p => p.Rewards)
                .HasForeignKey(d => d.ScanLogId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__rewards__scan_lo__6B24EA82");

            entity.HasOne(d => d.User).WithMany(p => p.Rewards)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__rewards__user_id__6A30C649");
        });

        modelBuilder.Entity<ScanLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__scan_log__3213E83F28FBF7D5");

            entity.ToTable("scan_logs");

            entity.HasIndex(e => e.ProductCodeId, "idx_scan_logs_product");

            entity.HasIndex(e => e.ScannedAt, "idx_scan_logs_time");

            entity.HasIndex(e => e.UserId, "idx_scan_logs_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProductCodeId).HasColumnName("product_code_id");
            entity.Property(e => e.ScanCity)
                .HasMaxLength(100)
                .HasColumnName("scan_city");
            entity.Property(e => e.ScanCountry)
                .HasMaxLength(100)
                .HasColumnName("scan_country");
            entity.Property(e => e.ScanType)
                .HasMaxLength(20)
                .HasColumnName("scan_type");
            entity.Property(e => e.ScannedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("scanned_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.ProductCode).WithMany(p => p.ScanLogs)
                .HasForeignKey(d => d.ProductCodeId)
                .HasConstraintName("FK__scan_logs__produ__6477ECF3");

            entity.HasOne(d => d.User).WithMany(p => p.ScanLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__scan_logs__user___656C112C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F35A0207F");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164D885489C").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserType)
                .HasMaxLength(20)
                .HasColumnName("user_type");
        });

        modelBuilder.Entity<VwProductTraceability>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ProductTraceability");

            entity.Property(e => e.Brand)
                .HasMaxLength(100)
                .HasColumnName("brand");
            entity.Property(e => e.Company)
                .HasMaxLength(100)
                .HasColumnName("company");
            entity.Property(e => e.FirstScan).HasColumnName("first_scan");
            entity.Property(e => e.LastScan).HasColumnName("last_scan");
            entity.Property(e => e.MaterialType)
                .HasMaxLength(20)
                .HasColumnName("material_type");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasColumnName("product_name");
            entity.Property(e => e.QrCode).HasColumnName("qr_code");
            entity.Property(e => e.ScanCount).HasColumnName("scan_count");
        });

        modelBuilder.Entity<VwRecyclingStatsByMaterial>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_RecyclingStatsByMaterial");

            entity.Property(e => e.MaterialType)
                .HasMaxLength(20)
                .HasColumnName("material_type");
            entity.Property(e => e.ProductsRecycled).HasColumnName("products_recycled");
            entity.Property(e => e.ProductsRegistered).HasColumnName("products_registered");
            entity.Property(e => e.TotalScans).HasColumnName("total_scans");
            entity.Property(e => e.UniqueUsers).HasColumnName("unique_users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using Microsoft.EntityFrameworkCore;

namespace AssetMgmt_WebApi.Models
{
    public partial class AssetMgmtContext : DbContext
    {
        public AssetMgmtContext()
        {
        }

        public AssetMgmtContext(DbContextOptions<AssetMgmtContext> options)
            : base(options)
        {
        }

        // Module A
        public virtual DbSet<Login> Logins { get; set; } = null!;
        public virtual DbSet<UserRegistration> UserRegistrations { get; set; } = null!;

        // (predefined)
        public virtual DbSet<AssetType> AssetTypes { get; set; } = null!;
        public virtual DbSet<AssetDefinition> AssetDefinitions { get; set; } = null!;

        //  (predefined)
        public virtual DbSet<Vendor> Vendors { get; set; } = null!;

        // (predefined)
        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;

        // Module E
        public virtual DbSet<AssetMaster> AssetMasters { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Login>(entity =>
            {
                entity.HasKey(e => e.LId).HasName("PK_Login");
                entity.ToTable("Login");

                entity.HasIndex(e => e.Username).IsUnique();

                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Password).HasMaxLength(200).IsRequired();
                entity.Property(e => e.UserType).HasMaxLength(30).IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            });

            modelBuilder.Entity<UserRegistration>(entity =>
            {
                entity.HasKey(e => e.UId).HasName("PK_UserRegistration");
                entity.ToTable("UserRegistration");

                entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Gender).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Address).HasMaxLength(200).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(15).IsRequired();

                entity.HasOne(d => d.Login).WithOne(p => p.UserRegistration)
                    .HasForeignKey<UserRegistration>(d => d.LId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_UserRegistration_Login");
            });

            modelBuilder.Entity<AssetType>(entity =>
            {
                entity.HasKey(e => e.AtId).HasName("PK_AssetType");
                entity.ToTable("AssetType");

                entity.Property(e => e.AtName).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<AssetDefinition>(entity =>
            {
                entity.HasKey(e => e.AdId).HasName("PK_AssetDefinition");
                entity.ToTable("AssetDefinition");

                entity.Property(e => e.AdName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.AdClass).HasMaxLength(5).IsRequired();

                entity.HasOne(d => d.AssetType).WithMany(p => p.AssetDefinitions)
                    .HasForeignKey(d => d.AdTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssetDefinition_AssetType");
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.HasKey(e => e.VdId).HasName("PK_Vendor");
                entity.ToTable("Vendor");

                entity.Property(e => e.VdName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.VdType).HasMaxLength(40).IsRequired();
                entity.Property(e => e.VdAddr).HasMaxLength(200).IsRequired();

                entity.HasOne(d => d.AssetType).WithMany()
                    .HasForeignKey(d => d.VdAtypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Vendor_AssetType");
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.HasKey(e => e.PdId).HasName("PK_PurchaseOrder");
                entity.ToTable("PurchaseOrder");

                entity.Property(e => e.PdOrderNo).HasMaxLength(10).IsRequired();
                entity.Property(e => e.PdStatus).HasMaxLength(60).IsRequired();

                entity.HasOne(d => d.AssetDefinition).WithMany()
                    .HasForeignKey(d => d.PdAdId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PurchaseOrder_AssetDefinition");

                entity.HasOne(d => d.AssetType).WithMany(p => p.PurchaseOrders)
                    .HasForeignKey(d => d.PdTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PurchaseOrder_AssetType");

                entity.HasOne(d => d.Vendor).WithMany()
                    .HasForeignKey(d => d.PdVendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PurchaseOrder_Vendor");
            });

            modelBuilder.Entity<AssetMaster>(entity =>
            {
                entity.HasKey(e => e.AmId).HasName("PK_AssetMaster");
                entity.ToTable("AssetMaster");

                entity.Property(e => e.AmModel).HasMaxLength(40).IsRequired();
                entity.Property(e => e.AmSnumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.AmMyyear).HasMaxLength(10).IsRequired();
                entity.Property(e => e.AmWarranty).HasMaxLength(1).IsRequired();

                entity.HasIndex(e => e.AmSnumber).IsUnique();

                entity.HasOne(d => d.AssetType).WithMany(p => p.AssetMasters)
                    .HasForeignKey(d => d.AmAtypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssetMaster_AssetType");

                entity.HasOne(d => d.Vendor).WithMany()
                    .HasForeignKey(d => d.AmMakeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssetMaster_Vendor");

                entity.HasOne(d => d.AssetDefinition).WithMany()
                    .HasForeignKey(d => d.AmAdId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssetMaster_AssetDefinition");

                entity.HasOne(d => d.PurchaseOrder).WithMany()
                    .HasForeignKey(d => d.AmPurchaseOrderId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_AssetMaster_PurchaseOrder");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

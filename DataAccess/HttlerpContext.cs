using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace HTTL_ERP.DataAccess;

public partial class HttlerpContext : DbContext
{
    public HttlerpContext()
    {
    }

    public HttlerpContext(DbContextOptions<HttlerpContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agency> Agencies { get; set; }

    public virtual DbSet<BrickFactory> BrickFactories { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Logistic> Logistics { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderdetail> Orderdetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=httlerp;uid=root;pwd=bach", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.31-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Agency>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("agency");

            entity.HasIndex(e => e.DistrictId, "district_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.Latitude)
                .HasPrecision(18, 15)
                .HasColumnName("latitude");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.Longitude)
                .HasPrecision(18, 15)
                .HasColumnName("longitude");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(255)
                .HasColumnName("phonenumber");
            entity.Property(e => e.SampleDelivery).HasColumnName("sample_delivery");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");

            entity.HasOne(d => d.District).WithMany(p => p.Agencies)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("agency_ibfk_1");
        });

        modelBuilder.Entity<BrickFactory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("brick_factory");

            entity.HasIndex(e => e.DistrictId, "district_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.Latitude)
                .HasPrecision(18, 15)
                .HasColumnName("latitude");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.Longitude)
                .HasPrecision(18, 15)
                .HasColumnName("longitude");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(255)
                .HasColumnName("phonenumber");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TakeSample).HasColumnName("take_sample");

            entity.HasOne(d => d.District).WithMany(p => p.BrickFactories)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("brick_factory_ibfk_1");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("city");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("district");

            entity.HasIndex(e => e.CityId, "city_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CityId).HasColumnName("city_id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.City).WithMany(p => p.Districts)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("district_ibfk_1");
        });

        modelBuilder.Entity<Logistic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("logistic");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(255)
                .HasColumnName("phonenumber");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("order");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdditionPrice)
                .HasPrecision(18, 2)
                .HasColumnName("addition_price");
            entity.Property(e => e.BasePrice)
                .HasPrecision(18, 2)
                .HasColumnName("base_price");
            entity.Property(e => e.DateDelivery)
                .HasColumnType("datetime")
                .HasColumnName("date_delivery");
            entity.Property(e => e.Revenue)
                .HasPrecision(18, 2)
                .HasColumnName("revenue");
            entity.Property(e => e.SellPrice)
                .HasPrecision(18, 2)
                .HasColumnName("sell_price");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(18, 2)
                .HasColumnName("total_price");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_ibfk_1");
        });

        modelBuilder.Entity<Orderdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("orderdetail");

            entity.HasIndex(e => e.AgencyId, "agency_id");

            entity.HasIndex(e => e.BrickFactoryid, "brick_factoryid");

            entity.HasIndex(e => e.LogisticId, "logistic_id");

            entity.HasIndex(e => e.OrderId, "order_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AgencyId).HasColumnName("agency_id");
            entity.Property(e => e.BrickFactoryid).HasColumnName("brick_factoryid");
            entity.Property(e => e.LogisticId).HasColumnName("logistic_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");

            entity.HasOne(d => d.Agency).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.AgencyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orderdetail_ibfk_2");

            entity.HasOne(d => d.BrickFactory).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.BrickFactoryid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orderdetail_ibfk_3");

            entity.HasOne(d => d.Logistic).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.LogisticId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orderdetail_ibfk_4");

            entity.HasOne(d => d.Order).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orderdetail_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.LoginTime)
                .HasColumnType("datetime")
                .HasColumnName("login_time");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(255)
                .HasColumnName("phone");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
            entity.Property(e => e.VerifyToken)
                .HasMaxLength(255)
                .HasColumnName("verify_token");
            entity.Property(e => e.VerifyTokenExpired)
                .HasColumnType("datetime")
                .HasColumnName("verify_token_expired");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Entities;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public AppDbContext() { }
    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Delivery> Deliveries { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    public virtual DbSet<Lab> Labs { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Subcategory> Subcategories { get; set; }

    public virtual DbSet<SupportRequest> SupportRequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      => optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection"));
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Carts__51BCD797E068D7C0");

            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carts__UserID__5BE2A6F2");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__488B0B2A05D0AE01");

            entity.Property(e => e.CartItemId).HasColumnName("CartItemID");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.Price).HasColumnType("decimal(19, 0)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItems__CartI__59FA5E80");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItems__Produ__5812160E");
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.DeliveryId).HasName("PK__Deliveri__626D8FEEAAB028A5");

            entity.Property(e => e.DeliveryId).HasColumnName("DeliveryID");
            entity.Property(e => e.DeliveryStatus).HasMaxLength(255);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");

            entity.HasOne(d => d.Order).WithMany(p => p.Deliveries)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Deliverie__Order__5CD6CB2B");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAD58FB1663A");

            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.InvoiceType).HasMaxLength(255);
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(19, 0)");

            entity.HasOne(d => d.Order).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Invoices__OrderI__5EBF139D");
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.InvoiceDetailId).HasName("PK__InvoiceD__1F1578F19EC09ACC");

            entity.Property(e => e.InvoiceDetailId).HasColumnName("InvoiceDetailID");
            entity.Property(e => e.InvoiceId).HasColumnName("InvoiceID");
            entity.Property(e => e.Price).HasColumnType("decimal(19, 0)");
            entity.Property(e => e.ProductName).HasMaxLength(255);

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK__InvoiceDe__Invoi__5DCAEF64");
        });

        modelBuilder.Entity<Lab>(entity =>
        {
            entity.HasKey(e => e.LabId).HasName("PK__Labs__EDBD773A741638A9");

            entity.Property(e => e.LabId).HasColumnName("LabID");
            entity.Property(e => e.LabFileUrl)
                .HasMaxLength(255)
                .HasColumnName("LabFileURL");
            entity.Property(e => e.LabName).HasMaxLength(255);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF9E0AD902");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(19, 0)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__UserID__619B8048");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C9A1503F3");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Price).HasColumnType("decimal(19, 0)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__5FB337D6");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Produ__5EBF139D");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__Permissi__EFA6FB0F82F2A92D");

            entity.Property(e => e.PermissionId).HasColumnName("PermissionID");
            entity.Property(e => e.Description).HasColumnType("ntext");
            entity.Property(e => e.PermissionName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6EDFFFB25B2");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Ages)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.ImagePath)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LabId).HasColumnName("LabID");
            entity.Property(e => e.Price).HasColumnType("decimal(19, 0)");
            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.SubcategoryId).HasColumnName("SubcategoryID");

            entity.HasOne(d => d.Lab).WithMany(p => p.Products)
                .HasForeignKey(d => d.LabId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Labs");

            entity.HasOne(d => d.Subcategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.SubcategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Subcategories");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RefreshT__3214EC07FCB1C378");

            entity.ToTable("RefreshToken");

            entity.Property(e => e.Created)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreatedByIp)
                .IsRequired()
                .HasMaxLength(45)
                .HasDefaultValue("");
            entity.Property(e => e.ExpirationTime).HasColumnType("datetime");
            entity.Property(e => e.ReplacedByToken).HasMaxLength(255);
            entity.Property(e => e.Revoked).HasColumnType("datetime");
            entity.Property(e => e.RevokedByIp).HasMaxLength(45);
            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(255)
                .HasDefaultValue("");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__RefreshTo__UserI__6477ECF3");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A3B3C4C3A");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .IsRequired()
                .HasMaxLength(20)
                .IsFixedLength();
        });

        modelBuilder.Entity<Subcategory>(entity =>
        {
            entity.HasKey(e => e.SubcategoryId).HasName("PK__Subcateg__9C4E707DD552F333");

            entity.Property(e => e.SubcategoryId).HasColumnName("SubcategoryID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.SubcategoryName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SupportRequest>(entity =>
        {
            entity.HasKey(e => e.SupportId).HasName("PK__SupportR__D82DBC6C73E0B15C");

            entity.ToTable("SupportRequest");

            entity.Property(e => e.SupportId).HasColumnName("SupportID");
            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.OrderDetail).WithMany(p => p.SupportRequests)
                .HasForeignKey(d => d.OrderDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SupportRe__Order__656C112C");

            entity.HasOne(d => d.User).WithMany(p => p.SupportRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SupportRe__UserI__66603565");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC3305E795");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.ExternalProvider).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(255);
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => e.UserPermissionId).HasName("PK__UserPerm__A90F88D27269C841");

            entity.Property(e => e.UserPermissionId).HasColumnName("UserPermissionID");
            entity.Property(e => e.PermissionId).HasColumnName("PermissionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.UserPermissionAssignedByNavigations)
                .HasForeignKey(d => d.AssignedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPermi__Assig__6754599E");

            entity.HasOne(d => d.Permission).WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPermi__Permi__68487DD7");

            entity.HasOne(d => d.User).WithMany(p => p.UserPermissionUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPermi__UserI__693CA210");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__3D978A55050DD839");

            entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__RoleI__6A30C649");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__UserI__6B24EA82");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
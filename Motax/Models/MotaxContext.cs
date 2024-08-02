using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Motax.Models;

public partial class MotaxContext : DbContext
{
    public MotaxContext()
    {
    }

    public MotaxContext(DbContextOptions<MotaxContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Accessories> Accessories { get; set; }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }
    public virtual DbSet<BlogDetail> BlogDetails { get; set; }
    public virtual DbSet<Brand> Brands { get; set; }
    public virtual DbSet<Car> Cars { get; set; }
    public virtual DbSet<CarRegistration> CarRegistrations { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CommentBlog> CommentBlogs { get; set; }

    public virtual DbSet<Compare> Compares { get; set; }

    public virtual DbSet<Dealer> Dealers { get; set; }

    public virtual DbSet<DealerDetail> DealerDetails { get; set; }
    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }
    public virtual DbSet<OrderStatus> OrderStatus { get; set; }
    public DbSet<ServiceUnit> ServiceUnits { get; set; }
    public DbSet<DeliveryUnit> DeliveryUnits { get; set; }
    public DbSet<OrderAccessories> OrderAccessories { get; set; }
    public DbSet<OrderDetailAccessories> OrderDetailAccessories { get; set; }
    public DbSet<Invoices> Invoices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-0QD3R2K;Initial Catalog=Motax;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Accessories>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Accessor__3214EC07DE0E68AB");

            entity.Property(e => e.CreateDay).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ImageSingle).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Slug).HasMaxLength(255);
            entity.Property(e => e.TypeCode).HasMaxLength(50);
            entity.Property(e => e.UpdateDay).HasColumnType("datetime");

            entity.HasOne(d => d.Brand).WithMany(p => p.Accessories)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Accessori__Brand__4BAC3F29");

            entity.HasOne(d => d.Category).WithMany(p => p.Accessories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Accessori__Categ__4CA06362");
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Accounts__3214EC076BF4BDA6");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Accounts__RoleId__398D8EEE");
        });


        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Blogs__3214EC0752C0A940");

            entity.Property(e => e.ImageSingle).HasMaxLength(255);
            entity.Property(e => e.Summary).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Author).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Blogs__AuthorId__6E01572D");
        });

        modelBuilder.Entity<BlogDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BlogDeta__3214EC07D05170DA");

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogDetails)
                .HasForeignKey(d => d.BlogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BlogDetai__BlogI__70DDC3D8");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brands__3214EC078C1AE7AE");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.ContactPerson).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(255);
            entity.Property(e => e.Slug).HasMaxLength(255);
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cars__3214EC070AA6E16B");

            entity.Property(e => e.BodyType).HasMaxLength(50);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Condition).HasMaxLength(50);
            entity.Property(e => e.DriverAirbag).HasMaxLength(50);
            entity.Property(e => e.EngineSize).HasMaxLength(50);
            entity.Property(e => e.FuelType).HasMaxLength(50);
            entity.Property(e => e.ImageSingle).HasMaxLength(255);
            entity.Property(e => e.Mileage).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PriceType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Transmission).HasMaxLength(50);
            entity.Property(e => e.Vin)
                .HasMaxLength(50)
                .HasColumnName("VIN");

            entity.HasOne(d => d.Brand).WithMany(p => p.Cars)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cars__BrandId__46E78A0C");

            entity.HasOne(d => d.Category).WithMany(p => p.Cars)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cars__CategoryId__47DBAE45");

            entity.HasOne(d => d.Dealer).WithMany(p => p.Cars)
                .HasForeignKey(d => d.DealerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cars__DealerId__48CFD27E");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07B9F0A21B");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Slug).HasMaxLength(255);
        });


        modelBuilder.Entity<CommentBlog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CommentB__3214EC075CE29F22");

            entity.Property(e => e.CommentDate).HasColumnType("datetime");

            entity.HasOne(d => d.Blog).WithMany(p => p.CommentBlogs)
                .HasForeignKey(d => d.BlogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentBl__BlogI__73BA3083");

            entity.HasOne(d => d.User).WithMany(p => p.CommentBlogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CommentBl__UserI__74AE54BC");
        });

        modelBuilder.Entity<Compare>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Compares__3214EC0761313135");

            entity.Property(e => e.CompareDate).HasColumnType("datetime");

            entity.HasOne(d => d.Car).WithMany(p => p.Compares)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compares__CarId__628FA481");

            entity.HasOne(d => d.User).WithMany(p => p.Compares)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compares__UserId__619B8048");
        });


        modelBuilder.Entity<Dealer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Dealers__3214EC07D9229A6B");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.ImageBackground).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<DealerDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DealerDe__3214EC07AFA984F3");

            entity.ToTable("DealerDetail");

            entity.Property(e => e.AvatarImage).HasMaxLength(255);
            entity.Property(e => e.ConsultantAvatar).HasMaxLength(255);
            entity.Property(e => e.ConsultantName).HasMaxLength(255);
            entity.Property(e => e.CoverImage).HasMaxLength(255);

            entity.HasOne(d => d.Dealer).WithMany(p => p.DealerDetails)
                .HasForeignKey(d => d.DealerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DealerDet__Deale__3E52440B");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Discount__3214EC079B649163");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Inventor__3214EC07BC8200AE");

            entity.ToTable("Inventory");

            entity.Property(e => e.LastUpdate).HasColumnType("datetime");
            entity.Property(e => e.WarehouseLocation).HasMaxLength(255);

            entity.HasOne(d => d.Accessories).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.AccessoriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Acces__5165187F");

            entity.HasOne(d => d.Car).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__CarId__5070F446");

            entity.HasOne(d => d.Dealer).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.DealerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Deale__4F7CD00D");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC070B6D9653");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreateDay).HasColumnType("datetime");
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.HowToPay).HasMaxLength(255);
            entity.Property(e => e.HowToTransport).HasMaxLength(255);
            entity.Property(e => e.Note).HasMaxLength(255);
            entity.Property(e => e.OrderCode).HasMaxLength(255);
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.Phone).HasMaxLength(255);
            entity.Property(e => e.UpdateDay).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(255);

            entity.HasOne(d => d.Account).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_AccountId");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderDet__3214EC07D1F4C80D");

            entity.HasOne(d => d.Accessories).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.AccessoriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Acces__59FA5E80");

            entity.HasOne(d => d.Car).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__CarId__59063A47");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__5812160E");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07AAF313BD");

            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Value).HasMaxLength(255);
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vouchers__3214EC07A929DF11");

            entity.Property(e => e.VoucherCode).HasMaxLength(50);
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Wishlist__3214EC07AA5D153C");

            entity.ToTable("Wishlist");

            entity.Property(e => e.SelectDate).HasColumnType("datetime");

            entity.HasOne(d => d.Accessories).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.AccessoriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlist__Access__6B24EA82");

            entity.HasOne(d => d.Car).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlist__CarId__6A30C649");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Wishlist__UserId__693CA210");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

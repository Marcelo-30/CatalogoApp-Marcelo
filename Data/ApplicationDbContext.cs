using CatalogoRopaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoRopaMVC.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Talla> Tallas => Set<Talla>();
    public DbSet<Color> Colores => Set<Color>();
    public DbSet<ImagenProducto> ImagenesProducto => Set<ImagenProducto>();
    public DbSet<Vendedor> Vendedores => Set<Vendedor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Categoria>()
            .HasIndex(c => c.Nombre)
            .IsUnique();

        modelBuilder.Entity<Talla>()
            .HasIndex(t => t.Nombre)
            .IsUnique();

        modelBuilder.Entity<Color>()
            .HasIndex(c => c.Nombre)
            .IsUnique();

        modelBuilder.Entity<Vendedor>()
            .HasIndex(v => v.EmailNormalizado)
            .IsUnique();

        modelBuilder.Entity<Vendedor>()
            .Property(v => v.LlaveUnica)
            .HasDefaultValue("VENDEDOR_DUENO");

        modelBuilder.Entity<Vendedor>()
            .HasIndex(v => v.LlaveUnica)
            .IsUnique();

        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Talla)
            .WithMany(t => t.Productos)
            .HasForeignKey(p => p.TallaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Color)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.ColorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ImagenProducto>()
            .HasOne(i => i.Producto)
            .WithMany(p => p.Imagenes)
            .HasForeignKey(i => i.ProductoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Playeras", Descripcion = "Playeras casuales y básicas." },
            new Categoria { Id = 2, Nombre = "Pantalones", Descripcion = "Jeans, joggers y pantalones casuales." },
            new Categoria { Id = 3, Nombre = "Chamarras", Descripcion = "Prendas para frío y outfits urbanos." },
            new Categoria { Id = 4, Nombre = "Accesorios", Descripcion = "Gorras, bolsas y complementos." }
        );

        modelBuilder.Entity<Talla>().HasData(
            new Talla { Id = 1, Nombre = "XS" },
            new Talla { Id = 2, Nombre = "S" },
            new Talla { Id = 3, Nombre = "M" },
            new Talla { Id = 4, Nombre = "L" },
            new Talla { Id = 5, Nombre = "XL" },
            new Talla { Id = 6, Nombre = "32" }
        );

        modelBuilder.Entity<Color>().HasData(
            new Color { Id = 1, Nombre = "Blanco", CodigoHex = "#FFFFFF" },
            new Color { Id = 2, Nombre = "Azul", CodigoHex = "#1F5AA6" },
            new Color { Id = 3, Nombre = "Negro", CodigoHex = "#111111" },
            new Color { Id = 4, Nombre = "Café", CodigoHex = "#7A4F36" }
        );

        modelBuilder.Entity<Producto>().HasData(
            new Producto
            {
                Id = 1,
                Nombre = "Playera básica blanca",
                Descripcion = "Playera de algodón para uso diario.",
                Precio = 199.00m,
                Stock = 12,
                Disponible = true,
                FechaRegistro = new DateTime(2026, 5, 15),
                CategoriaId = 1,
                TallaId = 3,
                ColorId = 1
            },
            new Producto
            {
                Id = 2,
                Nombre = "Jeans azul clásico",
                Descripcion = "Pantalón de mezclilla para outfit casual.",
                Precio = 549.00m,
                Stock = 8,
                Disponible = true,
                FechaRegistro = new DateTime(2026, 5, 15),
                CategoriaId = 2,
                TallaId = 6,
                ColorId = 2
            },
            new Producto
            {
                Id = 3,
                Nombre = "Chamarra negra urbana",
                Descripcion = "Chamarra ligera para clima fresco.",
                Precio = 899.00m,
                Stock = 5,
                Disponible = true,
                FechaRegistro = new DateTime(2026, 5, 15),
                CategoriaId = 3,
                TallaId = 4,
                ColorId = 3
            }
        );

        modelBuilder.Entity<ImagenProducto>().HasData(
            new ImagenProducto
            {
                Id = 1,
                ProductoId = 1,
                Url = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab",
                TextoAlternativo = "Playera básica blanca",
                EsPrincipal = true
            },
            new ImagenProducto
            {
                Id = 2,
                ProductoId = 2,
                Url = "https://images.unsplash.com/photo-1542272604-787c3835535d",
                TextoAlternativo = "Jeans azul clásico",
                EsPrincipal = true
            },
            new ImagenProducto
            {
                Id = 3,
                ProductoId = 3,
                Url = "https://images.unsplash.com/photo-1551028719-00167b16eac5",
                TextoAlternativo = "Chamarra negra urbana",
                EsPrincipal = true
            }
        );
    }
}

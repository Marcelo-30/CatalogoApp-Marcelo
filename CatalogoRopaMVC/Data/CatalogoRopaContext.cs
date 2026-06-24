using CatalogoRopaMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoRopaMVC.Data;

public class CatalogoRopaContext : DbContext
{
    public CatalogoRopaContext(DbContextOptions<CatalogoRopaContext> options)
        : base(options)
    {
    }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Playeras", Descripcion = "Playeras casuales y básicas." },
            new Categoria { Id = 2, Nombre = "Pantalones", Descripcion = "Jeans, joggers y pantalones casuales." },
            new Categoria { Id = 3, Nombre = "Chamarras", Descripcion = "Prendas para frío y outfits urbanos." },
            new Categoria { Id = 4, Nombre = "Accesorios", Descripcion = "Gorras, bolsas y complementos." }
        );

        modelBuilder.Entity<Producto>().HasData(
            new Producto
            {
                Id = 1,
                Nombre = "Playera básica blanca",
                Descripcion = "Playera de algodón para uso diario.",
                Precio = 199.00m,
                Talla = "M",
                Color = "Blanco",
                Stock = 12,
                Disponible = true,
                FechaRegistro = new DateTime(2026, 5, 15),
                CategoriaId = 1,
                ImagenUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab"
            },
            new Producto
            {
                Id = 2,
                Nombre = "Jeans azul clásico",
                Descripcion = "Pantalón de mezclilla para outfit casual.",
                Precio = 549.00m,
                Talla = "32",
                Color = "Azul",
                Stock = 8,
                Disponible = true,
                FechaRegistro = new DateTime(2026, 5, 15),
                CategoriaId = 2,
                ImagenUrl = "https://images.unsplash.com/photo-1542272604-787c3835535d"
            },
            new Producto
            {
                Id = 3,
                Nombre = "Chamarra negra urbana",
                Descripcion = "Chamarra ligera para clima fresco.",
                Precio = 899.00m,
                Talla = "L",
                Color = "Negro",
                Stock = 5,
                Disponible = true,
                FechaRegistro = new DateTime(2026, 5, 15),
                CategoriaId = 3,
                ImagenUrl = "https://images.unsplash.com/photo-1551028719-00167b16eac5"
            }
        );
    }
}

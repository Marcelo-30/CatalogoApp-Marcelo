# CatalogoAPP - ADR-01 Base MVC

## Objetivo

Esta rama establece la base funcional de CatalogoAPP usando ASP.NET Core MVC. Incluye la estructura principal del proyecto, las entidades del catalogo de ropa, el acceso a datos con Entity Framework Core y las vistas Razor necesarias para administrar productos y categorias.

## Tecnologias utilizadas

- C#
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Razor Views
- Git y GitHub

## Funciones incluidas

- Catalogo web de productos de ropa.
- CRUD de productos.
- CRUD de categorias.
- Entidades para Producto, Categoria, Talla, Color e ImagenProducto.
- Busqueda de productos por nombre, descripcion, talla o color.
- Filtro por categoria.
- Filtro para mostrar solo productos disponibles.
- Imagen principal de producto usando URL.
- Texto claro cuando un producto no tiene imagen.
- Datos semilla para categorias, tallas, colores, productos e imagenes.
- Migracion inicial de Entity Framework Core.

## Estructura del proyecto

- `Controllers`: controladores MVC para Home, Productos y Categorias.
- `Models`: entidades principales del catalogo y modelo de error.
- `Views`: vistas Razor para paginas, formularios y listados.
- `Data`: `ApplicationDbContext` con DbSet y configuracion de relaciones.
- `Services`: no aplica en esta rama; se incorporan en ADR-03 para separar responsabilidades.
- `ViewModels`: no aplica en esta rama; se incorporan cuando la autenticacion y la presentacion lo requieren.
- `DTOs`: no aplica en esta rama; se agregan en ADR-04 para la API REST.
- `docs`: no aplica en esta rama; se agrega en ADR-02 para vistas arquitectonicas.
- `wwwroot`: archivos estaticos de CSS y JavaScript.
- `Migrations`: migracion inicial y snapshot de EF Core.

## Como ejecutar el proyecto

```bash
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```

La cadena de conexion se encuentra en `appsettings.json`:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CatalogoRopaDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Si usas otro servidor de SQL Server, ajusta esa cadena antes de aplicar migraciones.

## Clausula de uso de IA

Para la elaboracion y organizacion de esta rama se utilizo inteligencia artificial como herramienta de apoyo. Las decisiones, ajustes y validaciones finales fueron revisadas dentro del contexto del proyecto CatalogoAPP.

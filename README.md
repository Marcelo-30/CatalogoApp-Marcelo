# Catálogo de Ropa MVC

Proyecto inicial basado en el ADR-01 de Marcelo Medina: **Arquitectura MVC para catálogo de ropa**.

## Objetivo

Sistema web para pequeños vendedores y revendedores que necesitan administrar y mostrar un catálogo de ropa sin usar plataformas complejas o costosas.

## Tecnologías utilizadas

- C#
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Arquitectura MVC
- Git y GitHub

## Funciones incluidas

- Página principal del catálogo.
- CRUD de productos:
  - Crear producto.
  - Ver detalles.
  - Editar producto.
  - Eliminar producto.
  - Filtrar por categoría.
  - Buscar por nombre, descripción, color o talla.
  - Mostrar solo productos disponibles.
- CRUD de categorías:
  - Crear categoría.
  - Ver detalles.
  - Editar categoría.
  - Eliminar categoría.
- Validaciones con Data Annotations.
- Conexión preparada para SQL Server LocalDB.
- Datos iniciales para categorías y productos.

## Estructura del proyecto

```text
CatalogoRopaMVC/
├── Controllers/
│   ├── HomeController.cs
│   ├── ProductosController.cs
│   └── CategoriasController.cs
├── Data/
│   └── CatalogoRopaContext.cs
├── Models/
│   ├── Producto.cs
│   ├── Categoria.cs
│   └── ErrorViewModel.cs
├── Views/
│   ├── Home/
│   ├── Productos/
│   ├── Categorias/
│   └── Shared/
├── wwwroot/
│   ├── css/site.css
│   └── js/site.js
├── Program.cs
├── appsettings.json
└── CatalogoRopaMVC.csproj
```

## Cómo ejecutar

1. Abre la carpeta del proyecto en Visual Studio o Visual Studio Code.
2. Restaura los paquetes:

```bash
dotnet restore
```

3. Instala la herramienta de Entity Framework Core si no la tienes:

```bash
dotnet tool install --global dotnet-ef
```

4. Crea la migración inicial:

```bash
dotnet ef migrations add InitialCreate
```

5. Crea la base de datos:

```bash
dotnet ef database update
```

6. Ejecuta el proyecto:

```bash
dotnet run
```

7. Abre el navegador en la URL que aparezca en consola, por ejemplo:

```text
https://localhost:7069
```

## Configuración de base de datos

La conexión está en `appsettings.json`:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CatalogoRopaDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Si usas SQL Server Express o un servidor diferente, cambia esa cadena de conexión.

## Primer commit sugerido

```bash
git init
git add .
git commit -m "Inicializa catálogo de ropa MVC con EF Core y SQL Server"
```

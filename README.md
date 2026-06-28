# CatalogoAPP - ADR-02 Vistas Arquitectonicas

## Objetivo

Esta rama documenta las vistas arquitectonicas iniciales de CatalogoAPP. Parte de la base MVC de ADR-01 y agrega material para explicar la estructura logica, fisica, de despliegue, de procesos y C4 del sistema.

## Tecnologias utilizadas

- C#
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Razor Views
- Git y GitHub
- Mermaid para diagramas C4 en Markdown

## Funciones incluidas

- Documentacion de vista logica.
- Documentacion de vista fisica.
- Documentacion de vista de despliegue.
- Documentacion de vista de procesos.
- Documentacion C4 Nivel 1.
- Documentacion C4 Nivel 2.
- Carpeta de diagramas versionada en `Docs/img`.
- Conservacion de la funcionalidad MVC creada en ADR-01.

## Estructura del proyecto

- `Controllers`: controladores MVC del catalogo.
- `Models`: entidades Producto, Categoria, Talla, Color e ImagenProducto.
- `Views`: vistas Razor para productos, categorias y pagina inicial.
- `Data`: `ApplicationDbContext` con EF Core.
- `Services`: no aplica en esta rama; se incorpora en ADR-03.
- `ViewModels`: no aplica en esta rama; se incorpora cuando haga falta separar presentacion.
- `DTOs`: no aplica en esta rama; se agrega en ADR-04.
- `Docs`: documentacion ADR-02 y diagramas arquitectonicos.
- `wwwroot`: estilos y scripts del sitio.

## Como ejecutar el proyecto

```bash
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```

La documentacion principal se encuentra en `Docs/ADR-02-Vistas-Arquitectonicas-CatalogoAPP.md`.

## Clausula de uso de IA

Para la elaboracion y organizacion de esta rama se utilizo inteligencia artificial como herramienta de apoyo. Las decisiones, ajustes y validaciones finales fueron revisadas dentro del contexto del proyecto CatalogoAPP.

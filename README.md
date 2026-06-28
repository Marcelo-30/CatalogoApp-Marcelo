# CatalogoAPP - ADR-05 Patrones GOF

## Objetivo

Esta rama aplica patrones de diseno GOF de forma realista dentro de CatalogoAPP, manteniendo la aplicacion MVC y la API REST funcionando. Se agregan Strategy para filtros del catalogo y Factory para construir DTOs de producto sin duplicar mapeos en los controladores.

## Tecnologias utilizadas

- C#
- ASP.NET Core MVC
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Razor Views
- Git y GitHub

## Funciones incluidas

- Patron Strategy para filtros de productos.
- Filtro por texto de busqueda.
- Filtro por categoria.
- Filtro por disponibilidad y stock.
- Patron Factory para crear `ProductoDto`.
- API REST de ADR-04 conservada.
- Roles Cliente y Vendedor conservados.
- Documentacion arquitectonica de ramas anteriores conservada.

## Estructura del proyecto

- `Controllers`: controladores MVC y API.
- `Models`: entidades de dominio del catalogo.
- `Views`: vistas Razor para catalogo, administracion y cuenta.
- `Data`: acceso a datos con EF Core.
- `Services`: servicios de aplicacion y patrones GOF.
- `ViewModels`: filtros y formularios de pantalla.
- `DTOs`: contratos de entrada y salida de la API.
- `Docs`: vistas arquitectonicas y diagramas.
- `wwwroot`: estilos y scripts.

## Patrones GOF implementados

Strategy:

- Problema que resuelve: evita que `ProductoCatalogoService` tenga toda la logica de filtrado en un solo metodo.
- Archivos principales:
  - `Services/Filtros/IFiltroProductoStrategy.cs`
  - `Services/Filtros/FiltroTextoProductoStrategy.cs`
  - `Services/Filtros/FiltroCategoriaProductoStrategy.cs`
  - `Services/Filtros/FiltroDisponibilidadProductoStrategy.cs`
  - `Services/ProductoCatalogoService.cs`
- Uso: el servicio recibe una coleccion de estrategias por inyeccion de dependencias y aplica cada filtro sobre el query de productos.

Factory:

- Problema que resuelve: evita repetir la construccion de `ProductoDto` dentro del controlador API.
- Archivos principales:
  - `Services/Factories/IProductoDtoFactory.cs`
  - `Services/Factories/ProductoDtoFactory.cs`
  - `Controllers/Api/ProductosApiController.cs`
- Uso: el controlador API solicita a la fabrica un DTO individual o una coleccion de DTOs para responder al cliente.

## Como ejecutar el proyecto

```bash
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```

Para probar la API:

```text
GET /api/productos
GET /api/productos/{id}
GET /api/categorias
GET /api/tallas
```

Para crear, editar o eliminar productos desde la API, inicia sesion como Vendedor desde la aplicacion MVC y usa la cookie de sesion en tu cliente HTTP.

## Clausula de uso de IA

Para la elaboracion y organizacion de esta rama se utilizo inteligencia artificial como herramienta de apoyo. Las decisiones, ajustes y validaciones finales fueron revisadas dentro del contexto del proyecto CatalogoAPP.

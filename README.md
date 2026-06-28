# CatalogoAPP - ADR-04 API REST

## Objetivo

Esta rama agrega una API REST al mismo proyecto ASP.NET Core MVC sin romper la experiencia web existente. La API permite consultar productos, categorias y tallas, y protege las operaciones de escritura para el rol Vendedor.

## Tecnologias utilizadas

- C#
- ASP.NET Core MVC
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Razor Views
- Git y GitHub

## Funciones incluidas

- API REST de productos.
- DTOs para requests y responses.
- Validacion de datos de entrada con Data Annotations.
- Validacion de categoria, talla y color antes de guardar productos.
- Respuestas HTTP con `200 OK`, `201 Created`, `204 No Content`, `400 Bad Request`, `404 Not Found`, `401 Unauthorized` y `403 Forbidden`.
- Endpoints de consulta para categorias y tallas.
- Operaciones POST, PUT y DELETE protegidas para Vendedor.
- Conservacion de la aplicacion MVC y roles agregados en ADR-03.

## Estructura del proyecto

- `Controllers`: controladores MVC y controladores API bajo `Controllers/Api`.
- `Models`: entidades de dominio usadas por EF Core.
- `Views`: vistas Razor de la aplicacion web.
- `Data`: `ApplicationDbContext` y configuracion de EF Core.
- `Services`: servicios de aplicacion usados por MVC.
- `ViewModels`: modelos de pantalla para vistas MVC.
- `DTOs`: modelos de entrada y salida para la API REST.
- `Docs`: documentacion arquitectonica.
- `wwwroot`: recursos estaticos.

## Como ejecutar el proyecto

```bash
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```

## Endpoints disponibles

```text
GET    /api/productos
GET    /api/productos/{id}
POST   /api/productos
PUT    /api/productos/{id}
DELETE /api/productos/{id}
GET    /api/categorias
GET    /api/tallas
```

## Ejemplos de requests y responses

Request:

```http
GET /api/productos
```

Response:

```json
[
  {
    "id": 1,
    "nombre": "Playera basica blanca",
    "descripcion": "Playera de algodon para uso diario.",
    "categoria": "Playeras",
    "talla": "M",
    "color": "Blanco",
    "precio": 199.00,
    "stock": 12,
    "disponible": true,
    "imagenUrl": "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab"
  }
]
```

Request:

```http
POST /api/productos
Content-Type: application/json
```

```json
{
  "nombre": "Sudadera gris",
  "descripcion": "Sudadera casual para clima fresco.",
  "precio": 499.00,
  "stock": 10,
  "disponible": true,
  "categoriaId": 3,
  "tallaId": 4,
  "colorId": 3,
  "imagenUrl": "https://example.com/sudadera.jpg"
}
```

Response esperada: `201 Created` si el usuario autenticado tiene rol Vendedor.

## Como probar la API

Puedes probar endpoints GET desde el navegador:

```text
http://localhost:5000/api/productos
http://localhost:5000/api/categorias
http://localhost:5000/api/tallas
```

Para POST, PUT y DELETE usa Postman, Thunder Client o una herramienta similar. Primero inicia sesion como Vendedor en la aplicacion web para que la cookie de autenticacion tenga permisos de escritura. No se agrego Swagger para evitar introducir dependencias nuevas en esta rama.

## Clausula de uso de IA

Para la elaboracion y organizacion de esta rama se utilizo inteligencia artificial como herramienta de apoyo. Las decisiones, ajustes y validaciones finales fueron revisadas dentro del contexto del proyecto CatalogoAPP.

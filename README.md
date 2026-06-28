# CatalogoAPP - ADR-06 Seguridad de Vendedor Unico

## Objetivo

Esta rama mejora la seguridad del acceso de vendedor despues de ADR-05. El catalogo sigue siendo publico para clientes, pero la administracion de productos y categorias solo puede usarse con una cuenta real de vendedor dueno guardada en la base de datos.

## Tecnologias utilizadas

- C#
- ASP.NET Core MVC
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Razor Views
- Git y GitHub
- Autenticacion por cookies
- `PasswordHasher` de ASP.NET Core para guardar contrasenas con hash

## Funciones incluidas

- Registro del primer y unico vendedor dueno.
- Bloqueo de nuevos registros de vendedor si ya existe una cuenta dueno.
- Login de vendedor usando correo y contrasena reales.
- Contrasena guardada como hash, no como texto plano.
- Tabla `Vendedores` en SQL Server.
- Indice unico para correo normalizado.
- Indice unico de cuenta dueno para reforzar que solo exista un vendedor creado por la app.
- Eliminacion del selector Cliente/Vendedor en login y registro.
- Clientes sin cuenta: pueden seguir viendo el catalogo publico.
- Administracion protegida con rol `Vendedor`.

## Estructura del proyecto

- `Controllers`: controladores MVC y API; `CuentaController` valida credenciales reales.
- `Models`: entidades de dominio, incluyendo `Vendedor`.
- `Views`: vistas Razor para catalogo, administracion y cuenta de vendedor.
- `Data`: `ApplicationDbContext`, EF Core y configuracion de la entidad `Vendedor`.
- `Services`: servicios de catalogo, filtros, factories y autenticacion de vendedor.
- `ViewModels`: formularios de login y registro sin selector de rol.
- `DTOs`: contratos de entrada y salida de la API.
- `Docs`: vistas arquitectonicas y diagramas.
- `wwwroot`: estilos y scripts.

## Como ejecutar el proyecto

```bash
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```

La primera vez que entres a `Cuenta/Registro`, registra al vendedor dueno. Despues de eso, la app ya no permitira registrar otro vendedor. Para administrar productos o categorias, entra desde `Cuenta/Login` con el correo y la contrasena registrados.

## API REST

Los endpoints GET siguen siendo publicos:

```text
GET /api/productos
GET /api/productos/{id}
GET /api/categorias
GET /api/tallas
```

Para crear, editar o eliminar productos desde la API, inicia sesion como Vendedor desde la aplicacion MVC y usa la cookie de sesion en tu cliente HTTP.

## Clausula de uso de IA

Para la elaboracion y organizacion de esta rama se utilizo inteligencia artificial como herramienta de apoyo. Las decisiones, ajustes y validaciones finales fueron revisadas dentro del contexto del proyecto CatalogoAPP.

# CatalogoAPP - ADR-03 Estilo Arquitectonico

## Objetivo

Esta rama alinea CatalogoAPP con un estilo cliente-servidor usando arquitectura en capas basada en MVC. Tambien agrega autenticacion demo por cookies para distinguir Cliente y Vendedor, restringiendo la administracion del catalogo al rol Vendedor.

## Tecnologias utilizadas

- C#
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Razor Views
- Git y GitHub
- Autenticacion por cookies de ASP.NET Core

## Funciones incluidas

- Catalogo visible para clientes y visitantes.
- Login y registro demo con roles Cliente y Vendedor.
- Cierre de sesion.
- Vista de acceso denegado.
- Botones de crear, editar y eliminar productos visibles solo para Vendedor.
- Controladores de productos y categorias protegidos para operaciones administrativas.
- Servicio `ProductoCatalogoService` para separar consultas y filtros del controlador.
- ViewModels para formularios de cuenta y filtros del catalogo.
- Conservacion de la documentacion de vistas arquitectonicas de ADR-02.

## Estructura del proyecto

- `Controllers`: capa de control MVC, con acciones para catalogo, categorias y cuenta.
- `Models`: capa de dominio con Producto, Categoria, Talla, Color e ImagenProducto.
- `Views`: capa de presentacion con Razor Views, formularios y navegacion por rol.
- `Data`: capa de acceso a datos con `ApplicationDbContext`, EF Core y migraciones.
- `Services`: capa de aplicacion para consultas del catalogo y listas auxiliares.
- `ViewModels`: modelos de pantalla para login, registro y filtros.
- `DTOs`: no aplica en esta rama; se agrega en ADR-04 para la API REST.
- `Docs`: documentos y diagramas arquitectonicos.
- `wwwroot`: CSS y JavaScript de la interfaz.

## Como ejecutar el proyecto

```bash
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```

Para probar roles, entra en `Cuenta/Login` o `Cuenta/Registro` y selecciona Cliente o Vendedor. Cliente puede consultar el catalogo; Vendedor puede administrar productos y categorias.

## Clausula de uso de IA

Para la elaboracion y organizacion de esta rama se utilizo inteligencia artificial como herramienta de apoyo. Las decisiones, ajustes y validaciones finales fueron revisadas dentro del contexto del proyecto CatalogoAPP.

# ADR-01: Base ASP.NET Core MVC de CatalogoRopaMVC

| Campo | Valor |
|---|---|
| Autor | Marcelo Medina |
| Fecha | 2026-06-28 |
| Estado | Aceptado |

## Contexto

CatalogoRopaMVC nació como un catálogo web para que un vendedor publique prendas y administre productos, categorías, tallas, colores e imágenes. La primera versión necesitaba una base comprensible, compatible con SQL Server y adecuada para una entrega académica individual.

## Problema arquitectónico

Era necesario elegir una estructura que separara la presentación, el manejo de solicitudes y la persistencia sin introducir la complejidad operativa de múltiples servicios desplegables.

## Decisión

Se adoptó ASP.NET Core 8 MVC con Razor Views, Entity Framework Core 8 y SQL Server. El proyecto se organiza principalmente en `Controllers`, `Views`, `Models`, `Data`, `Migrations` y `wwwroot`. `ApplicationDbContext` representa la unidad de acceso a datos y las migraciones de EF Core controlan la evolución del esquema.

## Justificación

MVC ofrece una separación suficiente para el tamaño del producto, Razor permite entregar la interfaz desde el mismo proceso y EF Core mantiene el modelo relacional y sus migraciones dentro del ecosistema .NET. La decisión reduce herramientas, repositorios y despliegues necesarios para una aplicación individual.

## Alternativas consideradas

- **Aplicación de una sola página y API separada:** descartada inicialmente por exigir dos proyectos y un proceso adicional de construcción y despliegue.
- **Acceso directo con ADO.NET:** descartado porque aumentaba el código de persistencia y migración.
- **Microservicios:** descartados por su costo operativo y porque el dominio y el equipo no justificaban distribución independiente.

## Consecuencias positivas

- Estructura conocida y fácil de ejecutar con el SDK de .NET.
- Renderizado del lado del servidor y API pueden compartir modelos, configuración y persistencia.
- EF Core proporciona mapeo, datos semilla y migraciones reproducibles.

## Consecuencias negativas

- La aplicación web, la API y la persistencia se despliegan y escalan como una sola unidad.
- Algunos controladores mantienen acceso directo a `ApplicationDbContext`.
- Las vistas Razor quedan acopladas al ciclo de entrega del backend.

## Riesgos y limitaciones

- Un error o saturación del único proceso puede afectar simultáneamente la interfaz y el API.
- La dependencia de SQL Server debe estar disponible para casi todos los flujos funcionales.
- La simplicidad inicial puede degradarse si nuevas responsabilidades no se extraen a servicios.

## Evidencia en el código

- `CatalogoRopaMVC.csproj`: proyecto web `net8.0` y proveedor `Microsoft.EntityFrameworkCore.SqlServer`.
- `Program.cs`: registro de MVC, autenticación, servicios y `ApplicationDbContext`.
- `Data/ApplicationDbContext.cs`: entidades, relaciones, índices y datos semilla.
- `Controllers/`, `Views/`, `Models/` y `Migrations/`: implementación de la estructura elegida.

## Relación con otros ADR

- ADR-02 documenta las vistas arquitectónicas de esta base.
- ADR-03 refina la organización como cliente-servidor en capas.
- ADR-04 incorpora la API REST dentro del mismo despliegue.
- ADR-08 traslada la solución a Azure sin cambiar el estilo base.

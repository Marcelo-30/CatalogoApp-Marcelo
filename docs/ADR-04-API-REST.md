# ADR-04: API REST integrada en la aplicación MVC

| Campo | Valor |
|---|---|
| Autor | Marcelo Medina |
| Fecha | 2026-06-28 |
| Estado | Aceptado |

## Contexto

Además de las vistas Razor, el catálogo necesitaba una interfaz HTTP consumible por herramientas y clientes externos sin retirar la experiencia web existente.

## Problema arquitectónico

Era necesario exponer recursos del catálogo con semántica HTTP y contratos estables, proteger las operaciones administrativas y evitar que las entidades de EF Core definieran por sí solas el contrato público.

## Decisión

Se agregó una API REST en el mismo proyecto y despliegue ASP.NET Core. Los endpoints públicos de lectura son `GET /api/productos`, `GET /api/productos/{id}`, `GET /api/categorias` y `GET /api/tallas`. `POST`, `PUT` y `DELETE` de productos requieren el rol `Vendedor`. `ProductoUpsertDto` modela entradas y `ProductoDtoFactory` genera salidas.

## Justificación

Compartir proceso y `ApplicationDbContext` evita duplicar dominio y persistencia. Los DTO separan el contrato HTTP de las entidades y los códigos `200`, `201`, `204`, `400`, `404`, `401` y `403` comunican resultados de forma estándar.

## Alternativas consideradas

- **Solo vistas MVC:** descartada porque no ofrece un contrato de integración.
- **API en un servicio independiente:** descartada por el costo de otro despliegue y sincronización de datos.
- **Exponer entidades directamente:** descartada para evitar sobreexposición y acoplamiento con EF Core.
- **Agregar Swagger desde el inicio:** pospuesto para limitar dependencias; la documentación se mantiene en Markdown.

## Consecuencias positivas

- El catálogo puede consumirse como HTML o JSON.
- Los DTO controlan los campos de entrada y salida.
- Los endpoints de consulta permanecen públicos y las escrituras exigen autorización.

## Consecuencias negativas

- MVC y API comparten disponibilidad, capacidad y versión.
- Las reglas de escritura siguen duplicadas en `ProductosController` y `ProductosApiController`.
- La autenticación por cookies es cómoda para el navegador, pero no es el esquema habitual para integraciones externas.

## Riesgos y limitaciones

- Las escrituras del API usan la cookie de sesión y no aplican antiforgery; un navegador autenticado podría quedar expuesto a CSRF si acepta una solicitud compatible desde otro origen. Se requiere mitigación antes de ampliar clientes web.
- No hay versionado explícito del API.
- No se implementan paginación, límites de tasa ni OpenAPI.

## Evidencia en el código

- `Controllers/Api/ProductosApiController.cs` y `Controllers/Api/CatalogosApiController.cs`.
- `DTOs/ProductoDto.cs`, `DTOs/ProductoUpsertDto.cs`, `DTOs/CategoriaDto.cs` y `DTOs/TallaDto.cs`.
- `Program.cs`: las rutas por atributo se habilitan mediante `AddControllersWithViews` y el mapeo de controladores.
- `ProductoDtoFactory` elimina el mapeo repetido de respuestas.

## Relación con otros ADR

- Extiende ADR-01 y ADR-03 dentro de la misma unidad desplegable.
- ADR-05 formaliza la fábrica usada por las respuestas.
- ADR-06 aplica la identidad del vendedor a las escrituras.
- ADR-07 y ATAM registran la duplicación y el riesgo CSRF.

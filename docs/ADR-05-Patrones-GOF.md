# ADR-05: Strategy para filtros y Factory para DTOs

| Campo | Valor |
|---|---|
| Autor | Marcelo Medina |
| Fecha | 2026-06-28 |
| Estado | Aceptado |

## Contexto

El catálogo acumuló filtros por texto, categoría y disponibilidad, mientras que el API necesitaba mapear productos con sus relaciones e imagen principal. Mantener esas decisiones como condicionales y mapeos dentro de controladores aumentaba duplicación.

## Problema arquitectónico

Se requería extender filtros y construir DTOs de forma consistente sin concentrar reglas heterogéneas en un único método.

## Decisión

Se implementó Strategy mediante `IFiltroProductoStrategy` y tres estrategias: texto, categoría y disponibilidad. `ProductoCatalogoService` las recibe como colección y las aplica al `IQueryable`. Se implementó Factory mediante `IProductoDtoFactory` y `ProductoDtoFactory` para crear un `ProductoDto` o una colección.

## Justificación

Strategy permite agregar o modificar filtros de manera localizada y conserva composición sobre la consulta de EF Core. Factory centraliza el contrato de salida y su tratamiento de navegaciones opcionales e imagen principal.

## Alternativas consideradas

- **Condicionales en el controlador:** descartados por mezcla de responsabilidades.
- **Un solo método con todos los filtros:** viable para pocos criterios, pero menos extensible y más difícil de probar aisladamente.
- **Mapeo manual en cada endpoint:** descartado por duplicación.
- **Automapper:** descartado porque el mapeo actual es pequeño y la imagen principal requiere una regla explícita.

## Consecuencias positivas

- Las reglas se prueban sin servidor web ni base de datos.
- El servicio compone filtros sin conocer su implementación concreta.
- Las respuestas del API conservan un único mapeo.

## Consecuencias negativas

- Se agregan interfaces y clases para reglas relativamente pequeñas.
- El orden de registro de estrategias puede volverse relevante si aparecen filtros con efectos no conmutativos.
- La fábrica depende de que las navegaciones necesarias hayan sido cargadas.

## Riesgos y limitaciones

- Agregar una estrategia sin registrarla en `Program.cs` hace que no se ejecute.
- Los filtros operan sobre expresiones compatibles con el proveedor de EF Core; una regla no traducible fallaría al consultar SQL Server.
- Factory no sustituye la validación de entrada ni el versionado del API.

## Evidencia en el código

- `Services/Filtros/IFiltroProductoStrategy.cs` y sus tres implementaciones.
- `Services/ProductoCatalogoService.cs`: aplicación secuencial de estrategias.
- `Services/Factories/ProductoDtoFactory.cs`.
- `tests/CatalogoRopaMVC.Tests/`: 16 pruebas, incluidas las estrategias y la fábrica.

## Relación con otros ADR

- Refina la separación definida por ADR-03.
- Apoya el contrato REST de ADR-04.
- ADR-07 identifica que las escrituras aún requieren una abstracción equivalente.

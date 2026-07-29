# ADR-03: Monolito modular cliente-servidor en capas

| Campo | Valor |
|---|---|
| Autor | Marcelo Medina |
| Fecha | 2026-06-28 |
| Estado | Aceptado |

## Contexto

La base MVC permitió publicar el catálogo, pero el crecimiento hacia filtros, autenticación y administración exigió responsabilidades más claras que las ofrecidas por controladores con toda la lógica incorporada.

## Problema arquitectónico

El sistema debía mantener una entrega sencilla y, al mismo tiempo, separar presentación, coordinación, reglas de aplicación y persistencia para limitar el acoplamiento.

## Decisión

CatalogoRopaMVC se mantiene como un monolito modular cliente-servidor. El navegador actúa como cliente; el proceso ASP.NET Core contiene controladores MVC y API, Razor Views, servicios, DTOs, ViewModels, modelos de dominio y acceso a datos. La inyección de dependencias conecta las capas. `ProductoCatalogoService` concentra consultas del catálogo y `VendedorAuthService` concentra registro y validación del vendedor.

## Justificación

El monolito modular conserva una sola unidad de compilación y despliegue, apropiada para un equipo individual, pero permite extraer lógica reutilizable a servicios. La separación es suficiente para probar filtros y fábricas sin asumir costos de comunicación distribuida.

## Alternativas consideradas

- **MVC sin capa de servicios:** descartado porque mantenía consultas y autenticación en controladores.
- **Arquitectura limpia con proyectos separados:** pospuesta; agregaría límites más fuertes, pero también más ensamblados y mapeos para el alcance actual.
- **Microservicios por catálogo y autenticación:** descartados por despliegue, observabilidad y consistencia innecesariamente complejos.

## Consecuencias positivas

- Un solo artefacto contiene la interfaz web y el API.
- Los servicios registrados por interfaz facilitan sustitución y pruebas.
- Las carpetas expresan responsabilidades reconocibles.

## Consecuencias negativas

- No existe aislamiento de fallos ni escalamiento independiente entre MVC y API.
- Algunos comandos de productos están duplicados entre controladores MVC y REST.
- Todas las capas comparten el mismo proceso y modelo de datos.

## Riesgos y limitaciones

- El crecimiento puede convertir el monolito modular en un monolito altamente acoplado.
- `ApplicationDbContext` es un punto de sensibilidad para catálogo, API y autenticación.
- La separación por carpetas no impide dependencias inadecuadas en tiempo de compilación.

## Evidencia en el código

- `Program.cs`: registro de `IProductoCatalogoService`, `IVendedorAuthService`, estrategias y fábrica.
- `Services/ProductoCatalogoService.cs` y `Services/VendedorAuthService.cs`.
- `Controllers/ProductosController.cs`, `Controllers/CuentaController.cs` y `Controllers/Api/`.
- `DTOs/` y `ViewModels/`: contratos diferenciados para API y vistas.

## Relación con otros ADR

- Evoluciona ADR-01 sin sustituir MVC.
- ADR-04 añade el canal REST al mismo monolito.
- ADR-05 detalla Strategy y Factory dentro de la capa de aplicación.
- ADR-07 registra el acoplamiento todavía pendiente en comandos.

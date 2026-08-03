# Evaluación ATAM de CatalogoRopaMVC

## Alcance de la evaluación

Esta evaluación aplica el enfoque Architecture Tradeoff Analysis Method a la versión final de CatalogoRopaMVC: SPA React 19, API REST en ASP.NET Core 8, autenticación por cookies con antiforgery, Entity Framework Core, Azure Container Apps, Azure SQL Database y entrega con GitHub Actions.

El análisis se basa en código y configuración versionados. No incluye pruebas de carga ni métricas inventadas; probabilidad e impacto son valoraciones cualitativas para priorizar trabajo.

## Objetivos del sistema

- Publicar un catálogo de ropa accesible sin autenticación.
- Permitir que un único vendedor dueño administre productos y categorías.
- Exponer consultas REST consistentes sin duplicar el modelo de presentación.
- Mantener datos persistentes en SQL Server mediante migraciones de EF Core.
- Entregar una demo académica pública, verificable y de costo controlado.

## Stakeholders

| Stakeholder | Interés principal |
|---|---|
| Cliente o visitante | Disponibilidad, facilidad de consulta y datos correctos. |
| Vendedor dueño | Seguridad de la cuenta e integridad de las operaciones administrativas. |
| Autor y mantenedor | Modificabilidad, pruebas reproducibles y costo bajo. |
| Profesor o evaluador | Evidencia trazable de arquitectura, CI, despliegue y funcionamiento. |
| Operador de la suscripción Azure | Protección del crédito, secretos y límite de gasto. |

## Atributos de calidad prioritarios

1. **Seguridad:** solo el vendedor puede escribir y los secretos no se versionan.
2. **Disponibilidad:** catálogo, autenticación y API deben recuperar acceso a datos ante fallos transitorios.
3. **Modificabilidad:** filtros, DTOs y reglas deben evolucionar sin duplicación creciente.
4. **Desplegabilidad:** una versión validada debe poder publicarse de forma repetible.
5. **Costo:** la demo debe mantenerse en cuotas gratuitas o detenerse antes de generar cargos no autorizados.
6. **Testabilidad:** las reglas críticas deben comprobarse sin infraestructura externa cuando sea posible.

## Escenarios de calidad

| ID | Estímulo y ambiente | Respuesta esperada | Atributos |
|---|---|---|---|
| S1 | Un visitante solicita `/`, `/catalogo` o un GET del API desde Internet. | Container Apps responde por HTTPS y obtiene datos de Azure SQL; la escala desde cero o una reanudación serverless pueden añadir latencia sin alterar datos. | Disponibilidad, rendimiento |
| S2 | Un usuario no autenticado intenta POST, PUT o DELETE en el API. | La aplicación responde `401`; un usuario autenticado sin rol recibe `403`. | Seguridad |
| S3 | Un navegador autenticado recibe una solicitud de escritura sin el token antiforgery asociado. | La aplicación rechaza la solicitud antes de modificar datos; el cliente React obtiene el token por el mismo origen. | Seguridad |
| S4 | Azure SQL presenta un error transitorio. | EF Core aplica reintentos limitados y la solicitud falla de forma observable si no se recupera. | Disponibilidad |
| S5 | Falta `ConnectionStrings__DefaultConnection` en producción. | El arranque se detiene con un mensaje de configuración claro y sin mostrar credenciales. | Desplegabilidad, seguridad |
| S6 | Un cambio se propone para la rama final. | GitHub Actions restaura, compila y ejecuta las 25 pruebas .NET y las pruebas del frontend; no despliega si falla la validación. | Testabilidad, desplegabilidad |
| S7 | Se alcanza la cuota mensual gratuita de Azure SQL. | La base se pausa hasta el siguiente mes en lugar de continuar con cargos. | Costo, disponibilidad |
| S8 | Debe añadirse un nuevo filtro del catálogo. | Se extiende la función pura de filtros en TypeScript y se prueba sin cambiar contratos del backend. | Modificabilidad, testabilidad |

## Decisiones arquitectónicas evaluadas

- React + API en un único despliegue (ADR-03 y ADR-09).
- API REST con DTOs y escrituras protegidas por rol (ADR-04).
- Strategy y Factory para lógica reutilizable (ADR-05).
- Cookie y vendedor dueño único (ADR-06).
- Configuración externa y migraciones controladas (ADR-07 y ADR-08).
- Container Apps Consumption, GHCR público y Azure SQL Free Offer para limitar costo (ADR-08).
- Frontend completo React con rutas públicas y administrativas (ADR-09).

## Resultados ATAM

| ID | Tipo | Decisión y evidencia | Atributos afectados | Probabilidad | Impacto | Mitigación |
|---|---|---|---|---|---|---|
| R1 | Riesgo mitigado | Login, registro, logout y las mutaciones de productos y categorías exigen `[ValidateAntiForgeryToken]`; el cliente envía `X-CSRF-TOKEN` y conserva la cookie en el mismo origen. | Seguridad | Baja | Alto | Mantener pruebas de atributos y exigir el mismo patrón a cualquier nueva escritura. |
| T1 | Trade-off | React estático y API comparten proyecto, imagen y revisión de Container Apps. Simplifica desarrollo y entrega, pero impide escalarlos por separado. `CatalogoRopaMVC.csproj`, `Program.cs`, `frontend/`. ADR-03, ADR-08 y ADR-09. | Desplegabilidad, costo, escalabilidad, disponibilidad | No aplica | Medio | Mantener módulos internos y extraer servicios solo cuando tráfico o cambios independientes lo justifiquen. |
| PS1 | Punto de sensibilidad | El valor y disponibilidad de `ConnectionStrings__DefaultConnection` determinan catálogo, API, autenticación y health check porque todos dependen de `ApplicationDbContext`. ADR-01, ADR-07 y ADR-08. | Disponibilidad, desplegabilidad, seguridad | Alta | Alto | Validación al arrancar, configuración externa, conexión cifrada, reintentos y health check de base. |
| R2 | Riesgo arquitectónico | `Database__ApplyMigrations=true` puede ejecutar migraciones durante el arranque. Un cambio largo o incompatible impediría servir tráfico. `Program.cs`, `Migrations/`. ADR-08. | Disponibilidad, integridad, desplegabilidad | Baja con máximo una réplica | Alto | Bandera deshabilitada por defecto; habilitar para una operación controlada, revisar logs y volver a `false`. |
| T2 | Trade-off | Azure App Settings y GitHub Secrets simplifican el despliegue, pero requieren coordinación manual y gobierno de credenciales. ADR-08 y workflow de despliegue. | Seguridad, operabilidad, desplegabilidad | No aplica | Medio | Preferir OIDC, mínimo privilegio, nombres documentados y nunca imprimir valores. |
| R3 | Riesgo arquitectónico | Container Apps escala a cero y la base gratuita puede pausarse; ambas reanudaciones añaden latencia y la demo no tiene SLA. ADR-08. | Disponibilidad, rendimiento, costo | Media | Medio | Calentar la demo antes de evaluar, limitar a una réplica y no añadir perfiles dedicados sin nueva autorización. |
| R4 | Riesgo arquitectónico | Los tipos TypeScript reflejan DTO del backend; una evolución unilateral puede romper formularios o vistas. ADR-04 y ADR-09. | Modificabilidad, consistencia, testabilidad | Media | Medio | Cambiar DTO y tipos en el mismo PR, compilar ambos toolchains y mantener pruebas de integración. |

## Análisis de los hallazgos principales

### Control: CSRF en operaciones REST autenticadas por cookie

La autorización por rol impide escrituras anónimas, pero una cookie se adjunta automáticamente en solicitudes del navegador. Por ello, React obtiene una cookie y un token antiforgery del mismo origen y envía el token en `X-CSRF-TOKEN`. Todos los endpoints de escritura actuales validan ese token; una solicitud sin él se rechaza. Si aparece un cliente desacoplado, debe reevaluarse bearer/OIDC en lugar de reutilizar la cookie.

### Trade-off: monolito modular

Un único despliegue reduce costo, configuración y coordinación; además, permite servir React y API en el mismo origen, sin CORS. A cambio, un fallo de proceso o base afecta todos los canales y no es posible escalar el API independientemente. Para una demo académica de bajo tráfico, la simplicidad domina; si aparecen equipos o cargas distintas, debe reevaluarse ADR-03.

### Punto de sensibilidad: conexión SQL Server

`ApplicationDbContext` soporta consultas del catálogo, escrituras, credenciales del vendedor y endpoints REST. Por eso, un cambio aparentemente pequeño en nombre de variable, firewall, cifrado, credenciales o estado serverless modifica varios atributos a la vez. La conexión se valida temprano y `/health` debe comprobarla sin revelar detalles.

## Conclusiones y recomendaciones

- El diseño es apropiado para una entrega individual y de bajo tráfico, siempre que se reconozcan las cuotas y ausencia de SLA.
- La protección CSRF está implementada; cualquier nuevo endpoint de escritura debe conservar autorización y antiforgery.
- La conexión SQL y las migraciones requieren disciplina operativa porque concentran disponibilidad e integridad.
- Los contratos TypeScript y DTO deben evolucionar juntos y validarse en CI.
- OIDC y los niveles gratuitos reducen exposición y costo; cualquier escalamiento o continuación con cargos requiere una nueva decisión.

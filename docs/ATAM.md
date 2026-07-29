# Evaluación ATAM de CatalogoRopaMVC

## Alcance de la evaluación

Esta evaluación aplica el enfoque Architecture Tradeoff Analysis Method a la versión final de CatalogoRopaMVC: aplicación ASP.NET Core 8 MVC y API REST, Razor Views, autenticación por cookies, Entity Framework Core, Azure App Service, Azure SQL Database y entrega con GitHub Actions.

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
| S1 | Un visitante solicita `/`, `/Productos` o un GET del API desde Internet. | App Service responde por HTTPS y obtiene datos de Azure SQL; una reanudación serverless puede añadir latencia sin alterar datos. | Disponibilidad, rendimiento |
| S2 | Un usuario no autenticado intenta POST, PUT o DELETE en el API. | La aplicación responde `401`; un usuario autenticado sin rol recibe `403`. | Seguridad |
| S3 | Un navegador autenticado recibe una solicitud de escritura originada por otro sitio. | El diseño actual autoriza por cookie, pero no ofrece antiforgery en el API; debe mitigarse antes de ampliar este canal. | Seguridad |
| S4 | Azure SQL presenta un error transitorio. | EF Core aplica reintentos limitados y la solicitud falla de forma observable si no se recupera. | Disponibilidad |
| S5 | Falta `ConnectionStrings__DefaultConnection` en producción. | El arranque se detiene con un mensaje de configuración claro y sin mostrar credenciales. | Desplegabilidad, seguridad |
| S6 | Un cambio se propone para la rama final. | GitHub Actions restaura, compila y ejecuta las 16 pruebas o más; no despliega si falla la validación. | Testabilidad, desplegabilidad |
| S7 | Se alcanza la cuota mensual gratuita de Azure SQL. | La base se pausa hasta el siguiente mes en lugar de continuar con cargos. | Costo, disponibilidad |
| S8 | Debe añadirse un nuevo filtro del catálogo. | Se implementa otra `IFiltroProductoStrategy`, se registra por DI y se prueba de forma aislada. | Modificabilidad, testabilidad |

## Decisiones arquitectónicas evaluadas

- Monolito modular MVC + API en un despliegue (ADR-01 y ADR-03).
- API REST con DTOs y escrituras protegidas por rol (ADR-04).
- Strategy y Factory para lógica reutilizable (ADR-05).
- Cookie y vendedor dueño único (ADR-06).
- Configuración externa y migraciones controladas (ADR-07 y ADR-08).
- App Service F1 y Azure SQL Free Offer para limitar costo (ADR-08).

## Resultados ATAM

| ID | Tipo | Decisión y evidencia | Atributos afectados | Probabilidad | Impacto | Mitigación |
|---|---|---|---|---|---|---|
| R1 | Riesgo arquitectónico | Las escrituras de `ProductosApiController` usan `[Authorize(Roles = "Vendedor")]` con la cookie configurada en `Program.cs`, pero no `[ValidateAntiForgeryToken]`. ADR-04, ADR-06 y ADR-07. | Seguridad | Media | Alto | No ampliar clientes de escritura; adoptar antiforgery para API, bearer tokens o una política de origen probada. |
| T1 | Trade-off | MVC, Razor y API comparten proyecto, proceso y App Service. Simplifica desarrollo y entrega, pero impide escalar o aislar el API por separado. `CatalogoRopaMVC.csproj`, `Program.cs`, `Controllers/`. ADR-03 y ADR-08. | Desplegabilidad, costo, escalabilidad, disponibilidad | No aplica | Medio | Mantener módulos internos y extraer servicios solo cuando tráfico o cambios independientes lo justifiquen. |
| PS1 | Punto de sensibilidad | El valor y disponibilidad de `ConnectionStrings__DefaultConnection` determinan catálogo, API, autenticación y health check porque todos dependen de `ApplicationDbContext`. ADR-01, ADR-07 y ADR-08. | Disponibilidad, desplegabilidad, seguridad | Alta | Alto | Validación al arrancar, configuración externa, conexión cifrada, reintentos y health check de base. |
| R2 | Riesgo arquitectónico | `Database__ApplyMigrations=true` puede ejecutar migraciones durante el arranque. Un cambio largo o incompatible impediría servir tráfico. `Program.cs`, `Migrations/`. ADR-08. | Disponibilidad, integridad, desplegabilidad | Baja en una instancia F1 | Alto | Bandera deshabilitada por defecto; habilitar para una operación controlada, revisar logs y volver a `false`. |
| T2 | Trade-off | Azure App Settings y GitHub Secrets simplifican el despliegue, pero requieren coordinación manual y gobierno de credenciales. ADR-08 y workflow de despliegue. | Seguridad, operabilidad, desplegabilidad | No aplica | Medio | Preferir OIDC, mínimo privilegio, nombres documentados y nunca imprimir valores. |
| R3 | Riesgo arquitectónico | F1 ofrece 60 minutos de CPU por día, no tiene SLA ni `Always On`; la base gratuita puede pausarse. ADR-08. | Disponibilidad, rendimiento, costo | Media | Medio | Reservar la demo, calentar la app antes de evaluar y no escalar a un nivel pagado sin nueva autorización. |
| R4 | Riesgo arquitectónico | Las reglas de escritura y de imagen están duplicadas entre `ProductosController` y `ProductosApiController`. ADR-07. | Modificabilidad, consistencia, testabilidad | Alta | Medio | Pruebas de caracterización y futuro `IProductoCommandService` compartido. |

## Análisis de los hallazgos principales

### Riesgo: CSRF en operaciones REST autenticadas por cookie

La autorización por rol impide escrituras anónimas, pero una cookie se adjunta automáticamente en solicitudes del navegador. Los formularios MVC sí usan antiforgery; los endpoints REST no. La combinación confirma una superficie de riesgo, no una explotación comprobada. La mitigación recomendada depende del consumidor futuro: antiforgery para llamadas desde Razor, o bearer/OIDC para clientes desacoplados.

### Trade-off: monolito modular

Un único despliegue reduce costo, configuración y coordinación; además, permite que Razor, API y servicios compartan modelos y migraciones. A cambio, un fallo de proceso o base afecta todos los canales y no es posible escalar el API independientemente. Para una demo académica de bajo tráfico, la simplicidad domina; si aparecen equipos o cargas distintas, debe reevaluarse ADR-03.

### Punto de sensibilidad: conexión SQL Server

`ApplicationDbContext` soporta consultas del catálogo, escrituras, credenciales del vendedor y endpoints REST. Por eso, un cambio aparentemente pequeño en nombre de variable, firewall, cifrado, credenciales o estado serverless modifica varios atributos a la vez. La conexión se valida temprano y `/health` debe comprobarla sin revelar detalles.

## Conclusiones y recomendaciones

- El diseño es apropiado para una entrega individual y de bajo tráfico, siempre que se reconozcan las cuotas y ausencia de SLA.
- El riesgo de mayor prioridad posterior a la entrega es definir una protección CSRF explícita para escrituras REST.
- La conexión SQL y las migraciones requieren disciplina operativa porque concentran disponibilidad e integridad.
- La duplicación de comandos debe resolverse antes de agregar nuevos canales de escritura.
- OIDC y los niveles gratuitos reducen exposición y costo; cualquier escalamiento o continuación con cargos requiere una nueva decisión.

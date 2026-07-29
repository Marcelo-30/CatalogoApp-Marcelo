# ADR-08: Despliegue de CatalogoRopaMVC en Azure

| Campo | Valor |
|---|---|
| Autor | Marcelo Medina |
| Fecha | 2026-07-29 |
| Estado | Aceptado |

## Contexto

La entrega final requiere una URL pública verificable, persistencia SQL Server administrada y un flujo repetible de integración y despliegue. La aplicación local depende de LocalDB, que no existe en App Service.

## Problema arquitectónico

Se necesita trasladar la aplicación a la nube sin versionar secretos, conservar SQL Server y las migraciones de EF Core, limitar el costo de una demo académica y mantener trazabilidad desde GitHub Actions.

## Decisión

Se eligen Azure App Service para Windows con .NET 8 y Azure SQL Database. Para la demo se priorizan:

- App Service Plan F1, con costo estimado de USD 0, sin `Always On`, sin SLA y con cuota de 60 minutos de CPU por día.
- Azure SQL Database Free Offer, si el portal muestra “Apply offer” y costo mensual estimado de USD 0: 100 000 vCore-segundos, 32 GB de datos y 32 GB de respaldos al mes.
- En la base gratuita se seleccionará **pausar la base hasta el mes siguiente** al agotar la cuota, nunca continuar con cargos adicionales.
- Si la oferta gratuita no está disponible, el aprovisionamiento se detendrá antes de seleccionar una opción pagada no autorizada.

La conexión de producción se suministra como `ConnectionStrings__DefaultConnection`; LocalDB queda solo en `appsettings.Development.json`. Los secretos viven en Azure App Settings o GitHub Secrets. La aplicación expone `/health`, respeta encabezados del proxy de Azure y configura cookies seguras en producción.

Las migraciones se aplican de forma controlada: la aplicación no usa `EnsureCreated`; una bandera externa `Database__ApplyMigrations` habilita `Database.Migrate()` para una operación de despliegue y permanece deshabilitada por defecto. Después de confirmar el esquema, la bandera debe volver a `false`.

El workflow de despliegue usa el environment `production`, espera validaciones exitosas, publica en Release, despliega App Service y ejecuta smoke tests sobre `/health` y `/`. Se prefiere OIDC con una identidad federada; un publish profile solo es alternativa si OIDC no resulta viable y siempre se almacena como secreto.

## Justificación

Azure App Service y Azure SQL conservan la tecnología implementada y eliminan la administración de servidores. Los niveles gratuitos son suficientes para una demostración de bajo tráfico y mantienen intactos el límite de gasto y el crédito promocional. OIDC reduce credenciales persistentes en GitHub.

## Alternativas consideradas

- **Máquina virtual con SQL Server:** descartada por costo, parches y operación.
- **Contenedor en otra plataforma con base no SQL Server:** descartado porque cambiaría silenciosamente el proveedor de datos.
- **App Service B1:** reservado como alternativa pagada; su precio publicado es considerablemente mayor y no se necesita para la demo.
- **Migraciones automáticas incondicionales al arrancar:** descartadas por riesgo de concurrencia, latencia y fallo total del proceso.
- **Publish profile en el repositorio:** descartado por ser una credencial.

## Consecuencias positivas

- URL HTTPS pública en el dominio `azurewebsites.net`.
- Infraestructura administrada y alineada con el proveedor SQL Server ya implementado.
- Configuración por ambiente sin secretos en Git.
- CI/CD auditable desde GitHub Actions.
- Costo esperado de USD 0 mientras se respeten las cuotas gratuitas.

## Consecuencias negativas

- F1 no tiene SLA, `Always On`, dominio personalizado ni capacidad dedicada; puede presentar arranque en frío.
- La base serverless puede pausarse y provocar latencia al reanudarse.
- El proceso de migración requiere una acción operativa controlada.
- MVC y API siguen escalando como una unidad.

## Riesgos y limitaciones

- Activar `Always On` o escalar el plan puede generar cargos; ambas acciones quedan fuera del alcance autorizado.
- Elegir “continuar con cargos” en Azure SQL eliminaría la protección de costo y está prohibido para esta entrega.
- Una conexión incorrecta deja indisponibles catálogo, autenticación y API.
- Los secretos de despliegue o base deben rotarse si se exponen fuera de Azure/GitHub.
- El nivel F1 puede agotar su cuota diaria durante una demostración extensa.

## Evidencia en el código

- `Program.cs`: validación de conexión, SQL Server con reintentos, proxy, cookies, health check y migración condicionada.
- `appsettings.Production.json`: opciones de producción sin secretos.
- `.github/workflows/ci.yml` y `.github/workflows/deploy-azure.yml`.
- `Migrations/`: historial de esquema conservado.
- Variables documentadas en `README.md` y en el índice de `docs`.

## Relación con otros ADR

- Despliega la base definida en ADR-01 y el monolito de ADR-03.
- Conserva el API de ADR-04 y la seguridad de ADR-06.
- Paga la deuda de configuración identificada en ADR-07; la duplicación de comandos continúa pendiente.

# ADR-08: Despliegue de CatalogoRopaMVC en Azure

| Campo | Valor |
|---|---|
| Autor | Marcelo Medina |
| Fecha | 2026-07-29 |
| Estado | Aceptado |

## Contexto

La entrega final requiere una URL pública verificable, persistencia SQL Server administrada y un flujo repetible de integración y despliegue. La aplicación local depende de LocalDB, que no existe en los servicios administrados de Azure.

La primera opción evaluada fue Azure App Service F1. La suscripción gratuita confirmó `Total VMs = 0` tanto para Windows como para Linux y no admite solicitar aumento de cuota sin actualizar la cuenta. Como la actualización, la eliminación del límite de gasto y el pago por uso están prohibidos, App Service no es desplegable en esta suscripción.

Azure SQL también restringió el aprovisionamiento en `East US 2` y `West US 2`. La API de capacidades de la suscripción confirmó `West US 3` como disponible.

## Problema arquitectónico

Se necesita trasladar la aplicación a la nube sin versionar secretos, conservar SQL Server y las migraciones de EF Core, limitar el costo de una demo académica y mantener trazabilidad desde GitHub Actions, sin modificar las protecciones financieras de la suscripción.

## Decisión

Se eligen los siguientes recursos en `West US 3`:

- Azure Container Apps en modo Consumption, 0,25 vCPU, 0,5 GiB de memoria, mínimo 0 y máximo 1 réplica.
- Entorno `cae-catalogoropa-final` sin perfiles dedicados y con destino de logs `none`, para evitar un workspace de Log Analytics.
- Aplicación `catalogoropa-marcelo-final-2026`, con ingreso HTTPS externo y puerto de destino 8080.
- Imagen pública e inmutable en GitHub Container Registry. No se crea Azure Container Registry.
- Azure SQL Database Free Offer: servidor `sql-catalogoropa-marcelo-2026-wus3`, base `CatalogoRopaDB`, 32 GiB, serverless Gen5, respaldo local y agotamiento gratuito `AutoPause`.

La conexión de producción se guarda como secreto `sql-connection` de Container Apps y se proyecta a `ConnectionStrings__DefaultConnection` mediante `secretref`. LocalDB queda solo en `appsettings.Development.json`. La aplicación expone `/health`, respeta encabezados del proxy administrado y configura cookies seguras en producción.

El firewall de Azure SQL permite únicamente las IP de salida confirmadas de Container Apps. No se activa la regla amplia “Allow Azure services”.

Las migraciones se aplican de forma controlada. La bandera externa `Database__ApplyMigrations` se habilita para el primer despliegue, se comprueba el esquema mediante `/health` y después vuelve a `false`.

GitHub Actions valida el frontend con `npm ci`, lint, pruebas y compilación; después restaura, compila y prueba la solución .NET. Publica una imagen etiquetada con el SHA del commit, inicia sesión en Azure mediante OIDC, actualiza la Container App y ejecuta smoke tests. La identidad federada tiene `Container Apps Contributor` limitado a la aplicación.

La imagen usa tres etapas: Node 24 compila React, .NET SDK 8 publica ASP.NET Core con el contenido de `frontend/dist` en `wwwroot`, y ASP.NET Core Runtime 8 ejecuta el artefacto como usuario no privilegiado. Los smoke tests comprueban `/`, `/catalogo`, `/api/productos`, `/health` y la descarga del bundle JavaScript generado.

## Justificación

Container Apps Consumption permite escalar a cero y dispone de una concesión gratuita mensual. GHCR evita el costo fijo de un registro administrado de Azure. Azure SQL Free conserva el proveedor EF Core ya implementado y se pausa cuando agota su cuota gratuita. La arquitectura mantiene intactos el límite de gasto y el crédito promocional.

OIDC evita credenciales de larga duración. La imagen se ejecuta con un usuario no privilegiado, y los secretos permanecen en Azure o GitHub, nunca en Git.

## Alternativas consideradas

- **App Service F1 Windows o Linux:** rechazado por cuota `Total VMs = 0`; la suscripción gratuita no puede solicitar el aumento sin actualizarse.
- **App Service B1 o pago por uso:** fuera del alcance autorizado y sujeto a la misma restricción de cuota.
- **Azure Container Registry Basic:** evitado porque genera un cargo diario; se usa GHCR público.
- **Máquina virtual con SQL Server:** descartada por costo, parches y operación.
- **Base no SQL Server:** descartada porque cambiaría silenciosamente el proveedor de datos.
- **Migraciones automáticas incondicionales:** descartadas por riesgo de concurrencia y fallo total del arranque.

## Consecuencias positivas

- URL HTTPS pública en `azurecontainerapps.io`.
- Escala a cero y límite máximo de una réplica.
- Sin ACR, Log Analytics ni perfiles dedicados.
- Persistencia SQL Server administrada y protegida por firewall.
- CI/CD auditable con imagen vinculada al commit.
- Configuración por ambiente sin secretos en Git.

## Consecuencias negativas

- El primer acceso después de escalar a cero puede tener arranque en frío.
- La base serverless también puede estar pausada y añadir latencia al reanudarse.
- La imagen GHCR debe ser pública para que Azure la descargue sin otra credencial; GitHub no permite volverla privada.
- La demostración no tiene SLA.
- El proceso de migración requiere una acción operativa controlada.

## Riesgos y limitaciones

- La concesión gratuita de Container Apps y la oferta de SQL tienen límites mensuales; superarlos puede reducir disponibilidad.
- Cambiar el comportamiento de agotamiento SQL, añadir perfiles dedicados o elevar réplicas queda fuera del alcance autorizado.
- Una conexión incorrecta deja indisponibles catálogo, autenticación y API.
- Las IP de salida deben reevaluarse si se reemplaza el entorno de Container Apps.
- Los secretos deben rotarse si se exponen fuera de Azure o GitHub.

## Evidencia en el código

- `Dockerfile` y `.dockerignore`: compilación multi-stage Node + .NET, imagen reproducible, puerto 8080 y usuario no privilegiado.
- `Program.cs`: validación de conexión, reintentos SQL, proxy, cookies, health check y migración condicionada.
- `appsettings.Production.json`: opciones de producción sin secretos.
- `.github/workflows/ci.yml` y `.github/workflows/deploy-azure.yml`.
- `Migrations/`: historial de esquema conservado.
- `docs/DESPLIEGUE-AZURE.md`: operación y controles financieros.

## Relación con otros ADR

- Despliega la base definida en ADR-01 y el monolito de ADR-03.
- Conserva el API de ADR-04 y la seguridad de ADR-06.
- Paga la deuda de configuración identificada en ADR-07; la duplicación de comandos continúa pendiente.
- Empaqueta el frontend público definido en ADR-09 sin crear un servicio separado.

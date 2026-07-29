# Operación y despliegue en Azure

## Recursos autorizados para la demo

| Recurso | Configuración |
|---|---|
| Grupo | `rg-catalogoropa-final` |
| Región | `West US 3` |
| Container Apps environment | `cae-catalogoropa-final`, Consumption, sin Log Analytics |
| Container App | `catalogoropa-marcelo-final-2026`, 0,25 vCPU, 0,5 GiB, escala 0–1 |
| Imagen | `ghcr.io/marcelo-30/catalogoropa-marcelo-final-2026`, pública |
| SQL Server | `sql-catalogoropa-marcelo-2026-wus3` |
| SQL Database | `CatalogoRopaDB`, Free Offer, 32 GiB, `AutoPause` |

No se crea App Service, Azure Container Registry, Log Analytics ni un perfil de carga dedicado. La suscripción conserva su límite de gasto y no se actualiza a pago por uso.

Container Apps Consumption escala a cero y aplica su concesión gratuita mensual. Azure SQL debe conservar `useFreeLimit=true` y `freeLimitExhaustionBehavior=AutoPause`; nunca se cambia a continuación con cargos sin autorización.

## Configuración de la aplicación

Los valores viven en secretos o variables de Container Apps; no se guardan en Git.

| Nombre | Uso |
|---|---|
| `ConnectionStrings__DefaultConnection` | Referencia al secreto `sql-connection`; conexión cifrada a Azure SQL. |
| `ASPNETCORE_ENVIRONMENT` | `Production`. |
| `ASPNETCORE_FORWARDEDHEADERS_ENABLED` | Habilita integración con el proxy administrado. |
| `Database__ApplyMigrations` | `false` normalmente; `true` solo durante una migración controlada. |

El secreto `sql-connection` contiene usuario y contraseña SQL generados en Azure. No debe imprimirse, descargarse ni copiarse a un archivo.

## Configuración del environment `production` en GitHub

Variables:

| Nombre | Valor esperado |
|---|---|
| `AZURE_RESOURCE_GROUP` | `rg-catalogoropa-final` |
| `AZURE_CONTAINER_APP_NAME` | `catalogoropa-marcelo-final-2026` |

Secrets:

| Nombre | Uso |
|---|---|
| `AZURE_CLIENT_ID` | Aplicación de Microsoft Entra federada. |
| `AZURE_TENANT_ID` | Directorio de Microsoft Entra. |
| `AZURE_SUBSCRIPTION_ID` | Suscripción que contiene los recursos. |

La credencial federada acepta únicamente el subject:

```text
repo:Marcelo-30/CatalogoApp-Marcelo:environment:production
```

La identidad tiene `Container Apps Contributor` limitado a la Container App. No usa client secret ni publish profile.

## Flujo CI/CD

1. Restaurar, compilar y ejecutar pruebas.
2. Construir la imagen desde `Dockerfile`.
3. Publicar en GHCR las etiquetas `latest` y el SHA completo.
4. Iniciar sesión en Azure con OIDC.
5. Desplegar la etiqueta inmutable del commit.
6. Consultar el FQDN real de Container Apps.
7. Ejecutar smoke tests sobre `/health` y `/`.

El paquete GHCR debe ser público para permitir descarga anónima. GitHub advierte que una imagen pública no puede volver a privada.

## Migraciones controladas

1. Confirmar que `ConnectionStrings__DefaultConnection` referencia `sql-connection`.
2. Confirmar que el firewall SQL contiene solo las IP de salida vigentes de Container Apps.
3. Configurar temporalmente `Database__ApplyMigrations=true`.
4. Desplegar una única revisión.
5. Esperar que `/health` responda `200` y revisar el log en vivo.
6. Configurar `Database__ApplyMigrations=false`.
7. Crear una nueva revisión y repetir el smoke test.

Con máximo una réplica se reduce la posibilidad de migraciones concurrentes. La bandera no debe permanecer activa porque los arranques en frío pueden repetir la comprobación.

## Comprobaciones posteriores

- `GET /health` responde `200`.
- `GET /` carga la página principal.
- `GET /Productos` carga el catálogo.
- `GET /api/productos`, `/api/categorias` y `/api/tallas` responden.
- Una escritura sin sesión de vendedor responde `401`.
- CSS, JavaScript e imágenes cargan por HTTPS.
- GitHub Actions muestra CI, publicación de imagen y despliegue exitosos.
- Azure SQL conserva `useFreeLimit=true` y `AutoPause`.
- Container Apps conserva mínimo 0, máximo 1, 0,25 vCPU y 0,5 GiB.

## Límites operativos

- Escalar a cero causa arranque en frío.
- La base serverless puede pausarse y tardar al reanudarse.
- No hay SLA para esta demo.
- No se deben añadir ACR, Log Analytics, réplicas adicionales, perfiles dedicados o continuidad SQL con cargos sin autorización.
- No se debe cambiar la suscripción, retirar el límite de gasto ni habilitar pago por uso.

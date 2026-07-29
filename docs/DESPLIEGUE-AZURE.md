# Operación y despliegue en Azure

## Recursos autorizados para la demo

- **Azure App Service Plan F1 (Windows):** costo estimado USD 0, 60 minutos de CPU por día, 1 GB de memoria y 1 GB de almacenamiento. No habilitar `Always On`, dominio personalizado ni escalamiento a un nivel pagado.
- **Azure SQL Database Free Offer:** costo estimado USD 0 dentro de 100 000 vCore-segundos, 32 GB de datos y 32 GB de respaldos mensuales.
- Al crear Azure SQL se debe elegir **Auto-pause the database until next month** cuando se alcance el límite gratuito. No elegir la opción que continúa con cargos.

La suscripción conserva su límite de gasto. No se requiere actualizarla ni habilitar pago por uso.

## Configuración requerida

Los valores se configuran en App Service o como secretos del environment `production`; no se guardan en Git.

| Nombre | Obligatorio | Uso |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | Sí | Conexión cifrada a Azure SQL Database. |
| `ASPNETCORE_ENVIRONMENT` | Recomendado | Debe ser `Production` en App Service. |
| `Database__ApplyMigrations` | Sí | `false` normalmente; `true` solo durante una aplicación controlada de migraciones. |

Para OIDC en GitHub Actions se requieren estos secrets o variables protegidos:

| Nombre | Uso |
|---|---|
| `AZURE_CLIENT_ID` | Identificador de la aplicación o identidad federada. |
| `AZURE_TENANT_ID` | Directorio de Microsoft Entra. |
| `AZURE_SUBSCRIPTION_ID` | Suscripción que contiene los recursos. |
| `AZURE_WEBAPP_NAME` | Nombre del App Service destino; puede configurarse como variable del environment. |

Si OIDC no está disponible, la alternativa es `AZURE_WEBAPP_PUBLISH_PROFILE`, cuyo valor debe ser el perfil completo guardado exclusivamente como GitHub Secret. Nunca debe copiarse a un archivo versionado.

## Migraciones controladas

La aplicación conserva las migraciones de Entity Framework Core y no usa `EnsureCreated`.

1. Confirmar que `ConnectionStrings__DefaultConnection` apunta a la base correcta.
2. Configurar temporalmente `Database__ApplyMigrations=true`.
3. Desplegar o reiniciar una sola instancia.
4. Revisar el log `DatabaseMigration` y esperar el mensaje de finalización.
5. Comprobar `/health`, la página principal y los GET del API.
6. Cambiar `Database__ApplyMigrations=false` y reiniciar.

En F1 solo se usa una instancia, lo que reduce la posibilidad de migraciones concurrentes. La bandera no debe permanecer activa porque los arranques en frío son frecuentes.

## Comprobaciones posteriores

- `GET /health` responde `200`.
- `GET /` carga la página principal.
- `GET /Productos` carga el catálogo.
- `GET /api/productos`, `/api/categorias` y `/api/tallas` responden.
- Una escritura sin sesión de vendedor responde `401`.
- CSS, JavaScript e imágenes externas cargan por HTTPS.
- GitHub Actions muestra CI y despliegue exitosos.

## Límites operativos

- F1 no tiene SLA ni `Always On`; puede existir arranque en frío.
- La base serverless puede pausarse y tardar al reanudarse.
- Si el portal no ofrece costo estimado USD 0 para Azure SQL, se debe detener el aprovisionamiento.
- No se debe cambiar la suscripción, retirar el límite de gasto ni habilitar continuidad con cargos sin autorización nueva.

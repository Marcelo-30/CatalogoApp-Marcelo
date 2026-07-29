# Evidencia de entrega final

## Alcance

Evidencia verificada el 29 de julio de 2026 para la rama `release/final-cloud-deployment`. La solicitud de cambios permanece en borrador hacia `main` y no se fusionó.

## Enlaces comprobados

| Evidencia | Enlace |
|---|---|
| Demo pública | [CatalogoRopaMVC en Azure](https://catalogoropa-marcelo-final-2026.wittycliff-20eeb5f5.westus3.azurecontainerapps.io/) |
| Solicitud de cambios | [PR #2](https://github.com/Marcelo-30/CatalogoApp-Marcelo/pull/2) |
| Primer despliegue completo exitoso | [GitHub Actions 30495053748](https://github.com/Marcelo-30/CatalogoApp-Marcelo/actions/runs/30495053748) |
| Imagen pública | [Paquete GHCR](https://github.com/users/Marcelo-30/packages/container/package/catalogoropa-marcelo-final-2026) |

El flujo `Deploy to Azure Container Apps` completó sus tres trabajos: validación de la solución, construcción y publicación de la imagen, y despliegue con smoke tests. La autenticación de Azure se realizó con OIDC.

## Validación técnica

| Comprobación | Resultado |
|---|---|
| Compilación Release | Correcta, 0 advertencias y 0 errores |
| Pruebas automatizadas | 16 correctas, 0 fallidas, 0 omitidas |
| Workflow YAML | Sintaxis válida |
| Contenedor local | Usuario `app`, puerto `8080/tcp` |
| Imagen de entrega | Etiquetas por SHA y `latest` en GHCR |
| `GET /health` | HTTP 200 en smoke test |
| `GET /` | HTTP 200 en smoke test |
| `GET /Productos` | HTTP 200 en verificación independiente |
| `GET /api/productos` | HTTP 200 en verificación independiente |
| `GET /api/categorias` | HTTP 200 en verificación independiente |
| `GET /api/tallas` | HTTP 200 en verificación independiente |
| `POST /api/productos` sin sesión | HTTP 401, escritura rechazada |

El workflow aplica las migraciones solo durante la primera revisión, cambia `Database__ApplyMigrations` a `false` y repite los smoke tests sobre la revisión endurecida.

## Recursos de Azure

| Recurso | Configuración comprobada |
|---|---|
| Grupo de recursos | `rg-catalogoropa-final` |
| Región | `West US 3` |
| Container Apps environment | `cae-catalogoropa-final`, Consumption, sin Log Analytics |
| Container App | `catalogoropa-marcelo-final-2026` |
| Capacidad de aplicación | 0,25 vCPU, 0,5 GiB, mínimo 0 y máximo 1 réplica |
| Ingreso | HTTPS externo, puerto de destino 8080 |
| SQL Server | `sql-catalogoropa-marcelo-2026-wus3` |
| SQL Database | `CatalogoRopaDB`, Free Offer, 32 GiB |
| Agotamiento gratuito SQL | `AutoPause` |
| Firewall SQL | Solo IP de salida confirmada de Container Apps |

La conexión SQL vive en el secreto `sql-connection` de Container Apps y se proyecta mediante `secretref`. Los identificadores OIDC viven en el environment protegido `production` de GitHub. Ningún valor secreto está versionado.

## Controles financieros

- No se actualizó la cuenta.
- No se eliminó el límite de gasto.
- No se habilitó pago por uso.
- No se creó Azure Container Registry.
- No se creó Log Analytics.
- No se añadió un perfil dedicado ni más de una réplica.
- Azure SQL conserva la oferta gratuita y se pausa al agotar su límite.

## Resumen ATAM

- Riesgo principal: escrituras REST protegidas por cookie sin antiforgery específico para API.
- Trade-off principal: MVC y API comparten proceso, imagen y revisión; reduce costo y complejidad, pero no permite escalarlos por separado.
- Punto de sensibilidad: la conexión SQL afecta catálogo, autenticación, API y health check.
- Riesgo operativo: el escalamiento a cero y la pausa de SQL añaden latencia de arranque.
- Mitigaciones: OIDC de mínimo privilegio, secretos externos, health check, reintentos SQL, una sola réplica, migraciones controladas y smoke tests.

## Pendientes operativos

- Mantener el PR sin fusionar hasta la revisión humana.
- Calentar la demo antes de una evaluación por el posible arranque en frío.
- Vigilar las concesiones gratuitas mensuales; no cambiar a continuidad con cargos sin autorización.
- Revisar antiforgery o autenticación bearer antes de ampliar clientes de escritura de la API.

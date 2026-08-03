# Evidencia de entrega final

## Alcance

Evidencia verificada el 2 de agosto de 2026 para la rama `feature/react-frontend`, basada en `release/final-cloud-deployment`. Esta rama reemplaza toda la interfaz Razor por una sola SPA React: catálogo público, acceso del vendedor y administración.

La demo pública y el PR #2 corresponden a la línea base desplegada antes de este cambio. La nueva interfaz debe publicarse mediante una solicitud de cambios separada hacia `release/final-cloud-deployment`; no se considera desplegada hasta que su workflow termine correctamente.

## Validación de la rama React

| Comprobación | Resultado |
|---|---|
| ESLint | Correcto, sin errores |
| Pruebas del frontend | 2 correctas, 0 fallidas |
| Compilación Vite + TypeScript | Correcta, 54 módulos transformados |
| Compilación .NET Release | Correcta, 0 advertencias y 0 errores |
| Pruebas .NET | 25 correctas, 0 fallidas, 0 omitidas |
| Imagen Docker | Construcción multi-stage Node 24 + .NET 8 correcta |
| Contenedor | Usuario `app`, puerto `8080/tcp`, health check correcto |
| Rutas React verificadas | 11 rutas con `index.html` y `#root` |
| Compatibilidad histórica | `/Cuenta/Login`, `/Cuenta/Registro`, `/Productos`, `/Categorias` y `/Home/Index` cargan React y redirigen dentro de la SPA |
| API pública | Productos, categorías, tallas, colores y estado de autenticación responden correctamente |
| Ruta API inexistente | HTTP 404 y nunca devuelve el HTML de React |
| Registro y autenticación | Registro inicial, login, estado de sesión y logout correctos |
| Escritura anónima | HTTP 401 |
| Escritura autenticada sin antiforgery | HTTP 400 |
| CRUD de categorías | Crear, editar y eliminar correctos |
| CRUD de productos | Crear, consultar, editar y eliminar correctos |
| Revisión visual | Login y panel React comprobados; la URL histórica `/Cuenta/Login` termina en `/vendedor/login` |

La validación integral se ejecutó con la imagen de producción y SQL Server temporal. Las cookies de producción se comprobaron con atributo `Secure`; para completar el flujo funcional sobre HTTP local se usó la misma imagen en ambiente de desarrollo, sin modificar la política segura de producción.

## Interfaz unificada

- Ya no existen controladores MVC de interfaz, Razor Views ni los archivos CSS/JavaScript del frontend anterior.
- El enlace **Acceso vendedor** usa navegación de React hacia `/vendedor/login`.
- El vendedor administra resumen, productos, formularios y categorías desde rutas `/admin`.
- ASP.NET Core reserva `/api` y `/health`; cualquier otra ruta de interfaz válida se resuelve mediante el fallback de la SPA.
- Los accesos históricos se conservan únicamente como redirecciones de compatibilidad dentro de React.

## Controles de seguridad

- Login, registro, logout y todas las mutaciones de productos y categorías validan antiforgery.
- Cada escritura administrativa exige además una sesión con rol `Vendedor`.
- Las pruebas de reflexión cubren los atributos de autorización y antiforgery de los endpoints de escritura.
- Las cookies son `HttpOnly`; en producción se emiten como `Secure` y el token antiforgery no se guarda en `localStorage`.
- Las contraseñas se procesan en el servidor con `PasswordHasher` y no se persisten en el navegador.

## Línea base publicada

| Evidencia | Enlace |
|---|---|
| Demo pública actual | [CatalogoRopaMVC en Azure](https://catalogoropa-marcelo-final-2026.wittycliff-20eeb5f5.westus3.azurecontainerapps.io/) |
| PR de la línea base | [PR #2](https://github.com/Marcelo-30/CatalogoApp-Marcelo/pull/2) |
| Primer despliegue completo exitoso | [GitHub Actions 30495053748](https://github.com/Marcelo-30/CatalogoApp-Marcelo/actions/runs/30495053748) |
| Imagen pública de la línea base | [Paquete GHCR](https://github.com/users/Marcelo-30/packages/container/package/catalogoropa-marcelo-final-2026) |

## Recursos y límites de Azure

La nueva rama conserva la topología de la línea base: Azure Container Apps Consumption, Azure SQL Database Free Offer, autenticación OIDC desde GitHub Actions, secretos externos y un máximo de una réplica. No solicita Azure Container Registry, Log Analytics, perfiles dedicados, retiro del límite de gasto ni continuidad con cargos.

## Pendientes operativos

- Publicar `feature/react-frontend` y abrir un PR en borrador hacia `release/final-cloud-deployment`.
- No fusionar hasta completar revisión humana y CI.
- Confirmar el frontend React en la URL de revisión o producción después del despliegue.
- Calentar la demo antes de una evaluación por el posible arranque en frío.
- Mantener las cuotas gratuitas y no habilitar continuidad con cargos sin autorización.

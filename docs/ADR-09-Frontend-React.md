# ADR-09: Frontend completo con React, Vite y TypeScript

| Campo | Valor |
|---|---|
| Autor | Marcelo Medina |
| Fecha | 2026-08-02 |
| Estado | Aceptado |

## Contexto

La interfaz pública original se renderizaba con Razor Views y compartía el mismo lenguaje visual que las pantallas administrativas. El catálogo necesitaba una experiencia más interactiva, responsive y expresiva, sin convertir la solución en una tienda electrónica ni aumentar el riesgo de las escrituras autenticadas.

El backend ya exponía los contratos de lectura necesarios mediante `GET /api/productos`, `GET /api/productos/{id}`, `GET /api/categorias` y `GET /api/tallas`. En una primera etapa se consideró conservar Razor para el vendedor, pero eso producía un salto visible hacia la interfaz anterior y rompía la coherencia del producto.

## Problema

Se necesita una única experiencia visual para visitantes y vendedor, conservar los contratos REST, proteger todas las escrituras con cookie y antiforgery, y continuar desplegando una sola imagen en Azure Container Apps. El frontend debe admitir navegación cliente, filtros, autenticación y CRUD sin convertirse en un servicio desplegable independiente.

## Decisión

Se adopta React 19 con TypeScript, Vite 8 y React Router 7 para todas las rutas de interfaz: públicas, autenticación y administración.

- La SPA se organiza en páginas, componentes reutilizables, hooks, tipos y una capa HTTP central en `frontend/src/api/catalogApi.ts`.
- Todas las llamadas de producción usan rutas relativas `/api`; Vite redirige `/api` a ASP.NET Core durante el desarrollo local.
- El filtrado público ocurre en el navegador sobre las respuestas de lectura existentes y actualiza parámetros de consulta sin recargar la página.
- `AbortController` cancela solicitudes obsoletas. La capa HTTP diferencia fallos de red, respuestas HTTP no exitosas, contenido no JSON y JSON inválido.
- React implementa carga con skeletons, error, catálogo vacío, producto no encontrado e imagen ausente o rota.
- La navegación, el foco, los labels, el contraste y `prefers-reduced-motion` forman parte del sistema visual.
- React implementa `/vendedor/login`, `/vendedor/registro`, `/admin`, `/admin/productos` y `/admin/categorias`, además de los formularios de creación y edición.
- Las URLs históricas de `Cuenta`, `Productos` y `Categorias` redirigen dentro de React. Los controladores MVC, Razor Views y recursos visuales anteriores se retiran.
- `AuthApiController` conserva la cookie y el rol `Vendedor`. El cliente solicita un token antiforgery antes de login, registro, logout y cada mutación; no guarda contraseñas ni tokens en `localStorage`.
- ASP.NET Core sirve los archivos compilados desde `wwwroot`. Solo mapea controladores API por atributos y el fallback excluye explícitamente `/api` y `/health`.
- Docker compila el frontend en una etapa Node, copia `frontend/dist` a `wwwroot` y publica ASP.NET Core en una etapa separada. El artefacto final sigue siendo una sola imagen y se ejecuta como usuario no privilegiado.

## Alternativas consideradas

- **Rediseñar únicamente las Razor Views públicas:** menor cambio tecnológico, pero conserva recargas completas y limita las transiciones y el estado interactivo del catálogo.
- **Desplegar React como aplicación separada:** facilita ciclos independientes, pero introduce CORS, dos despliegues, más configuración y un costo operativo innecesario para este alcance.
- **Conservar la administración Razor:** descartada porque crea dos interfaces y un salto visual; su seguridad se trasladó a APIs con antiforgery verificable.
- **Usar una biblioteca grande de animación:** descartada; las transiciones requeridas se resuelven con CSS e `IntersectionObserver`.
- **Codificar la URL pública de Azure en React:** descartada; rompería portabilidad entre desarrollo, revisión y producción.

## Consecuencias positivas

- Experiencia pública moderna, responsive y navegable sin recargas completas.
- Componentes y contratos TypeScript reutilizables.
- Manejo consistente de carga, errores, vacíos e imágenes fallidas.
- Despliegue sin CORS y sin un segundo servicio.
- Caché eficiente de dependencias y capas de Docker mediante `package-lock.json` y `npm ci`.
- Una sola interfaz cubre catálogo, autenticación y administración.
- Las escrituras REST corrigen el riesgo CSRF documentado mediante antiforgery.

## Consecuencias negativas

- El repositorio incorpora Node, npm y un segundo toolchain de compilación.
- La imagen requiere una etapa adicional y tarda más en construirse.
- El catálogo público depende de JavaScript para su experiencia completa.
- La administración requiere JavaScript; ante una falla del bundle, las APIs siguen protegidas pero no existe una segunda UI de respaldo.

## Riesgos

- Un fallback mal ordenado podría devolver `index.html` a una ruta API; `Program.cs` reserva los prefijos de API y health.
- Una actualización de dependencias puede introducir avisos de seguridad. React Router 7.18.2 corrige los avisos aplicables a enlaces y navegación cliente; el aviso npm restante corresponde a acciones de servidor/RSC, capacidades no incluidas en esta SPA. Debe reevaluarse al actualizar React Router.
- Las imágenes remotas de productos pueden dejar de responder; `ImageWithFallback` mantiene una interfaz utilizable.
- El contrato REST puede evolucionar; los tipos de `frontend/src/types/catalog.ts` deben cambiar junto con los DTO del backend.
- Una omisión de antiforgery en una nueva mutación reabriría el riesgo CSRF; las pruebas inspeccionan los atributos de seguridad en todos los endpoints de escritura actuales.

## Evidencia en el código

- `frontend/src/App.tsx`: rutas públicas, autenticación, administración y compatibilidad histórica.
- `frontend/src/api/httpClient.ts`: cliente HTTP relativo, antiforgery y manejo de errores.
- `frontend/src/components/` y `frontend/src/pages/`: sistema visual y vistas reutilizables.
- `frontend/src/styles/global.css`: responsive, foco, transiciones y movimiento reducido.
- `frontend/src/utils/catalog.test.ts`: cobertura del filtrado cliente.
- `Program.cs` y `CatalogoRopaMVC.csproj`: archivos estáticos, fallback y publicación.
- `Dockerfile`: compilación multi-stage Node + .NET.
- `.github/workflows/ci.yml` y `.github/workflows/deploy-azure.yml`: validación y smoke tests del frontend.

## Relación con otros ADR

- Sustituye la capa de presentación Razor del ADR-01; conserva ASP.NET Core, EF Core y SQL Server.
- Mantiene el monolito desplegable del ADR-03.
- Consume sin modificar los DTO y endpoints públicos del ADR-04.
- Conserva la autenticación y el límite de un vendedor del ADR-06, y añade antiforgery a las escrituras REST.
- Se integra con la imagen y el despliegue de Azure definidos por el ADR-08.

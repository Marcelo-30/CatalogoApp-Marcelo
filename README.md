# CatalogoRopaMVC

Catálogo de ropa completamente desarrollado en React 19, Vite y TypeScript, respaldado por ASP.NET Core 8, API REST, Entity Framework Core y SQL Server. Tanto la experiencia pública como la administración comparten el mismo sistema visual moderno.

## Demo pública

[Abrir CatalogoRopaMVC en Azure](https://catalogoropa-marcelo-final-2026.wittycliff-20eeb5f5.westus3.azurecontainerapps.io/)

El primer acceso puede tardar por el escalamiento a cero de Azure Container Apps y la pausa automática de Azure SQL.

## Funciones principales

- Inicio editorial, catálogo público interactivo y detalle de producto en React.
- Búsqueda, categorías y disponibilidad sin recargar la página.
- Diseño oscuro responsive, navegación móvil, skeletons y estados de error.
- Login, registro inicial, panel y CRUD de productos y categorías en React.
- Administración protegida por cookie, rol `Vendedor` y antiforgery en cada escritura.
- Registro controlado del primer y único vendedor dueño.
- Contraseñas almacenadas con `PasswordHasher`, nunca como texto plano.
- API REST con lecturas públicas y escrituras autorizadas.
- Migraciones de Entity Framework Core aplicadas de forma controlada.
- Health check de aplicación y base de datos en `/health`.

## Arquitectura y nube

React se compila como archivos estáticos y ASP.NET Core lo sirve desde `wwwroot`; no es un servicio separado. La aplicación completa se entrega como una sola imagen no privilegiada y se ejecuta en Azure Container Apps Consumption. Los datos se conservan en Azure SQL Database Free Offer. GitHub Actions valida ambos toolchains, publica la imagen en GHCR, se autentica en Azure mediante OIDC, despliega y ejecuta smoke tests.

- Región: `West US 3`.
- Container Apps: 0,25 vCPU, 0,5 GiB y escala de 0 a 1 réplica.
- Azure SQL: oferta gratuita, 32 GiB y comportamiento de agotamiento `AutoPause`.
- Sin Azure Container Registry, Log Analytics ni perfiles dedicados.
- Sin secretos en el repositorio.
- Sin actualizar la suscripción, retirar el límite de gasto o habilitar pago por uso.

La explicación completa está en:

- [Índice de documentación](docs/README.md).
- [ADR-08: despliegue en la nube](docs/ADR-08-Despliegue-en-la-Nube.md).
- [ADR-09: frontend completo con React](docs/ADR-09-Frontend-React.md).
- [Modelo C4, niveles 1 a 3](docs/C4.md).
- [Evaluación ATAM](docs/ATAM.md).
- [Guía operativa de Azure](docs/DESPLIEGUE-AZURE.md).
- [Evidencia de entrega final](docs/EVIDENCIA-ENTREGA-FINAL.md).

## Ejecución local

Requisitos: Node.js 20.19 o superior, .NET SDK 8 y SQL Server LocalDB.

```bash
cd frontend
npm ci
npm run dev
```

En otra terminal:

```bash
dotnet restore
dotnet ef database update
dotnet run
```

Vite se ejecuta en `http://localhost:5173` y redirige `/api` a ASP.NET Core en `http://localhost:5225`. La configuración local se encuentra en `appsettings.Development.json`. Producción exige `ConnectionStrings__DefaultConnection` desde una fuente externa.

## Pruebas

```bash
cd frontend
npm ci
npm run lint
npm run test
npm run build

cd ..
dotnet build CatalogoRopaMVC.slnx --configuration Release
dotnet test CatalogoRopaMVC.slnx --configuration Release --no-build
```

La entrega compila sin advertencias y supera 2 pruebas del frontend y 25 pruebas .NET.

## API REST

```text
GET /api/productos
GET /api/productos/{id}
GET /api/categorias
GET /api/tallas
GET /api/colores
GET /api/auth/status
GET /api/auth/antiforgery
```

Las operaciones de creación, edición y eliminación requieren una sesión autenticada con rol `Vendedor` y un token antiforgery emitido por el servidor. Las contraseñas nunca se guardan en el navegador.

## Entrega

- Rama base: `release/final-cloud-deployment`.
- Rama del frontend: `feature/react-frontend`.
- Destino de la nueva solicitud de cambios: `release/final-cloud-deployment`.
- Automatización: [Deploy to Azure Container Apps](https://github.com/Marcelo-30/CatalogoApp-Marcelo/actions/workflows/deploy-azure.yml).

## Uso de inteligencia artificial

Se utilizó inteligencia artificial como apoyo para organizar documentación, preparar configuración y revisar la entrega. Las decisiones, cambios y validaciones finales se comprobaron en el contexto de CatalogoRopaMVC.

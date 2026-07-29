# CatalogoRopaMVC

Catálogo de ropa desarrollado con ASP.NET Core 8 MVC, Razor Views, API REST, Entity Framework Core y SQL Server. El catálogo es público y las operaciones administrativas están protegidas para una única cuenta de vendedor.

## Demo pública

[Abrir CatalogoRopaMVC en Azure](https://catalogoropa-marcelo-final-2026.wittycliff-20eeb5f5.westus3.azurecontainerapps.io/)

El primer acceso puede tardar por el escalamiento a cero de Azure Container Apps y la pausa automática de Azure SQL.

## Funciones principales

- Catálogo público de productos, categorías y tallas.
- Administración MVC protegida por cookie y rol `Vendedor`.
- Registro controlado del primer y único vendedor dueño.
- Contraseñas almacenadas con `PasswordHasher`, nunca como texto plano.
- API REST con lecturas públicas y escrituras autorizadas.
- Migraciones de Entity Framework Core aplicadas de forma controlada.
- Health check de aplicación y base de datos en `/health`.

## Arquitectura y nube

La aplicación se entrega como una imagen .NET 8 no privilegiada y se ejecuta en Azure Container Apps Consumption. Los datos se conservan en Azure SQL Database Free Offer. GitHub Actions valida, prueba, publica la imagen en GHCR, se autentica en Azure mediante OIDC, despliega y ejecuta smoke tests.

- Región: `West US 3`.
- Container Apps: 0,25 vCPU, 0,5 GiB y escala de 0 a 1 réplica.
- Azure SQL: oferta gratuita, 32 GiB y comportamiento de agotamiento `AutoPause`.
- Sin Azure Container Registry, Log Analytics ni perfiles dedicados.
- Sin secretos en el repositorio.
- Sin actualizar la suscripción, retirar el límite de gasto o habilitar pago por uso.

La explicación completa está en:

- [Índice de documentación](docs/README.md).
- [ADR-08: despliegue en la nube](docs/ADR-08-Despliegue-en-la-Nube.md).
- [Modelo C4, niveles 1 a 3](docs/C4.md).
- [Evaluación ATAM](docs/ATAM.md).
- [Guía operativa de Azure](docs/DESPLIEGUE-AZURE.md).
- [Evidencia de entrega final](docs/EVIDENCIA-ENTREGA-FINAL.md).

## Ejecución local

Requisitos: .NET SDK 8 y SQL Server LocalDB.

```bash
dotnet restore
dotnet ef database update
dotnet run
```

La configuración local se encuentra en `appsettings.Development.json`. Producción exige `ConnectionStrings__DefaultConnection` desde una fuente externa.

## Pruebas

```bash
dotnet build CatalogoRopaMVC.slnx --configuration Release
dotnet test CatalogoRopaMVC.slnx --configuration Release --no-build
```

La entrega final compila sin advertencias y supera 16 pruebas automatizadas.

## API REST

```text
GET /api/productos
GET /api/productos/{id}
GET /api/categorias
GET /api/tallas
```

Las operaciones de creación, edición y eliminación requieren una sesión autenticada con rol `Vendedor`.

## Entrega

- Rama: `release/final-cloud-deployment`.
- Solicitud de cambios: [PR #2 hacia `main`](https://github.com/Marcelo-30/CatalogoApp-Marcelo/pull/2), mantenida en borrador y sin fusión automática.
- Automatización: [Deploy to Azure Container Apps](https://github.com/Marcelo-30/CatalogoApp-Marcelo/actions/workflows/deploy-azure.yml).

## Uso de inteligencia artificial

Se utilizó inteligencia artificial como apoyo para organizar documentación, preparar configuración y revisar la entrega. Las decisiones, cambios y validaciones finales se comprobaron en el contexto de CatalogoRopaMVC.

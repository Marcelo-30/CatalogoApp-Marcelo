# ADR-03: Registro de deuda técnica de CatalogoRopaMVC

| Campo  | Valor |
|--------|-------|
| Autor  | Marcelo Medina |
| Fecha  | 15/07/2026 |
| Estado | Propuesto |

---

## Contexto

La deuda técnica representa decisiones, limitaciones o descuidos que facilitan el desarrollo actual, pero generan costos futuros de seguridad, despliegue, mantenimiento o evolución. Este registro documenta hallazgos comprobados en el código y la configuración de CatalogoRopaMVC; no implica que las soluciones propuestas ya estén implementadas.

El análisis consideró la aplicación ASP.NET Core MVC y su API REST, el acceso a SQL Server mediante Entity Framework Core, la autenticación por cookies, los servicios registrados por inyección de dependencias, las vistas Razor, las migraciones y la documentación de arquitectura existente.

---

## Deuda técnica 1: conexión a LocalDB definida en la configuración base

### Qué es

La configuración base de la aplicación contiene una cadena de conexión completa y específica de una estación de desarrollo. `appsettings.json` define `DefaultConnection` con la instancia `(localdb)\mssqllocaldb`, el nombre fijo `CatalogoRopaDB`, autenticación integrada y `TrustServerCertificate=True`. `Program.cs` consume esa entrada directamente al registrar `ApplicationDbContext` con el proveedor de SQL Server.

El valor actual no contiene usuario ni contraseña y, por lo tanto, no se documenta como un secreto expuesto. La deuda consiste en que un parámetro de infraestructura dependiente del ambiente vive en el archivo base versionado, que también sería la fuente predeterminada fuera de desarrollo.

### Evidencia en el proyecto

- `appsettings.json`, líneas 2 a 4: contiene `ConnectionStrings:DefaultConnection` con servidor LocalDB, base `CatalogoRopaDB` y opciones propias del entorno local.
- `Program.cs`, líneas 55 a 57: registra `ApplicationDbContext` mediante `UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))`.
- `appsettings.Development.json`: solo redefine niveles de registro; no es el lugar que actualmente contiene la conexión local.
- `Properties/launchSettings.json`, líneas 8 y 17: los perfiles también fijan puertos locales, lo que confirma que la infraestructura disponible está orientada únicamente al desarrollo en una computadora.
- No existen en el repositorio un archivo de configuración para producción, un manifiesto de contenedor, una plantilla de variables de entorno ni instrucciones para suministrar la conexión durante un despliegue.
- El historial muestra que `appsettings.json` conserva esta configuración desde el commit inicial `109bdb1`.

### Por qué existe

Parece haberse originado como una simplificación inicial para ejecutar el proyecto individual rápidamente con SQL Server LocalDB. El uso de `builder.Configuration` indica que la aplicación ya aprovecha el sistema de configuración de ASP.NET Core, pero la separación por ambientes no se completó. No hay evidencia de que se haya decidido usar LocalDB como infraestructura de producción.

### Costo de no pagarla

- Un despliegue en otro equipo, un servidor, Linux o un contenedor no puede asumir que existe la instancia `(localdb)\mssqllocaldb`.
- Al quedar la conexión local en la configuración base, es fácil ejecutar la aplicación contra un destino equivocado o depender de una sustitución manual no documentada.
- Mantener nombre de base, opciones de certificado y servidor en un solo valor versionado aumenta la diferencia entre desarrollo y producción y dificulta reproducir despliegues.
- Aunque hoy no hay credenciales en el archivo, conservar este patrón aumenta la probabilidad de que una contraseña o un endpoint sensible se agregue después al repositorio.
- `TrustServerCertificate=True` puede trasladarse por accidente a un ambiente donde la validación del certificado sí debe exigirse.

### Propuesta de solución

1. Mantener en `appsettings.json` únicamente configuración común y retirar la cadena concreta de LocalDB.
2. Suministrar la conexión de desarrollo desde secretos de usuario de .NET o, si el equipo decide versionar un valor no sensible, desde `appsettings.Development.json`.
3. Definir la conexión de cada despliegue mediante la variable `ConnectionStrings__DefaultConnection` en el proveedor de hosting o gestor de secretos correspondiente.
4. Documentar en `README.md` el nombre de la variable requerida y usar valores ficticios en los ejemplos, nunca credenciales reales.
5. Validar al iniciar que `DefaultConnection` exista y producir un mensaje claro si falta, antes de intentar abrir la base.
6. Configurar opciones de cifrado y confianza del certificado por ambiente, sin heredar `TrustServerCertificate=True` en producción.

### Técnica de refactorización

**Extraer configuración y reemplazar el valor específico del ambiente por configuración externa.** La aplicación puede conservar `GetConnectionString("DefaultConnection")`; el cambio principal consiste en aprovechar correctamente la precedencia de configuración de ASP.NET Core para separar desarrollo y despliegue.

### Prioridad

**Alta.** No bloquea el desarrollo local actual, pero debe resolverse antes de cualquier despliegue compartido o productivo porque afecta portabilidad, repetibilidad y el tratamiento seguro de futuros secretos.

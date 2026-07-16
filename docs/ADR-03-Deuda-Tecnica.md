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

---

## Deuda técnica 2: reglas de escritura de productos duplicadas entre MVC y API

### Qué es

`ProductosController` y `Controllers/Api/ProductosApiController` implementan por separado la creación, actualización y eliminación de productos usando directamente `ApplicationDbContext`. Ambos controladores asignan las mismas propiedades de `Producto`, guardan con Entity Framework Core y contienen una copia del método `ActualizarImagenPrincipal`.

Esto mezcla en los controladores la traducción HTTP con reglas de aplicación sobre persistencia e imágenes. Como el flujo MVC y el flujo API no comparten un servicio de comandos, una corrección aplicada en uno no se propaga al otro.

### Evidencia en el proyecto

- `Controllers/ProductosController.cs`, líneas 14 a 20: el controlador MVC depende tanto de `ApplicationDbContext` como del servicio de consulta `IProductoCatalogoService`.
- `Controllers/ProductosController.cs`, líneas 71 a 90 y 124 a 169: las acciones MVC crean o copian las propiedades del producto, administran la imagen y llaman a `SaveChangesAsync`.
- `Controllers/ProductosController.cs`, líneas 243 a 275: contiene el método privado `ActualizarImagenPrincipal`.
- `Controllers/Api/ProductosApiController.cs`, líneas 15 a 21: el controlador API también depende directamente de `ApplicationDbContext`.
- `Controllers/Api/ProductosApiController.cs`, líneas 59 a 90 y 99 a 126: los endpoints repiten la construcción o asignación de `Nombre`, `Descripcion`, `Precio`, `Stock`, `Disponible`, `CategoriaId`, `TallaId`, `ColorId` e imagen antes de guardar.
- `Controllers/Api/ProductosApiController.cs`, líneas 187 a 219: repite el mismo algoritmo y la misma estructura de `ActualizarImagenPrincipal` que el controlador MVC.
- `Services/IProductoCatalogoService.cs` y `Services/ProductoCatalogoService.cs` abstraen las consultas del catálogo, pero no existe un servicio equivalente para los comandos de producto.
- El historial muestra que el controlador MVC existía antes y que el API se agregó en `5a54890`; por ello, la duplicación parece haberse originado al incorporar un segundo canal de entrada sin extraer primero las reglas compartidas.

### Por qué existe

Probablemente surgió durante el crecimiento incremental del proyecto. La aplicación comenzó con el flujo MVC y después añadió endpoints REST; repetir una implementación pequeña permitió entregar el API con rapidez. Refactorizaciones posteriores sí separaron la consulta del catálogo y la creación de DTOs, pero las operaciones de escritura quedaron dentro de ambos controladores.

### Costo de no pagarla

- Una regla nueva —por ejemplo, validar referencias, normalizar texto o cambiar el comportamiento de la imagen principal— debe implementarse y probarse en dos lugares.
- Los canales ya presentan diferencias: el API valida explícitamente que categoría, talla y color existan, mientras que el MVC delega el fallo a la validación del modelo o a la base de datos.
- La lógica duplicada puede divergir y producir resultados distintos para la misma operación según se use una vista Razor o el API REST.
- Los controladores quedan acoplados a EF Core, por lo que probar reglas de negocio requiere preparar un `DbContext` y detalles HTTP en lugar de probar un componente de aplicación aislado.
- Cada nuevo canal, como una tarea en segundo plano o una importación, tendría que volver a copiar la lógica o depender de un controlador, aumentando las regresiones y el tiempo de mantenimiento.

### Propuesta de solución

1. Definir un contrato de entrada común para las operaciones de producto, independiente de `ProductoUpsertDto` y del modelo enlazado por MVC.
2. Introducir `IProductoCommandService` con operaciones para crear, actualizar y eliminar productos, y registrarlo mediante inyección de dependencias en `Program.cs`.
3. Mover al servicio la carga del producto, la validación de categoría, talla y color, la asignación de propiedades, la actualización de la imagen principal y `SaveChangesAsync`.
4. Hacer que cada controlador se limite a validar su formato de entrada, invocar el servicio y traducir el resultado a una vista, redirección o respuesta HTTP.
5. Extraer `ActualizarImagenPrincipal` como método privado del servicio o como una política específica si el manejo de varias imágenes continúa creciendo.
6. Agregar pruebas del servicio para creación, actualización sin imagen, reemplazo de imagen, referencia inexistente y eliminación; después retirar las implementaciones duplicadas.
7. Aplicar el cambio en pasos pequeños: primero cubrir el comportamiento actual con pruebas, luego migrar un canal y finalmente el otro.

### Técnica de refactorización

**Extraer clase, introducir una interfaz, mover método y eliminar duplicación.** `IProductoCommandService` separaría las reglas de aplicación de los detalles MVC/API y permitiría que ambos controladores dependan de la misma abstracción mediante inyección de dependencias.

### Prioridad

**Alta.** Ya existen dos entradas públicas que modifican las mismas entidades y presentan validaciones diferentes. La probabilidad de divergencia crece con cada cambio funcional, por lo que conviene centralizar el comportamiento antes de ampliar el API o el manejo de imágenes.

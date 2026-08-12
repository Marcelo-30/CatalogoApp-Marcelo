# CatalogoRopaMVC - Mejora Visual Profesional

## Objetivo de la mejora visual

Esta rama mejora la interfaz de CatalogoRopaMVC para que la aplicacion se perciba como un catalogo de ropa moderno, profesional y atractivo. El foco visual pasa de explicar la tecnologia del proyecto a presentar prendas, imagenes, precios y acciones de compra o administracion de forma clara.

## Cambios realizados

- Rediseño de la pagina principal con hero visual de moda.
- Eliminacion de tarjetas informativas tecnicas en la pagina de inicio.
- Navbar mas moderna con marca visual simple.
- Footer profesional.
- Sistema visual renovado en `wwwroot/css/site.css`.
- Catalogo de productos con tarjetas mas grandes, imagen destacada, precio visible y metadatos limpios.
- Fallback visual para productos sin imagen o con URL rota.
- Vistas de crear, editar, detalles y eliminar producto mas ordenadas.
- Botones y formularios con estilos consistentes.
- Diseño responsive para escritorio y celular.
- Conservacion de reglas de rol: clientes ven catalogo, vendedores administran.

## Vistas modificadas

- `Views/Home/Index.cshtml`
- `Views/Shared/_Layout.cshtml`
- `Views/Productos/Index.cshtml`
- `Views/Productos/Create.cshtml`
- `Views/Productos/Edit.cshtml`
- `Views/Productos/Details.cshtml`
- `Views/Productos/Delete.cshtml`
- `Views/Productos/_ProductoForm.cshtml`
- `wwwroot/css/site.css`

## Tecnologias utilizadas

- C#
- ASP.NET Core MVC
- Razor Views
- Entity Framework Core
- SQL Server
- HTML
- CSS responsive
- Git y GitHub

## Como ejecutar el proyecto

```bash
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```

## Como probar la vista

1. Ejecuta el proyecto con `dotnet run`.
2. Abre la URL local indicada por la consola.
3. Entra a la pagina principal y revisa el hero visual.
4. Entra a `Productos` o `Catalogo` para revisar tarjetas, filtros e imagenes.
5. Inicia sesion como vendedor para ver botones de crear, editar y eliminar.
6. Revisa crear, editar, detalles y eliminar producto desde el rol Vendedor.
7. Prueba el sitio en una ventana estrecha o desde herramientas responsive del navegador.

## Clausula de uso de IA

Para la elaboracion y mejora de esta interfaz se utilizo inteligencia artificial como herramienta de apoyo en la organizacion visual, redaccion, analisis de codigo y propuesta de diseno. Las decisiones finales, revision y adaptacion del contenido fueron realizadas por el autor del proyecto.

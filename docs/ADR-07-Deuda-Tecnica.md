# ADR-07: Registro y tratamiento de deuda técnica

| Campo | Valor |
|---|---|
| Autor | Marcelo Medina |
| Fecha | 2026-07-15 |
| Estado | Propuesto |

## Contexto

CatalogoRopaMVC creció por actividades incrementales: base MVC, documentación, servicios, API REST, patrones, seguridad, pruebas y CI. Ese crecimiento entregó funcionalidad, pero dejó decisiones que aumentan el costo futuro de despliegue, mantenimiento y seguridad. Este ADR conserva el registro de deuda creado en la rama `deuda-tecnica`; el README de esa rama no se usa como fuente porque repite ADR-06.

## Problema arquitectónico

La configuración local estaba acoplada a LocalDB y los comandos de productos están duplicados entre MVC y API. Además, el uso de cookies para escrituras REST no cuenta con una mitigación CSRF equivalente a los formularios MVC.

## Decisión

Se mantiene un registro priorizado y verificable de deuda, con criterios de cierre:

1. **Configuración por ambiente — prioridad alta, remediación aceptada en ADR-08.** Retirar LocalDB de la configuración base, usar `ConnectionStrings__DefaultConnection`, validar su presencia en producción y no versionar secretos.
2. **Comandos de productos duplicados — prioridad alta, pendiente.** Introducir en una iteración posterior un servicio de comandos compartido por MVC y API, cubierto por pruebas.
3. **Protección CSRF del API — prioridad alta, pendiente.** Antes de exponer escrituras a clientes web adicionales, adoptar antiforgery explícito, autenticación bearer o una política de origen demostrablemente suficiente.

La preparación cloud paga la primera deuda. Las otras dos no se ocultan ni se amplían durante esta entrega final.

## Justificación

Un registro explícito evita presentar limitaciones conocidas como decisiones definitivas. Priorizar configuración permite desplegar sin alterar comportamiento funcional; posponer el servicio de comandos evita una refactorización extensa durante una entrega enfocada en documentación y nube.

## Alternativas consideradas

- **Corregir todas las deudas en la rama final:** descartado por aumentar el alcance y el riesgo de regresión.
- **No documentarlas:** descartado porque impide planear y evaluar el sistema.
- **Tratar la duplicación como patrón aceptado:** descartado porque MVC y API ya presentan validaciones diferentes.
- **Cambiar inmediatamente el API a JWT:** pospuesto porque requiere emisión, rotación, almacenamiento y pruebas adicionales.

## Consecuencias positivas

- El estado real del sistema queda visible y priorizado.
- Cada deuda tiene evidencia, mitigación y criterio de cierre.
- La entrega cloud puede avanzar sin fingir que la mantenibilidad o seguridad están resueltas por completo.

## Consecuencias negativas

- Persisten reglas duplicadas en dos controladores.
- Las escrituras REST siguen limitadas a un escenario controlado con cookie.
- El equipo debe reservar una iteración futura para cerrar las deudas pendientes.

## Riesgos y limitaciones

- La duplicación puede producir comportamiento distinto entre MVC y API.
- Un navegador con cookie de vendedor podría ser objetivo de CSRF en endpoints de escritura sin antiforgery.
- La remediación de configuración no se considera cerrada hasta validar el arranque real con Azure SQL y sin secretos versionados.

## Evidencia en el código

### Deuda 1: configuración por ambiente

- Estado inicial: `appsettings.json` contenía una cadena LocalDB específica de desarrollo.
- Remediación: `appsettings.Development.json` conserva LocalDB; producción exige `ConnectionStrings__DefaultConnection`.
- `Program.cs` valida la cadena y aplica reintentos del proveedor SQL Server.
- `appsettings.Production.json` no contiene credenciales.

### Deuda 2: comandos duplicados

- `Controllers/ProductosController.cs` crea, actualiza y elimina con `ApplicationDbContext`.
- `Controllers/Api/ProductosApiController.cs` repite asignaciones, persistencia y manejo de imagen.
- No existe `IProductoCommandService`.

### Deuda 3: CSRF del API

- Los POST MVC incluyen `[ValidateAntiForgeryToken]`.
- Los POST, PUT y DELETE de `ProductosApiController` requieren `[Authorize(Roles = "Vendedor")]`, pero no antiforgery.
- La autenticación configurada en `Program.cs` acepta cookie para MVC y API.

## Plan de remediación

1. Validar configuración y migraciones en el despliegue de ADR-08.
2. Añadir pruebas de caracterización para escrituras MVC y API.
3. Extraer `IProductoCommandService` con validación de referencias e imágenes.
4. Elegir y probar el modelo de autenticación/CSRF del API antes de habilitar clientes de escritura adicionales.
5. Actualizar este ADR a **Aceptado** cuando los criterios de cierre estén comprobados.

## Relación con otros ADR

- Registra consecuencias no resueltas de ADR-03 y ADR-04.
- El riesgo CSRF deriva de ADR-06.
- ADR-08 remedia la configuración por ambiente y documenta el riesgo de migraciones.

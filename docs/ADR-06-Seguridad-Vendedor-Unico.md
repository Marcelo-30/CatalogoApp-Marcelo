# ADR-06: Autenticación por cookies y vendedor dueño único

| Campo | Valor |
|---|---|
| Autor | Marcelo Medina |
| Fecha | 2026-06-28 |
| Estado | Aceptado |

## Contexto

El catálogo es público, pero la administración de productos y categorías debe pertenecer a un vendedor dueño real. La versión anterior con selección demostrativa de roles no ofrecía una identidad persistente ni verificaba contraseñas.

## Problema arquitectónico

Se necesitaba autenticar al administrador, evitar contraseñas en texto plano y reforzar que la aplicación registre como máximo una cuenta dueña.

## Decisión

Se usa autenticación de ASP.NET Core por cookies. `VendedorAuthService` registra el primer vendedor y valida credenciales. `PasswordHasher<Vendedor>` genera y verifica el hash. La entidad `Vendedor` conserva correo normalizado y una llave única; los índices únicos de EF Core refuerzan un correo no repetido y una sola `LlaveUnica`. Las acciones administrativas requieren el rol `Vendedor`.

## Justificación

Las cookies se integran directamente con MVC, Razor y `[Authorize]`. El hasher del framework evita diseñar criptografía propia. La validación en servicio y los índices de base de datos cubren tanto la experiencia normal como condiciones de carrera.

## Alternativas consideradas

- **Rol elegido desde el formulario:** sustituido por no representar una autenticación real.
- **Credenciales fijas en configuración:** descartadas por exposición y falta de rotación.
- **ASP.NET Core Identity completo:** pospuesto porque el dominio solo requiere una cuenta dueña y no necesita recuperación, confirmación ni múltiples roles.
- **Proveedor de identidad externo:** pospuesto por costo y complejidad para la demo.

## Consecuencias positivas

- Las contraseñas se almacenan como hash.
- La autorización se expresa declarativamente con el rol `Vendedor`.
- El catálogo y los GET del API siguen accesibles sin cuenta.
- El API responde `401` o `403` en lugar de redirigir a HTML.

## Consecuencias negativas

- No existe recuperación de contraseña, MFA ni administración de sesiones.
- El primer registro es un flujo sensible durante la puesta en marcha.
- La cookie no es una credencial adecuada para consumidores no interactivos del API.

## Riesgos y limitaciones

- Las escrituras del API carecen de antiforgery aunque aceptan cookies; el riesgo CSRF debe mitigarse con antiforgery para API, tokens bearer o una política de origen estricta.
- Una pérdida de credenciales exige intervención directa en datos.
- La unicidad limita deliberadamente el sistema a un solo vendedor.

## Evidencia en el código

- `Models/Vendedor.cs` y la migración `AgregarVendedorUnico`.
- `Services/VendedorAuthService.cs`.
- `Controllers/CuentaController.cs`.
- `Program.cs`: cookie, rutas de login/acceso denegado y respuestas especiales para `/api`.
- `[Authorize(Roles = "Vendedor")]` en controladores y acciones administrativas.

## Relación con otros ADR

- Amplía la aplicación por capas de ADR-03.
- Protege las escrituras introducidas por ADR-04.
- ATAM evalúa el riesgo CSRF y la dependencia compartida de la base de datos.

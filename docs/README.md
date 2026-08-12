# Documentación arquitectónica de CatalogoRopaMVC

Este directorio reúne las decisiones y evaluaciones de la versión final. El nombre oficial del proyecto es **CatalogoRopaMVC**; “CatalogoAPP” se conserva únicamente como antecedente histórico en ADR-02.

## Decisiones arquitectónicas

| ADR | Estado | Decisión |
|---|---|---|
| [ADR-01](ADR-01-Base-MVC.md) | Aceptado | ASP.NET Core MVC, Razor, EF Core y SQL Server como base. |
| [ADR-02](ADR-02-Vistas-Arquitectonicas-CatalogoAPP.md) | Aceptado | Vistas arquitectónicas y modelo C4 como documentación viva. |
| [ADR-03](ADR-03-Estilo-Arquitectonico.md) | Aceptado | Monolito modular cliente-servidor en capas. |
| [ADR-04](ADR-04-API-REST.md) | Aceptado | API REST integrada con DTOs y escrituras protegidas. |
| [ADR-05](ADR-05-Patrones-GOF.md) | Aceptado | Strategy para filtros y Factory para DTOs. |
| [ADR-06](ADR-06-Seguridad-Vendedor-Unico.md) | Aceptado | Cookie, rol Vendedor y dueño único persistente. |
| [ADR-07](ADR-07-Deuda-Tecnica.md) | Propuesto | Registro y plan de remediación de deuda técnica. |
| [ADR-08](ADR-08-Despliegue-en-la-Nube.md) | Aceptado | Container Apps, GHCR y Azure SQL con configuración externa y costo limitado. |
| [ADR-09](ADR-09-Frontend-React.md) | Aceptado | React, Vite y TypeScript para toda la interfaz dentro de la misma imagen. |

## Vistas y evaluaciones

- [Modelo C4, niveles 1 a 3](C4.md).
- [Evaluación ATAM](ATAM.md).
- [Operación y despliegue en Azure](DESPLIEGUE-AZURE.md).
- [Evidencia de entrega final](EVIDENCIA-ENTREGA-FINAL.md).

La evidencia contiene únicamente enlaces y resultados comprobados.

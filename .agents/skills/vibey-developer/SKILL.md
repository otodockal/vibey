---
name: vibey-developer
description: Project-specific conventions for the vibey Nx monorepo (Angular 21 web + .NET 10 microservices). Trigger when adding a microservice, a feature page, a data-access lib, a shared contract/model, or wiring FE↔BE communication. Complements [[angular-developer]] with vibey-specific patterns.
license: MIT
metadata:
  version: '1.0'
---

# Vibey Developer Guidelines

Vibey is an Nx 22 monorepo: Angular 21 web app under [apps/web/](apps/web/) talking to multiple .NET 10 minimal-API microservices under `apps/<name>-service/` via a dev proxy. TS uses the `@vibey/*` scope; C# uses the `Vibey.*` namespace.

For general Angular guidance, defer to [[angular-developer]]. This skill only documents what is **specific to this repo**.

## Workspace map

- One microservice per bounded context, each on its own port (51xx) — see [README.md](README.md).
- One Angular data-access lib per service: [libs/<name>-data-access/](libs/).
- Shared C# DTOs in [libs/dotnet/Contracts/](libs/dotnet/Contracts/), referenced by every service via `ProjectReference`.
- TS mirror of those DTOs in [libs/shared-ui/src/lib/models.ts](libs/shared-ui/src/lib/models.ts) — kept in sync manually.
- Shared FE primitives (`CardComponent`, `PageGridComponent`) in [libs/shared-ui/](libs/shared-ui/).
- Cross-cutting FE helpers in [libs/shared-util/](libs/shared-util/).
- Dev proxy mapping `/api/<name>` → `localhost:51xx` in [apps/web/proxy.conf.json](apps/web/proxy.conf.json).

## When to consult references

- **Adding/modifying a .NET microservice** (endpoints, domain, repository, DI, inter-service HTTP): read [references/backend-service.md](references/backend-service.md).
- **Adding/modifying a FE feature page or data-access lib** (route resolver, refetch, signals + rxMethod, HttpClient wrapper): read [references/frontend-feature.md](references/frontend-feature.md).

## Cross-cutting rules

1. **Shared contract changes are two-step.** Edit the C# record in [libs/dotnet/Contracts/](libs/dotnet/Contracts/) AND its mirror in [libs/shared-ui/src/lib/models.ts](libs/shared-ui/src/lib/models.ts). They are not generated — drift silently breaks the FE.
2. **Never call a backend service from the FE by hostname.** Always use the `/api/<name>` path so the dev proxy and prod gateway can rewrite it.
3. **Never call a backend service from another backend service by hardcoded URL.** Use a typed `HttpClient` whose `BaseAddress` comes from `Services:<Name>` config — see [HttpProductCatalog](apps/orders-service/OrdersService/Infrastructure/HttpProductCatalog.cs) wired in [Program.cs](apps/orders-service/OrdersService/Program.cs).
4. **Use Nx commands, not raw `ng`/`dotnet`.** `pnpm start`, `pnpm build`, `pnpm nx g @nx/angular:lib …`. Adding a service does not require a `project.json` — `@nx/dotnet` infers targets from the `.csproj`.
5. **Build verification.** After BE changes: `pnpm build:services`. After FE changes: `pnpm build:web`.

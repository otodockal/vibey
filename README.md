# vibey

> A vibey monorepo.

A minimal Nx 22 workspace combining an Angular 21 frontend with multiple
.NET 10 microservices, sharing typed contracts and orchestrated through Nx.

## Stack

- Nx 22.7.1 with the official `@nx/dotnet` plugin (auto-infers targets from `.csproj`)
- Angular 21 standalone, signals, lazy-loaded routes
- .NET 10 minimal APIs (`Microsoft.NET.Sdk.Web`)
- pnpm 11 workspaces

## Layout

```
apps/
  web/                                Angular FE (port 4200)
  products-service/
    ProductsService/                  .NET 10 service (port 5101)
  orders-service/
    OrdersService/                    .NET 10 service (port 5102)
libs/
  shared-ui/                          Angular UI + shared TS models
  products-data-access/               Angular HttpClient wrapper
  orders-data-access/                 Angular HttpClient wrapper
  dotnet/
    Contracts/                        Shared C# DTOs (referenced by both services)
Backend.sln                           Opens all three .NET projects in VS/Rider
```

TS imports use the `@vibey/*` scope; C# code uses the
`Vibey.*` namespace.

## Prerequisites

- Node 22+
- pnpm 11+ (`npm i -g pnpm`)
- .NET SDK 10.0.x — https://dotnet.microsoft.com/download

## Install

```bash
pnpm install
dotnet restore Backend.sln
```

## Run everything

```bash
pnpm start
```

Runs `nx run-many -t serve -p web products-service orders-service`. Nx
streams logs from all three processes. Open http://localhost:4200.

The Angular dev server proxies API calls (see `apps/web/proxy.conf.json`):
- `/api/products` -> http://localhost:5101
- `/api/orders`   -> http://localhost:5102

Individually:

```bash
pnpm start:web
pnpm start:products
pnpm start:orders
```

## Build

```bash
pnpm build              # all three
pnpm build:web          # Angular only
pnpm build:services     # both .NET services
pnpm publish:services   # dotnet publish (release artifacts) for both
```

## Project graph

```bash
pnpm graph
```

Shows how Nx sees cross-language dependencies. The Angular side reads:

```
web -> products-data-access -> shared-ui
web -> orders-data-access   -> shared-ui
web -> shared-ui
```

The .NET side (inferred by `@nx/dotnet` from `ProjectReference` elements in
each `.csproj`):

```
products-service -> Contracts
orders-service   -> Contracts
```

A change inside `libs/dotnet/Contracts` invalidates the build cache for both
services automatically — no manual `implicitDependencies` needed.

## Inter-service example

`OrdersService` shows how one service calls another. When you POST an order,
it looks up each line's product price via an `HttpClient` whose `BaseAddress`
comes from `Services:Products` in `appsettings.json`. Override that in any
environment (env var: `Services__Products`) to swap in a real URL — Container
Apps service binding, Kubernetes Service DNS, .NET Aspire service discovery,
or a YARP gateway.

Note: if you have real consistency requirements, reach for a saga, the
outbox pattern, or a message bus — none of which are in this scaffold.

## Shared contracts

`libs/dotnet/Contracts/` holds C# records used as DTOs by every service.
The TypeScript equivalents live in `libs/shared-ui/src/lib/models.ts`. They
are kept manually in sync here for simplicity; in a larger setup, generate
the TS types from each service's OpenAPI document
(http://localhost:5101/openapi/v1.json) with `nswag` or `openapi-typescript`
and emit them into a generated `libs/api-clients/{products,orders}/` library
that the FE consumes instead of `shared-ui`.

## Adding another microservice

1. Create `apps/<name>-service/<Name>Service/<Name>Service.csproj` referencing
   `Contracts.csproj`.
2. Add a `Program.cs` and `Properties/launchSettings.json` with a free port.
3. Add the csproj to `Backend.sln`.
4. Add a route in `apps/web/proxy.conf.json` if the FE talks to it.
5. (Optional) Generate a TypeScript data-access lib:
   `pnpm nx g @nx/angular:lib libs/<name>-data-access --standalone`.

`@nx/dotnet` picks the new service up automatically — no `project.json`
needed unless you want to override defaults.

## Production deployment notes

- Angular -> static hosting (Azure SWA, Cloudflare Pages, S3+CloudFront, nginx).
- Each .NET service -> its own container (`dotnet publish /t:PublishContainer`)
  on Azure Container Apps / AKS / Fargate.
- Route `/api/products/*` and `/api/orders/*` from the FE host or a YARP /
  Azure Front Door gateway to the right service.
- Replace the in-memory stores with EF Core + Postgres/SQL, add health checks,
  add OpenTelemetry. .NET Aspire is the recommended next step for dev-time
  orchestration once you cross ~3 services.

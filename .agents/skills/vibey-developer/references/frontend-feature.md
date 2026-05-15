# Frontend feature patterns (Angular 21)

The web app at [apps/web/](apps/web/) is standalone-only, signal-first, lazy-loaded, OnPush everywhere. For general Angular guidance read [[angular-developer]] — this file documents vibey-specific patterns.

## Stack additions

- [`@ngrx/signals`](https://ngrx.io/guide/signals) — used only for `rxMethod` in pages.
- [`ngxtension`](https://ngxtension.netlify.app/) — used for `injectRouteData`.

## App composition

[app.config.ts](apps/web/src/app/app.config.ts):

```ts
provideRouter(appRoutes, withRouterConfig({ onSameUrlNavigation: 'reload' })),
provideHttpClient(withFetch()),
```

`onSameUrlNavigation: 'reload'` combined with route-level `runGuardsAndResolvers: 'always'` is what makes the [`refetchAll()`](libs/shared-util/src/lib/refetch-all.ts) pattern (below) re-run resolvers when you navigate to the same URL.

## Data-access libs

One library per backend service: `libs/<name>-data-access/`. A data-access lib is a thin `HttpClient` wrapper, nothing more. See [products.service.ts](libs/products-data-access/src/lib/products.service.ts):

```ts
@Injectable({ providedIn: 'root' })
export class ProductsService {
  readonly #http = inject(HttpClient);
  readonly #base = '/api/products';

  list(): Observable<Product[]> {
    return this.#http.get<Product[]>(this.#base);
  }
}
```

Rules:
- Base URL is **always** `/api/<name>` — never a hostname. Dev proxy + prod gateway handle rewriting.
- Models are imported from `@vibey/shared-ui` as `type` imports.
- Use private fields (`#http`, `#base`) and `inject()` — no constructor injection.
- Return `Observable<T>`; no signal/Promise conversions inside the lib.
- Generate with: `pnpm nx g @nx/angular:lib libs/<name>-data-access --standalone`.

## Routes & resolvers

[app.routes.ts](apps/web/src/app/app.routes.ts) — each feature is a single lazy `loadComponent` with a route resolver fetching its initial data:

```ts
{
  path: 'orders',
  runGuardsAndResolvers: 'always',
  resolve: { data: () => inject(OrdersService).list() },
  loadComponent: () => import('./orders/orders.page'),
}
```

The resolver's result is exposed to the page via [`injectRouteData<T>('data')`](https://ngxtension.netlify.app/utilities/injectors/inject-route-data/) (ngxtension), which returns a `Signal<T>`. This is the standard pattern — pages do **not** subscribe in `ngOnInit` or pull data themselves.

## Refetch on mutation — the `refetchAll()` pattern

After a mutation (POST/PUT/DELETE) the page must show fresh data. The pattern (see [orders.page.ts](apps/web/src/app/orders/orders.page.ts)):

```ts
readonly #service = inject(OrdersService);
readonly #refetch = refetchAll();
readonly items = injectRouteData<Order[]>('data');

readonly create = rxMethod<void>(
  pipe(
    switchMap(() =>
      this.#service.create({ ... }).pipe(tap(() => this.#refetch())),
    ),
  ),
);
```

How it works: [`refetchAll`](libs/shared-util/src/lib/refetch-all.ts) calls `router.navigate([])`. Because the app has `onSameUrlNavigation: 'reload'` and the route declares `runGuardsAndResolvers: 'always'`, the resolver re-runs and `injectRouteData` emits the new value. **Do not** keep a local signal copy of the list and `update` it manually — break this pattern and the resolver/signal contract gets out of sync.

## Feature page anatomy

```ts
@Component({
  selector: 'app-orders-page',
  styleUrl: './orders.page.scss',
  template: `
    <ui-page-grid title="Orders">
      <button page-action (click)="create()">Create sample order</button>
      @for (o of items(); track o.id) {
        <ui-card [title]="'Order #' + o.id"> ... </ui-card>
      }
    </ui-page-grid>
  `,
  imports: [CardComponent, PageGridComponent, DecimalPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class OrdersPage { ... }
```

Rules:
- `export default class` — the route's `loadComponent` uses default export.
- `ChangeDetectionStrategy.OnPush` always.
- Inline template; SCSS via `styleUrl`.
- Wrap the page in `<ui-page-grid title="…">`. Action buttons use the `page-action` attribute slot.
- Render cards with `<ui-card [title]="…">`.
- Use `@for` / `@if` control flow, not `*ngFor` / `*ngIf`.
- User actions that trigger HTTP go through `rxMethod` from `@ngrx/signals/rxjs-interop`, ending in `tap(() => this.#refetch())`.

## Shared models

TS interfaces in [libs/shared-ui/src/lib/models.ts](libs/shared-ui/src/lib/models.ts) mirror C# records in [libs/dotnet/Contracts/](libs/dotnet/Contracts/). When changing one, change the other in the same commit.

## Adding a new feature page

1. Add a route to [app.routes.ts](apps/web/src/app/app.routes.ts) with a resolver calling `<X>Service.list()` and a `loadComponent` import.
2. Create `apps/web/src/app/<name>/<name>.page.ts` (+ `.scss`) following the anatomy above.
3. Add the nav link to [app.html](apps/web/src/app/app.html).
4. If the page needs a new BE, see [references/backend-service.md](backend-service.md).

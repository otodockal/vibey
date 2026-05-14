import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { InventoryService } from '@vibey/inventory-data-access';
import { CardComponent, PageGridComponent, StockItem } from '@vibey/shared-ui';
import { refetchAll } from '@vibey/shared-util';
import { injectRouteData } from 'ngxtension/inject-route-data';
import { pipe, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-inventory-page',
  styleUrl: './inventory.page.scss',
  template: `
    <ui-page-grid title="Inventory">
      @for (s of items(); track s.productId) {
        <ui-card [title]="'Product #' + s.productId">
          <dl>
            <div>
              <dt>Available</dt>
              <dd class="num">{{ s.available }}</dd>
            </div>
            <div>
              <dt>Reserved</dt>
              <dd class="num">{{ s.reserved }}</dd>
            </div>
          </dl>
          <button class="card-action" (click)="reserve(s.productId)">
            Reserve 1
          </button>
        </ui-card>
      }
    </ui-page-grid>
  `,
  imports: [CardComponent, PageGridComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class InventoryPage {
  readonly #service = inject(InventoryService);
  readonly #refetch = refetchAll();

  readonly items = injectRouteData<StockItem[]>('data');

  readonly reserve = rxMethod<number>(
    pipe(
      switchMap((productId) =>
        this.#service
          .reserve({ productId, quantity: 1 })
          .pipe(tap(() => this.#refetch())),
      ),
    ),
  );
}

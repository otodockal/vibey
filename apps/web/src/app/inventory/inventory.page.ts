import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { InventoryService } from '@vibey/inventory-data-access';
import { CardComponent, StockItem } from '@vibey/shared-ui';
import { injectRouteData } from 'ngxtension/inject-route-data';
import { refetchAll } from '@vibey/shared-util';

@Component({
  selector: 'app-inventory-page',
  styleUrl: './inventory.page.scss',
  template: `
    <h2>Inventory</h2>
    <div class="grid">
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
          <button (click)="reserve(s.productId)">Reserve 1</button>
        </ui-card>
      }
    </div>
  `,
  imports: [CardComponent],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class InventoryPage {
  readonly #service = inject(InventoryService);
  readonly #refetch = refetchAll();

  readonly items = injectRouteData<StockItem[]>('data');

  protected reserve(productId: number): void {
    this.#service
      .reserve({ productId, quantity: 1 })
      .subscribe(() => this.#refetch());
  }
}

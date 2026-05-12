import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { OrdersService } from '@vibey/orders-data-access';
import { CardComponent, Order } from '@vibey/shared-ui';
import { injectRouteData } from 'ngxtension/inject-route-data';
import { refetchAll } from '@vibey/shared-util';

@Component({
  selector: 'app-orders-page',
  styleUrl: './orders.page.scss',
  template: `
    <div class="page-actions">
      <h2>Orders</h2>
      <button (click)="create()">Create sample order</button>
    </div>
    <div class="grid">
      @for (o of items(); track o.id) {
        <ui-card [title]="'Order #' + o.id">
          <dl>
            <div>
              <dt>Customer</dt>
              <dd>{{ o.customerEmail }}</dd>
            </div>
            <div>
              <dt>Lines</dt>
              <dd>{{ o.lines.length }}</dd>
            </div>
            <div class="total">
              <dt>Total</dt>
              <dd>{{ o.total | number: '1.2-2' }}</dd>
            </div>
          </dl>
          <time>{{ o.createdAt }}</time>
        </ui-card>
      }
    </div>
  `,
  imports: [CardComponent, DecimalPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class OrdersPage {
  readonly #service = inject(OrdersService);
  readonly #refetch = refetchAll();

  readonly items = injectRouteData<Order[]>('data');

  protected create(): void {
    this.#service
      .create({
        customerEmail: 'demo@example.com',
        lines: [
          { productId: 1, quantity: 2 },
          { productId: 2, quantity: 1 },
        ],
      })
      .subscribe(() => this.#refetch());
  }
}

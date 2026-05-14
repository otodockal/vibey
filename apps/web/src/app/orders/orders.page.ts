import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { OrdersService } from '@vibey/orders-data-access';
import { CardComponent, Order, PageGridComponent } from '@vibey/shared-ui';
import { refetchAll } from '@vibey/shared-util';
import { injectRouteData } from 'ngxtension/inject-route-data';
import { pipe, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-orders-page',
  styleUrl: './orders.page.scss',
  template: `
    <ui-page-grid title="Orders">
      <button page-action (click)="create()">Create sample order</button>
      @for (o of items(); track o.id) {
        <ui-card [title]="'Order #' + o.id">
          <dl>
            <div>
              <dt>Customer</dt>
              <dd class="ellipsis">{{ o.customerEmail }}</dd>
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
    </ui-page-grid>
  `,
  imports: [CardComponent, PageGridComponent, DecimalPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class OrdersPage {
  readonly #service = inject(OrdersService);
  readonly #refetch = refetchAll();

  readonly items = injectRouteData<Order[]>('data');

  readonly create = rxMethod<void>(
    pipe(
      switchMap(() =>
        this.#service
          .create({
            customerEmail: 'demo@example.com',
            lines: [
              { productId: 1, quantity: 2 },
              { productId: 2, quantity: 1 },
            ],
          })
          .pipe(tap(() => this.#refetch())),
      ),
    ),
  );
}

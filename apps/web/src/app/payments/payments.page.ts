import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { PaymentsService } from '@vibey/payments-data-access';
import { CardComponent, PageGridComponent, Payment } from '@vibey/shared-ui';
import { refetchAll } from '@vibey/shared-util';
import { injectRouteData } from 'ngxtension/inject-route-data';
import { pipe, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-payments-page',
  styleUrl: './payments.page.scss',
  template: `
    <ui-page-grid title="Payments">
      <button page-action (click)="create()">Charge sample payment</button>
      @for (p of items(); track p.id) {
        <ui-card [title]="'Payment #' + p.id">
          <dl>
            <div>
              <dt>Order</dt>
              <dd>#{{ p.orderId }}</dd>
            </div>
            <div>
              <dt>Status</dt>
              <dd>
                <span class="badge" [attr.data-status]="p.status">{{
                  p.status
                }}</span>
              </dd>
            </div>
            <div class="amount">
              <dt>Amount</dt>
              <dd>{{ p.amount | number: '1.2-2' }}</dd>
            </div>
          </dl>
          <time>{{ p.createdAt }}</time>
        </ui-card>
      }
    </ui-page-grid>
  `,
  imports: [CardComponent, PageGridComponent, DecimalPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class PaymentsPage {
  readonly #service = inject(PaymentsService);
  readonly #refetch = refetchAll();

  readonly items = injectRouteData<Payment[]>('data');

  readonly create = rxMethod<void>(
    pipe(
      switchMap(() =>
        this.#service
          .create({
            orderId: 1,
            amount: Math.round(Math.random() * 5000) / 100,
          })
          .pipe(tap(() => this.#refetch())),
      ),
    ),
  );
}

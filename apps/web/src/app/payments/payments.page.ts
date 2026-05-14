import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { PaymentsService } from '@vibey/payments-data-access';
import { CardComponent, Payment } from '@vibey/shared-ui';
import { injectRouteData } from 'ngxtension/inject-route-data';
import { refetchAll } from '@vibey/shared-util';

@Component({
  selector: 'app-payments-page',
  styleUrl: './payments.page.scss',
  template: `
    <div class="page-actions">
      <h2>Payments</h2>
      <button (click)="create()">Charge sample payment</button>
    </div>
    <div class="grid">
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
    </div>
  `,
  imports: [CardComponent, DecimalPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class PaymentsPage {
  readonly #service = inject(PaymentsService);
  readonly #refetch = refetchAll();

  readonly items = injectRouteData<Payment[]>('data');

  protected create(): void {
    // Even cents capture, odd cents fail — see PaymentService.
    const amount = Math.round(Math.random() * 5000) / 100;
    this.#service
      .create({ orderId: 1, amount })
      .subscribe(() => this.#refetch());
  }
}

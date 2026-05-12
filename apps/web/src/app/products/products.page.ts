import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CardComponent, Product } from '@vibey/shared-ui';
import { injectRouteData } from 'ngxtension/inject-route-data';

@Component({
  selector: 'app-products-page',
  styleUrl: './products.page.scss',
  template: `
    <h2>Products</h2>
    <div class="grid">
      @for (p of items(); track p.id) {
        <ui-card [title]="p.name">
          <dl>
            <div>
              <dt>In stock</dt>
              <dd>{{ p.stock }}</dd>
            </div>
            <div class="price">
              <dt>Price</dt>
              <dd>{{ p.price | number: '1.2-2' }}</dd>
            </div>
          </dl>
        </ui-card>
      }
    </div>
  `,
  imports: [CardComponent, DecimalPipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class ProductsPage {
  readonly items = injectRouteData<Product[]>('data');
}

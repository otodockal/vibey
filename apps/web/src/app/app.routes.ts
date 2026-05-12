import { inject } from '@angular/core';
import { Route } from '@angular/router';
import { OrdersService } from '@vibey/orders-data-access';
import { ProductsService } from '@vibey/products-data-access';

export const appRoutes: Route[] = [
  { path: '', pathMatch: 'full', redirectTo: 'products' },
  {
    path: 'products',
    runGuardsAndResolvers: 'always',
    resolve: {
      data: () => inject(ProductsService).list(),
    },
    loadComponent: () => import('./products/products.page'),
  },
  {
    path: 'orders',
    runGuardsAndResolvers: 'always',
    resolve: {
      data: () => inject(OrdersService).list(),
    },
    loadComponent: () => import('./orders/orders.page'),
  },
];

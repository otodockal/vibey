import { inject } from '@angular/core';
import { Route } from '@angular/router';
import { InventoryService } from '@vibey/inventory-data-access';
import { OrdersService } from '@vibey/orders-data-access';
import { PaymentsService } from '@vibey/payments-data-access';
import { ProductsService } from '@vibey/products-data-access';
import { UsersService } from '@vibey/users-data-access';

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
  {
    path: 'users',
    runGuardsAndResolvers: 'always',
    resolve: {
      data: () => inject(UsersService).list(),
    },
    loadComponent: () => import('./users/users.page'),
  },
  {
    path: 'inventory',
    runGuardsAndResolvers: 'always',
    resolve: {
      data: () => inject(InventoryService).list(),
    },
    loadComponent: () => import('./inventory/inventory.page'),
  },
  {
    path: 'payments',
    runGuardsAndResolvers: 'always',
    resolve: {
      data: () => inject(PaymentsService).list(),
    },
    loadComponent: () => import('./payments/payments.page'),
  },
];

import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import type { CreateOrderRequest, Order } from '@vibey/shared-ui';

@Injectable({ providedIn: 'root' })
export class OrdersService {
  private readonly http = inject(HttpClient);
  // Dev: proxy rewrites /api/orders → http://localhost:5102/api/orders
  private readonly base = '/api/orders';

  list(): Observable<Order[]> {
    return this.http.get<Order[]>(this.base);
  }

  create(req: CreateOrderRequest): Observable<Order> {
    return this.http.post<Order>(this.base, req);
  }
}

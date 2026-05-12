import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import type { Product } from '@vibey/shared-ui';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private readonly http = inject(HttpClient);
  // Dev: Angular proxy rewrites /api/products → http://localhost:5101/api/products
  // Prod: gateway / SWA routing rewrites to the ProductsService URL.
  private readonly base = '/api/products';

  list(): Observable<Product[]> {
    return this.http.get<Product[]>(this.base);
  }
}

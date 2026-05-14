import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import type {
  ReserveRequest,
  ReserveResponse,
  StockItem,
} from '@vibey/shared-ui';

@Injectable({ providedIn: 'root' })
export class InventoryService {
  readonly #http = inject(HttpClient);
  // Dev: proxy rewrites /api/inventory → http://localhost:5104/api/inventory
  readonly #base = '/api/inventory';

  list(): Observable<StockItem[]> {
    return this.#http.get<StockItem[]>(this.#base);
  }

  reserve(req: ReserveRequest): Observable<ReserveResponse> {
    return this.#http.post<ReserveResponse>(`${this.#base}/reserve`, req);
  }
}

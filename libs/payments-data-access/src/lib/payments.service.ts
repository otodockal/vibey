import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import type { CreatePaymentRequest, Payment } from '@vibey/shared-ui';

@Injectable({ providedIn: 'root' })
export class PaymentsService {
  readonly #http = inject(HttpClient);
  // Dev: proxy rewrites /api/payments → http://localhost:5105/api/payments
  readonly #base = '/api/payments';

  list(): Observable<Payment[]> {
    return this.#http.get<Payment[]>(this.#base);
  }

  create(req: CreatePaymentRequest): Observable<Payment> {
    return this.#http.post<Payment>(this.#base, req);
  }
}

import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import type { CreateUserRequest, User } from '@vibey/shared-ui';

@Injectable({ providedIn: 'root' })
export class UsersService {
  readonly #http = inject(HttpClient);
  // Dev: proxy rewrites /api/users → http://localhost:5103/api/users
  readonly #base = '/api/users';

  list(): Observable<User[]> {
    return this.#http.get<User[]>(this.#base);
  }

  create(req: CreateUserRequest): Observable<User> {
    return this.#http.post<User>(this.#base, req);
  }
}

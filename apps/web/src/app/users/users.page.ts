import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { CardComponent, PageGridComponent, User } from '@vibey/shared-ui';
import { refetchAll } from '@vibey/shared-util';
import { UsersService } from '@vibey/users-data-access';
import { injectRouteData } from 'ngxtension/inject-route-data';
import { pipe, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-users-page',
  styleUrl: './users.page.scss',
  template: `
    <ui-page-grid title="Users">
      <button page-action (click)="create()">Create sample user</button>
      @for (u of items(); track u.id) {
        <ui-card [title]="u.name">
          <dl>
            <div>
              <dt>Email</dt>
              <dd class="ellipsis">{{ u.email }}</dd>
            </div>
            <div>
              <dt>Joined</dt>
              <dd>{{ u.createdAt | date: 'mediumDate' }}</dd>
            </div>
          </dl>
        </ui-card>
      }
    </ui-page-grid>
  `,
  imports: [CardComponent, PageGridComponent, DatePipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class UsersPage {
  readonly #service = inject(UsersService);
  readonly #refetch = refetchAll();

  readonly items = injectRouteData<User[]>('data');

  readonly create = rxMethod<void>(
    pipe(
      switchMap(() => {
        const stamp = Date.now();
        return this.#service
          .create({
            email: `user${stamp}@example.com`,
            name: `User ${stamp}`,
          })
          .pipe(tap(() => this.#refetch()));
      }),
    ),
  );
}

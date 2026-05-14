import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { UsersService } from '@vibey/users-data-access';
import { CardComponent, User } from '@vibey/shared-ui';
import { injectRouteData } from 'ngxtension/inject-route-data';
import { refetchAll } from '@vibey/shared-util';

@Component({
  selector: 'app-users-page',
  styleUrl: './users.page.scss',
  template: `
    <div class="page-actions">
      <h2>Users</h2>
      <button (click)="create()">Create sample user</button>
    </div>
    <div class="grid">
      @for (u of items(); track u.id) {
        <ui-card [title]="u.name">
          <dl>
            <div>
              <dt>Email</dt>
              <dd>{{ u.email }}</dd>
            </div>
            <div>
              <dt>Joined</dt>
              <dd>{{ u.createdAt | date: 'mediumDate' }}</dd>
            </div>
          </dl>
        </ui-card>
      }
    </div>
  `,
  imports: [CardComponent, DatePipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export default class UsersPage {
  readonly #service = inject(UsersService);
  readonly #refetch = refetchAll();

  readonly items = injectRouteData<User[]>('data');

  protected create(): void {
    const stamp = Date.now();
    this.#service
      .create({
        email: `user${stamp}@example.com`,
        name: `User ${stamp}`,
      })
      .subscribe(() => this.#refetch());
  }
}

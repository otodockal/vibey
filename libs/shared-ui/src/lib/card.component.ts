import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'ui-card',
  styleUrl: './card.component.scss',
  template: `
    <article class="card">
      <header>
        <h3>{{ title() }}</h3>
      </header>
      <div class="body">
        <ng-content />
      </div>
    </article>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CardComponent {
  title = input.required<string>();
}

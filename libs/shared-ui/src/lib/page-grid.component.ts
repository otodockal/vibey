import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'ui-page-grid',
  styleUrl: './page-grid.component.scss',
  template: `
    <header class="page-actions">
      <h2>{{ title() }}</h2>
      <ng-content select="[page-action]" />
    </header>
    <div class="grid">
      <ng-content />
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PageGridComponent {
  title = input.required<string>();
}

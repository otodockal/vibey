import { inject } from '@angular/core';
import { Router } from '@angular/router';

export function refetchAll() {
  const router = inject(Router);
  return (path?: string[]) => router.navigate(path ?? []);
}

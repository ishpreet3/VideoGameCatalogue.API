import { Injectable, signal } from '@angular/core';

export interface Toast {
  id: number;
  message: string;
  type: 'success' | 'danger';
}

/** App-wide notifications, e.g. confirming a save after navigating away from the form. */
@Injectable({ providedIn: 'root' })
export class ToastService {
  private nextId = 0;
  private readonly toastList = signal<Toast[]>([]);

  readonly toasts = this.toastList.asReadonly();

  success(message: string): void {
    this.show(message, 'success');
  }

  error(message: string): void {
    this.show(message, 'danger');
  }

  dismiss(id: number): void {
    this.toastList.update(toasts => toasts.filter(toast => toast.id !== id));
  }

  private show(message: string, type: Toast['type']): void {
    this.toastList.update(toasts => [...toasts, { id: this.nextId++, message, type }]);
  }
}

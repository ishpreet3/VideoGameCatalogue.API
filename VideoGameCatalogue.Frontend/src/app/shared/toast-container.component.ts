import { Component, inject } from '@angular/core';
import { NgbToast } from '@ng-bootstrap/ng-bootstrap';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast-container',
  imports: [NgbToast],
  template: `
    <div class="toast-container position-fixed top-0 end-0 p-3">
      @for (toast of toastService.toasts(); track toast.id) {
        <ngb-toast
          [class.text-bg-success]="toast.type === 'success'"
          [class.text-bg-danger]="toast.type === 'danger'"
          [delay]="4000"
          (hidden)="toastService.dismiss(toast.id)">
          {{ toast.message }}
        </ngb-toast>
      }
    </div>
  `
})
export class ToastContainerComponent {
  protected readonly toastService = inject(ToastService);
}

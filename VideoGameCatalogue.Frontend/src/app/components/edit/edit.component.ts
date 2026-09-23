import { Component, OnInit, computed, inject, input, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgbAlert } from '@ng-bootstrap/ng-bootstrap';
import { forkJoin, of } from 'rxjs';
import { Lookups } from '../../models/video-game.model';
import { VideoGameService } from '../../services/video-game.service';
import { toErrorMessage } from '../../shared/http-error';
import { ToastService } from '../../shared/toast.service';

/** Not blank: rejects empty and whitespace-only text, matching the API's [Required] rule. */
const requiredText = [Validators.required, Validators.pattern(/\S/)];

// Rules mirror the API's VideoGameRequest validation so most errors are caught before submitting.
function createVideoGameForm(fb: NonNullableFormBuilder) {
  return fb.group({
    title: ['', [...requiredText, Validators.maxLength(200)]],
    developer: ['', [...requiredText, Validators.maxLength(150)]],
    publisher: ['', [...requiredText, Validators.maxLength(150)]],
    platform: ['', Validators.required],
    genre: ['', Validators.required],
    releaseDate: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0)]],
    metacriticScore: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
    description: ['', Validators.maxLength(1000)]
  });
}

type VideoGameField = keyof ReturnType<typeof createVideoGameForm>['controls'];

@Component({
  selector: 'app-edit',
  imports: [ReactiveFormsModule, RouterLink, NgbAlert],
  templateUrl: './edit.component.html'
})
export class EditComponent implements OnInit {
  /** The :id route parameter, bound by withComponentInputBinding(). Undefined when adding a game. */
  readonly id = input<string>();

  private readonly videoGameService = inject(VideoGameService);
  private readonly router = inject(Router);
  private readonly toastService = inject(ToastService);

  protected readonly form = createVideoGameForm(inject(NonNullableFormBuilder));
  protected readonly isEditMode = computed(() => this.id() !== undefined);
  protected readonly lookups = signal<Lookups>({ genres: [], platforms: [] });
  protected readonly isLoading = signal(true);
  protected readonly isSaving = signal(false);
  protected readonly loadError = signal<string | null>(null);
  protected readonly saveError = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.id();
    const game$ = id === undefined ? of(null) : this.videoGameService.getById(Number(id));

    forkJoin({ lookups: this.videoGameService.getLookups(), game: game$ }).subscribe({
      next: ({ lookups, game }) => {
        this.lookups.set(lookups);
        if (game) {
          this.form.patchValue(game);
        }
        this.isLoading.set(false);
      },
      error: error => {
        this.loadError.set(toErrorMessage(error, 'Could not load this game. It may have been deleted.'));
        this.isLoading.set(false);
      }
    });
  }

  protected isInvalid(field: VideoGameField): boolean {
    const control = this.form.controls[field];
    return control.invalid && (control.touched || control.dirty);
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.saveError.set(null);

    const id = this.id();
    const request = this.form.getRawValue();
    const save$ = id === undefined
      ? this.videoGameService.create(request)
      : this.videoGameService.update(Number(id), request);

    save$.subscribe({
      next: game => {
        this.toastService.success(`"${game.title}" was ${id === undefined ? 'added' : 'updated'}.`);
        this.router.navigate(['/games']);
      },
      error: error => {
        this.saveError.set(toErrorMessage(error, 'Failed to save the video game.'));
        this.isSaving.set(false);
      }
    });
  }
}

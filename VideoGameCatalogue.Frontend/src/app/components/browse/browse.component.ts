import { Component, OnInit, TemplateRef, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NgbAlert, NgbModal, NgbProgressbar } from '@ng-bootstrap/ng-bootstrap';
import { VideoGame } from '../../models/video-game.model';
import { VideoGameService } from '../../services/video-game.service';
import { toErrorMessage } from '../../shared/http-error';
import { ToastService } from '../../shared/toast.service';

@Component({
  selector: 'app-browse',
  imports: [RouterLink, CurrencyPipe, DatePipe, NgbAlert, NgbProgressbar],
  templateUrl: './browse.component.html'
})
export class BrowseComponent implements OnInit {
  private readonly videoGameService = inject(VideoGameService);
  private readonly modalService = inject(NgbModal);
  private readonly toastService = inject(ToastService);

  // The API returns games already sorted by title.
  protected readonly games = signal<VideoGame[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly loadError = signal<string | null>(null);
  protected readonly gameToDelete = signal<VideoGame | null>(null);

  ngOnInit(): void {
    this.videoGameService.getAll().subscribe({
      next: games => {
        this.games.set(games);
        this.isLoading.set(false);
      },
      error: error => {
        this.loadError.set(toErrorMessage(error, 'Failed to load video games. Please try again later.'));
        this.isLoading.set(false);
      }
    });
  }

  protected confirmDelete(game: VideoGame, dialog: TemplateRef<unknown>): void {
    this.gameToDelete.set(game);
    this.modalService
      .open(dialog, { ariaLabelledBy: 'delete-dialog-title' })
      .closed.subscribe(() => this.delete(game));
  }

  private delete(game: VideoGame): void {
    this.videoGameService.delete(game.id).subscribe({
      next: () => {
        this.games.update(games => games.filter(g => g.id !== game.id));
        this.toastService.success(`"${game.title}" was deleted.`);
      },
      error: error => this.toastService.error(toErrorMessage(error, `Failed to delete "${game.title}".`))
    });
  }
}

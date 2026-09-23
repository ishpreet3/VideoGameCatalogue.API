import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { NgbConfig, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { BrowseComponent } from './browse.component';
import { VideoGame } from '../../models/video-game.model';
import { ToastService } from '../../shared/toast.service';
import { aVideoGame } from '../../testing/test-data';

describe('BrowseComponent', () => {
  let fixture: ComponentFixture<BrowseComponent>;
  let http: HttpTestingController;
  let element: HTMLElement;

  const zelda = aVideoGame();
  const baldursGate = aVideoGame({ id: 2, title: 'Baldur\'s Gate 3', genre: 'RPG', platform: 'PC' });

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [BrowseComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()]
    });
    TestBed.inject(NgbConfig).animation = false;
    http = TestBed.inject(HttpTestingController);
    fixture = TestBed.createComponent(BrowseComponent);
    element = fixture.nativeElement;
  });

  afterEach(() => {
    TestBed.inject(NgbModal).dismissAll();
    http.verify();
  });

  async function load(games: VideoGame[]): Promise<void> {
    fixture.detectChanges();
    http.expectOne('/api/videogames').flush(games);
    await fixture.whenStable();
  }

  function cardTitles(): string[] {
    return Array.from(element.querySelectorAll('.card-title')).map(title => title.textContent!.trim());
  }

  it('shows a card for each game with its details', async () => {
    await load([baldursGate, zelda]);

    expect(cardTitles()).toEqual(['Baldur\'s Gate 3', 'The Legend of Zelda: Tears of the Kingdom']);
    const zeldaCard = element.querySelector('[aria-label="The Legend of Zelda: Tears of the Kingdom"]')!;
    expect(zeldaCard.textContent).toContain('$69.99');
    expect(zeldaCard.textContent).toContain('May 12, 2023');
    expect(zeldaCard.querySelector('a[href="/games/1/edit"]')).not.toBeNull();
  });

  it('invites the user to add a game when there are none', async () => {
    await load([]);

    expect(element.querySelector('.alert-info')!.textContent).toContain('No video games yet');
  });

  it('shows an error when the games cannot be loaded', async () => {
    fixture.detectChanges();
    http.expectOne('/api/videogames').flush(null, { status: 500, statusText: 'Server Error' });
    await fixture.whenStable();

    expect(element.querySelector('ngb-alert')!.textContent).toContain('Failed to load video games');
    expect(element.querySelector('.alert-info')).toBeNull();
  });

  it('deletes a game after the user confirms', async () => {
    await load([baldursGate, zelda]);

    element.querySelector<HTMLButtonElement>('[aria-label="Baldur\'s Gate 3"] .btn-outline-danger')!.click();
    await fixture.whenStable();
    const dialog = document.querySelector('ngb-modal-window')!;
    expect(dialog.textContent).toContain('Delete Baldur\'s Gate 3?');

    dialog.querySelector<HTMLButtonElement>('.modal-footer .btn-danger')!.click();
    http.expectOne({ method: 'DELETE', url: '/api/videogames/2' }).flush(null);
    await fixture.whenStable();

    expect(cardTitles()).toEqual(['The Legend of Zelda: Tears of the Kingdom']);
    expect(TestBed.inject(ToastService).toasts()[0].message).toContain('was deleted');
  });

  it('does not delete when the user cancels', async () => {
    await load([zelda]);

    element.querySelector<HTMLButtonElement>('.btn-outline-danger')!.click();
    await fixture.whenStable();
    document.querySelector<HTMLButtonElement>('ngb-modal-window .btn-outline-secondary')!.click();
    await fixture.whenStable();

    // No DELETE is sent; http.verify() in afterEach fails on unexpected requests.
    expect(cardTitles()).toEqual(['The Legend of Zelda: Tears of the Kingdom']);
  });
});

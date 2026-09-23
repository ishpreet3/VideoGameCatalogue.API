import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Router, provideRouter } from '@angular/router';
import { EditComponent } from './edit.component';
import { ToastService } from '../../shared/toast.service';
import { aVideoGame, aVideoGameRequest, testLookups } from '../../testing/test-data';

describe('EditComponent', () => {
  let fixture: ComponentFixture<EditComponent>;
  let http: HttpTestingController;
  let element: HTMLElement;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [EditComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()]
    });
    http = TestBed.inject(HttpTestingController);
    fixture = TestBed.createComponent(EditComponent);
    element = fixture.nativeElement;
  });

  afterEach(() => http.verify());

  async function load(id?: string): Promise<void> {
    if (id !== undefined) {
      fixture.componentRef.setInput('id', id);
    }
    fixture.detectChanges();
    http.expectOne('/api/lookups').flush(testLookups);
    if (id !== undefined) {
      http.expectOne(`/api/videogames/${id}`).flush(aVideoGame({ id: Number(id) }));
    }
    await fixture.whenStable();
  }

  function field<T extends HTMLInputElement | HTMLSelectElement>(id: string): T {
    return element.querySelector<T>(`#${id}`)!;
  }

  async function enter(id: string, value: string): Promise<void> {
    const input = field(id);
    input.value = value;
    input.dispatchEvent(new Event(input instanceof HTMLSelectElement ? 'change' : 'input'));
    await fixture.whenStable();
  }

  async function submit(): Promise<void> {
    element.querySelector<HTMLButtonElement>('button[type=submit]')!.click();
    await fixture.whenStable();
  }

  describe('adding a game', () => {
    it('shows an empty form with dropdowns filled from the lookups', async () => {
      await load();

      expect(element.querySelector('h1')!.textContent).toContain('Add Game');
      expect(field('title').value).toBe('');
      const platformOptions = Array.from(field<HTMLSelectElement>('platform').options).map(o => o.value);
      expect(platformOptions).toEqual(['', ...testLookups.platforms]);
    });

    it('flags missing required fields and does not submit', async () => {
      await load();

      await submit();

      // No POST is made; http.verify() in afterEach fails on unexpected requests.
      expect(element.querySelectorAll('.is-invalid').length).toBeGreaterThan(0);
      expect(field('title').classList).toContain('is-invalid');
    });

    it('treats a whitespace-only title as missing', async () => {
      await load();

      await enter('title', '   ');

      expect(field('title').classList).toContain('is-invalid');
    });

    it('posts a valid game, shows a toast and returns to the list', async () => {
      await load();
      const navigate = vi.spyOn(TestBed.inject(Router), 'navigate').mockResolvedValue(true);
      const request = aVideoGameRequest({ title: 'Baldur\'s Gate 3', genre: 'RPG', platform: 'PC' });
      for (const [key, value] of Object.entries(request)) {
        await enter(key, String(value));
      }

      await submit();
      const req = http.expectOne({ method: 'POST', url: '/api/videogames' });
      expect(req.request.body).toEqual(request);
      req.flush(aVideoGame({ id: 2, ...request }));
      await fixture.whenStable();

      expect(navigate).toHaveBeenCalledWith(['/games']);
      expect(TestBed.inject(ToastService).toasts()[0].message).toContain('Baldur\'s Gate 3');
    });

    it('shows the API validation errors when the server rejects the game', async () => {
      await load();
      for (const [key, value] of Object.entries(aVideoGameRequest())) {
        await enter(key, String(value));
      }

      await submit();
      http.expectOne({ method: 'POST', url: '/api/videogames' }).flush(
        { errors: { Title: ['A game with this title already exists.'] } },
        { status: 400, statusText: 'Bad Request' });
      await fixture.whenStable();

      expect(element.querySelector('ngb-alert')!.textContent).toContain('A game with this title already exists.');
    });
  });

  describe('editing a game', () => {
    it('fills the form with the game, including date and dropdowns', async () => {
      await load('1');

      expect(element.querySelector('h1')!.textContent).toContain('Edit Game');
      expect(field('title').value).toBe('The Legend of Zelda: Tears of the Kingdom');
      expect(field('releaseDate').value).toBe('2023-05-12');
      expect(field('genre').value).toBe('Action-Adventure');
      expect(field('platform').value).toBe('Nintendo Switch');
    });

    it('saves changes with a PUT to the game', async () => {
      await load('1');
      vi.spyOn(TestBed.inject(Router), 'navigate').mockResolvedValue(true);

      await enter('title', 'Updated Title');
      await submit();

      const req = http.expectOne({ method: 'PUT', url: '/api/videogames/1' });
      expect(req.request.body.title).toBe('Updated Title');
      req.flush(aVideoGame({ title: 'Updated Title' }));
    });

    it('shows an error instead of the form when the game cannot be loaded', async () => {
      fixture.componentRef.setInput('id', '999');
      fixture.detectChanges();
      http.expectOne('/api/lookups').flush(testLookups);
      http.expectOne('/api/videogames/999').flush(null, { status: 404, statusText: 'Not Found' });
      await fixture.whenStable();

      expect(element.querySelector('ngb-alert')!.textContent).toContain('Could not load this game');
      expect(element.querySelector('form')).toBeNull();
    });
  });
});

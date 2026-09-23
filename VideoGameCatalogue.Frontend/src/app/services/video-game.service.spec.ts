import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { VideoGameService } from './video-game.service';
import { aVideoGame, aVideoGameRequest, testLookups } from '../testing/test-data';

describe('VideoGameService', () => {
  let service: VideoGameService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(VideoGameService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('gets all games', () => {
    const games = [aVideoGame()];
    let result: unknown;

    service.getAll().subscribe(response => (result = response));
    http.expectOne({ method: 'GET', url: '/api/videogames' }).flush(games);

    expect(result).toEqual(games);
  });

  it('gets a game by id', () => {
    service.getById(7).subscribe();

    http.expectOne({ method: 'GET', url: '/api/videogames/7' }).flush(aVideoGame({ id: 7 }));
  });

  it('creates a game with a POST of the request body', () => {
    const request = aVideoGameRequest();

    service.create(request).subscribe();
    const req = http.expectOne({ method: 'POST', url: '/api/videogames' });

    expect(req.request.body).toEqual(request);
    req.flush(aVideoGame());
  });

  it('updates a game with a PUT to its url', () => {
    const request = aVideoGameRequest();

    service.update(1, request).subscribe();
    const req = http.expectOne({ method: 'PUT', url: '/api/videogames/1' });

    expect(req.request.body).toEqual(request);
    req.flush(aVideoGame());
  });

  it('deletes a game', () => {
    service.delete(3).subscribe();

    http.expectOne({ method: 'DELETE', url: '/api/videogames/3' }).flush(null);
  });

  it('gets the lookups', () => {
    let result: unknown;

    service.getLookups().subscribe(response => (result = response));
    http.expectOne({ method: 'GET', url: '/api/lookups' }).flush(testLookups);

    expect(result).toEqual(testLookups);
  });
});

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Lookups, VideoGame, VideoGameRequest } from '../models/video-game.model';

/**
 * Thin wrapper over the video game API. Errors are passed through untouched as
 * HttpErrorResponse so callers can decide how to present them (see toErrorMessage).
 */
@Injectable({ providedIn: 'root' })
export class VideoGameService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/videogames';

  getAll(): Observable<VideoGame[]> {
    return this.http.get<VideoGame[]>(this.baseUrl);
  }

  getById(id: number): Observable<VideoGame> {
    return this.http.get<VideoGame>(`${this.baseUrl}/${id}`);
  }

  create(request: VideoGameRequest): Observable<VideoGame> {
    return this.http.post<VideoGame>(this.baseUrl, request);
  }

  update(id: number, request: VideoGameRequest): Observable<VideoGame> {
    return this.http.put<VideoGame>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getLookups(): Observable<Lookups> {
    return this.http.get<Lookups>('/api/lookups');
  }
}

import { Lookups, VideoGame, VideoGameRequest } from '../models/video-game.model';

export function aVideoGameRequest(overrides: Partial<VideoGameRequest> = {}): VideoGameRequest {
  return {
    title: 'The Legend of Zelda: Tears of the Kingdom',
    developer: 'Nintendo EPD',
    publisher: 'Nintendo',
    releaseDate: '2023-05-12',
    genre: 'Action-Adventure',
    price: 69.99,
    platform: 'Nintendo Switch',
    metacriticScore: 98,
    description: 'An epic adventure.',
    ...overrides
  };
}

export function aVideoGame(overrides: Partial<VideoGame> = {}): VideoGame {
  return {
    id: 1,
    ...aVideoGameRequest(),
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    ...overrides
  };
}

export const testLookups: Lookups = {
  genres: ['Action-Adventure', 'RPG'],
  platforms: ['Nintendo Switch', 'PC']
};

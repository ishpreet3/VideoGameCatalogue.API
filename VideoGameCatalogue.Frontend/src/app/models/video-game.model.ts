/** A video game as returned by the API. */
export interface VideoGame {
  id: number;
  title: string;
  developer: string;
  publisher: string;
  /** Calendar date in ISO format (yyyy-MM-dd). */
  releaseDate: string;
  genre: string;
  price: number;
  platform: string;
  metacriticScore: number;
  description: string;
  createdAt: string;
  updatedAt: string;
}

/** The body sent to create or update a game. The server owns the id and timestamps. */
export type VideoGameRequest = Omit<VideoGame, 'id' | 'createdAt' | 'updatedAt'>;

/** The allowed genre and platform values, served by the API. */
export interface Lookups {
  genres: string[];
  platforms: string[];
}

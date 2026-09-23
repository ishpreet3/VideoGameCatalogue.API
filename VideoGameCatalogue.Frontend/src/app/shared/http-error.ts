import { HttpErrorResponse } from '@angular/common/http';

/** The shape of an ASP.NET Core ValidationProblemDetails response body. */
interface ValidationProblem {
  errors?: Record<string, string[]>;
}

/**
 * Turns an HTTP error into a message suitable for showing to the user.
 * Validation errors from the API are listed; anything else falls back to the given message.
 */
export function toErrorMessage(error: unknown, fallback: string): string {
  if (!(error instanceof HttpErrorResponse)) {
    return fallback;
  }

  if (error.status === 0) {
    return 'Cannot reach the server. Please check that the API is running.';
  }

  const validationErrors = (error.error as ValidationProblem | null)?.errors;
  if (error.status === 400 && validationErrors) {
    return Object.values(validationErrors).flat().join(' ');
  }

  return fallback;
}

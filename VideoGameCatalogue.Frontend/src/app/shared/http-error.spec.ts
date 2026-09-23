import { HttpErrorResponse } from '@angular/common/http';
import { toErrorMessage } from './http-error';

describe('toErrorMessage', () => {
  it('lists the validation errors from a 400 ValidationProblemDetails response', () => {
    const error = new HttpErrorResponse({
      status: 400,
      error: {
        title: 'One or more validation errors occurred.',
        errors: { Title: ['The Title field is required.'], Price: ['Price must be positive.'] }
      }
    });

    expect(toErrorMessage(error, 'fallback')).toBe('The Title field is required. Price must be positive.');
  });

  it('explains when the server cannot be reached', () => {
    const error = new HttpErrorResponse({ status: 0 });

    expect(toErrorMessage(error, 'fallback')).toContain('Cannot reach the server');
  });

  it.each([
    new HttpErrorResponse({ status: 500, error: { title: 'Server error' } }),
    new HttpErrorResponse({ status: 400, error: 'plain text' }),
    new Error('not an HTTP error')
  ])('falls back to the given message for %s', error => {
    expect(toErrorMessage(error, 'fallback')).toBe('fallback');
  });
});

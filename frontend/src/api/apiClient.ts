const API_URL = (import.meta.env.VITE_API_URL ?? 'http://localhost:5124').replace(/\/$/, '');
const TOKEN_KEY = 'subastaya_token';

interface ProblemDetails { detail?: string; title?: string; }

export class ApiError extends Error {
  readonly status: number;

  constructor(status: number, message: string) {
    super(message);
    this.status = status;
  }
}

export const guardarToken = (token: string) => localStorage.setItem(TOKEN_KEY, token);
export const obtenerToken = () => localStorage.getItem(TOKEN_KEY);
export const eliminarToken = () => localStorage.removeItem(TOKEN_KEY);

export async function apiRequest<T>(path: string, init: RequestInit = {}): Promise<T> {
  const headers = new Headers(init.headers);
  const token = obtenerToken();

  if (init.body && !(init.body instanceof FormData)) headers.set('Content-Type', 'application/json');
  if (token) headers.set('Authorization', `Bearer ${token}`);

  const response = await fetch(`${API_URL}${path}`, { ...init, headers });

  if (!response.ok) {
    let message = `Error ${response.status}`;
    try {
      const problem = await response.json() as ProblemDetails;
      message = problem.detail || problem.title || message;
    } catch { /* respuesta sin JSON */ }
    throw new ApiError(response.status, message);
  }

  if (response.status === 204) return undefined as T;
  return await response.json() as T;
}

import { apiRequest } from './apiClient';
import type {
  MisPublicacionesResponse,
  MisPujasResponse,
} from '../types/activity';

export const listarMisPujas = (signal?: AbortSignal) =>
  apiRequest<MisPujasResponse>('/api/v1/usuarios/me/pujas', { signal });

export const listarMisPublicaciones = (signal?: AbortSignal) =>
  apiRequest<MisPublicacionesResponse>('/api/v1/usuarios/me/subastas', { signal });

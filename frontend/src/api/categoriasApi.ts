import { apiRequest } from './apiClient';
import type { Categoria } from '../types/auction';

export const listarCategorias = (signal?: AbortSignal) =>
  apiRequest<Categoria[]>('/api/v1/categorias', { signal });

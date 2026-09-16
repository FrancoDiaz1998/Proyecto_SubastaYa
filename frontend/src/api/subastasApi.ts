import { apiRequest } from './apiClient';
import type { FiltrosSubasta, ListarSubastasResponse, SubastaDetalle } from '../types/auction';

export function listarSubastas(filtros: FiltrosSubasta, signal?: AbortSignal) {
  const params = new URLSearchParams({ pagina: '1', tamanoPagina: '100', orden: filtros.orden });

  if (filtros.busqueda.trim()) params.set('busqueda', filtros.busqueda.trim());
  if (filtros.estado !== 'todas') params.set('estado', filtros.estado);
  if (filtros.categoriaId !== 'todas') params.set('categoriaId', String(filtros.categoriaId));
  if (filtros.precioMin !== '') params.set('precioMin', String(filtros.precioMin));
  if (filtros.precioMax !== '') params.set('precioMax', String(filtros.precioMax));

  return apiRequest<ListarSubastasResponse>(`/api/v1/subastas?${params}`, { signal });
}

export const obtenerSubasta = (id: number, signal?: AbortSignal) =>
  apiRequest<SubastaDetalle>(`/api/v1/subastas/${id}`, { signal });

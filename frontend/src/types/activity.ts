import type { EstadoSubasta } from './auction';

export type EstadoParticipacion =
  | 'Liderando'
  | 'Superado'
  | 'Ganada'
  | 'NoGanada'
  | 'Desierta'
  | 'Programada';

export interface MisPujaItem {
  subastaId: number;
  titulo: string;
  urlImagen: string;
  categoriaNombre: string;
  estadoSubasta: EstadoSubasta;
  estadoParticipacion: EstadoParticipacion;
  miMayorPuja: number;
  pujaActual: number;
  soyLider: boolean;
  gane: boolean;
  cantidadPujas: number;
  fechaFin: string;
}

export interface MisPujasResponse {
  items: MisPujaItem[];
  participaciones: number;
  activas: number;
  ganadas: number;
}

export type EstadoAdjudicacion =
  | 'Programada'
  | 'EnCurso'
  | 'Adjudicada'
  | 'SinAdjudicar';

export interface MisPublicacionItem {
  subastaId: number;
  titulo: string;
  urlImagen: string;
  categoriaNombre: string;
  estadoSubasta: EstadoSubasta;
  estadoAdjudicacion: EstadoAdjudicacion;
  precioActual: number;
  recaudacion: number;
  cantidadPujas: number;
  fechaInicio: string;
  fechaFin: string;
}

export interface MisPublicacionesResponse {
  items: MisPublicacionItem[];
  totalPublicaciones: number;
  activas: number;
  finalizadas: number;
  recaudacionTotal: number;
}

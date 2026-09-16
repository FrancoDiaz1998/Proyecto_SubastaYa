export type EstadoSubasta = 'Activa' | 'Programada' | 'Finalizada' | 'Desierta';

export interface Categoria {
  id: number;
  nombre: string;
  urlIcono?: string | null;
}

export interface Puja {
  id: number;
  subastaId: number;
  postorNombre: string;
  monto: number;
  fecha: string;
}

export interface SubastaResumen {
  id: number;
  categoriaId: number;
  categoriaNombre: string;
  titulo: string;
  urlImagen: string;
  pujaActual: number;
  cantidadPujas: number;
  fechaInicio: string;
  fechaFin: string;
  estado: EstadoSubasta;
}

export interface SubastaDetalle extends SubastaResumen {
  vendedorId?: string;
  vendedorNombre?: string | null;
  descripcion: string;
  precioBase: number;
  incrementoMinimo: number;
  postorLiderId?: string | null;
  postorLider?: string | null;
  historialPujas: Puja[];
}

export interface CrearSubastaRequest {
  titulo: string;
  descripcion: string;
  urlImagen: string;
  categoriaId: number;
  precioBase: number;
  incrementoMinimo: number;
  fechaInicio: string;
  fechaFin: string;
}

export interface CrearSubastaResponse {
  id: number;
  vendedorId: string;
  categoriaId: number;
  titulo: string;
  descripcion: string;
  urlImagen: string;
  precioBase: number;
  incrementoMinimo: number;
  fechaInicio: string;
  fechaFin: string;
  estado: EstadoSubasta;
}

export interface RegistrarPujaRequest {
  monto: number;
}

export interface RegistrarPujaResponse {
  id: number;
  subastaId: number;
  monto: number;
  fecha: string;
  fechaFin: string;
  extendidaPorAntiSniping: boolean;
}

export type TipoOrden = 'tiempo_asc' | 'puja_desc';

export interface FiltrosSubasta {
  busqueda: string;
  estado: 'todas' | EstadoSubasta;
  categoriaId: number | 'todas';
  precioMin: number | '';
  precioMax: number | '';
  orden: TipoOrden;
}

export interface ListarSubastasResponse {
  items: SubastaResumen[];
  pagina: number;
  tamanoPagina: number;
  totalItems: number;
  totalPaginas: number;
}

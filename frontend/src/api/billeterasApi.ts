import { apiRequest } from './apiClient';
import type {
  AcreditarSaldoRequest,
  AcreditarSaldoResponse,
  MovimientoBilletera,
  SaldoBilletera,
} from '../types/wallet';

export const obtenerSaldoBilletera = (signal?: AbortSignal) =>
  apiRequest<SaldoBilletera>('/api/v1/billeteras/saldos', { signal });

export const listarMovimientosBilletera = (signal?: AbortSignal) =>
  apiRequest<MovimientoBilletera[]>('/api/v1/billeteras/movimientos', { signal });

export const acreditarSaldo = (request: AcreditarSaldoRequest) =>
  apiRequest<AcreditarSaldoResponse>('/api/v1/billeteras/depositos', {
    method: 'POST',
    body: JSON.stringify(request),
  });

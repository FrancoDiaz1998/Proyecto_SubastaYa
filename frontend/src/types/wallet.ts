export type TipoMovimientoBilletera = 'Deposito' | 'Retencion' | 'Liberacion' | 'Pago' | 'Cobro';

export interface SaldoBilletera {
  id: number;
  saldoTotal: number;
  saldoRetenido: number;
  saldoDisponible: number;
  version: number;
}

export interface MovimientoBilletera {
  id: number;
  billeteraId: number;
  subastaId?: number | null;
  tipo: TipoMovimientoBilletera;
  monto: number;
  operacionId: string;
  fecha: string;
}

export interface AcreditarSaldoRequest {
  monto: number;
}

export interface AcreditarSaldoResponse {
  operacionId: string;
  monto: number;
  fecha: string;
  saldo: SaldoBilletera;
}

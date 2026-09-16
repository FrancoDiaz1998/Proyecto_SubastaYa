import { useEffect, useMemo, useState } from 'react';
import {
  ArrowDownToLine,
  ArrowUpFromLine,
  BadgeDollarSign,
  CircleDollarSign,
  Clock3,
  Gavel,
  History,
  LoaderCircle,
  LockKeyhole,
  RefreshCcw,
  ShieldCheck,
  Wallet,
  X,
} from 'lucide-react';
import { acreditarSaldo, listarMovimientosBilletera } from '../api/billeterasApi';
import { ApiError } from '../api/apiClient';
import type { MovimientoBilletera, SaldoBilletera, TipoMovimientoBilletera } from '../types/wallet';
import { formatCurrency, formatDate } from '../utils/formatters';

interface BilleteraModalProps {
  abierto: boolean;
  saldo: SaldoBilletera | null;
  cargandoSaldo: boolean;
  onClose: () => void;
  onSaldoActualizado: (saldo: SaldoBilletera) => void;
}

const configuracionMovimiento: Record<TipoMovimientoBilletera, {
  titulo: string;
  descripcion: string;
  clases: string;
  signo: string;
}> = {
  Deposito: {
    titulo: 'Acreditación', descripcion: 'Ingreso de saldo simulado',
    clases: 'bg-emerald-50 text-emerald-700 border-emerald-200', signo: '+',
  },
  Retencion: {
    titulo: 'Retención', descripcion: 'Saldo congelado por una puja',
    clases: 'bg-amber-50 text-amber-700 border-amber-200', signo: '',
  },
  Liberacion: {
    titulo: 'Liberación', descripcion: 'Saldo liberado al ser superado',
    clases: 'bg-blue-50 text-blue-700 border-blue-200', signo: '',
  },
  Pago: {
    titulo: 'Pago', descripcion: 'Débito por una subasta ganada',
    clases: 'bg-red-50 text-red-700 border-red-200', signo: '-',
  },
  Cobro: {
    titulo: 'Cobro', descripcion: 'Crédito por una venta finalizada',
    clases: 'bg-violet-50 text-violet-700 border-violet-200', signo: '+',
  },
};

export function BilleteraModal({
  abierto, saldo, cargandoSaldo, onClose, onSaldoActualizado,
}: BilleteraModalProps) {
  const [movimientos, setMovimientos] = useState<MovimientoBilletera[]>([]);
  const [cargandoMovimientos, setCargandoMovimientos] = useState(false);
  const [monto, setMonto] = useState<number | ''>('');
  const [depositando, setDepositando] = useState(false);
  const [mensaje, setMensaje] = useState<{ tipo: 'success' | 'error'; texto: string } | null>(null);

  const cargarMovimientos = async (signal?: AbortSignal) => {
    setCargandoMovimientos(true);
    try {
      setMovimientos(await listarMovimientosBilletera(signal));
    } catch (error) {
      if (!(error instanceof DOMException && error.name === 'AbortError')) {
        setMensaje({
          tipo: 'error',
          texto: error instanceof ApiError ? error.message : 'No se pudo cargar el historial.',
        });
      }
    } finally {
      if (!signal?.aborted) setCargandoMovimientos(false);
    }
  };

  useEffect(() => {
    if (!abierto) return;
    const controller = new AbortController();
    setMensaje(null);
    cargarMovimientos(controller.signal);
    return () => controller.abort();
  }, [abierto]);

  const porcentajeRetenido = useMemo(() => {
    if (!saldo || saldo.saldoTotal <= 0) return 0;
    return Math.min(100, Math.round((saldo.saldoRetenido / saldo.saldoTotal) * 100));
  }, [saldo]);

  if (!abierto) return null;

  const handleDepositar = async () => {
    if (monto === '' || monto <= 0) {
      setMensaje({ tipo: 'error', texto: 'Ingresá un monto mayor a cero.' });
      return;
    }

    setDepositando(true);
    setMensaje(null);
    try {
      const respuesta = await acreditarSaldo({ monto });
      onSaldoActualizado(respuesta.saldo);
      setMonto('');
      setMensaje({
        tipo: 'success',
        texto: `${formatCurrency(respuesta.monto)} acreditados correctamente.`,
      });
      await cargarMovimientos();
    } catch (error) {
      setMensaje({
        tipo: 'error',
        texto: error instanceof ApiError ? error.message : 'No se pudo acreditar el saldo.',
      });
    } finally {
      setDepositando(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto bg-black/60 backdrop-blur-xs flex items-center justify-center p-3 sm:p-6" onClick={onClose}>
      <div className="bg-white w-full max-w-5xl rounded-3xl shadow-2xl border border-slate-200 overflow-hidden max-h-[92vh] flex flex-col" onClick={(event) => event.stopPropagation()}>
        <div className="px-5 sm:px-6 py-4 border-b border-slate-100 bg-slate-50/80 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-xl bg-blue-600 text-white flex items-center justify-center"><Wallet className="w-5 h-5" /></div>
            <div><h2 className="font-black text-slate-900">Billetera Virtual</h2><p className="text-[11px] text-slate-500">Consultá tu saldo y tus movimientos</p></div>
          </div>
          <button type="button" onClick={onClose} className="p-2 rounded-full text-slate-400 hover:text-slate-700 hover:bg-slate-200 cursor-pointer"><X className="w-5 h-5" /></button>
        </div>

        <div className="overflow-y-auto p-5 sm:p-6 space-y-6">
          {cargandoSaldo && !saldo ? (
            <div className="py-16 flex items-center justify-center gap-2 text-sm text-slate-500"><LoaderCircle className="w-4 h-4 animate-spin" />Cargando billetera...</div>
          ) : saldo ? (
            <>
              <section className="grid grid-cols-1 sm:grid-cols-3 gap-3">
                <div className="rounded-2xl border border-slate-200 bg-slate-50 p-4">
                  <div className="flex items-center justify-between"><span className="text-[11px] uppercase font-bold tracking-wider text-slate-400">Saldo Total</span><CircleDollarSign className="w-4 h-4 text-slate-500" /></div>
                  <p className="mt-2 text-2xl font-black text-slate-900">{formatCurrency(saldo.saldoTotal)}</p>
                  <p className="mt-1 text-[10px] text-slate-500">Fondos totales depositados en tu cuenta.</p>
                </div>
                <div className="rounded-2xl border border-amber-200 bg-amber-50 p-4">
                  <div className="flex items-center justify-between"><span className="text-[11px] uppercase font-bold tracking-wider text-amber-700">En Garantía</span><LockKeyhole className="w-4 h-4 text-amber-600" /></div>
                  <p className="mt-2 text-2xl font-black text-amber-900">{formatCurrency(saldo.saldoRetenido)}</p>
                  <p className="mt-1 text-[10px] text-amber-700">{porcentajeRetenido}% del saldo total está retenido por pujas líderes.</p>
                </div>
                <div className="rounded-2xl border border-emerald-200 bg-emerald-50 p-4">
                  <div className="flex items-center justify-between"><span className="text-[11px] uppercase font-bold tracking-wider text-emerald-700">Disponible</span><ShieldCheck className="w-4 h-4 text-emerald-600" /></div>
                  <p className="mt-2 text-2xl font-black text-emerald-900">{formatCurrency(saldo.saldoDisponible)}</p>
                  <p className="mt-1 text-[10px] text-emerald-700">Único saldo utilizable para nuevas pujas.</p>
                </div>
              </section>

              <section className="grid grid-cols-1 lg:grid-cols-12 gap-5">
                <div className="lg:col-span-4 rounded-2xl border border-slate-200 p-4 sm:p-5 bg-white">
                  <div className="flex items-center gap-2"><BadgeDollarSign className="w-4 h-4 text-blue-600" /><h3 className="text-sm font-black text-slate-900">Agregar saldo</h3></div>
                  <p className="mt-1 text-[11px] text-slate-500">Acreditá saldo para participar en nuevas subastas.</p>

                  <div className="mt-4 space-y-3">
                    <div>
                      <label className="block text-[11px] font-bold text-slate-600 mb-1">Monto a acreditar (ARS)</label>
                      <input type="number" min="0.01" step="100" value={monto} onChange={(event) => setMonto(event.target.value === '' ? '' : Number(event.target.value))} placeholder="Ej: 25000" className="w-full px-3 py-2.5 rounded-xl border border-slate-300 bg-slate-50 text-sm font-bold focus:outline-none focus:border-blue-500 focus:bg-white" />
                    </div>
                    <div className="grid grid-cols-3 gap-1.5">
                      {[10000, 25000, 50000].map((valor) => <button key={valor} type="button" onClick={() => setMonto(valor)} className="py-1.5 rounded-lg bg-slate-100 hover:bg-slate-200 text-[10px] font-bold text-slate-700 cursor-pointer">+{formatCurrency(valor)}</button>)}
                    </div>
                    <button type="button" onClick={handleDepositar} disabled={depositando} className="w-full py-2.5 rounded-xl bg-blue-600 hover:bg-blue-700 disabled:bg-slate-400 text-white text-xs font-black flex items-center justify-center gap-2 cursor-pointer disabled:cursor-wait"><ArrowDownToLine className="w-4 h-4" />{depositando ? 'Acreditando...' : 'Acreditar saldo'}</button>
                  </div>

                  {mensaje && <div className={`mt-3 p-3 rounded-xl text-xs font-medium border ${mensaje.tipo === 'success' ? 'bg-emerald-50 text-emerald-800 border-emerald-200' : 'bg-red-50 text-red-800 border-red-200'}`}>{mensaje.texto}</div>}
                </div>

                <div className="lg:col-span-8 rounded-2xl border border-slate-200 overflow-hidden bg-white">
                  <div className="px-4 py-3 border-b border-slate-100 flex items-center justify-between bg-slate-50/70">
                    <div className="flex items-center gap-2"><History className="w-4 h-4 text-blue-600" /><div><h3 className="text-sm font-black text-slate-900">Historial de movimientos</h3><p className="text-[10px] text-slate-500">Consultá los movimientos de tu cuenta</p></div></div>
                    <button type="button" onClick={() => cargarMovimientos()} disabled={cargandoMovimientos} className="p-2 rounded-lg text-slate-500 hover:bg-slate-200 cursor-pointer disabled:cursor-wait" title="Actualizar"><RefreshCcw className={`w-4 h-4 ${cargandoMovimientos ? 'animate-spin' : ''}`} /></button>
                  </div>

                  <div className="overflow-x-auto max-h-96 overflow-y-auto">
                    <table className="w-full text-left text-xs">
                      <thead className="sticky top-0 bg-white border-b border-slate-100 text-[10px] uppercase tracking-wider text-slate-400"><tr><th className="px-4 py-2.5">Movimiento</th><th className="px-4 py-2.5">Subasta</th><th className="px-4 py-2.5">Fecha</th><th className="px-4 py-2.5 text-right">Monto</th></tr></thead>
                      <tbody>
                        {cargandoMovimientos && movimientos.length === 0 ? <tr><td colSpan={4} className="px-4 py-10 text-center text-slate-400"><LoaderCircle className="w-4 h-4 animate-spin inline mr-2" />Cargando movimientos...</td></tr> : movimientos.length === 0 ? <tr><td colSpan={4} className="px-4 py-10 text-center text-slate-400">Todavía no hay movimientos registrados.</td></tr> : movimientos.map((movimiento) => {
                          const config = configuracionMovimiento[movimiento.tipo];
                          return (
                            <tr key={movimiento.id} className="border-b border-slate-50 hover:bg-slate-50/70">
                              <td className="px-4 py-3"><div className="flex items-center gap-2"><span className={`w-8 h-8 rounded-lg border flex items-center justify-center ${config.clases}`}>{movimiento.tipo === 'Deposito' || movimiento.tipo === 'Cobro' ? <ArrowDownToLine className="w-3.5 h-3.5" /> : movimiento.tipo === 'Pago' ? <ArrowUpFromLine className="w-3.5 h-3.5" /> : <LockKeyhole className="w-3.5 h-3.5" />}</span><div><p className="font-bold text-slate-800">{config.titulo}</p><p className="text-[10px] text-slate-400">{config.descripcion}</p></div></div></td>
                              <td className="px-4 py-3 text-slate-500">{movimiento.subastaId ? <span className="inline-flex items-center gap-1"><Gavel className="w-3 h-3 text-blue-500" />#{movimiento.subastaId}</span> : <span className="text-slate-300">—</span>}</td>
                              <td className="px-4 py-3 whitespace-nowrap text-slate-500"><Clock3 className="w-3 h-3 inline mr-1" />{formatDate(movimiento.fecha)}</td>
                              <td className="px-4 py-3 text-right"><span className={`font-black ${movimiento.tipo === 'Deposito' || movimiento.tipo === 'Cobro' ? 'text-emerald-700' : movimiento.tipo === 'Pago' ? 'text-red-700' : 'text-slate-800'}`}>{config.signo}{formatCurrency(movimiento.monto)}</span></td>
                            </tr>
                          );
                        })}
                      </tbody>
                    </table>
                  </div>
                </div>
              </section>
            </>
          ) : (
            <div className="py-16 text-center text-sm text-red-700">No se pudo cargar la billetera.</div>
          )}
        </div>
      </div>
    </div>
  );
}

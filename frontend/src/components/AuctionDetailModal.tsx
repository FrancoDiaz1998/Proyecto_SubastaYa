import { useEffect, useRef, useState } from 'react';
import {
  AlertCircle,
  CheckCircle2,
  Flame,
  Gavel,
  History,
  LogIn,
  Radio,
  ShieldCheck,
  Tag,
  UserCheck,
  X,
} from 'lucide-react';
import { ApiError } from '../api/apiClient';
import { useAuth } from '../auth/AuthContext';
import type { RegistrarPujaResponse, SubastaDetalle } from '../types/auction';
import { formatCurrency, formatDate } from '../utils/formatters';
import { CountdownTimer } from './CountdownTimer';

interface Props {
  subasta: SubastaDetalle | null;
  onClose: () => void;
  onLogin: () => void;
  onPlaceBid: (subastaId: number, monto: number) => Promise<RegistrarPujaResponse>;
}

type Feedback = { tipo: 'success' | 'warning' | 'error'; mensaje: string };

export function AuctionDetailModal({ subasta, onClose, onLogin, onPlaceBid }: Props) {
  const { usuario } = useAuth();
  const [montoPersonalizado, setMontoPersonalizado] = useState(0);
  const [feedback, setFeedback] = useState<Feedback | null>(null);
  const [enviando, setEnviando] = useState(false);
  const eraLiderRef = useRef(false);
  const subastaAnteriorRef = useRef<number | null>(null);

  const proximoMontoMinimo = subasta
    ? subasta.pujaActual + subasta.incrementoMinimo
    : 0;
  const usuarioEsLider = !!usuario && !!subasta && subasta.postorLiderId === usuario.id;

  useEffect(() => {
    if (!subasta) return;
    setMontoPersonalizado(subasta.pujaActual + subasta.incrementoMinimo);
    setFeedback(null);
    eraLiderRef.current = !!usuario && subasta.postorLiderId === usuario.id;
    subastaAnteriorRef.current = subasta.id;
  }, [subasta?.id, usuario?.id]);

  useEffect(() => {
    if (!subasta) return;
    setMontoPersonalizado((actual) =>
      actual < proximoMontoMinimo ? proximoMontoMinimo : actual,
    );
  }, [proximoMontoMinimo, subasta?.id]);

  useEffect(() => {
    if (!subasta || subastaAnteriorRef.current !== subasta.id) return;

    const esLiderAhora = !!usuario && subasta.postorLiderId === usuario.id;
    if (eraLiderRef.current && !esLiderAhora && subasta.postorLiderId) {
      setFeedback({
        tipo: 'warning',
        mensaje: `Te superaron. La oferta actual es ${formatCurrency(subasta.pujaActual)}.`,
      });
    }
    eraLiderRef.current = esLiderAhora;
  }, [subasta?.postorLiderId, subasta?.pujaActual, subasta?.id, usuario?.id]);

  if (!subasta) return null;

  const activa = subasta.estado === 'Activa';
  const programada = subasta.estado === 'Programada';
  const finalizada = subasta.estado === 'Finalizada' || subasta.estado === 'Desierta';
  const esVendedor = !!usuario && subasta.vendedorId === usuario.id;

  const handleEnviarPuja = async (monto: number) => {
    if (monto < proximoMontoMinimo) {
      setFeedback({
        tipo: 'error',
        mensaje: `La oferta debe ser de al menos ${formatCurrency(proximoMontoMinimo)}.`,
      });
      return;
    }

    setEnviando(true);
    setFeedback(null);

    try {
      const resultado = await onPlaceBid(subasta.id, monto);
      setFeedback({
        tipo: resultado.extendidaPorAntiSniping ? 'warning' : 'success',
        mensaje: resultado.extendidaPorAntiSniping
          ? 'Puja aceptada. Entró en los últimos 60 segundos y el cierre se extendió 2 minutos.'
          : 'Puja registrada. Ahora tu saldo correspondiente queda retenido en garantía.',
      });
      setMontoPersonalizado(monto + subasta.incrementoMinimo);
    } catch (error) {
      setFeedback({
        tipo: 'error',
        mensaje: error instanceof ApiError || error instanceof Error
          ? error.message
          : 'No se pudo registrar la puja.',
      });
    } finally {
      setEnviando(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto bg-black/60 backdrop-blur-xs flex items-center justify-center p-3 sm:p-6" onClick={onClose}>
      <div className="bg-white w-full max-w-4xl rounded-3xl shadow-2xl border border-slate-200 overflow-hidden flex flex-col max-h-[90vh]" onClick={(e) => e.stopPropagation()}>
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-100 bg-slate-50/70">
          <div className="flex items-center gap-2.5">
            <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold bg-blue-100 text-blue-800"><Tag className="w-3.5 h-3.5" />{subasta.categoriaNombre}</span>
            <span className="text-xs font-mono text-slate-400">ID #{subasta.id}</span>
            {activa && <span className="hidden sm:inline-flex items-center gap-1 text-[10px] font-bold text-emerald-700 bg-emerald-50 border border-emerald-200 px-2 py-1 rounded-full"><Radio className="w-3 h-3" />LIVE · sync 2,5 s</span>}
          </div>
          <button type="button" onClick={onClose} className="p-1.5 rounded-full text-slate-400 hover:text-slate-700 hover:bg-slate-200/60 cursor-pointer"><X className="w-5 h-5" /></button>
        </div>

        <div className="p-6 overflow-y-auto grid grid-cols-1 lg:grid-cols-12 gap-6">
          <div className="lg:col-span-7 space-y-4">
            <div className="aspect-16/10 rounded-2xl overflow-hidden bg-slate-100 relative">
              <img src={subasta.urlImagen} alt={subasta.titulo} className="w-full h-full object-cover" />
              <div className="absolute top-3 left-3"><CountdownTimer targetDate={programada ? subasta.fechaInicio : subasta.fechaFin} isScheduled={programada} isEnded={finalizada} /></div>
            </div>
            <div><h2 className="text-xl sm:text-2xl font-black text-slate-900">{subasta.titulo}</h2><p className="mt-2 text-xs sm:text-sm text-slate-600 leading-relaxed">{subasta.descripcion}</p></div>
            <div className="grid grid-cols-2 gap-3 pt-2">
              <div className="p-3 rounded-xl bg-slate-50 border border-slate-100 text-xs"><span className="text-slate-400 block text-[10px] uppercase font-bold">Vendedor</span><span className="font-semibold text-slate-800">{subasta.vendedorNombre || 'Sin nombre'}</span></div>
              <div className="p-3 rounded-xl bg-slate-50 border border-slate-100 text-xs"><span className="text-slate-400 block text-[10px] uppercase font-bold">Incremento Mín.</span><span className="font-bold text-slate-800">+ {formatCurrency(subasta.incrementoMinimo)}</span></div>
            </div>
          </div>

          <div className="lg:col-span-5 space-y-5 bg-slate-50/60 p-5 rounded-2xl border border-slate-100">
            <div className="flex items-center justify-between pb-3 border-b border-slate-200">
              <div><span className="text-xs uppercase font-bold text-slate-400 block">Oferta Actual</span><span className="text-2xl font-black text-slate-900">{formatCurrency(subasta.pujaActual)}</span></div>
              <div className="text-right"><span className="text-xs uppercase font-bold text-slate-400 block">Ofertas</span><span className="text-sm font-bold text-blue-600">{subasta.cantidadPujas} registradas</span></div>
            </div>

            {activa && subasta.postorLider && (
              <div className={`flex items-center gap-2 p-2.5 rounded-xl border text-xs font-semibold ${usuarioEsLider ? 'bg-emerald-50 border-emerald-200 text-emerald-800' : 'bg-slate-100 border-slate-200 text-slate-700'}`}>
                {usuarioEsLider ? <UserCheck className="w-4 h-4 text-emerald-600" /> : <AlertCircle className="w-4 h-4 text-amber-500" />}
                <span>{usuarioEsLider ? '¡Estás liderando esta subasta!' : <>Líder actual: <strong>{subasta.postorLider}</strong></>}</span>
              </div>
            )}

            {feedback && (
              <div className={`p-3 rounded-xl text-xs flex items-start gap-2 border ${feedback.tipo === 'success' ? 'bg-emerald-50 border-emerald-200 text-emerald-900' : feedback.tipo === 'warning' ? 'bg-amber-50 border-amber-200 text-amber-900' : 'bg-red-50 border-red-200 text-red-900'}`}>
                {feedback.tipo === 'success' ? <CheckCircle2 className="w-4 h-4 text-emerald-600 shrink-0" /> : feedback.tipo === 'warning' ? <Flame className="w-4 h-4 text-amber-600 shrink-0" /> : <AlertCircle className="w-4 h-4 text-red-600 shrink-0" />}
                <span className="font-medium">{feedback.mensaje}</span>
              </div>
            )}

            {activa ? !usuario ? (
              <button type="button" onClick={onLogin} className="w-full py-3 px-4 rounded-xl bg-blue-600 text-white font-bold text-xs flex items-center justify-center gap-2 hover:bg-blue-700 cursor-pointer"><LogIn className="w-4 h-4" />Iniciar sesión para pujar</button>
            ) : esVendedor ? (
              <div className="p-4 rounded-xl bg-amber-50 border border-amber-200 text-xs text-amber-900 font-medium">Sos el vendedor de esta publicación. No podés ofertar sobre tu propia subasta.</div>
            ) : (
              <div className="space-y-3">
                <button type="button" disabled={enviando} onClick={() => handleEnviarPuja(proximoMontoMinimo)} className="btn-primary-action w-full py-3 px-4 rounded-xl bg-blue-600 disabled:bg-blue-300 text-white font-bold text-xs flex items-center justify-between hover:bg-blue-700 shadow-md shadow-blue-500/20 cursor-pointer disabled:cursor-wait">
                  <span className="flex items-center gap-1.5"><Gavel className="w-4 h-4" />{enviando ? 'Registrando...' : 'Pujar sugerido'}</span>
                  <span className="font-mono text-sm bg-blue-700/80 px-2 py-0.5 rounded">{formatCurrency(proximoMontoMinimo)}</span>
                </button>
                <div>
                  <label className="block text-[11px] font-semibold text-slate-500 mb-1">O ingresá un monto superior:</label>
                  <div className="flex gap-2">
                    <input type="number" min={proximoMontoMinimo} step={subasta.incrementoMinimo} value={montoPersonalizado} onChange={(e) => setMontoPersonalizado(Number(e.target.value))} className="flex-1 min-w-0 px-3 py-2 bg-white border border-slate-300 rounded-xl text-xs font-bold text-slate-900 focus:outline-none focus:border-blue-600" />
                    <button type="button" disabled={enviando} onClick={() => handleEnviarPuja(montoPersonalizado)} className="px-4 py-2 rounded-xl bg-slate-900 disabled:bg-slate-400 text-white font-bold text-xs hover:bg-slate-800 cursor-pointer disabled:cursor-wait">Pujar</button>
                  </div>
                </div>
                <p className="text-[10px] leading-relaxed text-slate-500"><ShieldCheck className="w-3 h-3 inline mr-1 text-emerald-600" />La API valida saldo, libera al líder anterior y retiene tu oferta en una transacción atómica.</p>
              </div>
            ) : (
              <div className="p-4 rounded-xl bg-slate-100 text-slate-600 text-xs text-center font-medium">{programada ? 'Esta subasta iniciará próximamente. Las pujas se habilitan al comenzar.' : 'Subasta finalizada. No se admiten nuevas ofertas.'}</div>
            )}

            <div className="pt-4 border-t border-slate-200">
              <div className="flex items-center justify-between mb-2"><span className="text-xs font-bold text-slate-800 flex items-center gap-1.5"><History className="w-3.5 h-3.5 text-blue-600" />Historial de Ofertas</span><span className="text-[10px] text-slate-400 font-mono">{subasta.historialPujas.length} pujas</span></div>
              <div className="space-y-1.5 max-h-44 overflow-y-auto pr-1">
                {subasta.historialPujas.length ? subasta.historialPujas.map((puja, index) => (
                  <div key={puja.id} className={`flex items-center justify-between px-3 py-1.5 rounded-lg text-xs ${index === 0 ? 'bg-blue-50 text-blue-900 font-bold border border-blue-100' : 'bg-white text-slate-600 border border-slate-100'}`}>
                    <span className="truncate max-w-32">{puja.postorNombre}</span>
                    <div className="flex items-center gap-2"><span className="font-mono font-bold text-slate-900">{formatCurrency(puja.monto)}</span><span className="text-[10px] text-slate-400">{formatDate(puja.fecha)}</span></div>
                  </div>
                )) : <p className="text-xs text-slate-400 italic text-center py-2">Aún no se han registrado pujas.</p>}
              </div>
            </div>
          </div>
        </div>

        <div className="px-6 py-3 border-t border-slate-100 bg-slate-50 flex items-center justify-between text-xs text-slate-500"><div className="flex items-center gap-2"><ShieldCheck className="w-4 h-4 text-emerald-600" /><span>Escrow + Optimistic Locking · sincronización en vivo por short-polling</span></div><button type="button" onClick={onClose} className="px-4 py-1.5 rounded-lg bg-white border border-slate-300 font-semibold text-slate-700 hover:bg-slate-100 cursor-pointer">Cerrar</button></div>
      </div>
    </div>
  );
}

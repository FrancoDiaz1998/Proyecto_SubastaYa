import { useCallback, useEffect, useState, type ReactNode } from 'react';
import {
  Activity,
  BadgeCheck,
  CircleDollarSign,
  Clock3,
  Eye,
  Gavel,
  LoaderCircle,
  RefreshCcw,
  Store,
  Trophy,
  X,
} from 'lucide-react';
import { listarMisPublicaciones, listarMisPujas } from '../api/actividadesApi';
import { ApiError } from '../api/apiClient';
import type {
  EstadoAdjudicacion,
  EstadoParticipacion,
  MisPublicacionesResponse,
  MisPujasResponse,
} from '../types/activity';
import { formatCurrency, formatDate } from '../utils/formatters';

interface MisActividadesModalProps {
  abierto: boolean;
  onClose: () => void;
  onAbrirSubasta: (subastaId: number) => void;
}

type Pestana = 'pujas' | 'publicaciones';

const clasesParticipacion: Record<EstadoParticipacion, string> = {
  Liderando: 'bg-emerald-50 text-emerald-700 border-emerald-200',
  Superado: 'bg-amber-50 text-amber-700 border-amber-200',
  Ganada: 'bg-violet-50 text-violet-700 border-violet-200',
  NoGanada: 'bg-slate-100 text-slate-600 border-slate-200',
  Desierta: 'bg-slate-100 text-slate-600 border-slate-200',
  Programada: 'bg-blue-50 text-blue-700 border-blue-200',
};

const etiquetaParticipacion: Record<EstadoParticipacion, string> = {
  Liderando: 'Liderando',
  Superado: 'Superado',
  Ganada: 'Ganada',
  NoGanada: 'No ganada',
  Desierta: 'Desierta',
  Programada: 'Programada',
};

const clasesAdjudicacion: Record<EstadoAdjudicacion, string> = {
  Programada: 'bg-blue-50 text-blue-700 border-blue-200',
  EnCurso: 'bg-emerald-50 text-emerald-700 border-emerald-200',
  Adjudicada: 'bg-violet-50 text-violet-700 border-violet-200',
  SinAdjudicar: 'bg-slate-100 text-slate-600 border-slate-200',
};

const etiquetaAdjudicacion: Record<EstadoAdjudicacion, string> = {
  Programada: 'Programada',
  EnCurso: 'En curso',
  Adjudicada: 'Adjudicada',
  SinAdjudicar: 'Sin adjudicar',
};

export function MisActividadesModal({
  abierto,
  onClose,
  onAbrirSubasta,
}: MisActividadesModalProps) {
  const [pestana, setPestana] = useState<Pestana>('pujas');
  const [misPujas, setMisPujas] = useState<MisPujasResponse | null>(null);
  const [misPublicaciones, setMisPublicaciones] = useState<MisPublicacionesResponse | null>(null);
  const [cargando, setCargando] = useState(false);
  const [actualizando, setActualizando] = useState(false);
  const [error, setError] = useState('');

  const cargar = useCallback(async (signal?: AbortSignal, silencioso = false) => {
    if (silencioso) setActualizando(true);
    else setCargando(true);

    try {
      const [pujas, publicaciones] = await Promise.all([
        listarMisPujas(signal),
        listarMisPublicaciones(signal),
      ]);

      setMisPujas(pujas);
      setMisPublicaciones(publicaciones);
      setError('');
    } catch (errorCarga) {
      if (!(errorCarga instanceof DOMException && errorCarga.name === 'AbortError')) {
        setError(
          errorCarga instanceof ApiError
            ? errorCarga.message
            : 'No se pudieron cargar tus actividades.',
        );
      }
    } finally {
      if (!signal?.aborted) {
        setCargando(false);
        setActualizando(false);
      }
    }
  }, []);

  useEffect(() => {
    if (!abierto) return;

    const controller = new AbortController();
    void cargar(controller.signal);

    const interval = window.setInterval(() => {
      void cargar(undefined, true);
    }, 5000);

    return () => {
      controller.abort();
      window.clearInterval(interval);
    };
  }, [abierto, cargar]);

  if (!abierto) return null;

  const abrirSubasta = (subastaId: number) => {
    onClose();
    onAbrirSubasta(subastaId);
  };

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto bg-black/60 backdrop-blur-xs flex items-center justify-center p-3 sm:p-6" onClick={onClose}>
      <div className="bg-white w-full max-w-6xl rounded-3xl shadow-2xl border border-slate-200 overflow-hidden max-h-[92vh] flex flex-col" onClick={(event) => event.stopPropagation()}>
        <div className="px-5 sm:px-6 py-4 border-b border-slate-100 bg-slate-50/80 flex items-center justify-between gap-3">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-xl bg-slate-900 text-white flex items-center justify-center"><Activity className="w-5 h-5" /></div>
            <div>
              <h2 className="font-black text-slate-900">Mis Actividades</h2>
              <p className="text-[11px] text-slate-500">Tus compras, pujas y publicaciones en un solo lugar</p>
            </div>
          </div>
          <div className="flex items-center gap-1.5">
            <button type="button" onClick={() => void cargar(undefined, true)} disabled={actualizando} className="p-2 rounded-full text-slate-400 hover:text-slate-700 hover:bg-slate-200 cursor-pointer disabled:cursor-wait" title="Actualizar"><RefreshCcw className={`w-4 h-4 ${actualizando ? 'animate-spin' : ''}`} /></button>
            <button type="button" onClick={onClose} className="p-2 rounded-full text-slate-400 hover:text-slate-700 hover:bg-slate-200 cursor-pointer"><X className="w-5 h-5" /></button>
          </div>
        </div>

        <div className="px-5 sm:px-6 pt-4 border-b border-slate-100 bg-white">
          <div className="inline-flex p-1 rounded-xl bg-slate-100 border border-slate-200">
            <button type="button" onClick={() => setPestana('pujas')} className={`px-4 py-2 rounded-lg text-xs font-bold flex items-center gap-2 cursor-pointer ${pestana === 'pujas' ? 'bg-white text-blue-700 shadow-xs' : 'text-slate-500'}`}><Gavel className="w-4 h-4" />Mis Compras / Pujas</button>
            <button type="button" onClick={() => setPestana('publicaciones')} className={`px-4 py-2 rounded-lg text-xs font-bold flex items-center gap-2 cursor-pointer ${pestana === 'publicaciones' ? 'bg-white text-blue-700 shadow-xs' : 'text-slate-500'}`}><Store className="w-4 h-4" />Mis Publicaciones</button>
          </div>
        </div>

        <div className="overflow-y-auto p-5 sm:p-6">
          {error && <div className="mb-4 p-3 rounded-xl bg-red-50 text-red-800 border border-red-200 text-xs font-medium">{error}</div>}

          {cargando && !misPujas && !misPublicaciones ? (
            <div className="py-20 flex items-center justify-center gap-2 text-sm text-slate-500"><LoaderCircle className="w-4 h-4 animate-spin" />Cargando actividades...</div>
          ) : pestana === 'pujas' ? (
            <section>
              <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 mb-5">
                <StatCard icon={<Gavel className="w-4 h-4" />} titulo="Participaciones" valor={String(misPujas?.participaciones ?? 0)} />
                <StatCard icon={<Clock3 className="w-4 h-4" />} titulo="Subastas activas" valor={String(misPujas?.activas ?? 0)} />
                <StatCard icon={<Trophy className="w-4 h-4" />} titulo="Ganadas" valor={String(misPujas?.ganadas ?? 0)} />
              </div>

              {!misPujas || misPujas.items.length === 0 ? (
                <EmptyActivity icon={<Gavel className="w-6 h-6" />} titulo="Todavía no participaste en subastas" texto="Cuando realices una puja, la vas a encontrar acá con su estado actualizado." />
              ) : (
                <div className="space-y-3">
                  {misPujas.items.map((item) => (
                    <article key={item.subastaId} className="rounded-2xl border border-slate-200 bg-white p-3 sm:p-4 flex flex-col sm:flex-row gap-4 hover:border-blue-200 hover:shadow-sm transition-all">
                      <img src={item.urlImagen} alt={item.titulo} className="w-full sm:w-28 h-28 rounded-xl object-cover bg-slate-100 shrink-0" />
                      <div className="flex-1 min-w-0">
                        <div className="flex flex-wrap items-center gap-2">
                          <span className={`px-2 py-1 rounded-lg border text-[10px] font-black ${clasesParticipacion[item.estadoParticipacion]}`}>{etiquetaParticipacion[item.estadoParticipacion]}</span>
                          <span className="text-[10px] text-slate-400">{item.categoriaNombre}</span>
                          <span className="text-[10px] text-slate-300">#{item.subastaId}</span>
                        </div>
                        <h3 className="mt-1.5 font-black text-slate-900 truncate">{item.titulo}</h3>
                        <div className="mt-3 grid grid-cols-2 lg:grid-cols-4 gap-2 text-xs">
                          <Metric label="Mi mayor puja" value={formatCurrency(item.miMayorPuja)} />
                          <Metric label="Puja actual" value={formatCurrency(item.pujaActual)} />
                          <Metric label="Ofertas" value={String(item.cantidadPujas)} />
                          <Metric label="Cierre" value={formatDate(item.fechaFin)} />
                        </div>
                      </div>
                      <div className="flex sm:flex-col items-center justify-end gap-2 shrink-0">
                        {item.gane && <BadgeCheck className="w-5 h-5 text-violet-600" />}
                        <button type="button" onClick={() => abrirSubasta(item.subastaId)} className="px-3 py-2 rounded-xl bg-slate-900 hover:bg-slate-800 text-white text-xs font-bold flex items-center gap-1.5 cursor-pointer"><Eye className="w-3.5 h-3.5" />Ver</button>
                      </div>
                    </article>
                  ))}
                </div>
              )}
            </section>
          ) : (
            <section>
              <div className="grid grid-cols-2 lg:grid-cols-4 gap-3 mb-5">
                <StatCard icon={<Store className="w-4 h-4" />} titulo="Publicaciones" valor={String(misPublicaciones?.totalPublicaciones ?? 0)} />
                <StatCard icon={<Clock3 className="w-4 h-4" />} titulo="Activas" valor={String(misPublicaciones?.activas ?? 0)} />
                <StatCard icon={<BadgeCheck className="w-4 h-4" />} titulo="Finalizadas" valor={String(misPublicaciones?.finalizadas ?? 0)} />
                <StatCard icon={<CircleDollarSign className="w-4 h-4" />} titulo="Recaudación" valor={formatCurrency(misPublicaciones?.recaudacionTotal ?? 0)} />
              </div>

              {!misPublicaciones || misPublicaciones.items.length === 0 ? (
                <EmptyActivity icon={<Store className="w-6 h-6" />} titulo="Todavía no publicaste subastas" texto="Tus publicaciones aparecerán acá con sus métricas y estado de adjudicación." />
              ) : (
                <div className="space-y-3">
                  {misPublicaciones.items.map((item) => (
                    <article key={item.subastaId} className="rounded-2xl border border-slate-200 bg-white p-3 sm:p-4 flex flex-col sm:flex-row gap-4 hover:border-blue-200 hover:shadow-sm transition-all">
                      <img src={item.urlImagen} alt={item.titulo} className="w-full sm:w-28 h-28 rounded-xl object-cover bg-slate-100 shrink-0" />
                      <div className="flex-1 min-w-0">
                        <div className="flex flex-wrap items-center gap-2">
                          <span className={`px-2 py-1 rounded-lg border text-[10px] font-black ${clasesAdjudicacion[item.estadoAdjudicacion]}`}>{etiquetaAdjudicacion[item.estadoAdjudicacion]}</span>
                          <span className="text-[10px] text-slate-400">{item.categoriaNombre}</span>
                          <span className="text-[10px] text-slate-300">#{item.subastaId}</span>
                        </div>
                        <h3 className="mt-1.5 font-black text-slate-900 truncate">{item.titulo}</h3>
                        <div className="mt-3 grid grid-cols-2 lg:grid-cols-4 gap-2 text-xs">
                          <Metric label="Precio / puja actual" value={formatCurrency(item.precioActual)} />
                          <Metric label="Recaudación" value={formatCurrency(item.recaudacion)} />
                          <Metric label="Ofertas" value={String(item.cantidadPujas)} />
                          <Metric label="Cierre" value={formatDate(item.fechaFin)} />
                        </div>
                      </div>
                      <div className="flex items-center justify-end shrink-0">
                        <button type="button" onClick={() => abrirSubasta(item.subastaId)} className="px-3 py-2 rounded-xl bg-slate-900 hover:bg-slate-800 text-white text-xs font-bold flex items-center gap-1.5 cursor-pointer"><Eye className="w-3.5 h-3.5" />Ver</button>
                      </div>
                    </article>
                  ))}
                </div>
              )}
            </section>
          )}
        </div>
      </div>
    </div>
  );
}

function StatCard({ icon, titulo, valor }: { icon: ReactNode; titulo: string; valor: string }) {
  return (
    <div className="rounded-2xl border border-slate-200 bg-slate-50 p-3.5">
      <div className="flex items-center gap-1.5 text-slate-500">{icon}<span className="text-[10px] uppercase font-bold tracking-wider">{titulo}</span></div>
      <p className="mt-1.5 text-xl font-black text-slate-900 truncate" title={valor}>{valor}</p>
    </div>
  );
}

function Metric({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-lg bg-slate-50 px-2.5 py-2">
      <span className="block text-[9px] uppercase font-bold text-slate-400">{label}</span>
      <span className="block mt-0.5 font-bold text-slate-800 truncate" title={value}>{value}</span>
    </div>
  );
}

function EmptyActivity({ icon, titulo, texto }: { icon: ReactNode; titulo: string; texto: string }) {
  return (
    <div className="py-16 rounded-2xl border border-dashed border-slate-300 bg-slate-50 text-center">
      <div className="mx-auto w-12 h-12 rounded-2xl bg-white border border-slate-200 text-blue-600 flex items-center justify-center">{icon}</div>
      <h3 className="mt-3 text-sm font-black text-slate-900">{titulo}</h3>
      <p className="mt-1 text-xs text-slate-500 max-w-md mx-auto">{texto}</p>
    </div>
  );
}

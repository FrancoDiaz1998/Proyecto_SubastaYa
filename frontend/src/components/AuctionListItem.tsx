import type { SubastaResumen } from '../types/auction';
import { CountdownTimer } from './CountdownTimer';
import { formatCurrency } from '../utils/formatters';
import { ArrowUpRight, Gavel, Tag, TrendingUp } from 'lucide-react';

interface Props { subasta: SubastaResumen; onSelect: (subasta: SubastaResumen) => void; }

export function AuctionListItem({ subasta, onSelect }: Props) {
  const activa = subasta.estado === 'Activa';
  const programada = subasta.estado === 'Programada';
  const finalizada = subasta.estado === 'Finalizada' || subasta.estado === 'Desierta';

  return (
    <div className="auction-card flex flex-col sm:flex-row items-center gap-4 p-3 sm:p-4 cursor-pointer" onClick={() => onSelect(subasta)}>
      <div className="w-full sm:w-44 h-32 rounded-xl overflow-hidden bg-slate-100 shrink-0 relative">
        <img src={subasta.urlImagen} alt={subasta.titulo} className="w-full h-full object-cover card-image" loading="lazy" />
        <span className={`absolute top-2 left-2 px-2 py-0.5 rounded-md text-[10px] font-bold text-white uppercase ${activa ? 'bg-emerald-600' : programada ? 'bg-blue-600' : 'bg-slate-700'}`}>{subasta.estado}</span>
      </div>
      <div className="flex-1 min-w-0 w-full">
        <div className="flex items-center gap-2 mb-1"><span className="inline-flex items-center gap-1 px-2 py-0.5 rounded text-[11px] font-medium bg-slate-100 text-slate-700"><Tag className="w-3 h-3 text-blue-600" />{subasta.categoriaNombre}</span><span className="text-xs text-slate-400">ID #{subasta.id}</span></div>
        <h3 className="text-base font-bold text-slate-900 truncate">{subasta.titulo}</h3>
        <div className="mt-3 flex items-center gap-4"><CountdownTimer targetDate={programada ? subasta.fechaInicio : subasta.fechaFin} isScheduled={programada} isEnded={finalizada} compact /><span className="text-xs text-slate-400 flex items-center gap-1"><Gavel className="w-3.5 h-3.5 text-blue-600" />{subasta.cantidadPujas} ofertas</span></div>
      </div>
      <div className="w-full sm:w-auto flex sm:flex-col items-center sm:items-end justify-between gap-3 pt-3 sm:pt-0 border-t sm:border-t-0 border-slate-100 shrink-0">
        <div className="text-left sm:text-right"><span className="text-[10px] uppercase font-bold text-slate-400 block">{subasta.cantidadPujas > 0 ? 'Oferta más alta' : 'Precio base'}</span><span className="text-base sm:text-lg font-black text-slate-900">{formatCurrency(subasta.pujaActual)}</span></div>
        <button type="button" onClick={(e) => { e.stopPropagation(); onSelect(subasta); }} className={`btn-primary-action px-4 py-2 rounded-xl text-xs font-bold flex items-center gap-1.5 cursor-pointer ${activa ? 'bg-blue-600 text-white hover:bg-blue-700' : 'bg-slate-100 text-slate-700 hover:bg-slate-200'}`}>{activa ? <><TrendingUp className="w-3.5 h-3.5" />Ver</> : <>Detalles<ArrowUpRight className="w-3.5 h-3.5" /></>}</button>
      </div>
    </div>
  );
}

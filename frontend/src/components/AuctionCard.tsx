import type { SubastaResumen } from '../types/auction';
import { CountdownTimer } from './CountdownTimer';
import { formatCurrency } from '../utils/formatters';
import { ArrowUpRight, Gavel, ShieldCheck, Tag, TrendingUp } from 'lucide-react';

interface Props { subasta: SubastaResumen; onSelect: (subasta: SubastaResumen) => void; }

export function AuctionCard({ subasta, onSelect }: Props) {
  const activa = subasta.estado === 'Activa';
  const programada = subasta.estado === 'Programada';
  const finalizada = subasta.estado === 'Finalizada' || subasta.estado === 'Desierta';

  return (
    <article className="auction-card flex flex-col group cursor-pointer" onClick={() => onSelect(subasta)} tabIndex={0} role="button" onKeyDown={(e) => { if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); onSelect(subasta); } }} aria-label={`Ver subasta: ${subasta.titulo}`}>
      <div className="card-image-container aspect-16/10 w-full bg-slate-100 relative">
        <img src={subasta.urlImagen} alt={subasta.titulo} className="card-image w-full h-full object-cover" loading="lazy" onError={(e) => { (e.currentTarget as HTMLImageElement).src = 'https://images.unsplash.com/photo-1579546929518-9e396f3cc809?auto=format&fit=crop&w=800&q=80'; }} />
        <div className="absolute inset-0 bg-gradient-to-t from-black/50 via-transparent to-black/20 pointer-events-none" />
        <div className="absolute top-3 left-3">
          {activa && <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold bg-emerald-500/90 text-white"><span className="w-2 h-2 rounded-full bg-white live-indicator-dot" />EN VIVO</span>}
          {programada && <span className="px-2.5 py-1 rounded-full text-xs font-semibold bg-blue-600/90 text-white">PRÓXIMA</span>}
          {finalizada && <span className="px-2.5 py-1 rounded-full text-xs font-semibold bg-slate-800/90 text-slate-200">{subasta.estado === 'Desierta' ? 'DESIERTA' : 'FINALIZADA'}</span>}
        </div>
        <div className="absolute top-3 right-3"><span className="inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-medium bg-white/90 text-slate-800"><Tag className="w-3 h-3 text-blue-600" />{subasta.categoriaNombre}</span></div>
        <div className="absolute bottom-3 left-3 right-3 flex items-center justify-between">
          <CountdownTimer targetDate={programada ? subasta.fechaInicio : subasta.fechaFin} isScheduled={programada} isEnded={finalizada} />
          {activa && <div className="inline-flex items-center gap-1 px-2 py-0.5 rounded-md text-[11px] font-medium bg-black/60 text-slate-200"><ShieldCheck className="w-3.5 h-3.5 text-blue-400" />Oferta protegida</div>}
        </div>
      </div>

      <div className="p-4 flex-1 flex flex-col justify-between">
        <h3 className="font-bold text-slate-900 text-base line-clamp-2 group-hover:text-blue-600 transition-colors">{subasta.titulo}</h3>
        <div className="mt-4 pt-3 border-t border-slate-100">
          <div className="flex items-end justify-between">
            <div><span className="text-[11px] font-medium uppercase tracking-wider text-slate-400 block">{subasta.cantidadPujas > 0 ? 'Oferta más alta' : 'Precio base'}</span><span className="text-lg font-extrabold text-slate-900">{formatCurrency(subasta.pujaActual)}</span></div>
            <div className="inline-flex items-center gap-1 text-xs font-semibold px-2 py-1 rounded-md bg-slate-100 text-slate-700"><Gavel className="w-3.5 h-3.5 text-blue-600" />{subasta.cantidadPujas} {subasta.cantidadPujas === 1 ? 'oferta' : 'ofertas'}</div>
          </div>
          <button type="button" className={`btn-primary-action mt-3 w-full py-2 px-3 rounded-lg text-xs font-bold flex items-center justify-center gap-1.5 cursor-pointer ${activa ? 'bg-blue-600 text-white hover:bg-blue-700' : 'bg-slate-100 text-slate-700 hover:bg-slate-200'}`} onClick={(e) => { e.stopPropagation(); onSelect(subasta); }}>
            {activa ? <><TrendingUp className="w-3.5 h-3.5" />Ver subasta</> : <>Ver detalles<ArrowUpRight className="w-3.5 h-3.5" /></>}
          </button>
        </div>
      </div>
    </article>
  );
}

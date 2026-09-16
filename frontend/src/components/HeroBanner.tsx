import React from 'react';
import { ShieldCheck, Sparkles, Clock } from 'lucide-react';

export const HeroBanner: React.FC = () => {
  return (
    <div className="relative overflow-hidden bg-gradient-to-br from-slate-900 via-blue-950 to-slate-900 text-white rounded-3xl p-6 sm:p-10 mb-8 shadow-xl border border-slate-800">
      {/* Luces de fondo decorativas */}
      <div className="absolute top-0 right-0 -mr-24 -mt-24 w-96 h-96 bg-blue-500/15 rounded-full blur-3xl pointer-events-none" />
      <div className="absolute bottom-0 left-1/3 -mb-24 w-80 h-80 bg-indigo-500/10 rounded-full blur-3xl pointer-events-none" />

      <div className="relative z-10 max-w-3xl">
        {/* Tagline superior */}
        <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-blue-500/20 text-blue-300 text-xs font-semibold mb-4 border border-blue-400/30 backdrop-blur-md">
          <Sparkles className="w-3.5 h-3.5 text-blue-400" />
          <span>Plataforma Oficial de Subastas Competitivas</span>
        </div>

        {/* Título de alto impacto */}
        <h1 className="text-2xl sm:text-4xl font-extrabold tracking-tight text-white leading-tight">
          Compite y gana con respaldo total de fondos en tiempo real
        </h1>

        <p className="mt-3 text-sm sm:text-base text-slate-300 leading-relaxed max-w-2xl">
          Explora nuestro catálogo de productos verificados. Todas las ofertas operan bajo
          arquitectura transaccional garantizada para compradores y vendedores.
        </p>

        {/* Tarjetas de los dos pilares del TP (Página 1) */}
        <div className="mt-6 grid grid-cols-1 sm:grid-cols-2 gap-3 pt-6 border-t border-slate-800/80">
          {/* Pilar 1: Escrow */}
          <div className="flex items-start gap-3 bg-white/5 border border-white/10 rounded-xl p-3 backdrop-blur-xs">
            <div className="p-2 rounded-lg bg-emerald-500/20 text-emerald-400 shrink-0">
              <ShieldCheck className="w-4 h-4" />
            </div>
            <div>
              <h4 className="text-xs font-bold text-white">Garantía Escrow</h4>
              <p className="text-[11px] text-slate-300 mt-0.5 leading-snug">
                Cada puja congela saldo real en billetera. Si te superan, se libera al instante.
              </p>
            </div>
          </div>

          {/* Pilar 2: Anti-Sniping */}
          <div className="flex items-start gap-3 bg-white/5 border border-white/10 rounded-xl p-3 backdrop-blur-xs">
            <div className="p-2 rounded-lg bg-amber-500/20 text-amber-400 shrink-0">
              <Clock className="w-4 h-4" />
            </div>
            <div>
              <h4 className="text-xs font-bold text-white">Juego Limpio Anti-Sniping</h4>
              <p className="text-[11px] text-slate-300 mt-0.5 leading-snug">
                Pujas en los últimos 60 segundos extienden automáticamente el cierre en +2 minutos.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

import { useState } from 'react';
import {
  Activity,
  ChevronDown,
  Gavel,
  LogIn,
  LogOut,
  Plus,
  UserRound,
  Wallet,
} from 'lucide-react';
import { useAuth } from '../auth/AuthContext';
import type { SaldoBilletera } from '../types/wallet';
import { formatCurrency } from '../utils/formatters';

interface NavbarProps {
  onLogin: () => void;
  onPublicar: () => void;
  onBilletera: () => void;
  onActividades: () => void;
  saldoBilletera: SaldoBilletera | null;
}

export function Navbar({
  onLogin,
  onPublicar,
  onBilletera,
  onActividades,
  saldoBilletera,
}: NavbarProps) {
  const { usuario, cargandoSesion, cerrarSesion } = useAuth();
  const [menuAbierto, setMenuAbierto] = useState(false);
  const iniciales = usuario?.nombre
    .split(' ')
    .map((parte) => parte[0])
    .join('')
    .slice(0, 2)
    .toUpperCase() ?? '';

  return (
    <header className="sticky top-0 z-40 glass-nav shadow-xs">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between gap-4">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-gradient-to-tr from-blue-600 to-indigo-600 flex items-center justify-center text-white shadow-md shadow-blue-500/20"><Gavel className="w-5 h-5" /></div>
          <div>
            <div className="flex items-center gap-2"><span className="font-extrabold text-lg text-slate-900 tracking-tight">Subasta<span className="text-blue-600">Ya</span></span><span className="inline-flex items-center gap-1 px-1.5 py-0.5 rounded text-[10px] font-extrabold bg-emerald-100 text-emerald-800 tracking-wider uppercase"><span className="w-1.5 h-1.5 rounded-full bg-emerald-500 live-indicator-dot" />Live</span></div>
            <p className="text-[10px] text-slate-400 font-medium hidden sm:block">Comercio electrónico y subastas en tiempo real</p>
          </div>
        </div>

        <nav className="hidden lg:flex items-center gap-1 text-xs font-semibold text-slate-600">
          <a href="#catalogo" className="px-3 py-2 rounded-lg text-blue-600 bg-blue-50/80 hover:bg-blue-100/70">Explorar Catálogo</a>
          <button type="button" onClick={onPublicar} className="px-3 py-2 rounded-lg hover:text-blue-700 hover:bg-blue-50 transition-colors cursor-pointer">Publicar Subasta</button>
          <button type="button" onClick={onBilletera} className="px-3 py-2 rounded-lg hover:text-blue-700 hover:bg-blue-50 transition-colors cursor-pointer">Billetera</button>
          <button type="button" onClick={onActividades} className="px-3 py-2 rounded-lg hover:text-blue-700 hover:bg-blue-50 transition-colors cursor-pointer">Mis Actividades</button>
        </nav>

        <div className="flex items-center gap-2">
          {usuario && (
            <button type="button" onClick={onBilletera} className="hidden sm:flex items-center gap-2 px-3 py-1.5 rounded-xl bg-emerald-50 hover:bg-emerald-100 border border-emerald-200 cursor-pointer" title="Abrir billetera">
              <Wallet className="w-4 h-4 text-emerald-700" />
              <div className="text-left"><span className="block text-[9px] uppercase font-bold text-emerald-600 leading-none">Disponible</span><span className="block text-xs font-black text-emerald-900 mt-0.5">{saldoBilletera ? formatCurrency(saldoBilletera.saldoDisponible) : '—'}</span></div>
            </button>
          )}

          <button type="button" onClick={onPublicar} className="lg:hidden w-9 h-9 rounded-xl bg-blue-600 hover:bg-blue-700 text-white flex items-center justify-center cursor-pointer" title="Publicar subasta"><Plus className="w-4 h-4" /></button>

          <div className="relative">
            {cargandoSesion ? <div className="w-28 h-9 rounded-xl bg-slate-100 animate-pulse" /> : usuario ? (
              <>
                <button type="button" onClick={() => setMenuAbierto((valor) => !valor)} className="flex items-center gap-2 px-2.5 py-1.5 rounded-xl bg-slate-100 hover:bg-slate-200 border border-slate-200 cursor-pointer">
                  <div className="w-7 h-7 rounded-full bg-slate-900 text-white flex items-center justify-center text-[10px] font-bold">{iniciales}</div>
                  <div className="hidden md:block text-left max-w-32"><span className="block text-xs font-bold text-slate-900 truncate">{usuario.nombre}</span><span className="block text-[9px] text-slate-500 truncate">{usuario.email}</span></div>
                  <ChevronDown className="w-3.5 h-3.5 text-slate-400" />
                </button>
                {menuAbierto && (
                  <div className="absolute right-0 mt-2 w-64 bg-white rounded-2xl shadow-xl border border-slate-200 p-3 z-50">
                    <div className="flex items-center gap-2 px-2 py-2 border-b border-slate-100"><UserRound className="w-4 h-4 text-blue-600" /><div className="min-w-0"><p className="text-xs font-bold truncate">{usuario.nombre}</p><p className="text-[10px] text-slate-500 truncate">{usuario.email}</p></div></div>
                    <button type="button" onClick={() => { setMenuAbierto(false); onActividades(); }} className="mt-2 w-full px-2 py-2 rounded-lg text-xs font-semibold text-slate-800 hover:bg-slate-100 flex items-center gap-2 cursor-pointer"><Activity className="w-4 h-4 text-blue-600" />Mis Actividades</button>
                    <button type="button" onClick={() => { setMenuAbierto(false); onBilletera(); }} className="mt-1 w-full px-2 py-2 rounded-lg text-xs font-semibold text-emerald-700 hover:bg-emerald-50 flex items-center gap-2 cursor-pointer"><Wallet className="w-4 h-4" />Mi billetera</button>
                    <button type="button" onClick={() => { setMenuAbierto(false); onPublicar(); }} className="mt-1 w-full px-2 py-2 rounded-lg text-xs font-semibold text-blue-700 hover:bg-blue-50 flex items-center gap-2 cursor-pointer"><Plus className="w-4 h-4" />Publicar subasta</button>
                    <button type="button" onClick={() => { cerrarSesion(); setMenuAbierto(false); }} className="mt-1 w-full px-2 py-2 rounded-lg text-xs font-semibold text-red-600 hover:bg-red-50 flex items-center gap-2 cursor-pointer"><LogOut className="w-4 h-4" />Cerrar sesión</button>
                  </div>
                )}
              </>
            ) : (
              <button type="button" onClick={onLogin} className="px-4 py-2 rounded-xl bg-blue-600 hover:bg-blue-700 text-white text-xs font-bold flex items-center gap-2 cursor-pointer"><LogIn className="w-4 h-4" /><span className="hidden sm:inline">Iniciar sesión</span></button>
            )}
          </div>
        </div>
      </div>
    </header>
  );
}

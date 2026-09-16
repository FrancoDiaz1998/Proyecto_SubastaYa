import { useEffect, useState } from 'react';
import type { Categoria, FiltrosSubasta, SubastaDetalle, SubastaResumen } from './types/auction';
import { listarCategorias } from './api/categoriasApi';
import { listarSubastas, obtenerSubasta } from './api/subastasApi';
import { ApiError } from './api/apiClient';
import { Navbar } from './components/Navbar';
import { HeroBanner } from './components/HeroBanner';
import { FilterBar } from './components/FilterBar';
import { AuctionCard } from './components/AuctionCard';
import { AuctionListItem } from './components/AuctionListItem';
import { AuctionDetailModal } from './components/AuctionDetailModal';
import { EmptyState } from './components/EmptyState';
import { ToastNotification, type ToastData } from './components/ToastNotification';
import { LoginModal } from './components/LoginModal';

const FILTROS_INICIALES: FiltrosSubasta = {
  busqueda: '', estado: 'todas', categoriaId: 'todas',
  precioMin: '', precioMax: '', orden: 'tiempo_asc',
};

export function App() {
  const [subastas, setSubastas] = useState<SubastaResumen[]>([]);
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [filtros, setFiltros] = useState<FiltrosSubasta>(FILTROS_INICIALES);
  const [vistaModo, setVistaModo] = useState<'grid' | 'list'>('grid');
  const [subastaSeleccionada, setSubastaSeleccionada] = useState<SubastaDetalle | null>(null);
  const [totalResultados, setTotalResultados] = useState(0);
  const [cargando, setCargando] = useState(true);
  const [errorCatalogo, setErrorCatalogo] = useState('');
  const [loginAbierto, setLoginAbierto] = useState(false);
  const [toasts, setToasts] = useState<ToastData[]>([]);

  const agregarToast = (tipo: ToastData['tipo'], titulo: string, mensaje: string) => {
    const id = Math.random().toString(36).slice(2);
    setToasts((prev) => [...prev, { id, tipo, titulo, mensaje }]);
    window.setTimeout(() => setToasts((prev) => prev.filter((t) => t.id !== id)), 4500);
  };

  useEffect(() => {
    const controller = new AbortController();
    listarCategorias(controller.signal)
      .then(setCategorias)
      .catch((err) => { if (!(err instanceof DOMException && err.name === 'AbortError')) agregarToast('error', 'Categorías', 'No se pudieron cargar las categorías.'); });
    return () => controller.abort();
  }, []);

  useEffect(() => {
    const controller = new AbortController();
    const timer = window.setTimeout(async () => {
      setCargando(true); setErrorCatalogo('');
      try {
        const respuesta = await listarSubastas(filtros, controller.signal);
        setSubastas(respuesta.items); setTotalResultados(respuesta.totalItems);
      } catch (err) {
        if (!(err instanceof DOMException && err.name === 'AbortError')) {
          setSubastas([]); setTotalResultados(0);
          setErrorCatalogo(err instanceof ApiError ? err.message : 'No se pudo conectar con la API.');
        }
      } finally { if (!controller.signal.aborted) setCargando(false); }
    }, filtros.busqueda.trim() ? 300 : 0);

    return () => { window.clearTimeout(timer); controller.abort(); };
  }, [filtros]);

  const handleSelectSubasta = async (subasta: SubastaResumen) => {
    try { setSubastaSeleccionada(await obtenerSubasta(subasta.id)); }
    catch (err) { agregarToast('error', 'Detalle no disponible', err instanceof ApiError ? err.message : 'No se pudo cargar la subasta.'); }
  };

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900 flex flex-col antialiased">
      <Navbar onLogin={() => setLoginAbierto(true)} />
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 flex-1 w-full" id="catalogo">
        <HeroBanner />
        <FilterBar filtros={filtros} onChangeFiltros={setFiltros} categorias={categorias} totalResultados={totalResultados} vistaModo={vistaModo} onToggleVista={setVistaModo} onResetFiltros={() => setFiltros(FILTROS_INICIALES)} />

        {errorCatalogo && <div className="mb-6 p-4 rounded-xl bg-red-50 border border-red-200 text-sm text-red-800"><strong>No se pudo cargar el catálogo.</strong> {errorCatalogo}</div>}
        {cargando ? <div className="py-16 text-center text-sm text-slate-500">Cargando subastas desde la API...</div> : subastas.length === 0 ? <EmptyState onReset={() => setFiltros(FILTROS_INICIALES)} /> : vistaModo === 'grid' ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">{subastas.map((subasta) => <AuctionCard key={subasta.id} subasta={subasta} onSelect={handleSelectSubasta} />)}</div>
        ) : (
          <div className="space-y-4">{subastas.map((subasta) => <AuctionListItem key={subasta.id} subasta={subasta} onSelect={handleSelectSubasta} />)}</div>
        )}
      </main>

      <AuctionDetailModal subasta={subastaSeleccionada} onClose={() => setSubastaSeleccionada(null)} onLogin={() => setLoginAbierto(true)} />
      <LoginModal abierto={loginAbierto} onClose={() => setLoginAbierto(false)} />
      <ToastNotification toasts={toasts} onDismiss={(id) => setToasts((prev) => prev.filter((t) => t.id !== id))} />

      <footer className="bg-white border-t border-slate-200 mt-16 py-8 text-xs text-slate-500"><div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex flex-col sm:flex-row items-center justify-between gap-4"><p><strong>SubastaYa</strong> &copy; {new Date().getFullYear()} — Proyecto de Software.</p><span className="inline-flex items-center gap-1.5 text-emerald-600 font-semibold"><span className="w-2 h-2 rounded-full bg-emerald-500 live-indicator-dot" />API conectada</span></div></footer>
    </div>
  );
}

export default App;

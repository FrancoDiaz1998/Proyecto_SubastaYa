import { useEffect, useState } from 'react';
import type {
  Categoria,
  CrearSubastaResponse,
  FiltrosSubasta,
  RegistrarPujaResponse,
  SubastaDetalle,
  SubastaResumen,
} from './types/auction';
import type { SaldoBilletera } from './types/wallet';
import { listarCategorias } from './api/categoriasApi';
import { listarSubastas, obtenerSubasta, registrarPuja } from './api/subastasApi';
import { obtenerSaldoBilletera } from './api/billeterasApi';
import { ApiError } from './api/apiClient';
import { useAuth } from './auth/AuthContext';
import { Navbar } from './components/Navbar';
import { HeroBanner } from './components/HeroBanner';
import { FilterBar } from './components/FilterBar';
import { AuctionCard } from './components/AuctionCard';
import { AuctionListItem } from './components/AuctionListItem';
import { AuctionDetailModal } from './components/AuctionDetailModal';
import { PublicarSubastaModal } from './components/PublicarSubastaModal';
import { BilleteraModal } from './components/BilleteraModal';
import { MisActividadesModal } from './components/MisActividadesModal';
import { EmptyState } from './components/EmptyState';
import { ToastNotification, type ToastData } from './components/ToastNotification';
import { LoginModal } from './components/LoginModal';
import { formatCurrency } from './utils/formatters';

const FILTROS_INICIALES: FiltrosSubasta = {
  busqueda: '', estado: 'todas', categoriaId: 'todas',
  precioMin: '', precioMax: '', orden: 'tiempo_asc',
};

type AccionPostLogin = 'publicar' | 'billetera' | 'actividades' | null;

function resumenDesdeDetalle(detalle: SubastaDetalle): SubastaResumen {
  return {
    id: detalle.id,
    categoriaId: detalle.categoriaId,
    categoriaNombre: detalle.categoriaNombre,
    titulo: detalle.titulo,
    urlImagen: detalle.urlImagen,
    pujaActual: detalle.pujaActual,
    cantidadPujas: detalle.cantidadPujas,
    fechaInicio: detalle.fechaInicio,
    fechaFin: detalle.fechaFin,
    estado: detalle.estado,
  };
}

export function App() {
  const { usuario } = useAuth();
  const [subastas, setSubastas] = useState<SubastaResumen[]>([]);
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [filtros, setFiltros] = useState<FiltrosSubasta>(FILTROS_INICIALES);
  const [vistaModo, setVistaModo] = useState<'grid' | 'list'>('grid');
  const [subastaSeleccionada, setSubastaSeleccionada] = useState<SubastaDetalle | null>(null);
  const [totalResultados, setTotalResultados] = useState(0);
  const [cargando, setCargando] = useState(true);
  const [errorCatalogo, setErrorCatalogo] = useState('');
  const [loginAbierto, setLoginAbierto] = useState(false);
  const [publicarAbierto, setPublicarAbierto] = useState(false);
  const [billeteraAbierta, setBilleteraAbierta] = useState(false);
  const [actividadesAbiertas, setActividadesAbiertas] = useState(false);
  const [saldoBilletera, setSaldoBilletera] = useState<SaldoBilletera | null>(null);
  const [cargandoSaldo, setCargandoSaldo] = useState(false);
  const [accionPostLogin, setAccionPostLogin] = useState<AccionPostLogin>(null);
  const [toasts, setToasts] = useState<ToastData[]>([]);

  const agregarToast = (tipo: ToastData['tipo'], titulo: string, mensaje: string) => {
    const id = Math.random().toString(36).slice(2);
    setToasts((prev) => [...prev, { id, tipo, titulo, mensaje }]);
    window.setTimeout(() => setToasts((prev) => prev.filter((toast) => toast.id !== id)), 4500);
  };

  const aplicarDetalle = (detalle: SubastaDetalle) => {
    setSubastaSeleccionada((actual) => actual?.id === detalle.id ? detalle : actual);
    setSubastas((actuales) => actuales.map((subasta) =>
      subasta.id === detalle.id ? resumenDesdeDetalle(detalle) : subasta,
    ));
  };

  const refrescarSaldo = async () => {
    if (!usuario) return;
    try { setSaldoBilletera(await obtenerSaldoBilletera()); }
    catch { /* El siguiente polling vuelve a intentar. */ }
  };

  useEffect(() => {
    const controller = new AbortController();
    listarCategorias(controller.signal)
      .then(setCategorias)
      .catch((error) => {
        if (!(error instanceof DOMException && error.name === 'AbortError'))
          agregarToast('error', 'Categorías', 'No se pudieron cargar las categorías.');
      });
    return () => controller.abort();
  }, []);

  useEffect(() => {
    const controller = new AbortController();
    const timer = window.setTimeout(async () => {
      setCargando(true); setErrorCatalogo('');
      try {
        const respuesta = await listarSubastas(filtros, controller.signal);
        setSubastas(respuesta.items); setTotalResultados(respuesta.totalItems);
      } catch (error) {
        if (!(error instanceof DOMException && error.name === 'AbortError')) {
          setSubastas([]); setTotalResultados(0);
          setErrorCatalogo(error instanceof ApiError ? error.message : 'No se pudo conectar con la API.');
        }
      } finally {
        if (!controller.signal.aborted) setCargando(false);
      }
    }, filtros.busqueda.trim() ? 300 : 0);

    return () => { window.clearTimeout(timer); controller.abort(); };
  }, [filtros]);

  useEffect(() => {
    const subastaId = subastaSeleccionada?.id;
    if (!subastaId) return;

    let cancelado = false;
    const sincronizar = async () => {
      try {
        const detalle = await obtenerSubasta(subastaId);
        if (!cancelado) aplicarDetalle(detalle);
      } catch { /* la siguiente iteración vuelve a intentar */ }
    };

    const interval = window.setInterval(sincronizar, 2500);
    return () => { cancelado = true; window.clearInterval(interval); };
  }, [subastaSeleccionada?.id]);

  useEffect(() => {
    if (!usuario) {
      setSaldoBilletera(null);
      setBilleteraAbierta(false);
      setActividadesAbiertas(false);
      return;
    }

    let cancelado = false;
    const cargar = async () => {
      setCargandoSaldo(true);
      try {
        const saldo = await obtenerSaldoBilletera();
        if (!cancelado) setSaldoBilletera(saldo);
      } catch { /* se conserva el último saldo conocido */ }
      finally { if (!cancelado) setCargandoSaldo(false); }
    };

    void cargar();
    const interval = window.setInterval(cargar, 5000);
    return () => { cancelado = true; window.clearInterval(interval); };
  }, [usuario?.id]);

  const abrirSubastaPorId = async (subastaId: number) => {
    try {
      setSubastaSeleccionada(await obtenerSubasta(subastaId));
    } catch (error) {
      agregarToast(
        'error',
        'Detalle no disponible',
        error instanceof ApiError ? error.message : 'No se pudo cargar la subasta.',
      );
    }
  };

  const handleSelectSubasta = (subasta: SubastaResumen) => {
    void abrirSubastaPorId(subasta.id);
  };

  const handlePublicar = () => {
    if (usuario) { setPublicarAbierto(true); return; }
    setAccionPostLogin('publicar'); setLoginAbierto(true);
  };

  const handleBilletera = () => {
    if (usuario) { setBilleteraAbierta(true); return; }
    setAccionPostLogin('billetera'); setLoginAbierto(true);
  };

  const handleActividades = () => {
    if (usuario) { setActividadesAbiertas(true); return; }
    setAccionPostLogin('actividades'); setLoginAbierto(true);
  };

  const handleLoginSuccess = () => {
    if (accionPostLogin === 'publicar') setPublicarAbierto(true);
    if (accionPostLogin === 'billetera') setBilleteraAbierta(true);
    if (accionPostLogin === 'actividades') setActividadesAbiertas(true);
    setAccionPostLogin(null);
  };

  const handleSubastaCreada = (subasta: CrearSubastaResponse) => {
    setPublicarAbierto(false);
    setFiltros({ ...FILTROS_INICIALES });
    agregarToast('success', 'Subasta publicada', `“${subasta.titulo}” se creó como ${subasta.estado === 'Programada' ? 'programada' : 'activa'} con ID #${subasta.id}.`);
  };

  const handlePlaceBid = async (subastaId: number, monto: number): Promise<RegistrarPujaResponse> => {
    try {
      const resultado = await registrarPuja(subastaId, { monto });
      const [detalle, catalogo] = await Promise.all([
        obtenerSubasta(subastaId),
        listarSubastas(filtros),
      ]);

      setSubastaSeleccionada(detalle);
      setSubastas(catalogo.items);
      setTotalResultados(catalogo.totalItems);
      void refrescarSaldo();

      agregarToast(
        resultado.extendidaPorAntiSniping ? 'warning' : 'success',
        resultado.extendidaPorAntiSniping ? 'Cierre extendido' : 'Puja registrada',
        resultado.extendidaPorAntiSniping
          ? `Oferta de ${formatCurrency(monto)} aceptada. El cierre se extendió 2 minutos.`
          : `Oferta de ${formatCurrency(monto)} registrada y protegida.`,
      );

      return resultado;
    } catch (error) {
      if (error instanceof ApiError && error.status === 401) setLoginAbierto(true);

      if (error instanceof ApiError && error.status === 409) {
        try { aplicarDetalle(await obtenerSubasta(subastaId)); } catch { /* se mantiene el último estado */ }
      }

      agregarToast(
        'error',
        'Puja rechazada',
        error instanceof ApiError ? error.message : 'No se pudo registrar la puja.',
      );
      throw error;
    }
  };

  return (
    <div className="min-h-screen bg-slate-50 text-slate-900 flex flex-col antialiased">
      <Navbar
        onLogin={() => { setAccionPostLogin(null); setLoginAbierto(true); }}
        onPublicar={handlePublicar}
        onBilletera={handleBilletera}
        onActividades={handleActividades}
        saldoBilletera={saldoBilletera}
      />

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 flex-1 w-full" id="catalogo">
        <HeroBanner />
        <FilterBar filtros={filtros} onChangeFiltros={setFiltros} categorias={categorias} totalResultados={totalResultados} vistaModo={vistaModo} onToggleVista={setVistaModo} onResetFiltros={() => setFiltros({ ...FILTROS_INICIALES })} />

        {errorCatalogo && <div className="mb-6 p-4 rounded-xl bg-red-50 border border-red-200 text-sm text-red-800"><strong>No se pudo cargar el catálogo.</strong> {errorCatalogo}</div>}
        {cargando ? <div className="py-16 text-center text-sm text-slate-500">Cargando subastas...</div> : subastas.length === 0 ? <EmptyState onReset={() => setFiltros({ ...FILTROS_INICIALES })} /> : vistaModo === 'grid' ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">{subastas.map((subasta) => <AuctionCard key={subasta.id} subasta={subasta} onSelect={handleSelectSubasta} />)}</div>
        ) : (
          <div className="space-y-4">{subastas.map((subasta) => <AuctionListItem key={subasta.id} subasta={subasta} onSelect={handleSelectSubasta} />)}</div>
        )}
      </main>

      <AuctionDetailModal
        subasta={subastaSeleccionada}
        onClose={() => setSubastaSeleccionada(null)}
        onLogin={() => { setAccionPostLogin(null); setLoginAbierto(true); }}
        onPlaceBid={handlePlaceBid}
      />
      <PublicarSubastaModal abierto={publicarAbierto} categorias={categorias} onClose={() => setPublicarAbierto(false)} onCreada={handleSubastaCreada} />
      <BilleteraModal abierto={billeteraAbierta} saldo={saldoBilletera} cargandoSaldo={cargandoSaldo} onClose={() => setBilleteraAbierta(false)} onSaldoActualizado={setSaldoBilletera} />
      <MisActividadesModal abierto={actividadesAbiertas} onClose={() => setActividadesAbiertas(false)} onAbrirSubasta={(subastaId) => void abrirSubastaPorId(subastaId)} />
      <LoginModal abierto={loginAbierto} onClose={() => { setLoginAbierto(false); setAccionPostLogin(null); }} onSuccess={handleLoginSuccess} />
      <ToastNotification toasts={toasts} onDismiss={(id) => setToasts((prev) => prev.filter((toast) => toast.id !== id))} />

      <footer className="bg-white border-t border-slate-200 mt-16 py-8 text-xs text-slate-500"><div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex items-center justify-center"><p><strong>SubastaYa</strong> &copy; {new Date().getFullYear()}</p></div></footer>
    </div>
  );
}

export default App;

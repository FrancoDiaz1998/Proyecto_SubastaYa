import { useEffect, useMemo, useState, type FormEvent } from 'react';
import { CalendarClock, CircleDollarSign, ImageIcon, PackagePlus, X } from 'lucide-react';
import { crearSubasta } from '../api/subastasApi';
import { ApiError } from '../api/apiClient';
import { useAuth } from '../auth/AuthContext';
import type { Categoria, CrearSubastaResponse } from '../types/auction';
import { formatCurrency } from '../utils/formatters';

interface Props {
  abierto: boolean;
  categorias: Categoria[];
  onClose: () => void;
  onCreada: (subasta: CrearSubastaResponse) => void;
}

interface ErroresFormulario {
  titulo?: string;
  descripcion?: string;
  urlImagen?: string;
  categoriaId?: string;
  precioBase?: string;
  incrementoMinimo?: string;
  fechaInicio?: string;
  fechaFin?: string;
}

const aValorLocal = (fecha: Date) => {
  const local = new Date(fecha.getTime() - fecha.getTimezoneOffset() * 60000);
  return local.toISOString().slice(0, 16);
};

const crearValoresIniciales = () => {
  const inicio = new Date(Date.now() + 10 * 60 * 1000);
  const fin = new Date(inicio.getTime() + 24 * 60 * 60 * 1000);
  return { titulo: '', descripcion: '', urlImagen: '', categoriaId: '', precioBase: '', incrementoMinimo: '', fechaInicio: aValorLocal(inicio), fechaFin: aValorLocal(fin) };
};

export function PublicarSubastaModal({ abierto, categorias, onClose, onCreada }: Props) {
  const { usuario } = useAuth();
  const [form, setForm] = useState(crearValoresIniciales);
  const [errores, setErrores] = useState<ErroresFormulario>({});
  const [errorApi, setErrorApi] = useState('');
  const [enviando, setEnviando] = useState(false);
  const [imagenValida, setImagenValida] = useState(true);

  useEffect(() => {
    if (!abierto) return;
    setForm(crearValoresIniciales()); setErrores({}); setErrorApi(''); setImagenValida(true);
  }, [abierto]);

  const categoriaSeleccionada = useMemo(
    () => categorias.find((categoria) => String(categoria.id) === form.categoriaId),
    [categorias, form.categoriaId],
  );

  if (!abierto || !usuario) return null;

  const cambiar = (campo: keyof typeof form, valor: string) => {
    setForm((actual) => ({ ...actual, [campo]: valor }));
    setErrores((actual) => ({ ...actual, [campo]: undefined }));
    setErrorApi('');
  };

  const validar = () => {
    const nuevos: ErroresFormulario = {};
    const titulo = form.titulo.trim();
    const descripcion = form.descripcion.trim();
    const urlImagen = form.urlImagen.trim();
    const precioBase = Number(form.precioBase);
    const incrementoMinimo = Number(form.incrementoMinimo);
    const inicio = new Date(form.fechaInicio);
    const fin = new Date(form.fechaFin);

    if (titulo.length < 3) nuevos.titulo = 'Ingresá al menos 3 caracteres.';
    else if (titulo.length > 150) nuevos.titulo = 'El título no puede superar 150 caracteres.';
    if (descripcion.length < 10) nuevos.descripcion = 'Ingresá una descripción de al menos 10 caracteres.';
    else if (descripcion.length > 4000) nuevos.descripcion = 'La descripción no puede superar 4000 caracteres.';

    try {
      const url = new URL(urlImagen);
      if (!['http:', 'https:'].includes(url.protocol)) nuevos.urlImagen = 'La URL debe comenzar con http:// o https://.';
    } catch { nuevos.urlImagen = 'Ingresá una URL de imagen válida.'; }

    if (form.categoriaId === '') nuevos.categoriaId = 'Seleccioná una categoría.';
    if (!Number.isFinite(precioBase) || precioBase <= 0) nuevos.precioBase = 'El precio base debe ser mayor a cero.';
    if (!Number.isFinite(incrementoMinimo) || incrementoMinimo <= 0) nuevos.incrementoMinimo = 'El incremento mínimo debe ser mayor a cero.';
    if (!form.fechaInicio || Number.isNaN(inicio.getTime())) nuevos.fechaInicio = 'Ingresá una fecha de inicio válida.';
    if (!form.fechaFin || Number.isNaN(fin.getTime())) nuevos.fechaFin = 'Ingresá una fecha de finalización válida.';
    else if (!nuevos.fechaInicio && fin <= inicio) nuevos.fechaFin = 'La finalización debe ser posterior al inicio.';
    else if (fin <= new Date()) nuevos.fechaFin = 'La fecha de finalización debe ser futura.';

    setErrores(nuevos);
    return Object.keys(nuevos).length === 0;
  };

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    if (!validar()) return;

    setEnviando(true); setErrorApi('');
    try {
      const creada = await crearSubasta({
        titulo: form.titulo.trim(), descripcion: form.descripcion.trim(), urlImagen: form.urlImagen.trim(),
        categoriaId: Number(form.categoriaId), precioBase: Number(form.precioBase), incrementoMinimo: Number(form.incrementoMinimo),
        fechaInicio: new Date(form.fechaInicio).toISOString(), fechaFin: new Date(form.fechaFin).toISOString(),
      });
      onCreada(creada);
    } catch (error) {
      setErrorApi(error instanceof ApiError ? error.message : 'No se pudo publicar la subasta.');
    } finally { setEnviando(false); }
  };

  const inputBase = 'mt-1 w-full px-3 py-2.5 rounded-xl border bg-white text-sm text-slate-900 focus:outline-none focus:ring-2 focus:ring-blue-100';
  const claseInput = (error?: string) => `${inputBase} ${error ? 'border-red-300 focus:border-red-500' : 'border-slate-300 focus:border-blue-500'}`;

  return (
    <div className="fixed inset-0 z-[70] bg-slate-950/65 backdrop-blur-sm overflow-y-auto p-3 sm:p-6" onClick={onClose}>
      <div className="min-h-full flex items-center justify-center">
        <div className="w-full max-w-5xl bg-white rounded-3xl shadow-2xl border border-slate-200 overflow-hidden" onClick={(event) => event.stopPropagation()}>
          <div className="px-5 sm:px-7 py-5 border-b border-slate-100 flex items-start justify-between gap-4 bg-slate-50/70">
            <div className="flex items-center gap-3">
              <div className="w-11 h-11 rounded-2xl bg-blue-600 text-white flex items-center justify-center"><PackagePlus className="w-5 h-5" /></div>
              <div><h2 className="text-lg font-black text-slate-900">Publicar nueva subasta</h2><p className="text-xs text-slate-500">Se publicará como <strong>{usuario.nombre}</strong> · {usuario.email}</p></div>
            </div>
            <button type="button" onClick={onClose} className="p-2 rounded-full text-slate-400 hover:text-slate-700 hover:bg-slate-200 cursor-pointer"><X className="w-5 h-5" /></button>
          </div>

          <form onSubmit={handleSubmit} className="p-5 sm:p-7 grid grid-cols-1 lg:grid-cols-12 gap-6">
            <div className="lg:col-span-7 space-y-6">
              <section>
                <div className="flex items-center gap-2 mb-3"><PackagePlus className="w-4 h-4 text-blue-600" /><h3 className="text-sm font-black text-slate-900">Producto</h3></div>
                <div className="space-y-4">
                  <div><label className="text-xs font-bold text-slate-600">Título</label><input maxLength={150} value={form.titulo} onChange={(e) => cambiar('titulo', e.target.value)} placeholder="Ej: Cámara Sony Alpha A7 III" className={claseInput(errores.titulo)} /><div className="flex justify-between mt-1"><span className="text-[10px] text-red-600">{errores.titulo}</span><span className="text-[10px] text-slate-400">{form.titulo.length}/150</span></div></div>
                  <div><label className="text-xs font-bold text-slate-600">Descripción detallada</label><textarea maxLength={4000} rows={5} value={form.descripcion} onChange={(e) => cambiar('descripcion', e.target.value)} placeholder="Estado, características, accesorios incluidos..." className={claseInput(errores.descripcion)} /><div className="flex justify-between mt-1"><span className="text-[10px] text-red-600">{errores.descripcion}</span><span className="text-[10px] text-slate-400">{form.descripcion.length}/4000</span></div></div>
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    <div><label className="text-xs font-bold text-slate-600">Categoría</label><select value={form.categoriaId} onChange={(e) => cambiar('categoriaId', e.target.value)} className={claseInput(errores.categoriaId)}><option value="">Seleccionar...</option>{categorias.map((categoria) => <option key={categoria.id} value={categoria.id}>{categoria.nombre}</option>)}</select>{errores.categoriaId && <p className="mt-1 text-[10px] text-red-600">{errores.categoriaId}</p>}</div>
                    <div><label className="text-xs font-bold text-slate-600">URL de imagen</label><input type="url" maxLength={2048} value={form.urlImagen} onChange={(e) => { cambiar('urlImagen', e.target.value); setImagenValida(true); }} placeholder="https://..." className={claseInput(errores.urlImagen)} />{errores.urlImagen && <p className="mt-1 text-[10px] text-red-600">{errores.urlImagen}</p>}</div>
                  </div>
                </div>
              </section>

              <section className="pt-5 border-t border-slate-100">
                <div className="flex items-center gap-2 mb-3"><CircleDollarSign className="w-4 h-4 text-emerald-600" /><h3 className="text-sm font-black text-slate-900">Configuración económica</h3></div>
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div><label className="text-xs font-bold text-slate-600">Precio base (ARS)</label><input type="number" min="1" step="1" value={form.precioBase} onChange={(e) => cambiar('precioBase', e.target.value)} placeholder="35000" className={claseInput(errores.precioBase)} />{errores.precioBase && <p className="mt-1 text-[10px] text-red-600">{errores.precioBase}</p>}</div>
                  <div><label className="text-xs font-bold text-slate-600">Incremento mínimo (ARS)</label><input type="number" min="1" step="1" value={form.incrementoMinimo} onChange={(e) => cambiar('incrementoMinimo', e.target.value)} placeholder="2000" className={claseInput(errores.incrementoMinimo)} />{errores.incrementoMinimo && <p className="mt-1 text-[10px] text-red-600">{errores.incrementoMinimo}</p>}</div>
                </div>
              </section>

              <section className="pt-5 border-t border-slate-100">
                <div className="flex items-center gap-2 mb-3"><CalendarClock className="w-4 h-4 text-violet-600" /><h3 className="text-sm font-black text-slate-900">Ventana temporal</h3></div>
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div><label className="text-xs font-bold text-slate-600">Fecha y hora de inicio</label><input type="datetime-local" value={form.fechaInicio} onChange={(e) => cambiar('fechaInicio', e.target.value)} className={claseInput(errores.fechaInicio)} />{errores.fechaInicio && <p className="mt-1 text-[10px] text-red-600">{errores.fechaInicio}</p>}</div>
                  <div><label className="text-xs font-bold text-slate-600">Fecha y hora de finalización</label><input type="datetime-local" value={form.fechaFin} onChange={(e) => cambiar('fechaFin', e.target.value)} className={claseInput(errores.fechaFin)} />{errores.fechaFin && <p className="mt-1 text-[10px] text-red-600">{errores.fechaFin}</p>}</div>
                </div>
                <p className="mt-2 text-[10px] text-slate-500">Las subastas con fecha futura comenzarán automáticamente cuando llegue el momento indicado.</p>
              </section>
            </div>

            <aside className="lg:col-span-5">
              <div className="sticky top-5 space-y-4">
                <div className="rounded-2xl border border-slate-200 overflow-hidden bg-slate-50">
                  <div className="aspect-16/10 bg-slate-100 flex items-center justify-center overflow-hidden">
                    {form.urlImagen && imagenValida ? <img src={form.urlImagen} alt="Vista previa" onError={() => setImagenValida(false)} className="w-full h-full object-cover" /> : <div className="text-center text-slate-400"><ImageIcon className="w-8 h-8 mx-auto mb-2" /><span className="text-xs">Vista previa de la imagen</span></div>}
                  </div>
                  <div className="p-4">
                    <div className="flex items-center justify-between gap-2"><span className="text-[10px] uppercase font-bold text-blue-600">{categoriaSeleccionada?.nombre ?? 'Categoría'}</span><span className="text-[10px] text-slate-400">Previsualización</span></div>
                    <h4 className="mt-1 font-black text-slate-900 line-clamp-2">{form.titulo.trim() || 'Título de la subasta'}</h4>
                    <p className="mt-1 text-xs text-slate-500 line-clamp-3">{form.descripcion.trim() || 'La descripción aparecerá acá.'}</p>
                    <div className="mt-4 pt-3 border-t border-slate-200 grid grid-cols-2 gap-3">
                      <div><span className="block text-[10px] uppercase text-slate-400 font-bold">Precio base</span><strong className="text-sm text-slate-900">{form.precioBase ? formatCurrency(Number(form.precioBase)) : '—'}</strong></div>
                      <div><span className="block text-[10px] uppercase text-slate-400 font-bold">Incremento</span><strong className="text-sm text-slate-900">{form.incrementoMinimo ? `+ ${formatCurrency(Number(form.incrementoMinimo))}` : '—'}</strong></div>
                    </div>
                  </div>
                </div>

                {errorApi && <div className="p-3 rounded-xl bg-red-50 border border-red-200 text-xs font-semibold text-red-700">{errorApi}</div>}
                <div className="flex gap-2"><button type="button" onClick={onClose} disabled={enviando} className="flex-1 py-2.5 rounded-xl border border-slate-300 text-xs font-bold text-slate-700 hover:bg-slate-50 cursor-pointer disabled:opacity-50">Cancelar</button><button type="submit" disabled={enviando || categorias.length === 0} className="flex-[1.4] py-2.5 rounded-xl bg-blue-600 hover:bg-blue-700 text-white text-xs font-bold cursor-pointer disabled:opacity-50">{enviando ? 'Publicando...' : 'Publicar subasta'}</button></div>
              </div>
            </aside>
          </form>
        </div>
      </div>
    </div>
  );
}

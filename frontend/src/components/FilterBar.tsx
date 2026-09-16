import React from 'react';
import type { Categoria, FiltrosSubasta, TipoOrden } from '../types/auction';
import {
  Search,
  Filter,
  X,
  SlidersHorizontal,
  ArrowUpDown,
  LayoutGrid,
  List,
  Sparkles,
} from 'lucide-react';

interface FilterBarProps {
  filtros: FiltrosSubasta;
  onChangeFiltros: (nuevosFiltros: FiltrosSubasta) => void;
  categorias: Categoria[];
  totalResultados: number;
  vistaModo: 'grid' | 'list';
  onToggleVista: (modo: 'grid' | 'list') => void;
  onResetFiltros: () => void;
}

export const FilterBar: React.FC<FilterBarProps> = ({
  filtros,
  onChangeFiltros,
  categorias,
  totalResultados,
  vistaModo,
  onToggleVista,
  onResetFiltros,
}) => {
  const [mostrarFiltrosAvanzados, setMostrarFiltrosAvanzados] = React.useState(false);

  const handleBusquedaChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    onChangeFiltros({ ...filtros, busqueda: e.target.value });
  };

  const handleEstadoChange = (estado: FiltrosSubasta['estado']) => {
    onChangeFiltros({ ...filtros, estado });
  };

  const handleCategoriaChange = (categoriaId: number | 'todas') => {
    onChangeFiltros({ ...filtros, categoriaId });
  };

  const handleOrdenChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    onChangeFiltros({ ...filtros, orden: e.target.value as TipoOrden });
  };

  const handlePrecioMinChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value === '' ? '' : Math.max(0, Number(e.target.value));
    onChangeFiltros({ ...filtros, precioMin: val });
  };

  const handlePrecioMaxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value === '' ? '' : Math.max(0, Number(e.target.value));
    onChangeFiltros({ ...filtros, precioMax: val });
  };

  const hayFiltrosActivos =
    filtros.busqueda.trim() !== '' ||
    filtros.estado !== 'todas' ||
    filtros.categoriaId !== 'todas' ||
    filtros.precioMin !== '' ||
    filtros.precioMax !== '' ||
    filtros.orden !== 'tiempo_asc';

  return (
    <section className="bg-white rounded-2xl shadow-xs border border-slate-200/80 p-4 md:p-6 mb-8">
      {/* Fila 1: Barra de Búsqueda y Selector de Ordenamiento */}
      <div className="flex flex-col md:flex-row gap-3 items-stretch md:items-center justify-between">
        {/* Input de búsqueda */}
        <div className="relative flex-1">
          <div className="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none text-slate-400">
            <Search className="w-4 h-4" />
          </div>
          <input
            type="text"
            value={filtros.busqueda}
            onChange={handleBusquedaChange}
            placeholder="Buscar por título, marca o especificación (ej: MacBook, Omega, PS5)..."
            className="w-full pl-10 pr-10 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-sm text-slate-900 placeholder-slate-400 focus:outline-none focus:border-blue-500 focus:bg-white focus:ring-2 focus:ring-blue-100 transition-all"
          />
          {filtros.busqueda && (
            <button
              type="button"
              onClick={() => onChangeFiltros({ ...filtros, busqueda: '' })}
              className="absolute inset-y-0 right-0 pr-3 flex items-center text-slate-400 hover:text-slate-600"
            >
              <X className="w-4 h-4" />
            </button>
          )}
        </div>

        {/* Controles de la derecha: Ordenamiento y Botón de Filtros Avanzados */}
        <div className="flex items-center gap-2">
          {/* Selector de Orden */}
          <div className="relative flex items-center">
            <div className="absolute left-3 text-slate-400 pointer-events-none">
              <ArrowUpDown className="w-3.5 h-3.5" />
            </div>
            <select
              value={filtros.orden}
              onChange={handleOrdenChange}
              className="pl-9 pr-8 py-2.5 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-700 focus:outline-none focus:border-blue-500 focus:bg-white cursor-pointer appearance-none"
            >
              <option value="tiempo_asc">Menor tiempo restante (Cierre)</option>
              <option value="puja_desc">Mayor puja actual</option>
            </select>
          </div>

          {/* Botón toggle filtros avanzados (precio / categorías) */}
          <button
            type="button"
            onClick={() => setMostrarFiltrosAvanzados(!mostrarFiltrosAvanzados)}
            className={`flex items-center gap-1.5 px-3 py-2.5 rounded-xl text-xs font-semibold border transition-colors cursor-pointer ${
              mostrarFiltrosAvanzados || filtros.precioMin !== '' || filtros.precioMax !== ''
                ? 'bg-blue-50 text-blue-700 border-blue-200'
                : 'bg-slate-50 text-slate-700 border-slate-200 hover:bg-slate-100'
            }`}
          >
            <SlidersHorizontal className="w-3.5 h-3.5" />
            <span className="hidden sm:inline">Precios</span>
          </button>

          {/* Selector de Vista (Grid vs Lista) */}
          <div className="hidden sm:flex items-center bg-slate-100 p-1 rounded-xl border border-slate-200">
            <button
              type="button"
              onClick={() => onToggleVista('grid')}
              className={`p-1.5 rounded-lg transition-colors cursor-pointer ${
                vistaModo === 'grid'
                  ? 'bg-white text-blue-600 shadow-xs'
                  : 'text-slate-500 hover:text-slate-800'
              }`}
              title="Vista en cuadrícula"
            >
              <LayoutGrid className="w-4 h-4" />
            </button>
            <button
              type="button"
              onClick={() => onToggleVista('list')}
              className={`p-1.5 rounded-lg transition-colors cursor-pointer ${
                vistaModo === 'list'
                  ? 'bg-white text-blue-600 shadow-xs'
                  : 'text-slate-500 hover:text-slate-800'
              }`}
              title="Vista en lista"
            >
              <List className="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>

      {/* Fila 2: Pestañas de Estado (Activas, Próximas, Finalizadas) */}
      <div className="mt-4 pt-4 border-t border-slate-100 flex flex-wrap items-center justify-between gap-3">
        <div className="flex flex-wrap items-center gap-1.5">
          <span className="text-xs font-semibold text-slate-400 mr-2 flex items-center gap-1">
            <Filter className="w-3 h-3" /> Estado:
          </span>

          <button
            type="button"
            onClick={() => handleEstadoChange('todas')}
            className={`px-3 py-1.5 rounded-full text-xs font-semibold transition-all cursor-pointer ${
              filtros.estado === 'todas'
                ? 'bg-slate-900 text-white shadow-xs'
                : 'bg-slate-100 text-slate-600 hover:bg-slate-200'
            }`}
          >
            Todas
          </button>

          <button
            type="button"
            onClick={() => handleEstadoChange('Activa')}
            className={`inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full text-xs font-semibold transition-all cursor-pointer ${
              filtros.estado === 'Activa'
                ? 'bg-emerald-600 text-white shadow-xs ring-2 ring-emerald-200'
                : 'bg-emerald-50 text-emerald-700 hover:bg-emerald-100 border border-emerald-200/60'
            }`}
          >
            <span className="w-1.5 h-1.5 rounded-full bg-emerald-400 live-indicator-dot" />
            Activas en Vivo
          </button>

          <button
            type="button"
            onClick={() => handleEstadoChange('Programada')}
            className={`px-3 py-1.5 rounded-full text-xs font-semibold transition-all cursor-pointer ${
              filtros.estado === 'Programada'
                ? 'bg-blue-600 text-white shadow-xs ring-2 ring-blue-200'
                : 'bg-blue-50 text-blue-700 hover:bg-blue-100 border border-blue-200/60'
            }`}
          >
            Próximas (+24h)
          </button>

          <button
            type="button"
            onClick={() => handleEstadoChange('Finalizada')}
            className={`px-3 py-1.5 rounded-full text-xs font-semibold transition-all cursor-pointer ${
              filtros.estado === 'Finalizada'
                ? 'bg-slate-700 text-white shadow-xs ring-2 ring-slate-300'
                : 'bg-slate-100 text-slate-700 hover:bg-slate-200 border border-slate-200'
            }`}
          >
            Finalizadas
          </button>
        </div>

        {/* Resumen de resultados y botón reset */}
        <div className="flex items-center gap-3 ml-auto">
          <span className="text-xs text-slate-500 font-medium">
            <strong className="text-slate-900 font-bold">{totalResultados}</strong>{' '}
            {totalResultados === 1 ? 'subasta' : 'subastas'}
          </span>

          {hayFiltrosActivos && (
            <button
              type="button"
              onClick={onResetFiltros}
              className="text-xs font-medium text-red-600 hover:text-red-700 flex items-center gap-1 hover:underline cursor-pointer"
            >
              <X className="w-3 h-3" />
              Limpiar filtros
            </button>
          )}
        </div>
      </div>

      {/* Fila 3: Filtro por Categorías (Chips rápidos) */}
      <div className="mt-3 flex items-center gap-2 overflow-x-auto pb-1 text-xs">
        <button
          type="button"
          onClick={() => handleCategoriaChange('todas')}
          className={`category-chip px-3 py-1.5 rounded-lg font-medium whitespace-nowrap cursor-pointer ${
            filtros.categoriaId === 'todas'
              ? 'bg-blue-600 text-white shadow-xs font-semibold active'
              : 'bg-slate-50 text-slate-600 border border-slate-200/80'
          }`}
        >
          Todas las categorías
        </button>

        {categorias.map((cat) => {
          const isSelected = filtros.categoriaId === cat.id;
          return (
            <button
              key={cat.id}
              type="button"
              onClick={() => handleCategoriaChange(cat.id)}
              className={`category-chip px-3 py-1.5 rounded-lg font-medium whitespace-nowrap cursor-pointer flex items-center gap-1.5 ${
                isSelected
                  ? 'bg-blue-600 text-white shadow-xs font-semibold active'
                  : 'bg-slate-50 text-slate-600 border border-slate-200/80'
              }`}
            >
              <Sparkles className={`w-3 h-3 ${isSelected ? 'text-white' : 'text-blue-500'}`} />
              {cat.nombre}
            </button>
          );
        })}
      </div>

      {/* Panel Plegable: Rango de Precios */}
      {mostrarFiltrosAvanzados && (
        <div className="mt-4 pt-4 border-t border-slate-100 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3 bg-slate-50/80 p-3 rounded-xl">
          <div>
            <label className="block text-[11px] font-bold text-slate-600 uppercase tracking-wider mb-1">
              Precio Mínimo ($)
            </label>
            <input
              type="number"
              min="0"
              step="1000"
              value={filtros.precioMin}
              onChange={handlePrecioMinChange}
              placeholder="Ej: 20000"
              className="w-full px-3 py-1.5 bg-white border border-slate-200 rounded-lg text-xs text-slate-800 focus:outline-none focus:border-blue-500"
            />
          </div>

          <div>
            <label className="block text-[11px] font-bold text-slate-600 uppercase tracking-wider mb-1">
              Precio Máximo ($)
            </label>
            <input
              type="number"
              min="0"
              step="5000"
              value={filtros.precioMax}
              onChange={handlePrecioMaxChange}
              placeholder="Ej: 200000"
              className="w-full px-3 py-1.5 bg-white border border-slate-200 rounded-lg text-xs text-slate-800 focus:outline-none focus:border-blue-500"
            />
          </div>

          <div className="sm:col-span-2 flex items-end">
            <p className="text-[11px] text-slate-500 italic">
              Filtra según la oferta más alta o precio base. Los precios están expresados en pesos argentinos (ARS).
            </p>
          </div>
        </div>
      )}
    </section>
  );
};

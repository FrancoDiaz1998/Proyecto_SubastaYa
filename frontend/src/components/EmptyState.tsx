import React from 'react';
import { SearchX, RotateCcw } from 'lucide-react';

interface EmptyStateProps {
  onReset: () => void;
}

export const EmptyState: React.FC<EmptyStateProps> = ({ onReset }) => {
  return (
    <div className="bg-white border border-slate-200 rounded-2xl p-12 text-center max-w-lg mx-auto my-12 shadow-xs">
      <div className="w-16 h-16 bg-blue-50 text-blue-600 rounded-full flex items-center justify-center mx-auto mb-4">
        <SearchX className="w-8 h-8" />
      </div>

      <h3 className="text-lg font-bold text-slate-900 mb-1">
        No se encontraron subastas
      </h3>

      <p className="text-xs text-slate-500 max-w-sm mx-auto mb-6 leading-relaxed">
        No hay publicaciones que coincidan con los filtros o términos de búsqueda seleccionados.
        Prueba ajustando el rango de precios o cambiando de categoría.
      </p>

      <button
        type="button"
        onClick={onReset}
        className="btn-primary-action inline-flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-xl text-xs font-bold hover:bg-blue-700 cursor-pointer shadow-xs"
      >
        <RotateCcw className="w-3.5 h-3.5" />
        Restablecer todos los filtros
      </button>
    </div>
  );
};


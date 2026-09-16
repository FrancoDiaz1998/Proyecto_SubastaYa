import React from 'react';
import { CheckCircle2, AlertTriangle, AlertCircle, X } from 'lucide-react';

export interface ToastData {
  id: string;
  tipo: 'success' | 'warning' | 'error';
  titulo: string;
  mensaje: string;
}

interface ToastNotificationProps {
  toasts: ToastData[];
  onDismiss: (id: string) => void;
}

export const ToastNotification: React.FC<ToastNotificationProps> = ({ toasts, onDismiss }) => {
  return (
    <div className="fixed bottom-5 right-5 z-50 flex flex-col gap-2.5 max-w-sm w-full pointer-events-none">
      {toasts.map((toast) => (
        <div
          key={toast.id}
          className={`pointer-events-auto p-4 rounded-2xl shadow-xl border flex items-start gap-3 transform transition-all duration-300 animate-in slide-in-from-bottom-5 ${
            toast.tipo === 'success'
              ? 'bg-slate-900 text-white border-slate-700'
              : toast.tipo === 'warning'
              ? 'bg-amber-950 text-amber-100 border-amber-800'
              : 'bg-red-950 text-red-100 border-red-800'
          }`}
        >
          {toast.tipo === 'success' && (
            <CheckCircle2 className="w-5 h-5 text-emerald-400 shrink-0 mt-0.5" />
          )}
          {toast.tipo === 'warning' && (
            <AlertTriangle className="w-5 h-5 text-amber-400 shrink-0 mt-0.5 animate-pulse" />
          )}
          {toast.tipo === 'error' && (
            <AlertCircle className="w-5 h-5 text-red-400 shrink-0 mt-0.5" />
          )}

          <div className="flex-1 min-w-0">
            <h4 className="text-xs font-bold leading-tight">{toast.titulo}</h4>
            <p className="text-[11px] opacity-80 mt-0.5 leading-snug">{toast.mensaje}</p>
          </div>

          <button
            type="button"
            onClick={() => onDismiss(toast.id)}
            className="text-white/60 hover:text-white p-1 rounded-md"
          >
            <X className="w-3.5 h-3.5" />
          </button>
        </div>
      ))}
    </div>
  );
};

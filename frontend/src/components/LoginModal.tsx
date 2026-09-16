import { useState, type FormEvent } from 'react';
import { Gavel, LogIn, X } from 'lucide-react';
import { ApiError } from '../api/apiClient';
import { useAuth } from '../auth/AuthContext';

const USUARIOS_DEMO = [
  ['Vendedor', 'vendedor@test.com'],
  ['Comprador 1', 'comprador1@test.com'],
  ['Comprador 2', 'comprador2@test.com'],
  ['Sin fondos', 'sinfondos@test.com'],
] as const;

interface LoginModalProps { abierto: boolean; onClose: () => void; }

export function LoginModal({ abierto, onClose }: LoginModalProps) {
  const { iniciarSesion } = useAuth();
  const [email, setEmail] = useState('comprador1@test.com');
  const [password, setPassword] = useState('Demo123!');
  const [error, setError] = useState('');
  const [cargando, setCargando] = useState(false);

  if (!abierto) return null;

  const seleccionarDemo = (demoEmail: string) => {
    setEmail(demoEmail); setPassword('Demo123!'); setError('');
  };

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault(); setError(''); setCargando(true);
    try { await iniciarSesion(email, password); onClose(); }
    catch (err) { setError(err instanceof ApiError ? err.message : 'No se pudo iniciar sesión.'); }
    finally { setCargando(false); }
  };

  return (
    <div className="fixed inset-0 z-[70] bg-slate-950/60 backdrop-blur-sm flex items-center justify-center p-4" onClick={onClose}>
      <div className="w-full max-w-md bg-white rounded-3xl shadow-2xl border border-slate-200 overflow-hidden" onClick={(e) => e.stopPropagation()}>
        <div className="p-6 pb-4 flex items-start justify-between">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 rounded-xl bg-blue-600 text-white flex items-center justify-center"><Gavel className="w-5 h-5" /></div>
            <div><h2 className="font-black text-slate-900">Iniciar sesión</h2><p className="text-xs text-slate-500">Accedé con uno de los usuarios semilla.</p></div>
          </div>
          <button type="button" onClick={onClose} className="p-1.5 rounded-full text-slate-400 hover:bg-slate-100 cursor-pointer"><X className="w-5 h-5" /></button>
        </div>

        <form onSubmit={handleSubmit} className="px-6 pb-6 space-y-4">
          <div>
            <label className="text-xs font-bold text-slate-600">Email</label>
            <input type="email" required value={email} onChange={(e) => setEmail(e.target.value)} className="mt-1 w-full px-3 py-2.5 rounded-xl border border-slate-300 text-sm focus:outline-none focus:border-blue-500" />
          </div>
          <div>
            <label className="text-xs font-bold text-slate-600">Contraseña</label>
            <input type="password" required value={password} onChange={(e) => setPassword(e.target.value)} className="mt-1 w-full px-3 py-2.5 rounded-xl border border-slate-300 text-sm focus:outline-none focus:border-blue-500" />
          </div>

          {error && <p className="text-xs font-semibold text-red-700 bg-red-50 border border-red-200 rounded-xl p-3">{error}</p>}

          <button type="submit" disabled={cargando} className="w-full py-2.5 rounded-xl bg-blue-600 hover:bg-blue-700 disabled:opacity-60 text-white text-sm font-bold flex items-center justify-center gap-2 cursor-pointer">
            <LogIn className="w-4 h-4" />{cargando ? 'Ingresando...' : 'Ingresar'}
          </button>

          <div className="pt-3 border-t border-slate-100">
            <p className="text-[11px] font-bold uppercase tracking-wider text-slate-400 mb-2">Usuarios demo · clave Demo123!</p>
            <div className="grid grid-cols-2 gap-2">
              {USUARIOS_DEMO.map(([nombre, demoEmail]) => (
                <button key={demoEmail} type="button" onClick={() => seleccionarDemo(demoEmail)} className="text-left px-3 py-2 rounded-xl bg-slate-50 border border-slate-200 hover:border-blue-300 hover:bg-blue-50 cursor-pointer">
                  <span className="block text-xs font-bold text-slate-800">{nombre}</span><span className="block text-[10px] text-slate-500 truncate">{demoEmail}</span>
                </button>
              ))}
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}

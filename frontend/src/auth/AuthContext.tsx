import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';
import { login as loginApi, obtenerSesion } from '../api/authApi';
import { eliminarToken, guardarToken, obtenerToken } from '../api/apiClient';
import type { UsuarioSesion } from '../types/auth';

interface AuthContextValue {
  usuario: UsuarioSesion | null;
  cargandoSesion: boolean;
  iniciarSesion: (email: string, password: string) => Promise<void>;
  cerrarSesion: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [usuario, setUsuario] = useState<UsuarioSesion | null>(null);
  const [cargandoSesion, setCargandoSesion] = useState(true);

  useEffect(() => {
    if (!obtenerToken()) { setCargandoSesion(false); return; }

    obtenerSesion()
      .then(setUsuario)
      .catch(() => eliminarToken())
      .finally(() => setCargandoSesion(false));
  }, []);

  const iniciarSesion = async (email: string, password: string) => {
    const respuesta = await loginApi({ email, password });
    guardarToken(respuesta.token);
    setUsuario(respuesta.usuario);
  };

  const cerrarSesion = () => {
    eliminarToken();
    setUsuario(null);
  };

  return <AuthContext.Provider value={{ usuario, cargandoSesion, iniciarSesion, cerrarSesion }}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth debe utilizarse dentro de AuthProvider.');
  return context;
}

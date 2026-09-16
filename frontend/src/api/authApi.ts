import { apiRequest } from './apiClient';
import type { LoginRequest, LoginResponse, UsuarioSesion } from '../types/auth';

export const login = (request: LoginRequest) =>
  apiRequest<LoginResponse>('/api/v1/auth/login', {
    method: 'POST', body: JSON.stringify(request),
  });

export const obtenerSesion = () => apiRequest<UsuarioSesion>('/api/v1/auth/me');

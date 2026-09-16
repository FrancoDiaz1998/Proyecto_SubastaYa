export interface UsuarioSesion {
  id: string;
  nombre: string;
  email: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiraEn: string;
  usuario: UsuarioSesion;
}

import { http } from './http';
import { LoginResponse } from './types';

export async function login(login: string, senha: string) {
  const { data } = await http.post<LoginResponse>('/api/Auth/login', { login, senha });
  localStorage.setItem('token', data.token);
  localStorage.setItem('nome', data.nome);
  localStorage.setItem('usuario', data.usuario);
  return data;
}

export function logout() {
  localStorage.removeItem('token');
  localStorage.removeItem('nome');
  localStorage.removeItem('usuario');
}

export function isAuthenticated() {
  return Boolean(localStorage.getItem('token'));
}


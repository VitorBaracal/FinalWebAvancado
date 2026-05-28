import { http } from './http';

export async function cadastrarUsuario(payload: {
  nome: string;
  login: string;
  senha: string;
  confirmarSenha: string;
}) {
  await http.post('/api/Usuarios', payload);
}


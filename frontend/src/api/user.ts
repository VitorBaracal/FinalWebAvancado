import { http } from './http';

export async function cadastrarUsuario(payload: {
  nome: string;
  login: string;
  senha: string;
  confirmarSenha: string;
}) {
  await http.post('/api/User', {
    name: payload.nome,
    login: payload.login,
    password: payload.senha,
    confirmPassword: payload.confirmarSenha,
  });
}
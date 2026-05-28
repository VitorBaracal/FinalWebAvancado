import { http } from './http';
import { CategoryDto } from './types';

export async function listarCategorias() {
  const { data } = await http.get<CategoryDto[]>('/api/Category');
  return data;
}

export async function criarCategoria(name: string) {
  const { data } = await http.post<CategoryDto>(
    '/api/Category',
    { name }
  );

  return data;
}

export async function atualizarCategoria(
  id: number,
  name: string
) {
  await http.put(
    `/api/Category/${id}`,
    { name }
  );
}

export async function removerCategoria(id: number) {
  await http.delete(`/api/Category/${id}`);
}
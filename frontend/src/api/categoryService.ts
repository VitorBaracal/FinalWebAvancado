import { http } from './http';
import { CategoryDto } from './types';

export async function listarCategorias() {
  const { data } =
    await http.get<CategoryDto[]>(
      '/api/Category'
    );

  return data;
}

export async function criarCategoria(
  userId:number,
  name:string,
  colorHex:string
){
  const { data } =
    await http.post<CategoryDto>(
      '/api/Category',
      {
        userId,
        name,
        colorHex
      }
    );

  return data;
}

export async function atualizarCategoria(
  id:number,
  userId:number,
  name:string,
  colorHex:string
){
  await http.put(
    `/api/Category/${id}`,
    {
      userId,
      name,
      colorHex
    }
  );
}

export async function removerCategoria(
  id:number
){
  await http.delete(
    `/api/Category/${id}`
  );
}
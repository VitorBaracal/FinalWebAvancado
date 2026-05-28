import { http } from './http';
import { TaskDto } from './types';

export async function listarTasks() {
  const { data } =
    await http.get<TaskDto[]>('/api/Task');

  return data;
}

export async function criarTask(
  task:Omit<TaskDto,'id'>
){
  const { data } =
    await http.post<TaskDto>(
      '/api/Task',
      task
    );

  return data;
}

export async function atualizarTask(
  id:number,
  task:Partial<TaskDto>
){
  await http.put(
    `/api/Task/${id}`,
    task
  );
}

export async function removerTask(
  id:number
){
  await http.delete(`/api/Task/${id}`);
}
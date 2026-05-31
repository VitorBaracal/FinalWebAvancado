import { http } from './http';
import { TaskDto } from './types';

export async function listarTasks() {
  const { data } =
    await http.get<TaskDto[]>('/api/Task');

  return data;
}

export async function criarTask(task: Omit<TaskDto, 'id'>) {
  try {
    const { data } = await http.post<TaskDto>(
      '/api/Task',
      task
    );

    return data;
  } catch (error: any) {
    console.log('Payload enviado:', task);
    console.log('Erro da API:', error.response?.data);
    throw error;
  }
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
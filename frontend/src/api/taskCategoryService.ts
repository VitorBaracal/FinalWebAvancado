import { http } from './http';

export type TaskCategoryDto = {
  id: number;
  userId: number;
  taskId: number;
  categoryId: number;
};

export async function criarTaskCategory(
  userId: number,
  taskId: number,
  categoryId: number
) {
  const { data } = await http.post<TaskCategoryDto>('/api/TaskCategory', {
    userId,
    taskId,
    categoryId,
  });

  return data;
}

export async function removerTaskCategory(id: number) {
  await http.delete(`/api/TaskCategory/${id}`);
}

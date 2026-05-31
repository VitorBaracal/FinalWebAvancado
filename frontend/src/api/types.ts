export type LoginResponse = {
  token: string;
  name: string;
  user: string;
};

export type TaskLevel =
  | 'Low'
  | 'Medium'
  | 'High'
  | 'Critical';

export type TaskStatus =
  | 'Pending'
  | 'InProgress'
  | 'Completed'
  | 'Cancelled';

export interface TaskDto {
  id: number;
  userId: number;
  name: string;
  description?: string;
  level: number;
  status: number;
  categories?: CategoryDto[];
}

export type CategoryDto = {
  id: number;
  userId:number;
  name: string;
  colorHex:string;
  taskCategoryId?: number;
};

export type TaskCategoryDto = {
  id: number;
  userId: number;
  taskId: number;
  categoryId: number;
};
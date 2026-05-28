export type LoginResponse = {
  token: string;
  nome: string;
  usuario: string;
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

export type TaskDto = {
  id: number;
  userId: number;
  name: string;
  description?: string;
  level: string;
  status: string;
};

export type CategoryDto = {
  id: number;
  userId:number;
  name: string;
  colorHex:string;
};
import axios from 'axios';

const baseURL = process.env.REACT_APP_API_BASE_URL ?? 'https://localhost:5001';

export const http = axios.create({ baseURL });

http.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers = config.headers ?? {};
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});


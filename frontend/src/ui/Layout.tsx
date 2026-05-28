import React from 'react';
import { Link, Outlet, useNavigate } from 'react-router-dom';
import { isAuthenticated, logout } from '../api/auth';

export function Layout() {
  const navigate = useNavigate();
  const auth = isAuthenticated();
  const nome = localStorage.getItem('nome');

  return (
    <div className="app-container">
      <header className="app-header">
        <div className="app-brand">
          React API
        </div>

        <nav className="app-nav">
          <Link to="/">Home</Link>

          {!auth && <Link to="/login">Login</Link>}
          {!auth && <Link to="/register">Cadastro</Link>}

          {auth && <Link to="/categorias">Categorias</Link>}
          {auth && <Link to="/produtos">Produtos</Link>}
        </nav>

        <div className="app-user">
          {auth && (
            <span className="app-user-name">
              Olá, {nome ?? 'usuário'}
            </span>
          )}

          {auth && (
            <button
              className="btn-danger"
              type="button"
              onClick={() => {
                logout();
                navigate('/login');
              }}
            >
              Sair
            </button>
          )}
        </div>
      </header>
      <main>
        <Outlet />
      </main>
    </div>
  );
}
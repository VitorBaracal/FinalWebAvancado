import React from 'react';
import { Link } from 'react-router-dom';

export function HomePage() {

  return (

    <div className="page">

      <div className="card">

        <h1 className="page-title">
          Gerenciador de Tarefas
        </h1>

        <p className="muted">
          Organize suas tarefas,
          acompanhe prioridades
          e mantenha seu fluxo
          de trabalho em dia.
        </p>

        <div
          style={{
            display:'grid',
            gridTemplateColumns:'repeat(auto-fit,minmax(220px,1fr))',
            gap:'16px',
            marginTop:'24px'
          }}
        >

          <Link
            to="/tasks"
            style={{
              textDecoration:'none',
              color:'inherit'
            }}
          >

            <div className="card">

              <h3>
                📋 Tarefas
              </h3>

              <p className="muted">
                Crie, edite e acompanhe
                suas atividades.
              </p>

            </div>

          </Link>

          <Link
            to="/categorias"
            style={{
              textDecoration:'none',
              color:'inherit'
            }}
          >

            <div className="card">

              <h3>
                🏷️ Categorias
              </h3>

              <p className="muted">
                Organize tarefas
                por grupos e cores.
              </p>

            </div>

          </Link>

          <Link
            to="/login"
            style={{
              textDecoration:'none',
              color:'inherit'
            }}
          >

            <div className="card">

              <h3>
                ⚡ Login
              </h3>

              <p className="muted">
                Entre no sistema
                para gerenciar
                suas tarefas.
              </p>

            </div>

          </Link>

        </div>

      </div>

    </div>
  );
}
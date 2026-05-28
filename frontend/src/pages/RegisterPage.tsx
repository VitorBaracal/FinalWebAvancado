import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { cadastrarUsuario } from '../api/user';

export function RegisterPage() {

  const navigate = useNavigate();

  const [nome,setNome] =
    useState('');

  const [login,setLogin] =
    useState('');

  const [senha,setSenha] =
    useState('');

  const [confirmarSenha,setConfirmarSenha] =
    useState('');

  const [loading,setLoading] =
    useState(false);

  const [error,setError] =
    useState<string | null>(null);

  async function handleSubmit(
    e:React.FormEvent
  ){

    e.preventDefault();

    setError(null);
    setLoading(true);

    try{

      await cadastrarUsuario({

        nome,
        login,
        senha,
        confirmarSenha

      });

      navigate('/login');

    }catch(err:any){

      setError(

        err?.response?.data?.message ??

        err?.message ??

        'Falha ao cadastrar usuário.'

      );

    }finally{

      setLoading(false);

    }
  }

  return (

    <div className="page">

      <form
        className="card"
        onSubmit={handleSubmit}
        style={{maxWidth:520}}
      >

        <h2 className="page-title">
          Cadastro de Usuário
        </h2>

        <div className="form-group">

          <label>Nome</label>

          <input
            value={nome}
            onChange={(e)=>
              setNome(e.target.value)
            }
          />

        </div>

        <div className="form-group">

          <label>Login</label>

          <input
            value={login}
            onChange={(e)=>
              setLogin(e.target.value)
            }
            autoComplete="username"
          />

        </div>

        <div className="form-group">

          <label>Senha</label>

          <input
            type="password"
            value={senha}
            onChange={(e)=>
              setSenha(e.target.value)
            }
            autoComplete="new-password"
          />

        </div>

        <div className="form-group">

          <label>
            Confirmar Senha
          </label>

          <input
            type="password"
            value={confirmarSenha}
            onChange={(e)=>
              setConfirmarSenha(
                e.target.value
              )
            }
            autoComplete="new-password"
          />

        </div>

        <button
          className="btn-primary"
          type="submit"
          disabled={loading}
        >

          {loading
            ? 'Cadastrando...'
            : 'Cadastrar'}

        </button>

        {error && (

          <div className="form-error">
            {error}
          </div>

        )}

      </form>

    </div>
  );
}
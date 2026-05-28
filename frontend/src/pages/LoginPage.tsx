import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { login } from '../api/auth';

export function LoginPage() {

  const navigate = useNavigate();

  const [loginValue,setLoginValue] =
    useState('');

  const [senha,setSenha] =
    useState('');

  const [loading,setLoading] =
    useState(false);

  const [error,setError] =
    useState<string | null>(null);

  async function handleSubmit(
    e:React.FormEvent
  ){

    e.preventDefault();

    setLoading(true);
    setError(null);

    try{

      await login(
        loginValue,
        senha
      );

      navigate('/tasks');

    }catch(err:any){

      setError(

        err?.response?.data?.message ??

        err?.message ??

        'Usuário ou senha inválidos.'
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
        style={{
          maxWidth:420
        }}
      >

        <h2 className="page-title">
          Gerenciador de Tarefas
        </h2>

        <div className="form-group">

          <label>
            Login
          </label>

          <input
            value={loginValue}
            onChange={(e)=>
              setLoginValue(
                e.target.value
              )
            }
            autoComplete="username"
          />

        </div>

        <div className="form-group">

          <label>
            Senha
          </label>

          <input
            type="password"
            value={senha}
            onChange={(e)=>
              setSenha(
                e.target.value
              )
            }
            autoComplete="current-password"
          />

        </div>

        <button
          className="btn-primary"
          type="submit"
          disabled={loading}
        >

          {loading
            ? 'Entrando...'
            : 'Entrar'}

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
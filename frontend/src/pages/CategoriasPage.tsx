import React, { useEffect, useMemo, useState } from 'react';

import {
  atualizarCategoria,
  criarCategoria,
  listarCategorias,
  removerCategoria,
} from '../api/categoryService';

import { CategoryDto } from '../api/types';

export function CategoriasPage() {

  const [items,setItems] =
    useState<CategoryDto[]>([]);

  const [editing,setEditing] =
    useState<CategoryDto | null>(null);

  const [userId,setUserId] =
    useState(1);

  const [name,setName] =
    useState('');

  const [colorHex,setColorHex] =
    useState('#3B82F6');

  const [error,setError] =
    useState<string | null>(null);

  const [loading,setLoading] =
    useState(false);

  const sortedItems = useMemo(
    ()=>
      [...items].sort(
        (a,b)=>
          a.name.localeCompare(b.name)
      ),
    [items]
  );

  async function refresh(){

    setLoading(true);
    setError(null);

    try{

      const data =
        await listarCategorias();

      setItems(data);

    }catch(err:any){

      setError(
        err?.response?.data?.message ??
        err?.message ??
        'Falha ao carregar categorias.'
      );

    }finally{
      setLoading(false);
    }
  }

  useEffect(()=>{
    void refresh();
  },[]);

  function resetForm(){

    setEditing(null);

    setName('');

    setColorHex('#3B82F6');
  }

  async function handleSubmit(
    e:React.FormEvent
  ){

    e.preventDefault();

    try{

      if(editing){

        await atualizarCategoria(
          editing.id,
          userId,
          name,
          colorHex
        );

      }else{

        await criarCategoria(
          userId,
          name,
          colorHex
        );
      }

      resetForm();

      await refresh();

    }catch(err:any){

      setError(
        err?.response?.data?.message ??
        err?.message ??
        'Falha ao salvar categoria.'
      );
    }
  }

  return (

    <div className="page">

      <h2 className="page-title">
        Categorias
      </h2>

      <form
        className="card"
        onSubmit={handleSubmit}
      >

        <div className="form-group">

          <label>Nome</label>

          <input
            value={name}
            onChange={(e)=>
              setName(e.target.value)
            }
          />

        </div>

        <div className="form-group">

          <label>Cor</label>

          <input
            type="color"
            value={colorHex}
            onChange={(e)=>
              setColorHex(
                e.target.value
              )
            }
          />

        </div>

        <div className="form-actions">

          <button
            className="btn-primary"
            type="submit"
          >
            {editing
              ? 'Atualizar'
              : 'Criar'}
          </button>

          {editing && (

            <button
              className="btn-secondary"
              type="button"
              onClick={
                resetForm
              }
            >
              Cancelar
            </button>

          )}

        </div>

      </form>

      {error &&
        <div className="form-error">
          {error}
        </div>
      }

      {loading &&
        <div className="loading">
          Carregando...
        </div>
      }

      <div className="list">

        {sortedItems.map((c)=>(

          <div
            key={c.id}
            className="list-item"
          >

            <div>

              <strong>
                {c.name}
              </strong>

              <span
                style={{
                  background:c.colorHex,
                  width:20,
                  height:20,
                  display:'inline-block',
                  borderRadius:'50%',
                  marginLeft:10
                }}
              />

              <div className="muted">
                #{c.id}
              </div>

            </div>

            <div className="form-actions">

              <button
                className="btn-outline"
                onClick={()=>{

                  setEditing(c);

                  setName(c.name);

                  setColorHex(
                    c.colorHex
                  );

                }}
              >
                Editar
              </button>

              <button
                className="btn-danger"
                onClick={async()=>{

                  if(
                    !window.confirm(
                      `Remover "${c.name}" ?`
                    )
                  ){
                    return;
                  }

                  await removerCategoria(
                    c.id
                  );

                  await refresh();

                }}
              >
                Remover
              </button>

            </div>

          </div>

        ))}

      </div>

    </div>
  );
}
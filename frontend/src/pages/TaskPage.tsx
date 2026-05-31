import React, { useEffect, useMemo, useState } from 'react';
import { listarTasks, criarTask, atualizarTask, removerTask } from '../api/taskService';
import { TaskDto } from '../api/types';

export function TaskPage() {

  const [tasks,setTasks] = useState<TaskDto[]>([]);
  const [error,setError] = useState<string | null>(null);
  const [loading,setLoading] = useState(false);

  const [editing,setEditing] = useState<TaskDto | null>(null);

  const [userId,setUserId] = useState(1);
  const [name,setName] = useState('');
  const [description,setDescription] = useState('');
  const [level,setLevel] = useState<number>(1);
  const [status,setStatus] = useState<number>(1);  
 

  const sortedTasks = useMemo(
    () => [...tasks].sort((a,b)=>a.name.localeCompare(b.name)),
    [tasks]
  );

  async function refresh(){

    setLoading(true);
    setError(null);

    try{

      const data = await listarTasks();
      setTasks(data);

    }catch(err:any){

      setError(
        err?.response?.data?.message ??
        err?.message ??
        'Falha ao carregar tarefas.'
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

    setUserId(1);
    setName('');
    setDescription('');
    setLevel(1);
    setStatus(1);
  }

  async function handleSubmit(
    e:React.FormEvent
  ){

    e.preventDefault();

    try{

      const payload = {
        userId,
        name,
        description,
        level,
        status
      };

      if(editing){

        await atualizarTask(
          editing.id,
          payload
        );

      }else{

        await criarTask(payload);
      }

      resetForm();
      await refresh();

    }catch(err:any){

      setError(
        err?.response?.data?.message ??
        err?.message ??
        'Falha ao salvar tarefa.'
      );
    }
  }

  return (

    <div className="page">

      <h2 className="page-title">
        Gerenciador de Tarefas
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

          <label>Descrição</label>

          <textarea
            value={description}
            onChange={(e)=>
              setDescription(e.target.value)
            }
          />

        </div>

      <div className="form-group">

            <label>Prioridade</label>

            <select
                value={level}
                onChange={(e) =>
                setLevel(Number(e.target.value))
                }
            >
                <option value={1}>
                Baixa
                </option>

                <option value={2}>
                Média
                </option>

                <option value={3}>
                Alta
                </option>

                <option value={4}>
                Crítica
                </option>
            </select>

            </div>

        <div className="form-group">

          <label>Status</label>

          <select
            value={status}
            onChange={(e) =>
                setStatus(Number(e.target.value))
            }
            >
            <option value={1}>Pendente</option>
            <option value={2}>Em andamento</option>
            <option value={3}>Concluída</option>
            <option value={4}>Cancelada</option>
            </select>

        </div>

        <button
          className="btn-primary"
          type="submit"
        >
          {editing
            ? 'Atualizar'
            : 'Criar'}
        </button>

      </form>

      {error &&
        <div className="form-error">
          {error}
        </div>
      }

      {loading &&
        <div>
          Carregando...
        </div>
      }

      <div className="list">

        {sortedTasks.map((task)=>(

          <div
            key={task.id}
            className="list-item"
          >

            <div>

              <strong>
                {task.name}
              </strong>

              <div>
                {task.description}
              </div>

              <div>
                Level: {task.level}
              </div>

              <div>
                Status: {task.status}
              </div>

            </div>

            <div className="form-actions">

              <button
                className="btn-outline"
                onClick={()=>{
                  setEditing(task);

                  setUserId(task.userId);
                  setName(task.name);
                  setDescription(
                    task.description ?? ''
                  );
                  setLevel(task.level);
                  setStatus(task.status);
                }}
              >
                Editar
              </button>

              <button
                className="btn-danger"
                onClick={async()=>{

                  await removerTask(
                    task.id
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
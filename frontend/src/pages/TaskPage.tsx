import React, { useEffect, useMemo, useState } from 'react';
import { listarTasks, criarTask, atualizarTask, removerTask } from '../api/taskService';
import { listarCategorias } from '../api/categoryService';
import {
  criarTaskCategory,
  removerTaskCategory,
} from '../api/taskCategoryService';
import { CategoryDto, TaskDto } from '../api/types';

function getSelectedCategoryId(task: TaskDto): number | null {
  return task.categories?.[0]?.id ?? null;
}

function getTaskCategoryLinkId(task: TaskDto): number | null {
  return task.categories?.[0]?.taskCategoryId ?? null;
}

async function salvarCategoriaDaTask(
  userId: number,
  taskId: number,
  categoryId: number | null,
  taskCategoryId: number | null
) {
  if (taskCategoryId !== null) {
    await removerTaskCategory(taskCategoryId);
  }

  if (categoryId !== null) {
    await criarTaskCategory(userId, taskId, categoryId);
  }
}

type CategoryChipsProps = {
  categories: CategoryDto[];
  selectedId: number | null;
  disabled?: boolean;
  onSelect: (categoryId: number | null) => void;
};

function CategoryChips({
  categories,
  selectedId,
  disabled = false,
  onSelect,
}: CategoryChipsProps) {
  if (categories.length === 0) {
    return (
      <p className="chip-empty">
        Nenhuma categoria cadastrada.
      </p>
    );
  }

  return (
    <div className="chip-group" role="group" aria-label="Categorias">
      {categories.map((category) => {
        const isSelected = selectedId === category.id;

        return (
          <button
            key={category.id}
            type="button"
            className={`chip ${isSelected ? 'chip-selected' : ''}`}
            style={{ '--chip-color': category.colorHex } as React.CSSProperties}
            disabled={disabled}
            aria-pressed={isSelected}
            onClick={() => onSelect(isSelected ? null : category.id)}
          >
            <span className="chip-dot" />
            {category.name}
          </button>
        );
      })}
    </div>
  );
}

export function TaskPage() {

  const [tasks,setTasks] = useState<TaskDto[]>([]);
  const [categories,setCategories] = useState<CategoryDto[]>([]);
  const [error,setError] = useState<string | null>(null);
  const [loading,setLoading] = useState(false);
  const [savingCategoryTaskId,setSavingCategoryTaskId] = useState<number | null>(null);

  const [editing,setEditing] = useState<TaskDto | null>(null);

  const [userId,setUserId] = useState(1);
  const [name,setName] = useState('');
  const [description,setDescription] = useState('');
  const [level,setLevel] = useState<number>(1);
  const [status,setStatus] = useState<number>(1);
  const [selectedCategoryId,setSelectedCategoryId] = useState<number | null>(null);

  const sortedTasks = useMemo(
    () => [...tasks].sort((a,b)=>a.name.localeCompare(b.name)),
    [tasks]
  );

  const sortedCategories = useMemo(
    () => [...categories].sort((a,b)=>a.name.localeCompare(b.name)),
    [categories]
  );

  async function refresh(){

    setLoading(true);
    setError(null);

    try{

      const [tasksData, categoriesData] = await Promise.all([
        listarTasks(),
        listarCategorias(),
      ]);

      setTasks(tasksData);
      setCategories(categoriesData);

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
    setSelectedCategoryId(null);
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

        await salvarCategoriaDaTask(
          userId,
          editing.id,
          selectedCategoryId,
          getTaskCategoryLinkId(editing)
        );

      }else{

        const created = await criarTask(payload);

        await salvarCategoriaDaTask(
          userId,
          created.id,
          selectedCategoryId,
          null
        );
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

  async function handleTaskCategoryChange(
    task: TaskDto,
    categoryId: number | null
  ) {
    setSavingCategoryTaskId(task.id);
    setError(null);

    try {
      await salvarCategoriaDaTask(
        task.userId,
        task.id,
        categoryId,
        getTaskCategoryLinkId(task)
      );

      await refresh();
    } catch (err: any) {
      setError(
        err?.response?.data?.message ??
        err?.message ??
        'Falha ao atualizar categoria da tarefa.'
      );
    } finally {
      setSavingCategoryTaskId(null);
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

        <div className="form-group">

          <label>Categoria</label>

          <CategoryChips
            categories={sortedCategories}
            selectedId={selectedCategoryId}
            onSelect={setSelectedCategoryId}
          />

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

            <div className="list-item-content">

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

              <div className="form-group task-category-group">

                <label>Categoria</label>

                <CategoryChips
                  categories={sortedCategories}
                  selectedId={getSelectedCategoryId(task)}
                  disabled={savingCategoryTaskId === task.id}
                  onSelect={(categoryId) =>
                    void handleTaskCategoryChange(task, categoryId)
                  }
                />

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
                  setSelectedCategoryId(getSelectedCategoryId(task));
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

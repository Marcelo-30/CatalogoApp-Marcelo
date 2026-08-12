import { type FormEvent, useState } from 'react'
import { createCategory, deleteCategory, updateCategory } from '../api/adminApi'
import { getCategories } from '../api/catalogApi'
import { ConfirmDialog } from '../components/admin/ConfirmDialog'
import { StatusPanel } from '../components/ui/StatusPanel'
import { useApiResource } from '../hooks/useApiResource'
import { useDocumentTitle } from '../hooks/useDocumentTitle'
import type { Category } from '../types/catalog'

export function AdminCategoriesPage() {
  const resource = useApiResource(getCategories)
  const [editing, setEditing] = useState<Category | null>(null)
  const [target, setTarget] = useState<Category | null>(null)
  const [form, setForm] = useState({ nombre: '', descripcion: '' })
  const [saving, setSaving] = useState(false)
  const [deleting, setDeleting] = useState(false)
  const [error, setError] = useState<string | null>(null)
  useDocumentTitle('Administrar categorías')

  const resetForm = () => {
    setEditing(null)
    setForm({ nombre: '', descripcion: '' })
  }

  const beginEdit = (category: Category) => {
    setEditing(category)
    setForm({ nombre: category.nombre, descripcion: category.descripcion ?? '' })
    setError(null)
    document.getElementById('category-name')?.focus()
  }

  const submit = async (event: FormEvent) => {
    event.preventDefault()
    setSaving(true)
    setError(null)
    try {
      const input = { nombre: form.nombre, descripcion: form.descripcion || null }
      if (editing) await updateCategory(editing.id, input)
      else await createCategory(input)
      resetForm()
      setSaving(false)
      resource.retry()
    } catch (submitError) {
      setError(submitError instanceof Error ? submitError.message : 'No pudimos guardar la categoría.')
      setSaving(false)
    }
  }

  const confirmDelete = async () => {
    if (!target) return
    setDeleting(true)
    setError(null)
    try {
      await deleteCategory(target.id)
      setTarget(null)
      setDeleting(false)
      resource.retry()
    } catch (deleteError) {
      setError(deleteError instanceof Error ? deleteError.message : 'No pudimos eliminar la categoría.')
      setDeleting(false)
      setTarget(null)
    }
  }

  return (
    <div className="admin-page">
      <header className="admin-page__header"><div><span className="eyebrow">Organización</span><h1>Categorías</h1><p>Crea grupos claros para que las prendas sean fáciles de descubrir.</p></div></header>
      {error && <div className="form-alert" role="alert"><span>!</span>{error}</div>}

      <div className="category-admin-layout">
        <section className="admin-panel category-form-panel">
          <span className="eyebrow">{editing ? 'Edición' : 'Nueva categoría'}</span>
          <h2>{editing ? `Editar ${editing.nombre}` : 'Añadir categoría'}</h2>
          <p>Usa un nombre breve y una descripción que ayude a entender la selección.</p>
          <form className="admin-form" onSubmit={submit}>
            <div className="form-field"><label htmlFor="category-name">Nombre</label><input id="category-name" required maxLength={60} value={form.nombre} onChange={(event) => setForm({ ...form, nombre: event.target.value })} /></div>
            <div className="form-field"><label htmlFor="category-description">Descripción</label><textarea id="category-description" rows={4} maxLength={200} value={form.descripcion} onChange={(event) => setForm({ ...form, descripcion: event.target.value })} /><small>{form.descripcion.length}/200</small></div>
            <div className="category-form-panel__actions">{editing && <button className="button button--ghost" type="button" onClick={resetForm}>Cancelar</button>}<button className="button button--primary" type="submit" disabled={saving}>{saving ? 'Guardando…' : editing ? 'Guardar cambios' : 'Crear categoría'}</button></div>
          </form>
        </section>

        <section className="admin-panel category-list-panel">
          <div className="admin-panel__heading"><div><span className="eyebrow">Catálogo</span><h2>Categorías activas</h2></div><span>{resource.data?.length ?? 0} en total</span></div>
          {resource.loading && <div className="admin-table-loading skeleton" aria-label="Cargando categorías" />}
          {resource.error && <StatusPanel compact title="No pudimos cargar las categorías" message={resource.error.message} actionLabel="Volver a intentar" onAction={resource.retry} />}
          {resource.data?.length === 0 && <StatusPanel compact title="No hay categorías" message="Crea la primera para organizar tus productos." />}
          {resource.data && resource.data.length > 0 && (
            <div className="category-admin-list">
              {resource.data.map((category, index) => (
                <article key={category.id}>
                  <span>0{index + 1}</span>
                  <div><h3>{category.nombre}</h3><p>{category.descripcion ?? 'Sin descripción.'}</p></div>
                  <div><button type="button" onClick={() => beginEdit(category)}>Editar</button><button type="button" onClick={() => setTarget(category)}>Eliminar</button></div>
                </article>
              ))}
            </div>
          )}
        </section>
      </div>

      <ConfirmDialog open={target !== null} title="¿Eliminar esta categoría?" message={target ? `“${target.nombre}” solo puede eliminarse si no contiene productos.` : ''} busy={deleting} onCancel={() => setTarget(null)} onConfirm={confirmDelete} />
    </div>
  )
}

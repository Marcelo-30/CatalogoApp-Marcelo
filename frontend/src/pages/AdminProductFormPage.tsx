import { type FormEvent, useCallback, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { createProduct, getProduct, getProductOptions, updateProduct } from '../api/adminApi'
import { CatalogApiError } from '../api/httpClient'
import { ImageWithFallback } from '../components/ui/ImageWithFallback'
import { StatusPanel } from '../components/ui/StatusPanel'
import { useApiResource } from '../hooks/useApiResource'
import { useDocumentTitle } from '../hooks/useDocumentTitle'
import type { ProductInput, ProductOptions } from '../types/admin'
import type { Product } from '../types/catalog'

const emptyProduct: ProductInput = {
  nombre: '', descripcion: '', precio: 0, stock: 0, disponible: true,
  categoriaId: 0, tallaId: 0, colorId: 0, imagenUrl: '',
}

interface ProductEditorProps {
  product: Product | null
  options: ProductOptions
}

function ProductEditor({ product, options }: ProductEditorProps) {
  const categoryId = product ? options.categories.find((item) => item.nombre === product.categoria)?.id ?? 0 : 0
  const sizeId = product ? options.sizes.find((item) => item.nombre === product.talla)?.id ?? 0 : 0
  const colorId = product ? options.colors.find((item) => item.nombre === product.color)?.id ?? 0 : 0
  const [form, setForm] = useState<ProductInput>(product ? {
    nombre: product.nombre,
    descripcion: product.descripcion,
    precio: product.precio,
    stock: product.stock,
    disponible: product.disponible,
    categoriaId: categoryId,
    tallaId: sizeId,
    colorId,
    imagenUrl: product.imagenUrl,
  } : emptyProduct)
  const [error, setError] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)
  const navigate = useNavigate()

  const submit = async (event: FormEvent) => {
    event.preventDefault()
    setSaving(true)
    setError(null)
    try {
      if (product) await updateProduct(product.id, form)
      else await createProduct(form)
      navigate('/admin/productos', { replace: true })
    } catch (submitError) {
      setError(submitError instanceof Error ? submitError.message : 'No pudimos guardar el producto.')
      setSaving(false)
    }
  }

  return (
    <form className="product-editor" onSubmit={submit}>
      <div className="product-editor__form">
        {error && <div className="form-alert" role="alert"><span>!</span>{error}</div>}
        <section className="admin-form-section">
          <div className="admin-form-section__heading"><span>01</span><div><h2>Información principal</h2><p>Lo primero que verán tus visitantes.</p></div></div>
          <div className="admin-form">
            <div className="form-field"><label htmlFor="product-name">Nombre</label><input id="product-name" required maxLength={80} value={form.nombre} onChange={(event) => setForm({ ...form, nombre: event.target.value })} /></div>
            <div className="form-field"><label htmlFor="product-description">Descripción</label><textarea id="product-description" maxLength={300} rows={4} value={form.descripcion ?? ''} onChange={(event) => setForm({ ...form, descripcion: event.target.value })} /><small>{(form.descripcion ?? '').length}/300</small></div>
            <div className="form-row">
              <div className="form-field"><label htmlFor="product-price">Precio (MXN)</label><input id="product-price" type="number" required min="0.01" max="999999" step="0.01" value={form.precio || ''} onChange={(event) => setForm({ ...form, precio: Number(event.target.value) })} /></div>
              <div className="form-field"><label htmlFor="product-stock">Existencias</label><input id="product-stock" type="number" required min="0" max="9999" step="1" value={form.stock} onChange={(event) => setForm({ ...form, stock: Number(event.target.value) })} /></div>
            </div>
          </div>
        </section>

        <section className="admin-form-section">
          <div className="admin-form-section__heading"><span>02</span><div><h2>Clasificación</h2><p>Ayuda a encontrar la prenda con rapidez.</p></div></div>
          <div className="admin-form form-grid-3">
            <div className="form-field"><label htmlFor="product-category">Categoría</label><select id="product-category" required value={form.categoriaId} onChange={(event) => setForm({ ...form, categoriaId: Number(event.target.value) })}><option value={0} disabled>Selecciona</option>{options.categories.map((item) => <option key={item.id} value={item.id}>{item.nombre}</option>)}</select></div>
            <div className="form-field"><label htmlFor="product-size">Talla</label><select id="product-size" required value={form.tallaId} onChange={(event) => setForm({ ...form, tallaId: Number(event.target.value) })}><option value={0} disabled>Selecciona</option>{options.sizes.map((item) => <option key={item.id} value={item.id}>{item.nombre}</option>)}</select></div>
            <div className="form-field"><label htmlFor="product-color">Color</label><select id="product-color" required value={form.colorId} onChange={(event) => setForm({ ...form, colorId: Number(event.target.value) })}><option value={0} disabled>Selecciona</option>{options.colors.map((item) => <option key={item.id} value={item.id}>{item.nombre}</option>)}</select></div>
          </div>
        </section>

        <section className="admin-form-section">
          <div className="admin-form-section__heading"><span>03</span><div><h2>Imagen y publicación</h2><p>Usa una URL HTTPS y controla su visibilidad.</p></div></div>
          <div className="admin-form">
            <div className="form-field"><label htmlFor="product-image">URL de imagen</label><input id="product-image" type="url" maxLength={500} placeholder="https://…" value={form.imagenUrl ?? ''} onChange={(event) => setForm({ ...form, imagenUrl: event.target.value })} /></div>
            <label className="admin-check"><input type="checkbox" checked={form.disponible} onChange={(event) => setForm({ ...form, disponible: event.target.checked })} /><span><strong>Producto disponible</strong><small>Se mostrará como elegible mientras tenga existencias.</small></span></label>
          </div>
        </section>

        <div className="product-editor__actions"><Link className="button button--ghost" to="/admin/productos">Cancelar</Link><button className="button button--primary" type="submit" disabled={saving}>{saving ? 'Guardando…' : product ? 'Guardar cambios' : 'Publicar producto'}</button></div>
      </div>
      <aside className="product-editor__preview">
        <span className="eyebrow">Vista previa</span>
        <div className="preview-card"><ImageWithFallback src={form.imagenUrl} alt={form.nombre || 'Vista previa del producto'} className="preview-card__image" /><div><span>{options.categories.find((item) => item.id === form.categoriaId)?.nombre || 'Categoría'}</span><h2>{form.nombre || 'Nombre del producto'}</h2><p>{form.precio ? `$${form.precio.toLocaleString('es-MX')}` : '$0'}</p></div></div>
      </aside>
    </form>
  )
}

export function AdminProductFormPage() {
  const { id } = useParams()
  const productId = Number(id)
  const editing = id !== undefined
  const options = useApiResource(getProductOptions)
  const productLoader = useCallback((signal: AbortSignal) => {
    if (!editing) return Promise.resolve<Product | null>(null)
    if (!Number.isInteger(productId) || productId <= 0) return Promise.reject(new CatalogApiError('El producto no existe.', 'http', 404))
    return getProduct(productId, signal)
  }, [editing, productId])
  const product = useApiResource(productLoader)
  useDocumentTitle(editing ? 'Editar producto' : 'Nuevo producto')

  const loading = options.loading || product.loading
  const error = options.error ?? product.error

  return (
    <div className="admin-page admin-page--editor">
      <header className="admin-page__header"><div><span className="eyebrow">{editing ? 'Editar inventario' : 'Nueva pieza'}</span><h1>{editing ? 'Editar producto' : 'Crear producto'}</h1><p>{editing ? 'Actualiza la información que verán tus visitantes.' : 'Completa los datos para añadirlo al catálogo público.'}</p></div></header>
      {loading && <div className="admin-table-loading skeleton" aria-label="Cargando formulario" />}
      {error && <StatusPanel title="No pudimos abrir el formulario" message={error.message} actionLabel="Volver a productos" actionTo="/admin/productos" />}
      {!loading && !error && options.data && (
        <ProductEditor key={product.data?.id ?? 'new'} product={product.data} options={options.data} />
      )}
    </div>
  )
}

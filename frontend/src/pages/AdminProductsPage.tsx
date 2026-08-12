import { useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import { deleteProduct } from '../api/adminApi'
import { getProducts } from '../api/catalogApi'
import { ConfirmDialog } from '../components/admin/ConfirmDialog'
import { ImageWithFallback } from '../components/ui/ImageWithFallback'
import { StatusPanel } from '../components/ui/StatusPanel'
import { useApiResource } from '../hooks/useApiResource'
import { useDocumentTitle } from '../hooks/useDocumentTitle'
import type { Product } from '../types/catalog'
import { formatPrice } from '../utils/catalog'

export function AdminProductsPage() {
  const resource = useApiResource(getProducts)
  const [search, setSearch] = useState('')
  const [target, setTarget] = useState<Product | null>(null)
  const [deleting, setDeleting] = useState(false)
  const [error, setError] = useState<string | null>(null)
  useDocumentTitle('Administrar productos')

  const products = useMemo(() => {
    const term = search.trim().toLocaleLowerCase('es-MX')
    if (!term) return resource.data ?? []
    return (resource.data ?? []).filter((product) =>
      [product.nombre, product.categoria, product.color, product.talla].join(' ').toLocaleLowerCase('es-MX').includes(term),
    )
  }, [resource.data, search])

  const confirmDelete = async () => {
    if (!target) return
    setDeleting(true)
    setError(null)
    try {
      await deleteProduct(target.id)
      setTarget(null)
      setDeleting(false)
      resource.retry()
    } catch (deleteError) {
      setError(deleteError instanceof Error ? deleteError.message : 'No pudimos eliminar el producto.')
      setDeleting(false)
      setTarget(null)
    }
  }

  return (
    <div className="admin-page">
      <header className="admin-page__header">
        <div><span className="eyebrow">Inventario</span><h1>Productos</h1><p>Crea, edita y controla la disponibilidad desde una sola vista.</p></div>
        <Link className="button button--primary" to="/admin/productos/nuevo">Nuevo producto <span aria-hidden="true">+</span></Link>
      </header>

      {error && <div className="form-alert" role="alert"><span>!</span>{error}</div>}

      <div className="admin-toolbar">
        <label className="admin-search"><span className="sr-only">Buscar productos</span><span aria-hidden="true">⌕</span><input type="search" value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Buscar por nombre, categoría, color o talla" /></label>
        <span>{products.length} {products.length === 1 ? 'producto' : 'productos'}</span>
      </div>

      {resource.loading && <div className="admin-table-loading skeleton" aria-label="Cargando productos" />}
      {resource.error && <StatusPanel title="No pudimos cargar los productos" message={resource.error.message} actionLabel="Volver a intentar" onAction={resource.retry} />}
      {resource.data && products.length === 0 && (
        <StatusPanel title={search ? 'No encontramos coincidencias' : 'Todavía no hay productos'} message={search ? 'Cambia el término de búsqueda para ver otros resultados.' : 'Crea tu primera pieza para publicarla en el catálogo.'} actionLabel={search ? 'Limpiar búsqueda' : 'Crear producto'} onAction={search ? () => setSearch('') : undefined} actionTo={search ? undefined : '/admin/productos/nuevo'} />
      )}
      {products.length > 0 && (
        <div className="admin-product-grid">
          {products.map((product) => {
            const available = product.disponible && product.stock > 0
            return (
              <article className="admin-product-card" key={product.id}>
                <ImageWithFallback src={product.imagenUrl} alt={product.nombre} className="admin-product-card__image" />
                <div className="admin-product-card__content">
                  <div className="admin-product-card__top"><span>{product.categoria}</span><span className={available ? 'is-available' : 'is-unavailable'}>{available ? 'Disponible' : 'No disponible'}</span></div>
                  <h2>{product.nombre}</h2>
                  <p>{product.talla} · {product.color} · {product.stock} en stock</p>
                  <strong>{formatPrice(product.precio)}</strong>
                  <div className="admin-product-card__actions">
                    <Link to={`/admin/productos/${product.id}/editar`}>Editar</Link>
                    <button type="button" onClick={() => setTarget(product)}>Eliminar</button>
                    <Link to={`/catalogo/${product.id}`} aria-label={`Ver ${product.nombre} en el catálogo`}>Ver ↗</Link>
                  </div>
                </div>
              </article>
            )
          })}
        </div>
      )}

      <ConfirmDialog open={target !== null} title="¿Eliminar este producto?" message={target ? `“${target.nombre}” desaparecerá del catálogo y esta acción no se puede deshacer.` : ''} busy={deleting} onCancel={() => setTarget(null)} onConfirm={confirmDelete} />
    </div>
  )
}

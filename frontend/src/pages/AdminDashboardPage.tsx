import { Link } from 'react-router-dom'
import { getAdminSummary } from '../api/adminApi'
import { ArrowIcon } from '../components/ui/ArrowIcon'
import { StatusPanel } from '../components/ui/StatusPanel'
import { useApiResource } from '../hooks/useApiResource'
import { useDocumentTitle } from '../hooks/useDocumentTitle'
import { formatPrice } from '../utils/catalog'

export function AdminDashboardPage() {
  const summary = useApiResource(getAdminSummary)
  useDocumentTitle('Panel de vendedor')

  const products = summary.data?.products ?? []
  const available = products.filter((product) => product.disponible && product.stock > 0).length
  const inventoryValue = products.reduce((total, product) => total + product.precio * product.stock, 0)

  return (
    <div className="admin-page">
      <header className="admin-page__header">
        <div><span className="eyebrow">Panel de vendedor</span><h1>Todo bajo control.</h1><p>Una vista clara de tu catálogo y las acciones más importantes.</p></div>
        <Link className="button button--primary" to="/admin/productos/nuevo">Nuevo producto <span aria-hidden="true">+</span></Link>
      </header>

      {summary.loading && <div className="admin-metrics" aria-busy="true">{Array.from({ length: 4 }, (_, index) => <div className="admin-metric skeleton" key={index} />)}</div>}
      {summary.error && <StatusPanel compact title="No pudimos cargar el resumen" message={summary.error.message} actionLabel="Volver a intentar" onAction={summary.retry} />}
      {summary.data && (
        <>
          <section className="admin-metrics" aria-label="Resumen del catálogo">
            <article className="admin-metric"><span>Productos</span><strong>{products.length}</strong><small>Total publicado</small></article>
            <article className="admin-metric"><span>Disponibles</span><strong>{available}</strong><small>Con existencias</small></article>
            <article className="admin-metric"><span>Categorías</span><strong>{summary.data.categories.length}</strong><small>Secciones activas</small></article>
            <article className="admin-metric admin-metric--accent"><span>Valor inventario</span><strong>{formatPrice(inventoryValue)}</strong><small>Precio × existencias</small></article>
          </section>

          <section className="admin-panel">
            <div className="admin-panel__heading"><div><span className="eyebrow">Inventario reciente</span><h2>Productos del catálogo</h2></div><Link className="section-link" to="/admin/productos">Gestionar todos <ArrowIcon /></Link></div>
            {products.length === 0 ? (
              <StatusPanel compact title="Tu catálogo está listo para comenzar" message="Agrega el primer producto para verlo aquí y en la experiencia pública." actionLabel="Crear producto" actionTo="/admin/productos/nuevo" />
            ) : (
              <div className="admin-recent-list">
                {products.slice(0, 5).map((product) => (
                  <article key={product.id}>
                    <span className={`admin-product-dot ${product.disponible && product.stock > 0 ? 'is-available' : ''}`} />
                    <div><strong>{product.nombre}</strong><small>{product.categoria} · Talla {product.talla}</small></div>
                    <span>{product.stock} en stock</span>
                    <b>{formatPrice(product.precio)}</b>
                    <Link to={`/admin/productos/${product.id}/editar`} aria-label={`Editar ${product.nombre}`}>Editar</Link>
                  </article>
                ))}
              </div>
            )}
          </section>
        </>
      )}
    </div>
  )
}

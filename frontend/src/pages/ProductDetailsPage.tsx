import { useCallback } from 'react'
import { Link, useParams } from 'react-router-dom'
import { CatalogApiError, getProduct } from '../api/catalogApi'
import { ProductSkeleton } from '../components/products/ProductSkeleton'
import { ArrowIcon } from '../components/ui/ArrowIcon'
import { ImageWithFallback } from '../components/ui/ImageWithFallback'
import { StatusPanel } from '../components/ui/StatusPanel'
import { useApiResource } from '../hooks/useApiResource'
import { useDocumentTitle } from '../hooks/useDocumentTitle'
import { formatPrice } from '../utils/catalog'

export function ProductDetailsPage() {
  const { id } = useParams()
  const productId = Number(id)
  const loader = useCallback((signal: AbortSignal) => {
    if (!Number.isInteger(productId) || productId <= 0) {
      return Promise.reject(new CatalogApiError('El producto que buscas no existe.', 'http', 404))
    }
    return getProduct(productId, signal)
  }, [productId])
  const resource = useApiResource(loader)

  useDocumentTitle(resource.data?.nombre ?? 'Detalle de producto')

  if (resource.loading) {
    return (
      <div className="detail-page">
        <div className="container detail-loading" aria-label="Cargando producto" aria-busy="true">
          <ProductSkeleton />
          <div className="detail-loading__copy">
            <div className="skeleton skeleton--short" />
            <div className="skeleton skeleton--heading" />
            <div className="skeleton skeleton--line" />
            <div className="skeleton skeleton--line" />
          </div>
        </div>
      </div>
    )
  }

  if (resource.error || !resource.data) {
    const notFound = resource.error instanceof CatalogApiError && resource.error.status === 404
    return (
      <div className="detail-page detail-page--status">
        <div className="container">
          <StatusPanel
            eyebrow={notFound ? '404 · Producto no encontrado' : 'Conexión interrumpida'}
            title={notFound ? 'Esta pieza ya no está aquí' : 'No pudimos cargar el producto'}
            message={resource.error?.message ?? 'No se encontró información para este producto.'}
            actionLabel={notFound ? 'Volver al catálogo' : 'Volver a intentar'}
            actionTo={notFound ? '/catalogo' : undefined}
            onAction={notFound ? undefined : resource.retry}
          />
        </div>
      </div>
    )
  }

  const product = resource.data
  const available = product.disponible && product.stock > 0

  return (
    <article className="detail-page">
      <div className="container detail-breadcrumb">
        <Link to="/catalogo"><ArrowIcon direction="left" /> Catálogo</Link>
        <span aria-hidden="true">/</span>
        <span>{product.nombre}</span>
      </div>

      <div className="container product-detail">
        <div className="product-detail__media">
          <ImageWithFallback src={product.imagenUrl} alt={product.nombre} className="product-detail__image" eager />
          <span className={`availability-badge availability-badge--detail ${available ? 'is-available' : 'is-unavailable'}`}>
            <span aria-hidden="true" /> {available ? 'Disponible' : 'No disponible'}
          </span>
        </div>

        <div className="product-detail__content">
          <span className="eyebrow">{product.categoria}</span>
          <h1>{product.nombre}</h1>
          <p className="product-detail__price">{formatPrice(product.precio)}</p>
          <p className="product-detail__description">
            {product.descripcion ?? 'Una pieza seleccionada por su versatilidad y estilo contemporáneo.'}
          </p>

          <dl className="product-specs">
            <div><dt>Categoría</dt><dd>{product.categoria}</dd></div>
            <div><dt>Talla</dt><dd>{product.talla}</dd></div>
            <div><dt>Color</dt><dd>{product.color}</dd></div>
            <div><dt>Existencias</dt><dd>{product.stock} {product.stock === 1 ? 'pieza' : 'piezas'}</dd></div>
          </dl>

          <div className={`stock-note ${available ? 'is-available' : 'is-unavailable'}`}>
            <span aria-hidden="true" />
            <p>
              <strong>{available ? 'Lista para elegir' : 'Sin existencias por ahora'}</strong>
              <small>{available ? `Hay ${product.stock} en inventario.` : 'Consulta de nuevo próximamente.'}</small>
            </p>
          </div>

          <Link className="button button--ghost product-detail__back" to="/catalogo">
            <ArrowIcon direction="left" /> Regresar al catálogo
          </Link>
        </div>
      </div>
    </article>
  )
}

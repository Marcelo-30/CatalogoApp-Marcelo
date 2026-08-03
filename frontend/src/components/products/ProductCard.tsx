import { Link } from 'react-router-dom'
import type { Product } from '../../types/catalog'
import { formatPrice } from '../../utils/catalog'
import { ArrowIcon } from '../ui/ArrowIcon'
import { ImageWithFallback } from '../ui/ImageWithFallback'

export function ProductCard({ product }: { product: Product }) {
  const available = product.disponible && product.stock > 0

  return (
    <article className="product-card">
      <Link
        className="product-card__image-link"
        to={`/catalogo/${product.id}`}
        aria-label={`Ver detalles de ${product.nombre}`}
      >
        <ImageWithFallback
          src={product.imagenUrl}
          alt={product.nombre}
          className="product-card__image"
        />
        <span className={`availability-badge ${available ? 'is-available' : 'is-unavailable'}`}>
          <span aria-hidden="true" />
          {available ? 'Disponible' : 'No disponible'}
        </span>
      </Link>
      <div className="product-card__body">
        <p className="product-card__category">{product.categoria}</p>
        <h3><Link to={`/catalogo/${product.id}`}>{product.nombre}</Link></h3>
        <div className="product-card__meta" aria-label="Características">
          <span>Talla {product.talla}</span>
          <span>{product.color}</span>
        </div>
        <div className="product-card__footer">
          <p className="product-card__price">{formatPrice(product.precio)}</p>
          <Link className="card-link" to={`/catalogo/${product.id}`}>
            Detalles <ArrowIcon />
          </Link>
        </div>
      </div>
    </article>
  )
}


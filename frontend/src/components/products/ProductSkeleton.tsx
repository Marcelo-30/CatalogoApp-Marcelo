export function ProductSkeleton() {
  return (
    <div className="product-card product-card--skeleton" aria-hidden="true">
      <div className="skeleton skeleton--image" />
      <div className="product-card__body">
        <div className="skeleton skeleton--short" />
        <div className="skeleton skeleton--title" />
        <div className="skeleton skeleton--line" />
        <div className="skeleton skeleton--price" />
      </div>
    </div>
  )
}

export function ProductGridSkeleton({ count = 8 }: { count?: number }) {
  return (
    <div className="product-grid" aria-label="Cargando productos" aria-busy="true">
      {Array.from({ length: count }, (_, index) => <ProductSkeleton key={index} />)}
    </div>
  )
}

import { useMemo } from 'react'
import { useSearchParams } from 'react-router-dom'
import { getCategories, getProducts } from '../api/catalogApi'
import { CatalogFilters } from '../components/products/CatalogFilters'
import { ProductCard } from '../components/products/ProductCard'
import { ProductGridSkeleton } from '../components/products/ProductSkeleton'
import { StatusPanel } from '../components/ui/StatusPanel'
import { useApiResource } from '../hooks/useApiResource'
import { useDocumentTitle } from '../hooks/useDocumentTitle'
import { useReveal } from '../hooks/useReveal'
import type { ProductFilters } from '../types/catalog'
import { filterProducts } from '../utils/catalog'

const emptyFilters: ProductFilters = { search: '', category: '', availableOnly: false }

export function CatalogPage() {
  const [searchParams, setSearchParams] = useSearchParams()
  const products = useApiResource(getProducts)
  const categoryResource = useApiResource(getCategories)

  useDocumentTitle('Catálogo')
  useReveal()

  const search = searchParams.get('buscar') ?? ''
  const category = searchParams.get('categoria') ?? ''
  const availableOnly = searchParams.get('disponibles') === '1'
  const filters = useMemo<ProductFilters>(() => ({ search, category, availableOnly }), [
    search,
    category,
    availableOnly,
  ])

  const visibleProducts = useMemo(
    () => filterProducts(products.data ?? [], filters),
    [filters, products.data],
  )

  const categories = useMemo(() => {
    if (categoryResource.data) return categoryResource.data.map((category) => category.nombre)
    return [...new Set((products.data ?? []).map((product) => product.categoria))].sort()
  }, [categoryResource.data, products.data])

  const updateFilters = (next: ProductFilters) => {
    const params = new URLSearchParams()
    if (next.search) params.set('buscar', next.search)
    if (next.category) params.set('categoria', next.category)
    if (next.availableOnly) params.set('disponibles', '1')
    setSearchParams(params, { replace: true })
  }

  return (
    <div className="catalog-page">
      <section className="page-hero page-hero--catalog">
        <div className="page-hero__grid" aria-hidden="true" />
        <div className="container page-hero__content">
          <span className="eyebrow">Colección completa</span>
          <h1>Prendas con <em>presencia.</em></h1>
          <p>Explora por nombre, categoría o disponibilidad. Sin recargas, sin distracciones.</p>
        </div>
      </section>

      <section className="section catalog-section">
        <div className="container catalog-layout">
          <div data-reveal>
            <CatalogFilters
              filters={filters}
              categories={categories}
              resultCount={visibleProducts.length}
              onChange={updateFilters}
              onClear={() => updateFilters(emptyFilters)}
            />
          </div>

          <div className="catalog-results" data-reveal>
            <div className="catalog-results__heading">
              <div>
                <span className="eyebrow">Catálogo</span>
                <h2>{filters.category || 'Todas las piezas'}</h2>
              </div>
              <span>{visibleProducts.length} {visibleProducts.length === 1 ? 'pieza' : 'piezas'}</span>
            </div>

            {products.loading && <ProductGridSkeleton />}
            {products.error && (
              <StatusPanel
                eyebrow="Conexión interrumpida"
                title="No pudimos abrir el catálogo"
                message={products.error.message}
                actionLabel="Volver a intentar"
                onAction={products.retry}
              />
            )}
            {products.data && visibleProducts.length === 0 && (
              <StatusPanel
                eyebrow="Sin coincidencias"
                title="No encontramos esa combinación"
                message="Prueba con otro término, cambia la categoría o muestra también las piezas no disponibles."
                actionLabel="Limpiar filtros"
                onAction={() => updateFilters(emptyFilters)}
              />
            )}
            {visibleProducts.length > 0 && (
              <div className="product-grid">
                {visibleProducts.map((product) => <ProductCard key={product.id} product={product} />)}
              </div>
            )}
          </div>
        </div>
      </section>
    </div>
  )
}

import type { Product, ProductFilters } from '../types/catalog'

const normalize = (value: string) => value
  .normalize('NFD')
  .replace(/[\u0300-\u036f]/g, '')
  .toLocaleLowerCase('es-MX')
  .trim()

export function filterProducts(products: Product[], filters: ProductFilters) {
  const search = normalize(filters.search)
  const searchTerms = search.split(/\s+/).filter(Boolean)

  return products.filter((product) => {
    const searchable = normalize([
      product.nombre,
      product.descripcion ?? '',
      product.categoria,
      product.talla,
      product.color,
    ].join(' '))

    const matchesSearch = searchTerms.every((term) => searchable.includes(term))
    const matchesCategory = !filters.category || product.categoria === filters.category
    const matchesAvailability = !filters.availableOnly || (product.disponible && product.stock > 0)

    return matchesSearch && matchesCategory && matchesAvailability
  })
}

export const formatPrice = new Intl.NumberFormat('es-MX', {
  style: 'currency',
  currency: 'MXN',
  maximumFractionDigits: 0,
}).format

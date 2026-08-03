import type { ProductFilters } from '../../types/catalog'

interface CatalogFiltersProps {
  filters: ProductFilters
  categories: string[]
  resultCount: number
  onChange: (filters: ProductFilters) => void
  onClear: () => void
}

export function CatalogFilters({
  filters,
  categories,
  resultCount,
  onChange,
  onClear,
}: CatalogFiltersProps) {
  const hasFilters = Boolean(filters.search || filters.category || filters.availableOnly)

  return (
    <aside className="filters" aria-label="Filtros del catálogo">
      <div className="filters__heading">
        <div>
          <span className="eyebrow">Afinar selección</span>
          <h2>Encuentra tu prenda</h2>
        </div>
        {hasFilters && (
          <button className="text-button" type="button" onClick={onClear}>
            Limpiar
          </button>
        )}
      </div>

      <div className="field">
        <label htmlFor="catalog-search">Buscar</label>
        <div className="search-input">
          <svg aria-hidden="true" viewBox="0 0 20 20">
            <circle cx="8.5" cy="8.5" r="5.5" />
            <path d="m13 13 4 4" />
          </svg>
          <input
            id="catalog-search"
            type="search"
            value={filters.search}
            placeholder="Nombre, color o talla"
            onChange={(event) => onChange({ ...filters, search: event.target.value })}
          />
        </div>
      </div>

      <div className="field">
        <label htmlFor="catalog-category">Categoría</label>
        <select
          id="catalog-category"
          value={filters.category}
          onChange={(event) => onChange({ ...filters, category: event.target.value })}
        >
          <option value="">Todas las categorías</option>
          {categories.map((category) => (
            <option key={category} value={category}>{category}</option>
          ))}
        </select>
      </div>

      <label className="toggle-field">
        <span>
          <strong>Solo disponibles</strong>
          <small>Oculta piezas sin existencias</small>
        </span>
        <input
          type="checkbox"
          checked={filters.availableOnly}
          onChange={(event) => onChange({ ...filters, availableOnly: event.target.checked })}
        />
        <span className="toggle" aria-hidden="true"><span /></span>
      </label>

      <p className="filters__result" aria-live="polite">
        <strong>{resultCount}</strong> {resultCount === 1 ? 'resultado' : 'resultados'}
      </p>
    </aside>
  )
}

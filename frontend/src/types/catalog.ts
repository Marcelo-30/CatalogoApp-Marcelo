export interface Product {
  id: number
  nombre: string
  descripcion: string | null
  categoria: string
  talla: string
  color: string
  precio: number
  stock: number
  disponible: boolean
  imagenUrl: string | null
}

export interface Category {
  id: number
  nombre: string
  descripcion: string | null
}

export interface Size {
  id: number
  nombre: string
}

export interface ProductFilters {
  search: string
  category: string
  availableOnly: boolean
}

import type { Category, Product, Size } from './catalog'

export interface AuthStatus {
  isAuthenticated: boolean
  userName: string | null
  role: string | null
  canRegisterSeller: boolean
}

export interface LoginInput {
  email: string
  password: string
}

export interface RegisterInput extends LoginInput {
  nombre: string
  confirmarPassword: string
}

export interface ColorOption {
  id: number
  nombre: string
  codigoHex: string | null
}

export interface ProductInput {
  nombre: string
  descripcion: string | null
  precio: number
  stock: number
  disponible: boolean
  categoriaId: number
  tallaId: number
  colorId: number
  imagenUrl: string | null
}

export interface CategoryInput {
  nombre: string
  descripcion: string | null
}

export interface ProductOptions {
  categories: Category[]
  sizes: Size[]
  colors: ColorOption[]
}

export interface AdminSummary {
  products: Product[]
  categories: Category[]
}

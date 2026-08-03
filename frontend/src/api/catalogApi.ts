import type { Category, Product, Size } from '../types/catalog'
import { requestJson } from './httpClient'

export { CatalogApiError } from './httpClient'

export const getProducts = (signal?: AbortSignal) =>
  requestJson<Product[]>('/api/productos', { signal })

export const getProduct = (id: number, signal?: AbortSignal) =>
  requestJson<Product>(`/api/productos/${id}`, { signal })

export const getCategories = (signal?: AbortSignal) =>
  requestJson<Category[]>('/api/categorias', { signal })

export const getSizes = (signal?: AbortSignal) =>
  requestJson<Size[]>('/api/tallas', { signal })

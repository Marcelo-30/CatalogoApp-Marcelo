import type { Category, Product } from '../types/catalog'
import type {
  AdminSummary,
  CategoryInput,
  ColorOption,
  ProductInput,
  ProductOptions,
} from '../types/admin'
import { getCategories, getProduct, getProducts, getSizes } from './catalogApi'
import { mutateJson, requestJson } from './httpClient'

export const getColors = (signal?: AbortSignal) =>
  requestJson<ColorOption[]>('/api/colores', { signal })

export const getProductOptions = async (signal?: AbortSignal): Promise<ProductOptions> => {
  const [categories, sizes, colors] = await Promise.all([
    getCategories(signal),
    getSizes(signal),
    getColors(signal),
  ])
  return { categories, sizes, colors }
}

export const getAdminSummary = async (signal?: AbortSignal): Promise<AdminSummary> => {
  const [products, categories] = await Promise.all([
    getProducts(signal),
    getCategories(signal),
  ])
  return { products, categories }
}

export const createProduct = (input: ProductInput) =>
  mutateJson<Product>('/api/productos', 'POST', input)

export const updateProduct = (id: number, input: ProductInput) =>
  mutateJson<void>(`/api/productos/${id}`, 'PUT', input)

export const deleteProduct = (id: number) =>
  mutateJson<void>(`/api/productos/${id}`, 'DELETE')

export const createCategory = (input: CategoryInput) =>
  mutateJson<Category>('/api/categorias', 'POST', input)

export const updateCategory = (id: number, input: CategoryInput) =>
  mutateJson<void>(`/api/categorias/${id}`, 'PUT', input)

export const deleteCategory = (id: number) =>
  mutateJson<void>(`/api/categorias/${id}`, 'DELETE')

export { getProduct }

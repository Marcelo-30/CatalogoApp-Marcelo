import { describe, expect, it } from 'vitest'
import type { Product } from '../types/catalog'
import { filterProducts } from './catalog'

const products: Product[] = [
  {
    id: 1,
    nombre: 'Playera básica blanca',
    descripcion: 'Algodón para uso diario',
    categoria: 'Playeras',
    talla: 'M',
    color: 'Blanco',
    precio: 199,
    stock: 12,
    disponible: true,
    imagenUrl: null,
  },
  {
    id: 2,
    nombre: 'Chamarra negra',
    descripcion: null,
    categoria: 'Chamarras',
    talla: 'L',
    color: 'Negro',
    precio: 899,
    stock: 0,
    disponible: true,
    imagenUrl: null,
  },
]

describe('filterProducts', () => {
  it('matches search text without requiring accents', () => {
    const result = filterProducts(products, {
      search: 'basica algodon',
      category: '',
      availableOnly: false,
    })

    expect(result).toHaveLength(1)
    expect(result[0].id).toBe(1)
  })

  it('combines category and effective availability filters', () => {
    const result = filterProducts(products, {
      search: '',
      category: 'Chamarras',
      availableOnly: true,
    })

    expect(result).toEqual([])
  })
})

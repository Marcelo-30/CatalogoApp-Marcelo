import { createBrowserRouter, Navigate, RouterProvider, useParams } from 'react-router-dom'
import { AdminLayout } from './components/admin/AdminLayout'
import { SiteLayout } from './components/layout/SiteLayout'
import { AdminCategoriesPage } from './pages/AdminCategoriesPage'
import { AdminDashboardPage } from './pages/AdminDashboardPage'
import { AdminProductFormPage } from './pages/AdminProductFormPage'
import { AdminProductsPage } from './pages/AdminProductsPage'
import { CatalogPage } from './pages/CatalogPage'
import { HomePage } from './pages/HomePage'
import { NotFoundPage } from './pages/NotFoundPage'
import { ProductDetailsPage } from './pages/ProductDetailsPage'
import { SellerLoginPage } from './pages/SellerLoginPage'
import { SellerRegisterPage } from './pages/SellerRegisterPage'

function LegacyProductRedirect({ edit = false }: { edit?: boolean }) {
  const { id } = useParams()
  return <Navigate to={edit ? `/admin/productos/${id}/editar` : `/catalogo/${id}`} replace />
}

const router = createBrowserRouter([
  {
    element: <SiteLayout />,
    children: [
      { path: '/', element: <HomePage /> },
      { path: '/catalogo', element: <CatalogPage /> },
      { path: '/catalogo/:id', element: <ProductDetailsPage /> },
      { path: '/vendedor/login', element: <SellerLoginPage /> },
      { path: '/vendedor/registro', element: <SellerRegisterPage /> },
      { path: '/Cuenta/Login', element: <Navigate to="/vendedor/login" replace /> },
      { path: '/Cuenta/Registro', element: <Navigate to="/vendedor/registro" replace /> },
      { path: '/Cuenta/AccesoDenegado', element: <Navigate to="/vendedor/login?denegado=1" replace /> },
      { path: '/Productos', element: <Navigate to="/admin/productos" replace /> },
      { path: '/Productos/Create', element: <Navigate to="/admin/productos/nuevo" replace /> },
      { path: '/Productos/Edit/:id', element: <LegacyProductRedirect edit /> },
      { path: '/Productos/Details/:id', element: <LegacyProductRedirect /> },
      { path: '/Productos/Delete/:id', element: <Navigate to="/admin/productos" replace /> },
      { path: '/Categorias', element: <Navigate to="/admin/categorias" replace /> },
      { path: '/Categorias/Create', element: <Navigate to="/admin/categorias" replace /> },
      { path: '/Categorias/Edit/:id', element: <Navigate to="/admin/categorias" replace /> },
      { path: '/Categorias/Details/:id', element: <Navigate to="/admin/categorias" replace /> },
      { path: '/Categorias/Delete/:id', element: <Navigate to="/admin/categorias" replace /> },
      { path: '/Home/Index', element: <Navigate to="/" replace /> },
      { path: '/Home/Privacy', element: <Navigate to="/" replace /> },
      { path: '*', element: <NotFoundPage /> },
    ],
  },
  {
    path: '/admin',
    element: <AdminLayout />,
    children: [
      { index: true, element: <AdminDashboardPage /> },
      { path: 'productos', element: <AdminProductsPage /> },
      { path: 'productos/nuevo', element: <AdminProductFormPage /> },
      { path: 'productos/:id/editar', element: <AdminProductFormPage /> },
      { path: 'categorias', element: <AdminCategoriesPage /> },
    ],
  },
])

export default function App() {
  return <RouterProvider router={router} />
}

import { useState } from 'react'
import { Link, Navigate, NavLink, Outlet, useNavigate } from 'react-router-dom'
import { getAuthStatus, logoutSeller } from '../../api/authApi'
import { useApiResource } from '../../hooks/useApiResource'

export function AdminLayout() {
  const auth = useApiResource(getAuthStatus)
  const [menuOpen, setMenuOpen] = useState(false)
  const [logoutError, setLogoutError] = useState<string | null>(null)
  const [loggingOut, setLoggingOut] = useState(false)
  const navigate = useNavigate()

  const logout = async () => {
    setLoggingOut(true)
    setLogoutError(null)
    try {
      await logoutSeller()
      navigate('/vendedor/login', { replace: true })
    } catch (error) {
      setLogoutError(error instanceof Error ? error.message : 'No pudimos cerrar la sesión.')
      setLoggingOut(false)
    }
  }

  if (auth.loading) {
    return (
      <div className="admin-gate" aria-busy="true">
        <span className="admin-loader" />
        <p>Verificando sesión segura…</p>
      </div>
    )
  }

  if (!auth.data?.isAuthenticated || auth.data.role !== 'Vendedor') {
    return <Navigate to="/vendedor/login" replace />
  }

  return (
    <div className="admin-shell">
      <aside className={`admin-sidebar ${menuOpen ? 'is-open' : ''}`}>
        <Link className="brand admin-brand" to="/admin" onClick={() => setMenuOpen(false)}>
          <span className="brand__mark" aria-hidden="true"><i /><i /><i /></span>
          <span className="brand__text">CATÁLOGO <b>30</b></span>
        </Link>

        <div className="admin-profile">
          <span>{auth.data.userName?.charAt(0).toLocaleUpperCase('es-MX') ?? 'V'}</span>
          <div>
            <strong>{auth.data.userName}</strong>
            <small>Vendedor</small>
          </div>
        </div>

        <nav className="admin-nav" aria-label="Administración">
          <NavLink to="/admin" end onClick={() => setMenuOpen(false)}>
            <span aria-hidden="true">⌂</span> Resumen
          </NavLink>
          <NavLink to="/admin/productos" onClick={() => setMenuOpen(false)}>
            <span aria-hidden="true">◇</span> Productos
          </NavLink>
          <NavLink to="/admin/categorias" onClick={() => setMenuOpen(false)}>
            <span aria-hidden="true">▦</span> Categorías
          </NavLink>
        </nav>

        <div className="admin-sidebar__footer">
          <Link to="/" className="admin-store-link">Ver catálogo público ↗</Link>
          <button type="button" onClick={logout} disabled={loggingOut}>
            {loggingOut ? 'Cerrando sesión…' : 'Cerrar sesión'}
          </button>
          {logoutError && <p role="alert">{logoutError}</p>}
        </div>
      </aside>

      <div className="admin-main">
        <header className="admin-mobile-header">
          <Link className="brand" to="/admin">
            <span className="brand__mark" aria-hidden="true"><i /><i /><i /></span>
            <span className="brand__text">CATÁLOGO <b>30</b></span>
          </Link>
          <button
            className={`menu-button ${menuOpen ? 'is-open' : ''}`}
            type="button"
            aria-label={menuOpen ? 'Cerrar menú' : 'Abrir menú administrativo'}
            aria-expanded={menuOpen}
            onClick={() => setMenuOpen((value) => !value)}
          >
            <span /><span /><span />
          </button>
        </header>
        <main className="admin-content" id="admin-content">
          <Outlet />
        </main>
      </div>
      {menuOpen && <button className="admin-overlay" type="button" aria-label="Cerrar menú" onClick={() => setMenuOpen(false)} />}
    </div>
  )
}


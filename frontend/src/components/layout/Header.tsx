import { useEffect, useState } from 'react'
import { Link, NavLink } from 'react-router-dom'

export function Header() {
  const [menuOpen, setMenuOpen] = useState(false)

  useEffect(() => {
    const closeOnEscape = (event: KeyboardEvent) => {
      if (event.key === 'Escape') setMenuOpen(false)
    }

    document.addEventListener('keydown', closeOnEscape)
    return () => document.removeEventListener('keydown', closeOnEscape)
  }, [])

  return (
    <header className="site-header">
      <div className="container header-inner">
        <Link className="brand" to="/" aria-label="Catálogo 30, ir al inicio">
          <span className="brand__mark" aria-hidden="true"><i /><i /><i /></span>
          <span className="brand__text">CATÁLOGO <b>30</b></span>
        </Link>

        <button
          className={`menu-button ${menuOpen ? 'is-open' : ''}`}
          type="button"
          aria-label={menuOpen ? 'Cerrar menú' : 'Abrir menú'}
          aria-expanded={menuOpen}
          aria-controls="primary-navigation"
          onClick={() => setMenuOpen((value) => !value)}
        >
          <span /><span /><span />
        </button>

        <nav
          id="primary-navigation"
          className={`primary-nav ${menuOpen ? 'is-open' : ''}`}
          aria-label="Navegación principal"
        >
          <NavLink to="/" end>Inicio</NavLink>
          <NavLink to="/catalogo">Catálogo</NavLink>
          <Link className="seller-link" to="/vendedor/login">
            Acceso vendedor
            <svg aria-hidden="true" viewBox="0 0 20 20">
              <path d="M7 4h9v9M16 4 7 13M13 16H4V7" />
            </svg>
          </Link>
        </nav>
      </div>
    </header>
  )
}

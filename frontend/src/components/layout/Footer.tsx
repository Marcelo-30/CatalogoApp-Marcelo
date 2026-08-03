import { Link } from 'react-router-dom'

export function Footer() {
  return (
    <footer className="site-footer">
      <div className="container footer-grid">
        <div className="footer-intro">
          <Link className="brand" to="/">
            <span className="brand__mark" aria-hidden="true"><i /><i /><i /></span>
            <span className="brand__text">CATÁLOGO <b>30</b></span>
          </Link>
          <p>Una selección contemporánea donde cada prenda se presenta con claridad, carácter y detalle.</p>
        </div>
        <div className="footer-links">
          <h2>Explora</h2>
          <Link to="/">Inicio</Link>
          <Link to="/catalogo">Catálogo</Link>
        </div>
        <div className="footer-links">
          <h2>Administración</h2>
          <Link to="/vendedor/login">Acceso vendedor</Link>
          <Link to="/admin/productos">Gestionar productos</Link>
        </div>
      </div>
      <div className="container footer-bottom">
        <span>© {new Date().getFullYear()} Catálogo 30</span>
        <span>React + ASP.NET Core</span>
      </div>
    </footer>
  )
}

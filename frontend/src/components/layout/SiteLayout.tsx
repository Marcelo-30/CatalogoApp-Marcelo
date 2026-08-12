import { Outlet, ScrollRestoration, useLocation } from 'react-router-dom'
import { Footer } from './Footer'
import { Header } from './Header'

export function SiteLayout() {
  const location = useLocation()

  return (
    <>
      <a className="skip-link" href="#main-content">Saltar al contenido</a>
      <Header key={location.pathname} />
      <main id="main-content">
        <div className="route-view" key={location.pathname}>
          <Outlet />
        </div>
      </main>
      <Footer />
      <ScrollRestoration />
    </>
  )
}

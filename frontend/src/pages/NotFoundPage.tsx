import { StatusPanel } from '../components/ui/StatusPanel'
import { useDocumentTitle } from '../hooks/useDocumentTitle'

export function NotFoundPage() {
  useDocumentTitle('Página no encontrada')

  return (
    <div className="not-found-page">
      <div className="container">
        <span className="not-found-page__number" aria-hidden="true">404</span>
        <StatusPanel
          eyebrow="Te saliste de la colección"
          title="Esta página no existe"
          message="Regresa al catálogo y encuentra una ruta que sí combine contigo."
          actionLabel="Ir al catálogo"
          actionTo="/catalogo"
        />
      </div>
    </div>
  )
}

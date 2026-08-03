import { Link } from 'react-router-dom'
import heroImage from '../assets/catalog-hero.png'
import { getCategories, getProducts } from '../api/catalogApi'
import { ProductCard } from '../components/products/ProductCard'
import { ProductGridSkeleton } from '../components/products/ProductSkeleton'
import { ArrowIcon } from '../components/ui/ArrowIcon'
import { StatusPanel } from '../components/ui/StatusPanel'
import { useApiResource } from '../hooks/useApiResource'
import { useDocumentTitle } from '../hooks/useDocumentTitle'
import { useReveal } from '../hooks/useReveal'

export function HomePage() {
  const products = useApiResource(getProducts)
  const categories = useApiResource(getCategories)

  useDocumentTitle('Inicio')
  useReveal()

  return (
    <>
      <section className="hero" aria-labelledby="hero-title">
        <img className="hero__image" src={heroImage} alt="" fetchPriority="high" />
        <div className="hero__overlay" />
        <div className="hero__glow" aria-hidden="true" />
        <div className="container hero__inner">
          <div className="hero__content">
            <span className="hero__edition"><i /> Colección contemporánea · 2026</span>
            <h1 id="hero-title">Tu estilo,<br /><em>sin ruido.</em></h1>
            <p>Prendas que hablan con claridad. Descubre una selección pensada para acompañarte todos los días.</p>
            <div className="hero__actions">
              <Link className="button button--primary" to="/catalogo">
                Explorar catálogo <ArrowIcon />
              </Link>
              <a className="button button--ghost" href="#seleccion">Ver selección</a>
            </div>
          </div>
          <div className="hero__aside" aria-hidden="true">
            <span>01</span>
            <i />
            <p>Esenciales<br />con carácter</p>
          </div>
        </div>
        <a className="hero__scroll" href="#categorias" aria-label="Ir a categorías">
          <span>Descubre</span><i />
        </a>
      </section>

      <section id="categorias" className="section section--categories">
        <div className="container">
          <header className="section-heading" data-reveal>
            <div>
              <span className="eyebrow">Explora a tu manera</span>
              <h2>Categorías con intención</h2>
            </div>
            <p>Encuentra la pieza adecuada sin perderte entre opciones.</p>
          </header>

          <div className="category-grid" data-reveal>
            {categories.loading && Array.from({ length: 4 }, (_, index) => (
              <div className="category-card category-card--skeleton" key={index} aria-hidden="true">
                <div className="skeleton skeleton--short" />
                <div className="skeleton skeleton--title" />
              </div>
            ))}
            {categories.data?.map((category, index) => (
              <Link
                className="category-card"
                key={category.id}
                to={`/catalogo?categoria=${encodeURIComponent(category.nombre)}`}
              >
                <span className="category-card__number">0{index + 1}</span>
                <div>
                  <h3>{category.nombre}</h3>
                  <p>{category.descripcion ?? 'Descubre nuestra selección.'}</p>
                </div>
                <span className="category-card__arrow"><ArrowIcon /></span>
              </Link>
            ))}
            {categories.error && (
              <StatusPanel
                compact
                eyebrow="Categorías"
                title="No pudimos cargar esta sección"
                message="Puedes seguir explorando el catálogo completo."
                actionLabel="Ver catálogo"
                actionTo="/catalogo"
              />
            )}
          </div>
        </div>
      </section>

      <section id="seleccion" className="section section--selection">
        <div className="container">
          <header className="section-heading" data-reveal>
            <div>
              <span className="eyebrow">Selección destacada</span>
              <h2>Piezas para volver a mirar</h2>
            </div>
            <Link className="section-link" to="/catalogo">Ver todo <ArrowIcon /></Link>
          </header>

          <div data-reveal>
            {products.loading && <ProductGridSkeleton count={3} />}
            {products.error && (
              <StatusPanel
                compact
                eyebrow="Conexión interrumpida"
                title="La selección está tomando un descanso"
                message={products.error.message}
                actionLabel="Volver a intentar"
                onAction={products.retry}
              />
            )}
            {products.data && products.data.length === 0 && (
              <StatusPanel
                compact
                title="Pronto habrá nuevas piezas"
                message="El catálogo todavía no tiene productos publicados."
                actionLabel="Explorar catálogo"
                actionTo="/catalogo"
              />
            )}
            {products.data && products.data.length > 0 && (
              <div className="product-grid product-grid--featured">
                {products.data.slice(0, 3).map((product) => (
                  <ProductCard key={product.id} product={product} />
                ))}
              </div>
            )}
          </div>
        </div>
      </section>

      <section className="section section--manifesto">
        <div className="container manifesto" data-reveal>
          <div className="manifesto__copy">
            <span className="eyebrow">La experiencia importa</span>
            <h2>Elegir ropa debería sentirse <em>así de simple.</em></h2>
          </div>
          <div className="manifesto__points">
            <article>
              <span>01</span>
              <div><h3>Selección clara</h3><p>La información esencial, justo donde la necesitas.</p></div>
            </article>
            <article>
              <span>02</span>
              <div><h3>Detalles a simple vista</h3><p>Talla, color y disponibilidad sin pasos innecesarios.</p></div>
            </article>
            <article>
              <span>03</span>
              <div><h3>Diseño para ti</h3><p>Una experiencia cómoda en móvil, tablet o escritorio.</p></div>
            </article>
          </div>
        </div>
      </section>

      <section className="section section--cta">
        <div className="container">
          <div className="cta-panel" data-reveal>
            <span className="cta-panel__orb" aria-hidden="true" />
            <div>
              <span className="eyebrow">Tu próxima pieza</span>
              <h2>Encuentra algo que se sienta tuyo.</h2>
            </div>
            <Link className="button button--light" to="/catalogo">
              Abrir catálogo <ArrowIcon />
            </Link>
          </div>
        </div>
      </section>
    </>
  )
}


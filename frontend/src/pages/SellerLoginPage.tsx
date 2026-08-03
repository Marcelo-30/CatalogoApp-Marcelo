import { type FormEvent, useState } from 'react'
import { Link, Navigate, useNavigate, useSearchParams } from 'react-router-dom'
import { getAuthStatus, loginSeller } from '../api/authApi'
import { ArrowIcon } from '../components/ui/ArrowIcon'
import { useApiResource } from '../hooks/useApiResource'
import { useDocumentTitle } from '../hooks/useDocumentTitle'

export function SellerLoginPage() {
  const auth = useApiResource(getAuthStatus)
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)
  const [searchParams] = useSearchParams()
  const navigate = useNavigate()

  useDocumentTitle('Acceso vendedor')

  const submit = async (event: FormEvent) => {
    event.preventDefault()
    setSubmitting(true)
    setError(null)
    try {
      await loginSeller({ email, password })
      const requested = searchParams.get('returnTo')
      navigate(requested?.startsWith('/admin') ? requested : '/admin', { replace: true })
    } catch (submitError) {
      setError(submitError instanceof Error ? submitError.message : 'No pudimos iniciar sesión.')
      setSubmitting(false)
    }
  }

  if (auth.data?.isAuthenticated) return <Navigate to="/admin" replace />

  return (
    <section className="auth-page">
      <div className="auth-page__visual" aria-hidden="true">
        <span className="auth-page__line" />
        <div><span>ÁREA PRIVADA</span><strong>Controla tu catálogo<br />con la misma claridad.</strong></div>
      </div>
      <div className="auth-panel">
        <div className="auth-panel__inner">
          <span className="eyebrow">Acceso seguro</span>
          <h1>Bienvenido de nuevo.</h1>
          <p>Inicia sesión para gestionar productos, inventario y categorías.</p>

          {error && <div className="form-alert" role="alert"><span>!</span>{error}</div>}

          <form className="admin-form auth-form" onSubmit={submit}>
            <div className="form-field">
              <label htmlFor="seller-email">Correo electrónico</label>
              <input
                id="seller-email"
                name="email"
                type="email"
                autoComplete="username"
                required
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                placeholder="vendedor@ejemplo.com"
              />
            </div>
            <div className="form-field">
              <label htmlFor="seller-password">Contraseña</label>
              <div className="password-input">
                <input
                  id="seller-password"
                  name="password"
                  type={showPassword ? 'text' : 'password'}
                  autoComplete="current-password"
                  required
                  value={password}
                  onChange={(event) => setPassword(event.target.value)}
                  placeholder="Tu contraseña"
                />
                <button type="button" onClick={() => setShowPassword((value) => !value)}>
                  {showPassword ? 'Ocultar' : 'Mostrar'}
                </button>
              </div>
            </div>
            <button className="button button--primary auth-submit" type="submit" disabled={submitting || auth.loading}>
              {submitting ? 'Verificando…' : <>Entrar al panel <ArrowIcon /></>}
            </button>
          </form>

          {auth.data?.canRegisterSeller && (
            <p className="auth-register">¿Es la primera configuración? <Link to="/vendedor/registro">Registrar vendedor</Link></p>
          )}
          <Link className="auth-back" to="/"><ArrowIcon direction="left" /> Volver al catálogo</Link>
        </div>
      </div>
    </section>
  )
}

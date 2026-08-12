import { type FormEvent, useState } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { getAuthStatus, registerSeller } from '../api/authApi'
import { ArrowIcon } from '../components/ui/ArrowIcon'
import { useApiResource } from '../hooks/useApiResource'
import { useDocumentTitle } from '../hooks/useDocumentTitle'

export function SellerRegisterPage() {
  const auth = useApiResource(getAuthStatus)
  const [form, setForm] = useState({ nombre: '', email: '', password: '', confirmarPassword: '' })
  const [error, setError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)
  const navigate = useNavigate()

  useDocumentTitle('Registrar vendedor')

  const submit = async (event: FormEvent) => {
    event.preventDefault()
    if (form.password !== form.confirmarPassword) {
      setError('Las contraseñas no coinciden.')
      return
    }
    setSubmitting(true)
    setError(null)
    try {
      await registerSeller(form)
      navigate('/admin', { replace: true })
    } catch (submitError) {
      setError(submitError instanceof Error ? submitError.message : 'No pudimos registrar al vendedor.')
      setSubmitting(false)
    }
  }

  if (auth.data?.isAuthenticated) return <Navigate to="/admin" replace />
  if (!auth.loading && auth.data && !auth.data.canRegisterSeller) return <Navigate to="/vendedor/login" replace />

  return (
    <section className="auth-page auth-page--register">
      <div className="auth-page__visual" aria-hidden="true">
        <span className="auth-page__line" />
        <div><span>CONFIGURACIÓN INICIAL</span><strong>Una cuenta.<br />Todo el catálogo.</strong></div>
      </div>
      <div className="auth-panel">
        <div className="auth-panel__inner">
          <span className="eyebrow">Vendedor propietario</span>
          <h1>Crea tu acceso.</h1>
          <p>Solo puede existir una cuenta de vendedor. Sus credenciales se protegen en el servidor.</p>
          {error && <div className="form-alert" role="alert"><span>!</span>{error}</div>}
          <form className="admin-form auth-form" onSubmit={submit}>
            <div className="form-field">
              <label htmlFor="register-name">Nombre completo</label>
              <input id="register-name" required maxLength={80} autoComplete="name" value={form.nombre} onChange={(event) => setForm({ ...form, nombre: event.target.value })} />
            </div>
            <div className="form-field">
              <label htmlFor="register-email">Correo electrónico</label>
              <input id="register-email" type="email" required autoComplete="username" value={form.email} onChange={(event) => setForm({ ...form, email: event.target.value })} />
            </div>
            <div className="form-row">
              <div className="form-field">
                <label htmlFor="register-password">Contraseña</label>
                <input id="register-password" type="password" required minLength={6} maxLength={100} autoComplete="new-password" value={form.password} onChange={(event) => setForm({ ...form, password: event.target.value })} />
              </div>
              <div className="form-field">
                <label htmlFor="register-confirm">Confirmar contraseña</label>
                <input id="register-confirm" type="password" required minLength={6} maxLength={100} autoComplete="new-password" value={form.confirmarPassword} onChange={(event) => setForm({ ...form, confirmarPassword: event.target.value })} />
              </div>
            </div>
            <button className="button button--primary auth-submit" type="submit" disabled={submitting || auth.loading}>
              {submitting ? 'Creando acceso…' : <>Crear cuenta y entrar <ArrowIcon /></>}
            </button>
          </form>
          <p className="auth-register">¿Ya tienes una cuenta? <Link to="/vendedor/login">Iniciar sesión</Link></p>
        </div>
      </div>
    </section>
  )
}

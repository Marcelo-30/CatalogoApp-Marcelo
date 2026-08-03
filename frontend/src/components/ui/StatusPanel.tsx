import { Link } from 'react-router-dom'

interface StatusPanelProps {
  eyebrow?: string
  title: string
  message: string
  actionLabel?: string
  onAction?: () => void
  actionTo?: string
  compact?: boolean
}

export function StatusPanel({
  eyebrow = 'Catálogo 30',
  title,
  message,
  actionLabel,
  onAction,
  actionTo,
  compact = false,
}: StatusPanelProps) {
  const action = actionLabel && actionTo
    ? <Link className="button button--primary" to={actionTo}>{actionLabel}</Link>
    : actionLabel && onAction
      ? <button className="button button--primary" type="button" onClick={onAction}>{actionLabel}</button>
      : null

  return (
    <section className={`status-panel ${compact ? 'status-panel--compact' : ''}`} aria-live="polite">
      <span className="eyebrow">{eyebrow}</span>
      <h2>{title}</h2>
      <p>{message}</p>
      {action}
    </section>
  )
}


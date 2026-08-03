import { useEffect, useRef } from 'react'

interface ConfirmDialogProps {
  open: boolean
  title: string
  message: string
  confirmLabel?: string
  busy?: boolean
  onConfirm: () => void
  onCancel: () => void
}

export function ConfirmDialog({
  open,
  title,
  message,
  confirmLabel = 'Eliminar',
  busy = false,
  onConfirm,
  onCancel,
}: ConfirmDialogProps) {
  const cancelButton = useRef<HTMLButtonElement>(null)

  useEffect(() => {
    if (!open) return
    cancelButton.current?.focus()

    const closeOnEscape = (event: KeyboardEvent) => {
      if (event.key === 'Escape' && !busy) onCancel()
    }
    document.addEventListener('keydown', closeOnEscape)
    return () => document.removeEventListener('keydown', closeOnEscape)
  }, [busy, onCancel, open])

  if (!open) return null

  return (
    <div className="dialog-backdrop" role="presentation" onMouseDown={() => !busy && onCancel()}>
      <section
        className="confirm-dialog"
        role="alertdialog"
        aria-modal="true"
        aria-labelledby="confirm-title"
        aria-describedby="confirm-message"
        onMouseDown={(event) => event.stopPropagation()}
      >
        <span className="confirm-dialog__icon" aria-hidden="true">!</span>
        <h2 id="confirm-title">{title}</h2>
        <p id="confirm-message">{message}</p>
        <div className="confirm-dialog__actions">
          <button ref={cancelButton} className="button button--ghost" type="button" disabled={busy} onClick={onCancel}>
            Cancelar
          </button>
          <button className="button button--danger" type="button" disabled={busy} onClick={onConfirm}>
            {busy ? 'Procesando…' : confirmLabel}
          </button>
        </div>
      </section>
    </div>
  )
}

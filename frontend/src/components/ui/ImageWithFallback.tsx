import { useState } from 'react'

interface ImageWithFallbackProps {
  src: string | null
  alt: string
  className?: string
  eager?: boolean
}

export function ImageWithFallback({ src, alt, className = '', eager = false }: ImageWithFallbackProps) {
  const [failed, setFailed] = useState(false)

  if (failed || !src) {
    return (
      <div className={`image-fallback ${className}`} role="img" aria-label={`Imagen no disponible: ${alt}`}>
        <svg aria-hidden="true" viewBox="0 0 64 64">
          <path d="M18 17 26 9h12l8 8 9 5-6 12-7-4v25H22V30l-7 4-6-12 9-5Z" />
          <path d="M25 10c1.5 5 12.5 5 14 0" />
        </svg>
        <span>Imagen no disponible</span>
      </div>
    )
  }

  return (
    <img
      src={src}
      alt={alt}
      className={className}
      loading={eager ? 'eager' : 'lazy'}
      fetchPriority={eager ? 'high' : 'auto'}
      onError={() => setFailed(true)}
    />
  )
}

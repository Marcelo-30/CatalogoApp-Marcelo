export type ApiErrorKind = 'http' | 'network' | 'invalid-response'

export class CatalogApiError extends Error {
  constructor(
    message: string,
    public readonly kind: ApiErrorKind,
    public readonly status?: number,
    public readonly validationErrors?: Record<string, string[]>,
  ) {
    super(message)
    this.name = 'CatalogApiError'
  }
}

interface ErrorPayload {
  message?: string
  detail?: string
  title?: string
  errors?: Record<string, string[]>
}

async function buildHttpError(response: Response) {
  let payload: ErrorPayload | null = null
  const contentType = response.headers.get('content-type') ?? ''

  if (contentType.toLowerCase().includes('application/json')) {
    try {
      payload = await response.json() as ErrorPayload
    } catch {
      payload = null
    }
  }

  const validationMessage = payload?.errors
    ? Object.values(payload.errors).flat()[0]
    : undefined
  const fallback = response.status === 401
    ? 'Tu sesión no está activa o las credenciales no son correctas.'
    : response.status === 403
      ? 'No tienes permiso para realizar esta acción.'
      : response.status === 404
        ? 'El recurso que buscas no existe o ya no está disponible.'
        : 'El servidor no pudo completar la solicitud.'

  return new CatalogApiError(
    payload?.message ?? payload?.detail ?? validationMessage ?? payload?.title ?? fallback,
    'http',
    response.status,
    payload?.errors,
  )
}

export async function requestJson<T>(path: string, init: RequestInit = {}): Promise<T> {
  let response: Response

  try {
    response = await fetch(path, {
      ...init,
      credentials: 'same-origin',
      headers: {
        Accept: 'application/json',
        ...init.headers,
      },
    })
  } catch (error) {
    if (error instanceof DOMException && error.name === 'AbortError') {
      throw error
    }

    throw new CatalogApiError(
      'No pudimos conectar con el servidor. Revisa tu conexión e inténtalo de nuevo.',
      'network',
    )
  }

  if (!response.ok) {
    throw await buildHttpError(response)
  }

  if (response.status === 204) {
    return undefined as T
  }

  const contentType = response.headers.get('content-type') ?? ''
  if (!contentType.toLowerCase().includes('application/json')) {
    throw new CatalogApiError(
      'El servidor devolvió una respuesta inesperada.',
      'invalid-response',
      response.status,
    )
  }

  try {
    return await response.json() as T
  } catch {
    throw new CatalogApiError(
      'No pudimos interpretar la respuesta del servidor.',
      'invalid-response',
      response.status,
    )
  }
}

async function getAntiforgeryToken() {
  return requestJson<{ token: string }>('/api/auth/antiforgery')
}

export async function mutateJson<T>(
  path: string,
  method: 'POST' | 'PUT' | 'DELETE',
  body?: unknown,
) {
  const { token } = await getAntiforgeryToken()

  return requestJson<T>(path, {
    method,
    headers: {
      'Content-Type': 'application/json',
      'X-CSRF-TOKEN': token,
    },
    body: body === undefined ? undefined : JSON.stringify(body),
  })
}


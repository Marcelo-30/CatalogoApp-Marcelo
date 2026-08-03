import { useCallback, useEffect, useState } from 'react'

interface ResourceState<T> {
  data: T | null
  error: Error | null
  loading: boolean
}

export function useApiResource<T>(
  loader: (signal: AbortSignal) => Promise<T>,
) {
  const [revision, setRevision] = useState(0)
  const [state, setState] = useState<ResourceState<T>>({
    data: null,
    error: null,
    loading: true,
  })

  useEffect(() => {
    const controller = new AbortController()

    loader(controller.signal)
      .then((data) => setState({ data, error: null, loading: false }))
      .catch((error: unknown) => {
        if (error instanceof DOMException && error.name === 'AbortError') {
          return
        }

        setState({
          data: null,
          error: error instanceof Error ? error : new Error('Ocurrió un error inesperado.'),
          loading: false,
        })
      })

    return () => controller.abort()
  }, [loader, revision])

  const retry = useCallback(() => {
    setState((current) => ({ ...current, error: null, loading: true }))
    setRevision((value) => value + 1)
  }, [])

  return { ...state, retry }
}

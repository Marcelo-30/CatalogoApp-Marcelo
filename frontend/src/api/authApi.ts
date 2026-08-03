import type { AuthStatus, LoginInput, RegisterInput } from '../types/admin'
import { mutateJson, requestJson } from './httpClient'

export const getAuthStatus = (signal?: AbortSignal) =>
  requestJson<AuthStatus>('/api/auth/status', { signal })

export const loginSeller = (input: LoginInput) =>
  mutateJson<AuthStatus>('/api/auth/login', 'POST', input)

export const registerSeller = (input: RegisterInput) =>
  mutateJson<AuthStatus>('/api/auth/register', 'POST', input)

export const logoutSeller = () =>
  mutateJson<void>('/api/auth/logout', 'POST')


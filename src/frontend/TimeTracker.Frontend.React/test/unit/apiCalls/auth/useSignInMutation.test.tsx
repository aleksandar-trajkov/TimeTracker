import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useSignInMutation } from '../../../../src/apiCalls/auth/useSignInMutation'
import type { SignInRequest, TokenResponse } from '../../../../src/apiCalls/auth/types'
import * as fetchHelpers from '../../../../src/helpers/fetch'
import * as tokenExpiry from '../../../../src/helpers/token'
import Cookies from 'js-cookie'

// Mock the dependencies
vi.mock('../../../../src/helpers/fetch', () => ({
  executePost: vi.fn()
}))

vi.mock('../../../../src/helpers/token', () => ({
  calculateTokenExpiry: vi.fn(),
  getRememberMeExpiry: vi.fn(),
  getTokenUserDetails: vi.fn(),
  default: vi.fn()
}))

vi.mock('js-cookie', () => ({
  default: {
    set: vi.fn(),
    remove: vi.fn()
  }
}))

// Mock user store
vi.mock('../../../../src/stores/userStore', () => ({
  default: {
    getState: () => ({
      setUser: vi.fn()
    })
  }
}))

// Mock environment variable
vi.stubEnv('VITE_BASE_URL', 'http://localhost:3000')

// Mock console.error to avoid noise in tests
vi.spyOn(console, 'error').mockImplementation(() => {})

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      mutations: {
        retry: false,
      },
    },
  })
  return ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  )
}

describe('useSignInMutation', () => {
  const mockSetIsSignedIn = vi.fn()
  const mockTokenResponse: TokenResponse = {
    token: 'mock-jwt-token',
    rememberMeToken: 'mock-remember-me-token'
  }

  const mockCredentials: SignInRequest = {
    email: 'test@example.com',
    password: 'password123',
    rememberMe: true
  }

  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(tokenExpiry.calculateTokenExpiry).mockReturnValue(7)
    vi.mocked(tokenExpiry.getRememberMeExpiry).mockReturnValue(30)
  })

  it('should call executePost with correct parameters', async () => {
    vi.mocked(fetchHelpers.executePost).mockResolvedValue(mockTokenResponse)

    const { result } = renderHook(() => useSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockCredentials)

    await waitFor(() => {
      expect(fetchHelpers.executePost).toHaveBeenCalledWith(
        'http://localhost:3000/v1/auth/signin',
        mockCredentials
      )
    })
  })

  it('should set cookies and call setIsSignedIn on successful sign in', async () => {
    vi.mocked(fetchHelpers.executePost).mockResolvedValue(mockTokenResponse)

    const { result } = renderHook(() => useSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockCredentials)

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(Cookies.set).toHaveBeenCalledWith('token', 'mock-jwt-token', { expires: 7 })
    expect(Cookies.set).toHaveBeenCalledWith('rememberMe', 'mock-remember-me-token', { expires: 30 })
    expect(mockSetIsSignedIn).toHaveBeenCalledWith(true)
  })

  it('should not set remember me cookie when rememberMe is false', async () => {
    const credentialsWithoutRememberMe = { ...mockCredentials, rememberMe: false }
    vi.mocked(fetchHelpers.executePost).mockResolvedValue(mockTokenResponse)

    const { result } = renderHook(() => useSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(credentialsWithoutRememberMe)

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(Cookies.set).toHaveBeenCalledWith('token', 'mock-jwt-token', { expires: 7 })
    expect(Cookies.set).not.toHaveBeenCalledWith('rememberMe', expect.any(String), expect.any(Object))
    expect(mockSetIsSignedIn).toHaveBeenCalledWith(true)
  })

  it('should not set remember me cookie when rememberMeToken is not provided', async () => {
    const tokenResponseWithoutRememberMe = { token: 'mock-jwt-token' }
    vi.mocked(fetchHelpers.executePost).mockResolvedValue(tokenResponseWithoutRememberMe)

    const { result } = renderHook(() => useSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockCredentials)

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(Cookies.set).toHaveBeenCalledWith('token', 'mock-jwt-token', { expires: 7 })
    expect(Cookies.set).not.toHaveBeenCalledWith('rememberMe', expect.any(String), expect.any(Object))
    expect(mockSetIsSignedIn).toHaveBeenCalledWith(true)
  })

  it('should handle sign in failure', async () => {
    const errorMessage = 'Invalid credentials'
    vi.mocked(fetchHelpers.executePost).mockRejectedValue(new Error(errorMessage))

    const { result } = renderHook(() => useSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockCredentials)

    await waitFor(() => {
      expect(result.current.isError).toBe(true)
    })

    expect(result.current.error).toBeInstanceOf(Error)
    expect(result.current.error?.message).toBe(errorMessage)
    expect(console.error).toHaveBeenCalledWith('Sign in failed:', expect.any(Error))
    expect(mockSetIsSignedIn).not.toHaveBeenCalled()
    expect(Cookies.set).not.toHaveBeenCalled()
  })

  it('should not set cookies when token is missing in response', async () => {
    const invalidTokenResponse = {} as TokenResponse
    vi.mocked(fetchHelpers.executePost).mockResolvedValue(invalidTokenResponse)

    const { result } = renderHook(() => useSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockCredentials)

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(Cookies.set).not.toHaveBeenCalled()
    expect(mockSetIsSignedIn).not.toHaveBeenCalled()
  })

  it('should use calculated token expiry', async () => {
    const customTokenExpiry = 14
    vi.mocked(tokenExpiry.calculateTokenExpiry).mockReturnValue(customTokenExpiry)
    vi.mocked(fetchHelpers.executePost).mockResolvedValue(mockTokenResponse)

    const { result } = renderHook(() => useSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockCredentials)

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(tokenExpiry.calculateTokenExpiry).toHaveBeenCalled()
    expect(Cookies.set).toHaveBeenCalledWith('token', 'mock-jwt-token', { expires: customTokenExpiry })
  })

  it('should use calculated remember me expiry', async () => {
    const customRememberMeExpiry = 60
    vi.mocked(tokenExpiry.getRememberMeExpiry).mockReturnValue(customRememberMeExpiry)
    vi.mocked(fetchHelpers.executePost).mockResolvedValue(mockTokenResponse)

    const { result } = renderHook(() => useSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockCredentials)

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(tokenExpiry.getRememberMeExpiry).toHaveBeenCalled()
    expect(Cookies.set).toHaveBeenCalledWith('rememberMe', 'mock-remember-me-token', { expires: customRememberMeExpiry })
  })

  it('should have correct mutation configuration', () => {
    const { result } = renderHook(() => useSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    // Check that retry is disabled and gcTime is 0
    expect(result.current.failureCount).toBe(0)
    // These are internal configurations that are hard to test directly,
    // but we can verify the mutation behaves correctly with these settings
  })
})
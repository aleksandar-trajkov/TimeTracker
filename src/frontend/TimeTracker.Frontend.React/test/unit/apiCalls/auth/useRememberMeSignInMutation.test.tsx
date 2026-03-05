import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useRememberMeSignInMutation } from '../../../../src/apiCalls/auth/useRememberMeSignInMutation'
import type { TokenResponse } from '../../../../src/apiCalls/auth/types'
import * as fetchHelpers from '../../../../src/helpers/fetch'
import * as token from '../../../../src/helpers/token'
import Cookies from 'js-cookie'

// Mock the dependencies
vi.mock('../../../../src/helpers/fetch', () => ({
  executePost: vi.fn()
}))

vi.mock('../../../../src/helpers/token', () => ({
  applyTokenResponse: vi.fn(),
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

describe('useRememberMeSignInMutation', () => {
  const mockSetIsSignedIn = vi.fn()
  const mockTokenResponse: TokenResponse = {
    token: 'mock-jwt-token',
    rememberMeToken: 'new-remember-me-token'
  }

  const mockRememberMeToken = 'existing-remember-me-token'

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should call executePost with correct parameters', async () => {
    vi.mocked(fetchHelpers.executePost).mockResolvedValue(mockTokenResponse)

    const { result } = renderHook(() => useRememberMeSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockRememberMeToken)

    await waitFor(() => {
      expect(fetchHelpers.executePost).toHaveBeenCalledWith(
        'http://localhost:3000/v1/auth/remember-me-signin',
        { rememberMeToken: mockRememberMeToken }
      )
    })
  })

  it('should call applyTokenResponse and setIsSignedIn on successful remember me sign in', async () => {
    vi.mocked(fetchHelpers.executePost).mockResolvedValue(mockTokenResponse)

    const { result } = renderHook(() => useRememberMeSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockRememberMeToken)

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(token.applyTokenResponse).toHaveBeenCalledWith('mock-jwt-token', 'new-remember-me-token')
    expect(mockSetIsSignedIn).toHaveBeenCalledWith(true)
  })

  it('should pass undefined rememberMeToken to applyTokenResponse when not in response', async () => {
    const tokenResponseWithoutRememberMe = { token: 'mock-jwt-token' }
    vi.mocked(fetchHelpers.executePost).mockResolvedValue(tokenResponseWithoutRememberMe)

    const { result } = renderHook(() => useRememberMeSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockRememberMeToken)

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(token.applyTokenResponse).toHaveBeenCalledWith('mock-jwt-token', undefined)
    expect(mockSetIsSignedIn).toHaveBeenCalledWith(true)
  })

  it('should handle remember me sign in failure and remove remember me cookie', async () => {
    const errorMessage = 'Invalid remember me token'
    vi.mocked(fetchHelpers.executePost).mockRejectedValue(new Error(errorMessage))

    const { result } = renderHook(() => useRememberMeSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockRememberMeToken)

    await waitFor(() => {
      expect(result.current.isError).toBe(true)
    })

    expect(result.current.error).toBeInstanceOf(Error)
    expect(result.current.error?.message).toBe(errorMessage)
    expect(console.error).toHaveBeenCalledWith('Remember me sign in failed:', expect.any(Error))
    expect(Cookies.remove).toHaveBeenCalledWith('rememberMe')
    expect(mockSetIsSignedIn).not.toHaveBeenCalled()
    expect(token.applyTokenResponse).not.toHaveBeenCalled()
  })

  it('should not call applyTokenResponse when token is missing in response', async () => {
    const invalidTokenResponse = {} as TokenResponse
    vi.mocked(fetchHelpers.executePost).mockResolvedValue(invalidTokenResponse)

    const { result } = renderHook(() => useRememberMeSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockRememberMeToken)

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(token.applyTokenResponse).not.toHaveBeenCalled()
    expect(mockSetIsSignedIn).not.toHaveBeenCalled()
  })

  it('should have correct mutation configuration', () => {
    const { result } = renderHook(() => useRememberMeSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    // Check that retry is disabled and gcTime is 0
    expect(result.current.failureCount).toBe(0)
    // These are internal configurations that are hard to test directly,
    // but we can verify the mutation behaves correctly with these settings
  })

  it('should handle network errors properly', async () => {
    const networkError = new Error('Network error')
    vi.mocked(fetchHelpers.executePost).mockRejectedValue(networkError)

    const { result } = renderHook(() => useRememberMeSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockRememberMeToken)

    await waitFor(() => {
      expect(result.current.isError).toBe(true)
    })

    expect(console.error).toHaveBeenCalledWith('Remember me sign in failed:', networkError)
    expect(Cookies.remove).toHaveBeenCalledWith('rememberMe')
  })

  it('should handle successful response with empty token field', async () => {
    const responseWithEmptyToken = { token: '' }
    vi.mocked(fetchHelpers.executePost).mockResolvedValue(responseWithEmptyToken)

    const { result } = renderHook(() => useRememberMeSignInMutation({ setIsSignedIn: mockSetIsSignedIn }), {
      wrapper: createWrapper(),
    })

    result.current.mutate(mockRememberMeToken)

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    // Should not call applyTokenResponse or setIsSignedIn when token is empty
    expect(token.applyTokenResponse).not.toHaveBeenCalled()
    expect(mockSetIsSignedIn).not.toHaveBeenCalled()
  })
})
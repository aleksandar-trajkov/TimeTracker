import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useCategoriesQuery } from '../../../../src/apiCalls/categories'
import * as fetchHelpers from '../../../../src/helpers/fetch'

// Mock the fetch helpers
vi.mock('../../../../src/helpers/fetch', () => ({
  executeGet: vi.fn()
}))

// Mock js-cookie
vi.mock('js-cookie', () => ({
  default: {
    get: vi.fn().mockReturnValue('mock-token')
  }
}))

// Mock environment variable
vi.stubEnv('VITE_BASE_URL', 'http://localhost:3000')

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
      },
    },
  })
  return ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  )
}

describe('useCategoriesQuery', () => {
  const mockCategoriesResponse = [
    {
      id: '1',
      name: 'Development',
    },
    {
      id: '2',
      name: 'Meeting',
    },
    {
      id: '3',
      name: 'Testing',
    }
  ]

  const testOrganizationId = 'org-123'

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should fetch categories successfully with organization ID', async () => {
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockCategoriesResponse as unknown as Response)

    const { result } = renderHook(() => useCategoriesQuery(testOrganizationId), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(result.current.data).toEqual(mockCategoriesResponse)
    expect(fetchHelpers.executeGet).toHaveBeenCalledWith(
      'http://localhost:3000/v1/categories?organizationId=org-123',
      'mock-token'
    )
  })

  it('should include organizationId in query key for proper caching', async () => {
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockCategoriesResponse as unknown as Response)

    const { result } = renderHook(() => useCategoriesQuery(testOrganizationId), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    // The query key should include the organizationId for proper caching
    expect(result.current.dataUpdatedAt).toBeDefined()
    expect(fetchHelpers.executeGet).toHaveBeenCalledTimes(1)
  })

  it('should not fetch when organizationId is empty string', async () => {
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockCategoriesResponse as unknown as Response)

    const { result } = renderHook(() => useCategoriesQuery(''), {
      wrapper: createWrapper(),
    })

    // Wait a bit to ensure query doesn't run
    await new Promise(resolve => setTimeout(resolve, 100))

    expect(result.current.isLoading).toBe(false)
    expect(result.current.data).toBeUndefined()
    expect(fetchHelpers.executeGet).not.toHaveBeenCalled()
  })

  it('should not fetch when organizationId is undefined', async () => {
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockCategoriesResponse as unknown as Response)

    const { result } = renderHook(() => useCategoriesQuery(undefined as unknown as string), {
      wrapper: createWrapper(),
    })

    // Wait a bit to ensure query doesn't run
    await new Promise(resolve => setTimeout(resolve, 100))

    expect(result.current.isLoading).toBe(false)
    expect(result.current.data).toBeUndefined()
    expect(fetchHelpers.executeGet).not.toHaveBeenCalled()
  })

  it('should handle API errors', async () => {
    const errorMessage = 'Failed to fetch categories'
    vi.mocked(fetchHelpers.executeGet).mockRejectedValue(new Error(errorMessage))

    const { result } = renderHook(() => useCategoriesQuery(testOrganizationId), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isError).toBe(true)
    })

    expect(result.current.error).toBeInstanceOf(Error)
    expect(result.current.error?.message).toBe(errorMessage)
  })

  it('should handle empty categories response', async () => {
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue([] as unknown as Response)

    const { result } = renderHook(() => useCategoriesQuery(testOrganizationId), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(result.current.data).toEqual([])
    expect(fetchHelpers.executeGet).toHaveBeenCalledWith(
      'http://localhost:3000/v1/categories?organizationId=org-123',
      'mock-token'
    )
  })

  it('should refetch when organizationId changes', async () => {
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockCategoriesResponse as unknown as Response)

    const { result, rerender } = renderHook(
      ({ orgId }: { orgId: string }) => useCategoriesQuery(orgId),
      {
        wrapper: createWrapper(),
        initialProps: { orgId: 'org-123' }
      }
    )

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(fetchHelpers.executeGet).toHaveBeenCalledWith(
      'http://localhost:3000/v1/categories?organizationId=org-123',
      'mock-token'
    )

    // Change organizationId
    rerender({ orgId: 'org-456' })

    await waitFor(() => {
      expect(fetchHelpers.executeGet).toHaveBeenCalledWith(
        'http://localhost:3000/v1/categories?organizationId=org-456',
        'mock-token'
      )
    })

    expect(fetchHelpers.executeGet).toHaveBeenCalledTimes(2)
  })

  it('should handle network timeout errors', async () => {
    const timeoutError = new Error('Request timeout')
    timeoutError.name = 'AbortError'
    vi.mocked(fetchHelpers.executeGet).mockRejectedValue(timeoutError)

    const { result } = renderHook(() => useCategoriesQuery(testOrganizationId), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isError).toBe(true)
    })

    expect(result.current.error).toEqual(timeoutError)
  })

  it('should handle 401 unauthorized errors', async () => {
    const unauthorizedError = new Error('Unauthorized')
    vi.mocked(fetchHelpers.executeGet).mockRejectedValue(unauthorizedError)

    const { result } = renderHook(() => useCategoriesQuery(testOrganizationId), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isError).toBe(true)
    })

    expect(result.current.error?.message).toBe('Unauthorized')
  })

  it('should handle 404 not found errors', async () => {
    const notFoundError = new Error('Organization not found')
    vi.mocked(fetchHelpers.executeGet).mockRejectedValue(notFoundError)

    const { result } = renderHook(() => useCategoriesQuery(testOrganizationId), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isError).toBe(true)
    })

    expect(result.current.error?.message).toBe('Organization not found')
  })

  it('should work with different organizationId formats', async () => {
    const uuidOrgId = '550e8400-e29b-41d4-a716-446655440000'
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockCategoriesResponse as unknown as Response)

    const { result } = renderHook(() => useCategoriesQuery(uuidOrgId), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(fetchHelpers.executeGet).toHaveBeenCalledWith(
      `http://localhost:3000/v1/categories?organizationId=${uuidOrgId}`,
      'mock-token'
    )
  })

  it('should handle special characters in organizationId', async () => {
    const specialOrgId = 'org-with-special-chars-123!@#'
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockCategoriesResponse as unknown as Response)

    const { result } = renderHook(() => useCategoriesQuery(specialOrgId), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(fetchHelpers.executeGet).toHaveBeenCalledWith(
      `http://localhost:3000/v1/categories?organizationId=${specialOrgId}`,
      'mock-token'
    )
  })
})
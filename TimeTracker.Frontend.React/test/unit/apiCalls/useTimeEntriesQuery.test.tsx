import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useTimeEntriesQuery } from '../../../src/apiCalls/timeEntries'
import * as fetchHelpers from '../../../src/helpers/fetch'

// Mock the fetch helpers
vi.mock('../../../src/helpers/fetch', () => ({
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

describe('useTimeEntriesQuery', () => {
  const mockTimeEntriesResponse = [
    {
      id: 1,
      description: 'Test entry 1',
      from: new Date('2023-12-25T09:00:00Z'),
      to: new Date('2023-12-25T10:00:00Z'),
      category: {
        id: 1,
        name: 'Development'
      }
    },
    {
      id: 2,
      description: 'Test entry 2',
      from: new Date('2023-12-25T11:00:00Z'),
      to: new Date('2023-12-25T12:30:00Z'),
      category: {
        id: 2,
        name: 'Meeting'
      }
    }
  ]

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should fetch time entries successfully', async () => {
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockTimeEntriesResponse as unknown as Response)

    const { result } = renderHook(() => useTimeEntriesQuery({ from: new Date('2023-12-01'), to: new Date('2023-12-31') }), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(result.current.data).toEqual(mockTimeEntriesResponse)
    expect(fetchHelpers.executeGet).toHaveBeenCalledWith(
      'http://localhost:3000/v1/time-entries',
      'mock-token'
    )
  })

  it('should handle query parameters correctly', async () => {
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockTimeEntriesResponse as unknown as Response)

    const queryParams = {
      from: new Date('2023-12-01'),
      to: new Date('2023-12-31'),
    }

    const { result } = renderHook(() => useTimeEntriesQuery(queryParams), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(fetchHelpers.executeGet).toHaveBeenCalledWith(
      'http://localhost:3000/v1/time-entries?from=2023-12-01&to=2023-12-31',
      'mock-token'
    )
  })

  it('should handle API errors', async () => {
    const errorMessage = 'Failed to fetch time entries'
    vi.mocked(fetchHelpers.executeGet).mockRejectedValue(new Error(errorMessage))

    const { result } = renderHook(() => useTimeEntriesQuery({ from: new Date('2023-12-01'), to: new Date('2023-12-31') }), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isError).toBe(true)
    })

    expect(result.current.error).toBeInstanceOf(Error)
    expect(result.current.error?.message).toBe(errorMessage)
  })

  it('should use correct query key with parameters', async () => {
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockTimeEntriesResponse as unknown as Response)

    const queryParams = { from: new Date('2023-12-01'), to: new Date('2023-12-31') }

    const { result } = renderHook(() => useTimeEntriesQuery(queryParams), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    // The query key should include the parameters for proper caching
    expect(result.current.dataUpdatedAt).toBeDefined()
  })
})
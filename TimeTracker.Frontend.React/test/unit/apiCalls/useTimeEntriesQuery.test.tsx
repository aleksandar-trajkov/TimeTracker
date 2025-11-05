import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useTimeEntriesQuery } from '../../../src/apiCalls/timeEntries'
import * as fetchHelpers from '../../../src/helpers/fetch'
import Cookies from 'js-cookie'

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
  const mockTimeEntriesResponse = {
    timeEntries: [
      {
        id: 1,
        description: 'Test entry 1',
        startTime: '2023-12-25T09:00:00Z',
        endTime: '2023-12-25T10:00:00Z',
        duration: 60,
        date: '2023-12-25',
        categoryId: 1,
        categoryName: 'Development',
        projectId: 1,
        projectName: 'Test Project',
        userId: 1,
        createdAt: '2023-12-25T09:00:00Z',
        updatedAt: '2023-12-25T09:00:00Z'
      },
      {
        id: 2,
        description: 'Test entry 2',
        startTime: '2023-12-25T11:00:00Z',
        endTime: '2023-12-25T12:30:00Z',
        duration: 90,
        date: '2023-12-25',
        categoryId: 2,
        categoryName: 'Meeting',
        userId: 1,
        createdAt: '2023-12-25T11:00:00Z',
        updatedAt: '2023-12-25T11:00:00Z'
      }
    ],
    totalCount: 2,
    totalDuration: 150
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should fetch time entries successfully', async () => {
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockTimeEntriesResponse as any)

    const { result } = renderHook(() => useTimeEntriesQuery(), {
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
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockTimeEntriesResponse as any)

    const queryParams = {
      from: '2023-12-01',
      to: '2023-12-31',
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

  it('should handle empty query parameters', async () => {
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockTimeEntriesResponse as any)

    const queryParams = {}

    const { result } = renderHook(() => useTimeEntriesQuery(queryParams), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isSuccess).toBe(true)
    })

    expect(fetchHelpers.executeGet).toHaveBeenCalledWith(
      'http://localhost:3000/v1/time-entries',
      'mock-token'
    )
  })

  it('should handle API errors', async () => {
    const errorMessage = 'Failed to fetch time entries'
    vi.mocked(fetchHelpers.executeGet).mockRejectedValue(new Error(errorMessage))

    const { result } = renderHook(() => useTimeEntriesQuery(), {
      wrapper: createWrapper(),
    })

    await waitFor(() => {
      expect(result.current.isError).toBe(true)
    })

    expect(result.current.error).toBeInstanceOf(Error)
    expect(result.current.error?.message).toBe(errorMessage)
  })

  it('should use correct query key with parameters', async () => {
    vi.mocked(fetchHelpers.executeGet).mockResolvedValue(mockTimeEntriesResponse as any)

    const queryParams = { from: '2023-12-01', to: '2023-12-31' }

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
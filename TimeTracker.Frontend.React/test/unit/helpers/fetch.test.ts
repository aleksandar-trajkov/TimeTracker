import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { 
  executeFetch, 
  executeGet, 
  executePost, 
  executePut, 
  executeDelete 
} from '../../../src/helpers/fetch'

// Mock global fetch
const mockFetch = vi.fn()
global.fetch = mockFetch

describe('fetch helpers', () => {
  const TEST_URL = 'http://localhost:5000/api/test'

  beforeEach(() => {
    mockFetch.mockClear()
  })

  afterEach(() => {
    vi.clearAllMocks()
  })

  describe('executeFetch', () => {
    describe('basic functionality', () => {
      it('should make fetch request with correct URL and method', async () => {
        const mockResponse = { ok: true, json: vi.fn().mockResolvedValue({ data: 'test' }) }
        mockFetch.mockResolvedValue(mockResponse)

        await executeFetch(TEST_URL, 'GET')

        expect(mockFetch).toHaveBeenCalledWith(
          TEST_URL,
          expect.objectContaining({
            method: 'GET',
            headers: expect.objectContaining({
              'Content-Type': 'application/json'
            }),
            body: null
          })
        )
      })

      it('should return Response object', async () => {
        const mockResponse = { ok: true, json: vi.fn().mockResolvedValue({ data: 'test' }) }
        mockFetch.mockResolvedValue(mockResponse)

        const result = await executeFetch(TEST_URL, 'GET')

        expect(result).toBe(mockResponse)
      })

      it('should handle network errors', async () => {
        mockFetch.mockRejectedValue(new Error('Network error'))

        await expect(executeFetch(TEST_URL, 'GET'))
          .rejects.toThrow('Network error')
      })

      it('should throw error for HTTP status 400 and above', async () => {
        const mockResponse = { 
          status: 400, 
          statusText: 'Bad Request',
          clone: vi.fn().mockReturnValue({
            json: vi.fn().mockRejectedValue(new Error('Invalid JSON'))
          })
        }
        mockFetch.mockResolvedValue(mockResponse)

        await expect(executeFetch(TEST_URL, 'GET'))
          .rejects.toThrow('HTTP Error 400: Bad Request')
      })

      it('should throw error with custom message from response body', async () => {
        const mockResponse = { 
          status: 422, 
          statusText: 'Unprocessable Entity',
          clone: vi.fn().mockReturnValue({
            json: vi.fn().mockResolvedValue({ message: 'Invalid data provided' })
          })
        }
        mockFetch.mockResolvedValue(mockResponse)

        await expect(executeFetch(TEST_URL, 'POST', {}))
          .rejects.toThrow('Invalid data provided')
      })

      it('should throw error with error field from response body', async () => {
        const mockResponse = { 
          status: 500, 
          statusText: 'Internal Server Error',
          clone: vi.fn().mockReturnValue({
            json: vi.fn().mockResolvedValue({ error: 'Database connection failed' })
          })
        }
        mockFetch.mockResolvedValue(mockResponse)

        const error = await executeFetch(TEST_URL, 'GET').catch(e => e)
        expect(error.message).toBe('Database connection failed')
        expect(error.status).toBe(500)
        expect(error.statusText).toBe('Internal Server Error')
      })

      it('should not throw error for HTTP status below 400', async () => {
        const mockResponse = { 
          status: 200, 
          statusText: 'OK',
          ok: true
        }
        mockFetch.mockResolvedValue(mockResponse)

        const result = await executeFetch(TEST_URL, 'GET')
        expect(result).toBe(mockResponse)
      })
    })

    describe('authentication', () => {
      it('should include authorization header when token is provided', async () => {
        const mockResponse = { ok: true }
        mockFetch.mockResolvedValue(mockResponse)

        await executeFetch(TEST_URL, 'GET', null, 'my-auth-token')

        expect(mockFetch).toHaveBeenCalledWith(
          TEST_URL,
          expect.objectContaining({
            headers: expect.objectContaining({
              'Authorization': 'Bearer my-auth-token',
              'Content-Type': 'application/json'
            })
          })
        )
      })

      it('should not include authorization header when token is null', async () => {
        const mockResponse = { ok: true }
        mockFetch.mockResolvedValue(mockResponse)

        await executeFetch(TEST_URL, 'GET', null, null)

        const fetchCall = mockFetch.mock.calls[0]
        const headers = fetchCall[1].headers
        expect(headers).not.toHaveProperty('Authorization')
        expect(headers).toHaveProperty('Content-Type', 'application/json')
      })

      it('should not include authorization header when token is empty string', async () => {
        const mockResponse = { ok: true }
        mockFetch.mockResolvedValue(mockResponse)

        await executeFetch(TEST_URL, 'GET', null, '')

        const fetchCall = mockFetch.mock.calls[0]
        const headers = fetchCall[1].headers
        expect(headers).not.toHaveProperty('Authorization')
      })
    })

    describe('request body handling', () => {
      it('should stringify object body when provided', async () => {
        const mockResponse = { ok: true }
        mockFetch.mockResolvedValue(mockResponse)

        const requestBody = { name: 'test', id: 1 }
        await executeFetch(TEST_URL, 'POST', requestBody, 'token')

        expect(mockFetch).toHaveBeenCalledWith(
          TEST_URL,
          expect.objectContaining({
            method: 'POST',
            body: JSON.stringify(requestBody),
            headers: expect.objectContaining({
              'Authorization': 'Bearer token',
              'Content-Type': 'application/json'
            })
          })
        )
      })

      it('should set body to null when no body provided', async () => {
        const mockResponse = { ok: true }
        mockFetch.mockResolvedValue(mockResponse)

        await executeFetch(TEST_URL, 'GET', null, 'token')

        expect(mockFetch).toHaveBeenCalledWith(
          TEST_URL,
          expect.objectContaining({
            body: null
          })
        )
      })

      it('should handle empty object body', async () => {
        const mockResponse = { ok: true }
        mockFetch.mockResolvedValue(mockResponse)

        await executeFetch(TEST_URL, 'POST', {}, 'token')

        expect(mockFetch).toHaveBeenCalledWith(
          TEST_URL,
          expect.objectContaining({
            body: JSON.stringify({})
          })
        )
      })

      it('should handle complex object body', async () => {
        const mockResponse = { ok: true }
        mockFetch.mockResolvedValue(mockResponse)

        const complexBody = {
          user: { id: 1, name: 'test' },
          data: [1, 2, 3],
          metadata: { created: new Date().toISOString() }
        }
        
        await executeFetch(TEST_URL, 'POST', complexBody, 'token')

        expect(mockFetch).toHaveBeenCalledWith(
          TEST_URL,
          expect.objectContaining({
            body: JSON.stringify(complexBody)
          })
        )
      })
    })

    describe('method handling', () => {
      it('should handle GET method', async () => {
        const mockResponse = { ok: true }
        mockFetch.mockResolvedValue(mockResponse)

        await executeFetch(TEST_URL, 'GET', null, 'token')

        expect(mockFetch).toHaveBeenCalledWith(
          TEST_URL,
          expect.objectContaining({ method: 'GET' })
        )
      })

      it('should handle POST method', async () => {
        const mockResponse = { ok: true }
        mockFetch.mockResolvedValue(mockResponse)

        await executeFetch(TEST_URL, 'POST', { data: 'test' }, 'token')

        expect(mockFetch).toHaveBeenCalledWith(
          TEST_URL,
          expect.objectContaining({ method: 'POST' })
        )
      })

      it('should handle PUT method', async () => {
        const mockResponse = { ok: true }
        mockFetch.mockResolvedValue(mockResponse)

        await executeFetch(TEST_URL, 'PUT', { data: 'test' }, 'token')

        expect(mockFetch).toHaveBeenCalledWith(
          TEST_URL,
          expect.objectContaining({ method: 'PUT' })
        )
      })

      it('should handle DELETE method', async () => {
        const mockResponse = { ok: true }
        mockFetch.mockResolvedValue(mockResponse)

        await executeFetch(TEST_URL, 'DELETE', null, 'token')

        expect(mockFetch).toHaveBeenCalledWith(
          TEST_URL,
          expect.objectContaining({ method: 'DELETE' })
        )
      })
    })
  })

  describe('executeGet', () => {
    it('should call executeFetch with GET method and return JSON', async () => {
      const mockData = { users: [{ id: 1, name: 'John' }] }
      const mockResponse = { ok: true, json: vi.fn().mockResolvedValue(mockData) }
      mockFetch.mockResolvedValue(mockResponse)

      const result = await executeGet(TEST_URL, 'token')

      expect(mockFetch).toHaveBeenCalledWith(
        TEST_URL,
        expect.objectContaining({
          method: 'GET',
          headers: expect.objectContaining({
            'Authorization': 'Bearer token',
            'Content-Type': 'application/json'
          }),
          body: null
        })
      )
      expect(result).toEqual(mockData)
      expect(mockResponse.json).toHaveBeenCalled()
    })

    it('should work without token', async () => {
      const mockData = { public: 'data' }
      const mockResponse = { ok: true, json: vi.fn().mockResolvedValue(mockData) }
      mockFetch.mockResolvedValue(mockResponse)

      const result = await executeGet(TEST_URL)

      expect(mockFetch).toHaveBeenCalledWith(
        TEST_URL,
        expect.objectContaining({
          method: 'GET',
          headers: { 'Content-Type': 'application/json' },
          body: null
        })
      )
      expect(result).toEqual(mockData)
    })

    it('should handle JSON parsing errors', async () => {
      const mockResponse = { 
        ok: true, 
        json: vi.fn().mockRejectedValue(new Error('Invalid JSON'))
      }
      mockFetch.mockResolvedValue(mockResponse)

      await expect(executeGet(TEST_URL, 'token'))
        .rejects.toThrow('Invalid JSON')
    })
  })

  describe('executePost', () => {
    it('should call executeFetch with POST method and return JSON', async () => {
      const postData = { name: 'New User', email: 'user@example.com' }
      const responseData = { id: 123, ...postData }
      const mockResponse = { ok: true, json: vi.fn().mockResolvedValue(responseData) }
      mockFetch.mockResolvedValue(mockResponse)

      const result = await executePost(TEST_URL, postData, 'token')

      expect(mockFetch).toHaveBeenCalledWith(
        TEST_URL,
        expect.objectContaining({
          method: 'POST',
          headers: expect.objectContaining({
            'Authorization': 'Bearer token',
            'Content-Type': 'application/json'
          }),
          body: JSON.stringify(postData)
        })
      )
      expect(result).toEqual(responseData)
      expect(mockResponse.json).toHaveBeenCalled()
    })

    it('should work without token', async () => {
      const postData = { data: 'test' }
      const responseData = { success: true }
      const mockResponse = { ok: true, json: vi.fn().mockResolvedValue(responseData) }
      mockFetch.mockResolvedValue(mockResponse)

      const result = await executePost(TEST_URL, postData)

      expect(mockFetch).toHaveBeenCalledWith(
        TEST_URL,
        expect.objectContaining({
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(postData)
        })
      )
      expect(result).toEqual(responseData)
    })

    it('should handle empty body object', async () => {
      const responseData = { created: true }
      const mockResponse = { ok: true, json: vi.fn().mockResolvedValue(responseData) }
      mockFetch.mockResolvedValue(mockResponse)

      const result = await executePost(TEST_URL, {}, 'token')

      expect(mockFetch).toHaveBeenCalledWith(
        TEST_URL,
        expect.objectContaining({
          body: JSON.stringify({})
        })
      )
      expect(result).toEqual(responseData)
    })
  })

  describe('executePut', () => {
    it('should call executeFetch with PUT method and return JSON', async () => {
      const putData = { id: 1, name: 'Updated User' }
      const responseData = { ...putData, updatedAt: '2023-12-25T12:00:00Z' }
      const mockResponse = { ok: true, json: vi.fn().mockResolvedValue(responseData) }
      mockFetch.mockResolvedValue(mockResponse)

      const result = await executePut(TEST_URL, putData, 'token')

      expect(mockFetch).toHaveBeenCalledWith(
        TEST_URL,
        expect.objectContaining({
          method: 'PUT',
          headers: expect.objectContaining({
            'Authorization': 'Bearer token',
            'Content-Type': 'application/json'
          }),
          body: JSON.stringify(putData)
        })
      )
      expect(result).toEqual(responseData)
      expect(mockResponse.json).toHaveBeenCalled()
    })

    it('should work without token', async () => {
      const putData = { status: 'updated' }
      const responseData = { success: true }
      const mockResponse = { ok: true, json: vi.fn().mockResolvedValue(responseData) }
      mockFetch.mockResolvedValue(mockResponse)

      const result = await executePut(TEST_URL, putData)

      expect(mockFetch).toHaveBeenCalledWith(
        TEST_URL,
        expect.objectContaining({
          method: 'PUT',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(putData)
        })
      )
      expect(result).toEqual(responseData)
    })
  })

  describe('executeDelete', () => {
    it('should call executeFetch with DELETE method and return Response', async () => {
      const mockResponse = { ok: true, status: 200 }
      mockFetch.mockResolvedValue(mockResponse)

      const result = await executeDelete(TEST_URL, 'token')

      expect(mockFetch).toHaveBeenCalledWith(
        TEST_URL,
        expect.objectContaining({
          method: 'DELETE',
          headers: expect.objectContaining({
            'Authorization': 'Bearer token',
            'Content-Type': 'application/json'
          }),
          body: null
        })
      )
      expect(result).toBe(mockResponse)
    })

    it('should work without token', async () => {
      const mockResponse = { ok: true, status: 204 }
      mockFetch.mockResolvedValue(mockResponse)

      const result = await executeDelete(TEST_URL)

      expect(mockFetch).toHaveBeenCalledWith(
        TEST_URL,
        expect.objectContaining({
          method: 'DELETE',
          headers: { 'Content-Type': 'application/json' },
          body: null
        })
      )
      expect(result).toBe(mockResponse)
    })

    it('should handle delete errors', async () => {
      mockFetch.mockRejectedValue(new Error('Delete failed'))

      await expect(executeDelete(TEST_URL, 'token'))
        .rejects.toThrow('Delete failed')
    })
  })

  describe('integration scenarios', () => {
    it('should handle complete CRUD workflow', async () => {
      // Create
      const createData = { name: 'Test User' }
      const createResponse = { ok: true, json: vi.fn().mockResolvedValue({ id: 1, ...createData }) }
      mockFetch.mockResolvedValueOnce(createResponse)

      const created = await executePost(TEST_URL, createData, 'token')
      expect(created).toEqual({ id: 1, name: 'Test User' })

      // Read
      const readResponse = { ok: true, json: vi.fn().mockResolvedValue({ id: 1, name: 'Test User' }) }
      mockFetch.mockResolvedValueOnce(readResponse)

      const read = await executeGet(`${TEST_URL}/1`, 'token')
      expect(read).toEqual({ id: 1, name: 'Test User' })

      // Update
      const updateData = { id: 1, name: 'Updated User' }
      const updateResponse = { ok: true, json: vi.fn().mockResolvedValue(updateData) }
      mockFetch.mockResolvedValueOnce(updateResponse)

      const updated = await executePut(`${TEST_URL}/1`, updateData, 'token')
      expect(updated).toEqual(updateData)

      // Delete
      const deleteResponse = { ok: true, status: 204 }
      mockFetch.mockResolvedValueOnce(deleteResponse)

      const deleted = await executeDelete(`${TEST_URL}/1`, 'token')
      expect(deleted).toBe(deleteResponse)

      expect(mockFetch).toHaveBeenCalledTimes(4)
    })

    it('should handle authentication flow', async () => {
      // Unauthenticated request
      const publicResponse = { ok: true, json: vi.fn().mockResolvedValue({ public: true }) }
      mockFetch.mockResolvedValueOnce(publicResponse)

      await executeGet('/public/data')
      expect(mockFetch).toHaveBeenCalledWith(
        '/public/data',
        expect.objectContaining({
          headers: { 'Content-Type': 'application/json' }
        })
      )

      // Authenticated request
      const privateResponse = { ok: true, json: vi.fn().mockResolvedValue({ private: true }) }
      mockFetch.mockResolvedValueOnce(privateResponse)

      await executeGet('/private/data', 'auth-token')
      expect(mockFetch).toHaveBeenCalledWith(
        '/private/data',
        expect.objectContaining({
          headers: expect.objectContaining({
            'Authorization': 'Bearer auth-token'
          })
        })
      )
    })

    it('should handle error responses consistently', async () => {
      // Network error
      mockFetch.mockRejectedValueOnce(new Error('Network error'))
      await expect(executeGet(TEST_URL)).rejects.toThrow('Network error')

      // JSON parsing error
      const badJsonResponse = { ok: true, json: vi.fn().mockRejectedValue(new Error('Bad JSON')) }
      mockFetch.mockResolvedValueOnce(badJsonResponse)
      await expect(executeGet(TEST_URL)).rejects.toThrow('Bad JSON')

      // Fetch error in POST
      mockFetch.mockRejectedValueOnce(new Error('Server error'))
      await expect(executePost(TEST_URL, {})).rejects.toThrow('Server error')
    })
  })

  describe('type safety and generics', () => {
    it('should maintain type safety with executePost generic', async () => {
      interface User {
        id: number
        name: string
        email: string
      }

      const userData = { name: 'John', email: 'john@example.com' }
      const responseData: User = { id: 1, name: 'John', email: 'john@example.com' }
      const mockResponse = { ok: true, json: vi.fn().mockResolvedValue(responseData) }
      mockFetch.mockResolvedValue(mockResponse)

      const result = await executePost<User>(TEST_URL, userData, 'token')

      expect(result).toEqual(responseData)
      expect(typeof result.id).toBe('number')
      expect(typeof result.name).toBe('string')
      expect(typeof result.email).toBe('string')
    })
  })
})
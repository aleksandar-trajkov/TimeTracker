import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { jwtDecode } from 'jwt-decode'
import isTokenValid, { calculateTokenExpiry, getRememberMeExpiry } from '../../../src/helpers/token'

// Mock jwt-decode
vi.mock('jwt-decode')

describe('tokenCheck', () => {
  const mockJwtDecode = vi.mocked(jwtDecode)

  beforeEach(() => {
    vi.useFakeTimers()
    mockJwtDecode.mockClear()
  })

  afterEach(() => {
    vi.useRealTimers()
    vi.clearAllMocks()
  })

  describe('isTokenValid', () => {
    describe('with invalid token types', () => {
      it('should throw error for null token', () => {
        expect(() => isTokenValid(null as unknown as string)).toThrow('Invalid token provided')
        expect(mockJwtDecode).not.toHaveBeenCalled()
      })

      it('should throw error for undefined token', () => {
        expect(() => isTokenValid(undefined as unknown as string)).toThrow('Invalid token provided')
        expect(mockJwtDecode).not.toHaveBeenCalled()
      })

      it('should throw error for number token', () => {
        expect(() => isTokenValid(123 as unknown as string)).toThrow('Invalid token provided')
        expect(mockJwtDecode).not.toHaveBeenCalled()
      })

      it('should throw error for object token', () => {
        expect(() => isTokenValid({} as unknown as string)).toThrow('Invalid token provided')
        expect(mockJwtDecode).not.toHaveBeenCalled()
      })
    })

    describe('with empty or invalid token strings', () => {
      it('should throw error for empty token', () => {
        expect(() => isTokenValid('')).toThrow('Invalid token provided')
        expect(mockJwtDecode).not.toHaveBeenCalled()
      })

      it('should throw error for whitespace-only token', () => {
        expect(() => isTokenValid('   ')).toThrow('Invalid token provided')
        expect(mockJwtDecode).not.toHaveBeenCalled()
      })
    })

    describe('with malformed tokens that cause jwt-decode to throw', () => {
      it('should throw when jwt-decode throws an error', () => {
        const malformedToken = 'not.a.valid.jwt'
        mockJwtDecode.mockImplementation(() => {
          throw new Error('Invalid token format')
        })

        expect(() => isTokenValid(malformedToken)).toThrow('Invalid token format')
        expect(mockJwtDecode).toHaveBeenCalledWith(malformedToken)
      })

      it('should throw when jwt-decode throws different error types', () => {
        const malformedToken = 'invalid-jwt'
        mockJwtDecode.mockImplementation(() => {
          throw new TypeError('Token malformed')
        })

        expect(() => isTokenValid(malformedToken)).toThrow('Token malformed')
        expect(mockJwtDecode).toHaveBeenCalledWith(malformedToken)
      })
    })

    describe('with valid token structure - returns isJwtExpired boolean', () => {
      it('should return false (not expired) for non-expired token', () => {
        const currentTime = new Date('2023-12-25T12:00:00.000Z')
        vi.setSystemTime(currentTime)
        
        const futureExp = Math.floor(new Date('2023-12-25T13:00:00.000Z').getTime() / 1000) // 1 hour later
        const validToken = 'valid.jwt.token'
        
        mockJwtDecode.mockReturnValue({ exp: futureExp })

        const result = isTokenValid(validToken)
        
        expect(result).toBe(true) // true means NOT expired (token is valid)
        expect(mockJwtDecode).toHaveBeenCalledWith(validToken)
      })

      it('should return true (expired) for expired token', () => {
        const currentTime = new Date('2023-12-25T12:00:00.000Z')
        vi.setSystemTime(currentTime)
        
        const pastExp = Math.floor(new Date('2023-12-25T11:00:00.000Z').getTime() / 1000) // 1 hour ago
        const expiredToken = 'expired.jwt.token'
        
        mockJwtDecode.mockReturnValue({ exp: pastExp })

        const result = isTokenValid(expiredToken)
        
        expect(result).toBe(false) // false means expired (token is invalid)
        expect(mockJwtDecode).toHaveBeenCalledWith(expiredToken)
      })

      it('should return true (expired) for token expiring exactly now', () => {
        const currentTime = new Date('2023-12-25T12:00:00.000Z')
        vi.setSystemTime(currentTime)
        
        const nowExp = Math.floor(currentTime.getTime() / 1000) // Exactly now
        const tokenExpiringNow = 'expiring.now.token'
        
        mockJwtDecode.mockReturnValue({ exp: nowExp })

        const result = isTokenValid(tokenExpiringNow)
        
        expect(result).toBe(false) // false means expired (currentTime > exp is false, but currentTime === exp should be treated as expired)
      })
    })

    describe('with edge cases', () => {
      it('should return false (not expired) for token without exp claim', () => {
        const tokenWithoutExp = 'token.without.exp'
        mockJwtDecode.mockReturnValue({}) // No exp property

        const result = isTokenValid(tokenWithoutExp)
        
        expect(result).toBe(true) // undefined exp is not a number, so not expired
        expect(mockJwtDecode).toHaveBeenCalledWith(tokenWithoutExp)
      })

      it('should return false (not expired) for token with null exp claim', () => {
        const tokenWithNullExp = 'token.with.null.exp'
        mockJwtDecode.mockReturnValue({ exp: null })

        const result = isTokenValid(tokenWithNullExp)
        
        expect(result).toBe(true) // null is not a number
        expect(mockJwtDecode).toHaveBeenCalledWith(tokenWithNullExp)
      })

      it('should return false (not expired) for token with undefined exp claim', () => {
        const tokenWithUndefinedExp = 'token.with.undefined.exp'
        mockJwtDecode.mockReturnValue({ exp: undefined })

        const result = isTokenValid(tokenWithUndefinedExp)
        
        expect(result).toBe(true) // undefined is not a number
        expect(mockJwtDecode).toHaveBeenCalledWith(tokenWithUndefinedExp)
      })

      it('should return false (not expired) for token with non-numeric exp claim', () => {
        const tokenWithStringExp = 'token.with.string.exp'
        mockJwtDecode.mockReturnValue({ exp: 'not-a-number' as unknown as number })

        const result = isTokenValid(tokenWithStringExp)
        
        expect(result).toBe(true) // string is not a number
        expect(mockJwtDecode).toHaveBeenCalledWith(tokenWithStringExp)
      })

      it('should return true (expired) for token with zero exp claim', () => {
        const currentTime = new Date('2023-12-25T12:00:00.000Z')
        vi.setSystemTime(currentTime)
        
        const tokenWithZeroExp = 'token.with.zero.exp'
        mockJwtDecode.mockReturnValue({ exp: 0 }) // Epoch time

        const result = isTokenValid(tokenWithZeroExp)
        
        expect(result).toBe(false) // 1970 is definitely in the past, so expired
        expect(mockJwtDecode).toHaveBeenCalledWith(tokenWithZeroExp)
      })

      it('should return false (not expired) for very long token strings with future exp', () => {
        const currentTime = new Date('2023-12-25T12:00:00.000Z')
        vi.setSystemTime(currentTime)
        
        const futureExp = Math.floor(new Date('2023-12-25T13:00:00.000Z').getTime() / 1000)
        const longToken = 'a'.repeat(1000) + '.very.long.token'
        
        mockJwtDecode.mockReturnValue({ exp: futureExp })

        const result = isTokenValid(longToken)
        
        expect(result).toBe(true) // Not expired
        expect(mockJwtDecode).toHaveBeenCalledWith(longToken)
      })
    })

    describe('time boundary tests', () => {
      it('should return false (not expired) for token expiring in 1 second', () => {
        const currentTime = new Date('2023-12-25T12:00:00.000Z')
        vi.setSystemTime(currentTime)
        
        const almostExpiredExp = Math.floor(new Date('2023-12-25T12:00:01.000Z').getTime() / 1000) // 1 second later
        const almostExpiredToken = 'almost.expired.token'
        
        mockJwtDecode.mockReturnValue({ exp: almostExpiredExp })

        const result = isTokenValid(almostExpiredToken)
        
        expect(result).toBe(true) // Still valid for 1 more second
      })

      it('should return true (expired) for token that expired 1 second ago', () => {
        const currentTime = new Date('2023-12-25T12:00:00.000Z')
        vi.setSystemTime(currentTime)
        
        const justExpiredExp = Math.floor(new Date('2023-12-25T11:59:59.000Z').getTime() / 1000) // 1 second ago
        const justExpiredToken = 'just.expired.token'
        
        mockJwtDecode.mockReturnValue({ exp: justExpiredExp })

        const result = isTokenValid(justExpiredToken)
        
        expect(result).toBe(false) // Expired 1 second ago
      })

      it('should return false (not expired) for far future expiration', () => {
        const currentTime = new Date('2023-12-25T12:00:00.000Z')
        vi.setSystemTime(currentTime)
        
        const farFutureExp = Math.floor(new Date('2030-12-25T12:00:00.000Z').getTime() / 1000) // 7 years later
        const longLivedToken = 'long.lived.token'
        
        mockJwtDecode.mockReturnValue({ exp: farFutureExp })

        const result = isTokenValid(longLivedToken)
        
        expect(result).toBe(true) // Not expired
      })

      it('should return true (expired) for very old expiration', () => {
        const currentTime = new Date('2023-12-25T12:00:00.000Z')
        vi.setSystemTime(currentTime)
        
        const veryOldExp = Math.floor(new Date('2020-01-01T00:00:00.000Z').getTime() / 1000) // Years ago
        const veryOldToken = 'very.old.token'
        
        mockJwtDecode.mockReturnValue({ exp: veryOldExp })

        const result = isTokenValid(veryOldToken)
        
        expect(result).toBe(false) // Expired years ago
      })
    })

    describe('multiple calls', () => {
      it('should return consistent results for same token', () => {
        const currentTime = new Date('2023-12-25T12:00:00.000Z')
        vi.setSystemTime(currentTime)
        
        const futureExp = Math.floor(new Date('2023-12-25T13:00:00.000Z').getTime() / 1000)
        const validToken = 'consistent.token'
        
        mockJwtDecode.mockReturnValue({ exp: futureExp })

        const result1 = isTokenValid(validToken)
        const result2 = isTokenValid(validToken)
        
        expect(result1).toBe(true) // Not expired
        expect(result2).toBe(true) // Not expired
        expect(mockJwtDecode).toHaveBeenCalledTimes(2)
      })

      it('should handle different tokens in sequence', () => {
        const currentTime = new Date('2023-12-25T12:00:00.000Z')
        vi.setSystemTime(currentTime)
        
        const futureExp = Math.floor(new Date('2023-12-25T13:00:00.000Z').getTime() / 1000)
        const pastExp = Math.floor(new Date('2023-12-25T11:00:00.000Z').getTime() / 1000)
        
        mockJwtDecode
          .mockReturnValueOnce({ exp: futureExp })
          .mockReturnValueOnce({ exp: pastExp })

        const validResult = isTokenValid('valid.token')
        const expiredResult = isTokenValid('expired.token')
        
        expect(validResult).toBe(true) // Not expired
        expect(expiredResult).toBe(false) // Expired
        expect(mockJwtDecode).toHaveBeenCalledTimes(2)
      })
    })

    describe('precision tests', () => {
      it('should handle fractional seconds correctly', () => {
        const currentTime = new Date('2023-12-25T12:00:00.500Z') // 500ms
        vi.setSystemTime(currentTime)
        
        const expWithFraction = Math.floor(new Date('2023-12-25T12:00:00.300Z').getTime() / 1000) // 300ms (earlier same second)
        const tokenWithFractionalExp = 'fractional.exp.token'
        
        mockJwtDecode.mockReturnValue({ exp: expWithFraction })

        const result = isTokenValid(tokenWithFractionalExp)
        
        expect(result).toBe(false) // Should be expired because floor makes them same second, but actual time is later
      })

      it('should handle millisecond precision edge case', () => {
        const currentTime = new Date('2023-12-25T12:00:00.999Z') // 999ms
        vi.setSystemTime(currentTime)
        
        const nextSecondExp = Math.floor(new Date('2023-12-25T12:00:01.000Z').getTime() / 1000) // Next second exactly
        const edgeCaseToken = 'edge.case.token'
        
        mockJwtDecode.mockReturnValue({ exp: nextSecondExp })

        const result = isTokenValid(edgeCaseToken)
        
        expect(result).toBe(true) // Should not be expired - exp is in next second
      })
    })
  })

  describe('calculateTokenExpiry', () => {
    beforeEach(() => {
      // Clear environment variable mocks
      vi.unstubAllEnvs()
    })

    afterEach(() => {
      vi.unstubAllEnvs()
    })

    it('should return default expiry in days (2 hours = 1/12 day)', () => {
      const result = calculateTokenExpiry()
      expect(result).toBe(2 / 24) // 2 hours / 24 hours = 1/12 day
    })

    it('should use custom default hours', () => {
      vi.stubEnv('VITE_TOKEN_EXPIRY_HOURS', 'invalid')
      const result = calculateTokenExpiry(4)
      expect(result).toBe(4 / 24) // 4 hours / 24 hours = 1/6 day
    })

    it('should use environment variable when set', () => {
      vi.stubEnv('VITE_TOKEN_EXPIRY_HOURS', '6')
      const result = calculateTokenExpiry()
      expect(result).toBe(6 / 24) // 6 hours / 24 hours = 1/4 day
    })

    it('should parse environment variable as number', () => {
      vi.stubEnv('VITE_TOKEN_EXPIRY_HOURS', '12')
      const result = calculateTokenExpiry()
      expect(result).toBe(12 / 24) // 12 hours / 24 hours = 0.5 day
    })

    it('should fallback to default when environment variable is invalid', () => {
      vi.stubEnv('VITE_TOKEN_EXPIRY_HOURS', 'invalid')
      const result = calculateTokenExpiry()
      expect(result).toBe(2 / 24) // Default 2 hours
    })

    it('should fallback to default when environment variable is empty', () => {
      vi.stubEnv('VITE_TOKEN_EXPIRY_HOURS', '')
      const result = calculateTokenExpiry()
      expect(result).toBe(2 / 24) // Default 2 hours
    })

    it('should handle zero hours', () => {
      vi.stubEnv('VITE_TOKEN_EXPIRY_HOURS', '0')
      const result = calculateTokenExpiry()
      expect(result).toBe(0) // 0 hours = 0 days
    })

    it('should handle fractional hours', () => {
      vi.stubEnv('VITE_TOKEN_EXPIRY_HOURS', '1.5')
      const result = calculateTokenExpiry()
      expect(result).toBe(1.5 / 24) // 1.5 hours
    })

    it('should handle large numbers', () => {
      vi.stubEnv('VITE_TOKEN_EXPIRY_HOURS', '168') // 1 week
      const result = calculateTokenExpiry()
      expect(result).toBe(168 / 24) // 7 days
    })

    it('should return consistent results', () => {
      vi.stubEnv('VITE_TOKEN_EXPIRY_HOURS', '3')
      const result1 = calculateTokenExpiry()
      const result2 = calculateTokenExpiry()
      expect(result1).toBe(result2)
      expect(result1).toBe(3 / 24)
    })

    it('should handle environment variable with leading/trailing spaces', () => {
      vi.stubEnv('VITE_TOKEN_EXPIRY_HOURS', ' 8 ')
      const result = calculateTokenExpiry()
      expect(result).toBe(8 / 24) // Should parse correctly
    })

    it('should fallback to custom default when environment variable is invalid', () => {
      vi.stubEnv('VITE_TOKEN_EXPIRY_HOURS', 'not-a-number')
      const result = calculateTokenExpiry(5)
      expect(result).toBe(5 / 24) // Custom default 5 hours
    })
  })

  describe('getRememberMeExpiry', () => {
    it('should return 14 days', () => {
      const result = getRememberMeExpiry()
      expect(result).toBe(14)
    })

    it('should return consistent results', () => {
      const result1 = getRememberMeExpiry()
      const result2 = getRememberMeExpiry()
      expect(result1).toBe(result2)
      expect(result1).toBe(14)
    })

    it('should return exact number type', () => {
      const result = getRememberMeExpiry()
      expect(typeof result).toBe('number')
      expect(Number.isInteger(result)).toBe(true)
    })

    it('should not be affected by environment variables', () => {
      vi.stubEnv('VITE_REMEMBER_ME_DAYS', '30')
      vi.stubEnv('VITE_TOKEN_EXPIRY_HOURS', '24')
      
      const result = getRememberMeExpiry()
      expect(result).toBe(14) // Always returns 14, not affected by env vars
    })
  })
})
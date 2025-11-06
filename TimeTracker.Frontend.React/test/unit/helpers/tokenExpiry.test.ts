import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { calculateTokenExpiry, getRememberMeExpiry } from '../../../src/helpers/tokenExpiry'

describe('tokenExpiry', () => {
  beforeEach(() => {
    // Clear environment variable mocks
    vi.unstubAllEnvs()
  })

  afterEach(() => {
    vi.unstubAllEnvs()
  })

  describe('calculateTokenExpiry', () => {
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
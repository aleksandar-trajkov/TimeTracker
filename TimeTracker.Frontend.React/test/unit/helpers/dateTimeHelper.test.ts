import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import {
  formatTime,
  formatDate,
  getToday,
  getTomorrow,
  getStartOfDay,
  getStartOfWeek,
  getEndOfWeek,
  addDays,
  dateFormat,
  timeFormat
} from '../../../src/helpers/dateTimeHelper'

describe('dateTimeHelper', () => {
  // Fixed date for consistent testing
  const fixedDate = new Date('2023-12-25T15:30:45.123Z') // Monday
  const fixedDateNoTime = new Date('2023-12-25T00:00:00.000Z');

  describe('constants', () => {
    it('should have correct date format', () => {
      expect(dateFormat).toBe('yyyy-MM-dd')
    })

    it('should have correct time format', () => {
      expect(timeFormat).toBe('HH:mm')
    })
  })

  describe('formatTime', () => {
    it('should format time correctly', () => {
      const result = formatTime(fixedDate)
      expect(result).toBe('15:30')
    })

    it('should format midnight correctly', () => {
      const midnight = new Date('2023-12-25T00:00:00.000Z')
      const result = formatTime(midnight)
      expect(result).toBe('00:00')
    })

    it('should format noon correctly', () => {
      const noon = new Date('2023-12-25T12:00:00.000Z')
      const result = formatTime(noon)
      expect(result).toBe('12:00')
    })

    it('should format late evening correctly', () => {
      const evening = new Date('2023-12-25T23:59:59.999Z')
      const result = formatTime(evening)
      expect(result).toBe('23:59')
    })

    it('should handle single digit hours and minutes', () => {
      const earlyMorning = new Date('2023-12-25T09:05:00.000Z')
      const result = formatTime(earlyMorning)
      expect(result).toBe('09:05')
    })
  })

  describe('formatDate', () => {
    it('should format date correctly', () => {
      const result = formatDate(fixedDate)
      expect(result).toBe('2023-12-25')
    })

    it('should format New Year correctly', () => {
      const newYear = new Date('2024-01-01T12:00:00.000Z')
      const result = formatDate(newYear)
      expect(result).toBe('2024-01-01')
    })

    it('should format end of year correctly', () => {
      const endOfYear = new Date('2023-12-31T23:59:59.999Z')
      const result = formatDate(endOfYear)
      expect(result).toBe('2023-12-31')
    })

    it('should handle leap year correctly', () => {
      const leapYear = new Date('2024-02-29T12:00:00.000Z')
      const result = formatDate(leapYear)
      expect(result).toBe('2024-02-29')
    })
  })

  describe('getToday', () => {
    beforeEach(() => {
      vi.useFakeTimers()
    })

    afterEach(() => {
      vi.useRealTimers()
    })

    it('should return current date', () => {
      vi.setSystemTime(fixedDate)
      const result = getToday()
      expect(result.getTime()).toBe(fixedDateNoTime.getTime())
    })

    it('should return a new Date instance each time', () => {
      vi.setSystemTime(fixedDate)
      const result1 = getToday()
      const result2 = getToday()
      
      expect(result1).not.toBe(result2) // Different instances
      expect(result1.getTime()).toBe(result2.getTime()) // Same time
    })
  })

  describe('getTomorrow', () => {
    beforeEach(() => {
      vi.useFakeTimers()
    })

    afterEach(() => {
      vi.useRealTimers()
    })

    it('should return tomorrow date', () => {
      vi.setSystemTime(fixedDate)
      const result = getTomorrow()
      const expected = new Date('2023-12-26T00:00:00.000Z')
      expect(result.getTime()).toBe(expected.getTime())
    })

    it('should handle month transition', () => {
      const endOfMonth = new Date('2023-11-30T12:00:00.000Z')
      vi.setSystemTime(endOfMonth)
      const result = getTomorrow()
      const expected = new Date('2023-12-01T00:00:00.000Z')
      expect(result.getTime()).toBe(expected.getTime())
    })

    it('should handle year transition', () => {
      const endOfYear = new Date('2023-12-31T12:00:00.000Z')
      vi.setSystemTime(endOfYear)
      const result = getTomorrow()
      const expected = new Date('2024-01-01T00:00:00.000Z')
      expect(result.getTime()).toBe(expected.getTime())
    })
  })

  describe('getStartOfDay', () => {
    it('should return start of day', () => {
      const result = getStartOfDay(fixedDate)
      expect(formatTime(result)).toBe('00:00')
      expect(formatDate(result)).toBe('2023-12-25')
    })

    it('should handle already at start of day', () => {
      const startOfDay = new Date('2023-12-25T00:00:00.000Z')
      const result = getStartOfDay(startOfDay)
      expect(result.getTime()).toBe(startOfDay.getTime())
    })

    it('should handle end of day', () => {
      const endOfDay = new Date('2023-12-25T23:59:59.999Z')
      const result = getStartOfDay(endOfDay)
      const expected = new Date('2023-12-25T00:00:00.000Z')
      expect(result.getTime()).toBe(expected.getTime())
    })
  })

  describe('getStartOfWeek', () => {
    it('should return start of week (Monday) for a Monday', () => {
      // fixedDate is a Monday
      const result = getStartOfWeek(fixedDate)
      expect(formatDate(result)).toBe('2023-12-25') // Same day
      expect(formatTime(result)).toBe('00:00')
    })

    it('should return start of week (Monday) for a Wednesday', () => {
      const wednesday = new Date('2023-12-27T15:30:45.123Z') // Wednesday
      const result = getStartOfWeek(wednesday)
      expect(formatDate(result)).toBe('2023-12-25') // Previous Monday
      expect(formatTime(result)).toBe('00:00')
    })

    it('should return start of week (Monday) for a Sunday', () => {
      const sunday = new Date('2023-12-31T15:30:45.123Z') // Sunday
      const result = getStartOfWeek(sunday)
      expect(formatDate(result)).toBe('2023-12-25') // Previous Monday
      expect(formatTime(result)).toBe('00:00')
    })
  })

  describe('getEndOfWeek', () => {
    it('should return end of week (Sunday) for a Monday', () => {
      // fixedDate is a Monday
      const result = getEndOfWeek(fixedDate)
      expect(formatDate(result)).toBe('2023-12-31') // Following Sunday
      expect(formatTime(result)).toBe('23:59')
    })

    it('should return end of week (Sunday) for a Wednesday', () => {
      const wednesday = new Date('2023-12-27T15:30:45.123Z') // Wednesday
      const result = getEndOfWeek(wednesday)
      expect(formatDate(result)).toBe('2023-12-31') // Following Sunday
      expect(formatTime(result)).toBe('23:59')
    })

    it('should return end of week (Sunday) for a Sunday', () => {
      const sunday = new Date('2023-12-31T15:30:45.123Z') // Sunday
      const result = getEndOfWeek(sunday)
      expect(formatDate(result)).toBe('2023-12-31') // Same day
      expect(formatTime(result)).toBe('23:59')
    })
  })

  describe('addDays', () => {
    it('should add positive days', () => {
      const result = addDays(fixedDate, 5)
      expect(formatDate(result)).toBe('2023-12-30')
      expect(formatTime(result)).toBe('15:30')
    })

    it('should add zero days', () => {
      const result = addDays(fixedDate, 0)
      expect(result.getTime()).toBe(fixedDate.getTime())
    })

    it('should subtract days (negative)', () => {
      const result = addDays(fixedDate, -5)
      expect(formatDate(result)).toBe('2023-12-20')
      expect(formatTime(result)).toBe('15:30')
    })

    it('should handle month transition when adding', () => {
      const result = addDays(fixedDate, 10) // Dec 25 + 10 = Jan 4
      expect(formatDate(result)).toBe('2024-01-04')
    })

    it('should handle month transition when subtracting', () => {
      const result = addDays(fixedDate, -30) // Dec 25 - 30 = Nov 25
      expect(formatDate(result)).toBe('2023-11-25')
    })

    it('should handle year transition', () => {
      const result = addDays(fixedDate, 365) // Add a full year
      expect(formatDate(result)).toBe('2024-12-24') // 2024 is a leap year
    })

    it('should handle leap year correctly', () => {
      const feb28LeapYear = new Date('2024-02-28T12:00:00.000Z')
      const result = addDays(feb28LeapYear, 1)
      expect(formatDate(result)).toBe('2024-02-29') // Leap day
    })

    it('should preserve time when adding days', () => {
      const specificTime = new Date('2023-12-25T08:45:30.500Z')
      const result = addDays(specificTime, 3)
      expect(formatDate(result)).toBe('2023-12-28')
      expect(formatTime(result)).toBe('08:45')
      expect(result.getSeconds()).toBe(30)
      expect(result.getMilliseconds()).toBe(500)
    })
  })
})
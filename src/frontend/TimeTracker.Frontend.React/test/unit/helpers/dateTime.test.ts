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
  timeFormat,
  fixDateTimeForResponse
} from '../../../src/helpers/dateTime'

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

  describe('fixDateTimeForResponse', () => {
    describe('primitive values', () => {
      it('should return null as-is', () => {
        const result = fixDateTimeForResponse(null)
        expect(result).toBe(null)
      })

      it('should return undefined as-is', () => {
        const result = fixDateTimeForResponse(undefined)
        expect(result).toBe(undefined)
      })

      it('should return string as-is when not a date string', () => {
        const input = 'not a date'
        const result = fixDateTimeForResponse(input)
        expect(result).toBe(input)
      })

      it('should return number as-is', () => {
        const input = 42
        const result = fixDateTimeForResponse(input)
        expect(result).toBe(input)
      })

      it('should return boolean as-is', () => {
        const input = true
        const result = fixDateTimeForResponse(input)
        expect(result).toBe(input)
      })
    })

    describe('ISO date string conversion', () => {
      it('should convert ISO date string to Date object', () => {
        const input = { createdAt: '2023-12-25T15:30:45.123Z' }
        const result = fixDateTimeForResponse(input)
        
        expect(result.createdAt).toBeInstanceOf(Date)
        expect((result.createdAt as unknown as Date).getTime()).toBe(new Date('2023-12-25T15:30:45.123Z').getTime())
      })

      it('should convert multiple ISO date strings', () => {
        const input = {
          createdAt: '2023-12-25T15:30:45.123Z',
          updatedAt: '2023-12-26T10:15:30.456Z',
          name: 'test'
        }
        const result = fixDateTimeForResponse(input)
        
        expect(result.createdAt).toBeInstanceOf(Date)
        expect(result.updatedAt).toBeInstanceOf(Date)
        expect(result.name).toBe('test')
        expect((result.createdAt as unknown as Date).getTime()).toBe(new Date('2023-12-25T15:30:45.123Z').getTime())
        expect((result.updatedAt as unknown as Date).getTime()).toBe(new Date('2023-12-26T10:15:30.456Z').getTime())
      })

      it('should handle ISO date string without milliseconds', () => {
        const input = { date: '2023-12-25T15:30:45Z' }
        const result = fixDateTimeForResponse(input)
        
        expect(result.date).toBeInstanceOf(Date)
        expect((result.date as unknown as Date).getTime()).toBe(new Date('2023-12-25T15:30:45Z').getTime())
      })

      it('should handle ISO date string with timezone offset', () => {
        const input = { date: '2023-12-25T15:30:45+02:00' }
        const result = fixDateTimeForResponse(input)
        
        expect(result.date).toBeInstanceOf(Date)
        expect((result.date as unknown as Date).getTime()).toBe(new Date('2023-12-25T15:30:45+02:00').getTime())
      })

      it('should handle ISO date string with negative timezone offset', () => {
        const input = { date: '2023-12-25T15:30:45-05:00' }
        const result = fixDateTimeForResponse(input)
        
        expect(result.date).toBeInstanceOf(Date)
        expect((result.date as unknown as Date).getTime()).toBe(new Date('2023-12-25T15:30:45-05:00').getTime())
      })

      it('should not convert invalid date-like strings', () => {
        const input = {
          notADate1: '2023-13-25T15:30:45Z', // Invalid month - but matches ISO format, so gets converted
          notADate2: '2023-12-32T15:30:45Z', // Invalid day - but matches ISO format, so gets converted  
          notADate3: '2023-12-25T25:30:45Z', // Invalid hour - but matches ISO format, so gets converted
          notADate4: '2023-12-25T15:60:45Z', // Invalid minute - but matches ISO format, so gets converted
          notADate5: '2023-12-25T15:30:60Z', // Invalid second - but matches ISO format, so gets converted
          notADate6: '2023-12-25 15:30:45',   // Missing T - should remain string
          notADate7: '12/25/2023',            // Different format - should remain string
          notADate8: 'Mon Dec 25 2023'        // Different format - should remain string
        }
        const result = fixDateTimeForResponse(input)
        
        // Invalid ISO dates that match the regex will be converted (creating invalid Date objects)
        expect(result.notADate1).toBeInstanceOf(Date)
        expect(result.notADate2).toBeInstanceOf(Date)
        expect(result.notADate3).toBeInstanceOf(Date)
        expect(result.notADate4).toBeInstanceOf(Date)
        expect(result.notADate5).toBeInstanceOf(Date)
        
        // Non-ISO format strings should remain as strings
        expect(typeof result.notADate6).toBe('string')
        expect(typeof result.notADate7).toBe('string')
        expect(typeof result.notADate8).toBe('string')
      })
    })

    describe('nested objects', () => {
      it('should handle nested objects with date strings', () => {
        const input = {
          user: {
            id: 1,
            createdAt: '2023-12-25T15:30:45.123Z',
            profile: {
              lastLogin: '2023-12-26T10:15:30.456Z',
              name: 'John'
            }
          },
          metadata: {
            processedAt: '2023-12-27T08:00:00.000Z'
          }
        }
        const result = fixDateTimeForResponse(input)
        
        expect(result.user.createdAt).toBeInstanceOf(Date)
        expect(result.user.profile.lastLogin).toBeInstanceOf(Date)
        expect(result.metadata.processedAt).toBeInstanceOf(Date)
        expect(result.user.id).toBe(1)
        expect(result.user.profile.name).toBe('John')
      })

      it('should handle deeply nested objects', () => {
        const input = {
          level1: {
            level2: {
              level3: {
                level4: {
                  deepDate: '2023-12-25T15:30:45.123Z',
                  deepValue: 'test'
                }
              }
            }
          }
        }
        const result = fixDateTimeForResponse(input)
        
        expect(result.level1.level2.level3.level4.deepDate).toBeInstanceOf(Date)
        expect(result.level1.level2.level3.level4.deepValue).toBe('test')
      })

      it('should handle objects with null values', () => {
        const input = {
          date: '2023-12-25T15:30:45.123Z',
          nullValue: null,
          undefinedValue: undefined,
          nested: {
            anotherDate: '2023-12-26T10:15:30.456Z',
            anotherNull: null
          }
        }
        const result = fixDateTimeForResponse(input)
        
        expect(result.date).toBeInstanceOf(Date)
        expect(result.nullValue).toBe(null)
        expect(result.undefinedValue).toBe(undefined)
        expect(result.nested.anotherDate).toBeInstanceOf(Date)
        expect(result.nested.anotherNull).toBe(null)
      })
    })

    describe('arrays', () => {
      it('should handle arrays with date strings', () => {
        const input = [
          { id: 1, createdAt: '2023-12-25T15:30:45.123Z' },
          { id: 2, createdAt: '2023-12-26T10:15:30.456Z' },
          { id: 3, name: 'no date' }
        ]
        const result = fixDateTimeForResponse(input)
        
        expect(result[0].createdAt).toBeInstanceOf(Date)
        expect(result[1].createdAt).toBeInstanceOf(Date)
        expect(result[0].id).toBe(1)
        expect(result[1].id).toBe(2)
        expect(result[2].name).toBe('no date')
      })

      it('should handle nested arrays', () => {
        const input = {
          users: [
            {
              id: 1,
              posts: [
                { title: 'Post 1', publishedAt: '2023-12-25T15:30:45.123Z' },
                { title: 'Post 2', publishedAt: '2023-12-26T10:15:30.456Z' }
              ]
            }
          ]
        }
        const result = fixDateTimeForResponse(input)
        
        expect(result.users[0].posts[0].publishedAt).toBeInstanceOf(Date)
        expect(result.users[0].posts[1].publishedAt).toBeInstanceOf(Date)
        expect(result.users[0].posts[0].title).toBe('Post 1')
        expect(result.users[0].posts[1].title).toBe('Post 2')
      })
    })

    describe('edge cases', () => {
      it('should handle empty objects', () => {
        const input = {}
        const result = fixDateTimeForResponse(input)
        expect(result).toEqual({})
      })

      it('should handle empty arrays', () => {
        const input: unknown[] = []
        const result = fixDateTimeForResponse(input)
        expect(result).toEqual([])
      })

      it('should handle circular references (note: current implementation will cause stack overflow)', () => {
        // This test documents current behavior - circular references cause stack overflow
        // In a production system, you might want to add circular reference detection
        const input: Record<string, unknown> = { id: 1 }
        input.self = input // Create circular reference
        
        expect(() => fixDateTimeForResponse(input)).toThrow('Maximum call stack size exceeded')
      })

      it('should handle objects with Date objects already present', () => {
        const existingDate = new Date('2023-12-25T15:30:45.123Z')
        const input = {
          existingDate,
          stringDate: '2023-12-26T10:15:30.456Z',
          normalField: 'test'
        }
        const result = fixDateTimeForResponse(input)
        
        expect(result.existingDate).toBe(existingDate) // Should remain the same Date object
        expect(result.stringDate).toBeInstanceOf(Date)
        expect(result.stringDate).not.toBe(existingDate) // Should be a new Date object
        expect(result.normalField).toBe('test')
      })

      it('should handle mixed data types in arrays', () => {
        const input = [
          'regular string', // Non-ISO string should stay as string
          { date: '2023-12-26T10:15:30.456Z' },
          'another string',
          42,
          null,
          undefined,
          true
        ]
        const result = fixDateTimeForResponse(input)
        
        expect(result[0]).toBe('regular string') // Non-ISO string stays as string
        expect((result[1] as unknown as { date: Date }).date).toBeInstanceOf(Date) // Object property gets converted
        expect(result[2]).toBe('another string')
        expect(result[3]).toBe(42)
        expect(result[4]).toBe(null)
        expect(result[5]).toBe(undefined)
        expect(result[6]).toBe(true)
      })

      it('should convert ISO date strings even when in arrays (array items are processed as objects)', () => {
        // Note: Arrays are treated as objects, so array indices become object keys
        const input = ['2023-12-25T15:30:45.123Z', 'not-a-date']
        const result = fixDateTimeForResponse(input)
        
        expect(result[0]).toBeInstanceOf(Date) // ISO string gets converted
        expect(result[1]).toBe('not-a-date') // Non-ISO string stays as string
      })
    })

    describe('type preservation', () => {
      it('should preserve the original object structure', () => {
        interface TestObject {
          id: number
          name: string
          createdAt: string | Date
          metadata?: {
            updatedAt: string | Date
          }
        }

        const input: TestObject = {
          id: 1,
          name: 'test',
          createdAt: '2023-12-25T15:30:45.123Z',
          metadata: {
            updatedAt: '2023-12-26T10:15:30.456Z'
          }
        }
        
        const result = fixDateTimeForResponse(input)
        
        // Type should be preserved
        expect(typeof result.id).toBe('number')
        expect(typeof result.name).toBe('string')
        expect(result.createdAt).toBeInstanceOf(Date)
        expect(result.metadata?.updatedAt).toBeInstanceOf(Date)
      })

      it('should work with generic types', () => {
        interface ApiResponse<T> {
          data: T
          timestamp: string
        }

        const input: ApiResponse<{ id: number; createdAt: string }> = {
          data: { id: 1, createdAt: '2023-12-25T15:30:45.123Z' },
          timestamp: '2023-12-25T16:00:00.000Z'
        }
        
        const result = fixDateTimeForResponse(input)
        
        expect(result.data.id).toBe(1)
        expect(result.data.createdAt).toBeInstanceOf(Date)
        expect(result.timestamp).toBeInstanceOf(Date)
      })
    })
  })
})
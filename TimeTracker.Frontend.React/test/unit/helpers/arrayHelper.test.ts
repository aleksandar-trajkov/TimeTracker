import { describe, it, expect } from 'vitest'
import { groupBy } from '../../../src/helpers/arrayHelper'

describe('arrayHelper', () => {
  describe('groupBy', () => {
    interface TestItem {
      id: number
      category: string
      status: 'active' | 'inactive'
      date: string
      value: number
    }

    const testData: TestItem[] = [
      { id: 1, category: 'A', status: 'active', date: '2023-01-01', value: 10 },
      { id: 2, category: 'B', status: 'inactive', date: '2023-01-01', value: 20 },
      { id: 3, category: 'A', status: 'active', date: '2023-01-02', value: 30 },
      { id: 4, category: 'C', status: 'inactive', date: '2023-01-01', value: 40 },
      { id: 5, category: 'B', status: 'active', date: '2023-01-02', value: 50 },
    ]

    it('should group by string key', () => {
      const result = groupBy(testData, 'category')

      expect(Object.keys(result)).toEqual(['A', 'B', 'C'])
      expect(result.A).toHaveLength(2)
      expect(result.B).toHaveLength(2)
      expect(result.C).toHaveLength(1)
      
      expect(result.A[0]).toEqual(testData[0])
      expect(result.A[1]).toEqual(testData[2])
      expect(result.B[0]).toEqual(testData[1])
      expect(result.B[1]).toEqual(testData[4])
      expect(result.C[0]).toEqual(testData[3])
    })

    it('should group by union type key', () => {
      const result = groupBy(testData, 'status')

      expect(Object.keys(result)).toEqual(['active', 'inactive'])
      expect(result.active).toHaveLength(3)
      expect(result.inactive).toHaveLength(2)
      
      expect(result.active.map(item => item.id)).toEqual([1, 3, 5])
      expect(result.inactive.map(item => item.id)).toEqual([2, 4])
    })

    it('should group by number key', () => {
      const numberData = [
        { id: 1, priority: 1 },
        { id: 2, priority: 2 },
        { id: 3, priority: 1 },
        { id: 4, priority: 3 },
      ]

      const result = groupBy(numberData, 'priority')

      expect(Object.keys(result)).toEqual(['1', '2', '3'])
      expect(result[1]).toHaveLength(2)
      expect(result[2]).toHaveLength(1)
      expect(result[3]).toHaveLength(1)
    })

    it('should group by function returning string', () => {
      const result = groupBy(testData, (item) => item.date)

      expect(Object.keys(result)).toEqual(['2023-01-01', '2023-01-02'])
      expect(result['2023-01-01']).toHaveLength(3)
      expect(result['2023-01-02']).toHaveLength(2)
      
      expect(result['2023-01-01'].map(item => item.id)).toEqual([1, 2, 4])
      expect(result['2023-01-02'].map(item => item.id)).toEqual([3, 5])
    })

    it('should group by function returning number', () => {
      const result = groupBy(testData, (item) => Math.floor(item.value / 20))

      expect(Object.keys(result).sort()).toEqual(['0', '1', '2'])
      expect(result[0]).toHaveLength(1) // value 10
      expect(result[1]).toHaveLength(2) // values 20, 30
      expect(result[2]).toHaveLength(2) // values 40, 50
    })

    it('should group by complex function logic', () => {
      const result = groupBy(testData, (item) => 
        item.status === 'active' ? `active_${item.category}` : `inactive_${item.category}`
      )

      expect(Object.keys(result).sort()).toEqual([
        'active_A', 'active_B', 'inactive_B', 'inactive_C'
      ])
      expect(result.active_A).toHaveLength(2)
      expect(result.active_B).toHaveLength(1)
      expect(result.inactive_B).toHaveLength(1)
      expect(result.inactive_C).toHaveLength(1)
    })

    it('should handle empty array', () => {
      const result = groupBy([], 'category')

      expect(result).toEqual({})
      expect(Object.keys(result)).toHaveLength(0)
    })

    it('should handle single item array', () => {
      const singleItem = [testData[0]]
      const result = groupBy(singleItem, 'category')

      expect(Object.keys(result)).toEqual(['A'])
      expect(result.A).toHaveLength(1)
      expect(result.A[0]).toEqual(testData[0])
    })

    it('should handle all items in same group', () => {
      const sameCategory = testData.map(item => ({ ...item, category: 'SAME' }))
      const result = groupBy(sameCategory, 'category')

      expect(Object.keys(result)).toEqual(['SAME'])
      expect(result.SAME).toHaveLength(5)
    })

    it('should handle undefined/null values in grouping key', () => {
      const dataWithNulls: { id: number; category: string | null | undefined }[] = [
        { id: 1, category: 'A' },
        { id: 2, category: null },
        { id: 3, category: undefined },
        { id: 4, category: 'A' },
      ]

      const result = groupBy(dataWithNulls, 'category')

      expect(Object.keys(result).sort()).toEqual(['A', 'null', 'undefined'])
      expect(result.A).toHaveLength(2)
      expect(result.null).toHaveLength(1)
      expect(result.undefined).toHaveLength(1)
    })

    it('should maintain original object references', () => {
      const result = groupBy(testData, 'category')

      // Check that objects are the same references, not copies
      expect(result.A[0]).toBe(testData[0])
      expect(result.A[1]).toBe(testData[2])
    })

    it('should preserve order within groups', () => {
      const result = groupBy(testData, 'category')

      // Items should appear in the same order they were in the original array
      expect(result.A[0].id).toBe(1) // First A item
      expect(result.A[1].id).toBe(3) // Second A item
      expect(result.B[0].id).toBe(2) // First B item
      expect(result.B[1].id).toBe(5) // Second B item
    })

    it('should work with different data types', () => {
      const mixedData = [
        { type: 'string', value: 'hello' },
        { type: 'number', value: 42 },
        { type: 'boolean', value: true },
        { type: 'string', value: 'world' },
      ]

      const result = groupBy(mixedData, 'type')

      expect(Object.keys(result).sort()).toEqual(['boolean', 'number', 'string'])
      expect(result.string).toHaveLength(2)
      expect(result.number).toHaveLength(1)
      expect(result.boolean).toHaveLength(1)
    })

    it('should handle complex nested objects', () => {
      const nestedData = [
        { id: 1, user: { role: 'admin', department: 'IT' } },
        { id: 2, user: { role: 'user', department: 'HR' } },
        { id: 3, user: { role: 'admin', department: 'Finance' } },
      ]

      const result = groupBy(nestedData, (item) => item.user.role)

      expect(Object.keys(result)).toEqual(['admin', 'user'])
      expect(result.admin).toHaveLength(2)
      expect(result.user).toHaveLength(1)
    })

    it('should work with dates when using function grouping', () => {
      const dateData = [
        { id: 1, createdAt: '2023-01-15T10:00:00Z' },
        { id: 2, createdAt: '2023-01-15T14:00:00Z' },
        { id: 3, createdAt: '2023-01-16T10:00:00Z' },
      ]

      const result = groupBy(dateData, (item) => item.createdAt.split('T')[0])

      expect(Object.keys(result)).toEqual(['2023-01-15', '2023-01-16'])
      expect(result['2023-01-15']).toHaveLength(2)
      expect(result['2023-01-16']).toHaveLength(1)
    })
  })
})
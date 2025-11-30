import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { showNotification } from '../../../src/helpers/notifications'
import useNotificationStore from '../../../src/stores/notificationStore'

// Mock the notification store
vi.mock('../../../src/stores/notificationStore', () => ({
  default: {
    getState: vi.fn()
  }
}))

describe('notifications', () => {
  const mockAddNotification = vi.fn()
  
  beforeEach(() => {
    vi.clearAllMocks()
    mockAddNotification.mockReturnValue('test-id-123')
    vi.mocked(useNotificationStore.getState).mockReturnValue({
      addNotification: mockAddNotification,
      removeNotification: vi.fn(),
      clearAllNotifications: vi.fn(),
      notifications: []
    })
  })

  afterEach(() => {
    vi.clearAllMocks()
  })

  describe('showNotification.success', () => {
    it('should call addNotification with success type and title', () => {
      showNotification.success('Success Title')

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'success',
        title: 'Success Title',
        message: undefined,
        duration: undefined
      })
    })

    it('should call addNotification with success type, title, and message', () => {
      showNotification.success('Success Title', 'Success message')

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'success',
        title: 'Success Title',
        message: 'Success message',
        duration: undefined
      })
    })

    it('should call addNotification with custom duration', () => {
      showNotification.success('Success Title', 'Success message', 3000)

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'success',
        title: 'Success Title',
        message: 'Success message',
        duration: 3000
      })
    })

    it('should return the notification id', () => {
      const result = showNotification.success('Success Title')
      expect(result).toBe('test-id-123')
    })
  })

  describe('showNotification.error', () => {
    it('should call addNotification with error type and title', () => {
      showNotification.error('Error Title')

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'Error Title',
        message: undefined,
        duration: undefined
      })
    })

    it('should call addNotification with error type, title, and message', () => {
      showNotification.error('Error Title', 'Error message')

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'Error Title',
        message: 'Error message',
        duration: undefined
      })
    })

    it('should call addNotification with custom duration', () => {
      showNotification.error('Error Title', 'Error message', 10000)

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'Error Title',
        message: 'Error message',
        duration: 10000
      })
    })
  })

  describe('showNotification.warning', () => {
    it('should call addNotification with warning type and title', () => {
      showNotification.warning('Warning Title')

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'warning',
        title: 'Warning Title',
        message: undefined,
        duration: undefined
      })
    })

    it('should call addNotification with warning type, title, and message', () => {
      showNotification.warning('Warning Title', 'Warning message')

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'warning',
        title: 'Warning Title',
        message: 'Warning message',
        duration: undefined
      })
    })
  })

  describe('showNotification.info', () => {
    it('should call addNotification with info type and title', () => {
      showNotification.info('Info Title')

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'info',
        title: 'Info Title',
        message: undefined,
        duration: undefined
      })
    })

    it('should call addNotification with info type, title, and message', () => {
      showNotification.info('Info Title', 'Info message')

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'info',
        title: 'Info Title',
        message: 'Info message',
        duration: undefined
      })
    })
  })

  describe('showNotification.errorList', () => {
    it('should show fallback error when errors array is empty', () => {
      showNotification.errorList('Error Title', [])

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'Error Title',
        message: 'An error occurred',
        duration: undefined
      })
    })

    it('should show fallback error when errors is null', () => {
      showNotification.errorList('Error Title', null as unknown as string[])

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'Error Title',
        message: 'An error occurred',
        duration: undefined
      })
    })

    it('should show fallback error when errors is undefined', () => {
      showNotification.errorList('Error Title', undefined as unknown as string[])

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'Error Title',
        message: 'An error occurred',
        duration: undefined
      })
    })

    it('should show single error as regular error notification', () => {
      showNotification.errorList('Error Title', ['Single error message'])

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'Error Title',
        message: 'Single error message',
        duration: undefined
      })
    })

    it('should format multiple errors as numbered list', () => {
      showNotification.errorList('Validation Errors', [
        'Field 1 is required',
        'Field 2 is invalid',
        'Field 3 must be unique'
      ])

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'Validation Errors',
        message: '1. Field 1 is required\n2. Field 2 is invalid\n3. Field 3 must be unique',
        duration: 7000
      })
    })

    it('should use custom duration for multiple errors', () => {
      showNotification.errorList('Errors', ['Error 1', 'Error 2'], 5000)

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'Errors',
        message: '1. Error 1\n2. Error 2',
        duration: 5000
      })
    })

    it('should use default 7000ms duration for multiple errors when not specified', () => {
      showNotification.errorList('Errors', ['Error 1', 'Error 2'])

      expect(mockAddNotification).toHaveBeenCalledWith(
        expect.objectContaining({
          duration: 7000
        })
      )
    })
  })

  describe('showNotification.apiError', () => {
    it('should handle error with errors array', () => {
      const apiError = {
        message: 'Validation failed',
        errors: ['Error 1', 'Error 2', 'Error 3']
      }

      showNotification.apiError('API Error', apiError)

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'API Error',
        message: '1. Error 1\n2. Error 2\n3. Error 3',
        duration: 7000
      })
    })

    it('should handle error with single message', () => {
      const apiError = {
        message: 'Something went wrong'
      }

      showNotification.apiError('API Error', apiError)

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'API Error',
        message: 'Something went wrong',
        duration: undefined
      })
    })

    it('should handle Error instance with message', () => {
      const error = new Error('Network error')

      showNotification.apiError('API Error', error)

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'API Error',
        message: 'Network error',
        duration: undefined
      })
    })

    it('should use fallback message when error has no message', () => {
      const error = {}

      showNotification.apiError('API Error', error, 'Default error message')

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'API Error',
        message: 'Default error message',
        duration: undefined
      })
    })

    it('should use default fallback message when none provided', () => {
      const error = {}

      showNotification.apiError('API Error', error)

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'API Error',
        message: 'An error occurred',
        duration: undefined
      })
    })

    it('should prioritize errors array over message field', () => {
      const apiError = {
        message: 'General error',
        errors: ['Specific error 1', 'Specific error 2']
      }

      showNotification.apiError('API Error', apiError)

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'API Error',
        message: '1. Specific error 1\n2. Specific error 2',
        duration: 7000
      })
    })

    it('should handle empty errors array', () => {
      const apiError = {
        message: 'Error message',
        errors: []
      }

      showNotification.apiError('API Error', apiError)

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'API Error',
        message: 'Error message',
        duration: undefined
      })
    })

    it('should handle null error', () => {
      showNotification.apiError('API Error', null, 'Fallback')

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'API Error',
        message: 'Fallback',
        duration: undefined
      })
    })

    it('should handle undefined error', () => {
      showNotification.apiError('API Error', undefined, 'Fallback')

      expect(mockAddNotification).toHaveBeenCalledWith({
        type: 'error',
        title: 'API Error',
        message: 'Fallback',
        duration: undefined
      })
    })
  })

  describe('integration tests', () => {
    it('should call getState for each notification type', () => {
      showNotification.success('Success')
      showNotification.error('Error')
      showNotification.warning('Warning')
      showNotification.info('Info')

      expect(useNotificationStore.getState).toHaveBeenCalledTimes(4)
    })

    it('should return notification id from all methods', () => {
      const successId = showNotification.success('Success')
      const errorId = showNotification.error('Error')
      const warningId = showNotification.warning('Warning')
      const infoId = showNotification.info('Info')
      const errorListId = showNotification.errorList('Errors', ['Error 1'])
      const apiErrorId = showNotification.apiError('API Error', new Error('Test'))

      expect(successId).toBe('test-id-123')
      expect(errorId).toBe('test-id-123')
      expect(warningId).toBe('test-id-123')
      expect(infoId).toBe('test-id-123')
      expect(errorListId).toBe('test-id-123')
      expect(apiErrorId).toBe('test-id-123')
    })
  })
})

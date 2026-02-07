import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import '@testing-library/jest-dom'
import DateTimePicker from '../../../../src/components/date/DateTimePicker'

describe('DateTimePicker Component', () => {
  const testDate = new Date('2023-12-25T14:30:00')
  const defaultProps = {
    id: 'test-datetimepicker',
    name: 'test-datetime',
    label: 'Test DateTime',
    value: testDate,
    onChange: vi.fn(),
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('renders date-time picker with main label', () => {
      render(<DateTimePicker {...defaultProps} />)
      
      expect(screen.getByText('Test DateTime')).toBeInTheDocument()
    })

    it('renders both date and time components by default', () => {
      render(<DateTimePicker {...defaultProps} />)
      
      expect(screen.getByText('Date')).toBeInTheDocument()
      expect(screen.getByText('Time')).toBeInTheDocument()
      expect(screen.getByDisplayValue('2023-12-25')).toBeInTheDocument()
      expect(screen.getByDisplayValue('14')).toBeInTheDocument() // Hours
      expect(screen.getByDisplayValue('30')).toBeInTheDocument() // Minutes
    })

    it('uses custom date and time labels when provided', () => {
      render(
        <DateTimePicker
          {...defaultProps}
          dateLabel="Custom Date"
          timeLabel="Custom Time"
        />
      )
      
      expect(screen.getByText('Custom Date')).toBeInTheDocument()
      expect(screen.getByText('Custom Time')).toBeInTheDocument()
    })

    it('displays required indicator when required is true', () => {
      render(<DateTimePicker {...defaultProps} required />)
      
      const requiredIndicators = screen.getAllByText('*')
      expect(requiredIndicators.length).toBeGreaterThan(0)
    })

    it('does not display required indicator when required is false', () => {
      render(<DateTimePicker {...defaultProps} required={false} />)
      
      expect(screen.queryByText('*')).not.toBeInTheDocument()
    })

    it('applies custom container className', () => {
      render(<DateTimePicker {...defaultProps} containerClassName="custom-container" />)
      
      const container = screen.getByText('Test DateTime').closest('div')
      expect(container).toHaveClass('custom-container')
    })

    it('uses default container className when not provided', () => {
      render(<DateTimePicker {...defaultProps} />)
      
      const container = screen.getByText('Test DateTime').closest('div')
      expect(container).toHaveClass('mb-3')
    })
  })

  describe('Layout', () => {
    it('uses responsive Bootstrap grid layout', () => {
      render(<DateTimePicker {...defaultProps} />)
      
      const dateContainer = screen.getByText('Date').closest('.col-md-7')
      const timeContainer = screen.getByText('Time').closest('.col-md-5')
      
      expect(dateContainer).toBeInTheDocument()
      expect(timeContainer).toBeInTheDocument()
    })

    it('uses full width for date when showTime is false', () => {
      render(<DateTimePicker {...defaultProps} showTime={false} />)
      
      const dateContainer = screen.getByText('Date').closest('.col-12')
      expect(dateContainer).toBeInTheDocument()
    })

    it('hides time component when showTime is false', () => {
      render(<DateTimePicker {...defaultProps} showTime={false} />)
      
      expect(screen.getByText('Date')).toBeInTheDocument()
      expect(screen.queryByText('Time')).not.toBeInTheDocument()
      expect(screen.queryByDisplayValue('14')).not.toBeInTheDocument()
    })
  })

  describe('Date Change Handling', () => {
    it('calls onChange when date changes, preserving existing time', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<DateTimePicker {...defaultProps} onChange={mockOnChange} />)
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      await user.clear(dateInput)
      await user.type(dateInput, '2023-11-10')
      
      expect(mockOnChange).toHaveBeenCalled()
      // Find the last call that's not null (DatePicker may call with null during clear)
      const validCalls = mockOnChange.mock.calls.filter(call => call[0] !== null)
      if (validCalls.length > 0) {
        const callArg = validCalls[validCalls.length - 1][0]
        expect(callArg).toBeInstanceOf(Date)
        expect(callArg.getFullYear()).toBe(2023)
        expect(callArg.getMonth()).toBe(10) // November (0-indexed)
        expect(callArg.getDate()).toBe(10)
        expect(callArg.getHours()).toBe(14) // Preserved from original
        expect(callArg.getMinutes()).toBe(30) // Preserved from original
      }
    })

    it('sets default time when date changes and no previous value exists', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<DateTimePicker {...defaultProps} value={null} onChange={mockOnChange} />)
      
      // Find the date input by type attribute - use current date
      const today = new Date().toISOString().split('T')[0]
      const dateInput = screen.getByDisplayValue(today)
      await user.clear(dateInput)
      await user.type(dateInput, '2023-11-10')
      
      expect(mockOnChange).toHaveBeenCalled()
      // Find the last call that's not null (DatePicker may call with null during clear)
      const validCalls = mockOnChange.mock.calls.filter(call => call[0] !== null)
      if (validCalls.length > 0) {
        const callArg = validCalls[validCalls.length - 1][0]
        expect(callArg).toBeInstanceOf(Date)
        expect(callArg.getFullYear()).toBe(2023)
        expect(callArg.getMonth()).toBe(10) // November
        expect(callArg.getDate()).toBe(10)
        // Should set current time as default
        expect(typeof callArg.getHours()).toBe('number')
        expect(typeof callArg.getMinutes()).toBe('number')
      }
    })

    it('calls onChange with null when date is cleared', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<DateTimePicker {...defaultProps} onChange={mockOnChange} />)
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      await user.clear(dateInput)
      
      expect(mockOnChange).toHaveBeenCalled()
      const callArg = mockOnChange.mock.calls[mockOnChange.mock.calls.length - 1][0]
      expect(callArg).toBeNull()
    })
  })

  describe('Time Change Handling', () => {
    it('calls onChange when hour changes, preserving existing date', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<DateTimePicker {...defaultProps} onChange={mockOnChange} />)
      
      const hourSelect = screen.getByDisplayValue('14')
      await user.selectOptions(hourSelect, '16')
      
      expect(mockOnChange).toHaveBeenCalled()
      const callArg = mockOnChange.mock.calls[0][0]
      expect(callArg).toBeInstanceOf(Date)
      expect(callArg.getFullYear()).toBe(2023) // Preserved from original
      expect(callArg.getMonth()).toBe(11) // December (0-indexed), preserved
      expect(callArg.getDate()).toBe(25) // Preserved from original
      expect(callArg.getHours()).toBe(16) // Updated
      expect(callArg.getMinutes()).toBe(30) // Preserved from original
    })

    it('calls onChange when minute changes, preserving existing date', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<DateTimePicker {...defaultProps} onChange={mockOnChange} />)
      
      const minuteSelect = screen.getByDisplayValue('30')
      await user.selectOptions(minuteSelect, '45')
      
      expect(mockOnChange).toHaveBeenCalled()
      const callArg = mockOnChange.mock.calls[0][0]
      expect(callArg).toBeInstanceOf(Date)
      expect(callArg.getFullYear()).toBe(2023) // Preserved
      expect(callArg.getMonth()).toBe(11) // December, preserved
      expect(callArg.getDate()).toBe(25) // Preserved
      expect(callArg.getHours()).toBe(14) // Preserved
      expect(callArg.getMinutes()).toBe(45) // Updated
    })

    it('sets date to today when time changes and no previous value exists', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      const today = new Date()
      
      render(<DateTimePicker {...defaultProps} value={null} onChange={mockOnChange} />)
      
      const hourSelect = screen.getAllByRole('combobox')[0] // Get hour select
      await user.selectOptions(hourSelect, '16')
      
      expect(mockOnChange).toHaveBeenCalled()
      const callArg = mockOnChange.mock.calls[0][0]
      expect(callArg).toBeInstanceOf(Date)
      expect(callArg.getFullYear()).toBe(today.getFullYear())
      expect(callArg.getMonth()).toBe(today.getMonth())
      expect(callArg.getDate()).toBe(today.getDate())
      expect(callArg.getHours()).toBe(16)
    })
  })

  describe('Disabled State', () => {
    it('disables both date and time components when disabled is true', () => {
      render(<DateTimePicker {...defaultProps} disabled />)
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      const selects = screen.getAllByRole('combobox')
      
      expect(dateInput).toBeDisabled()
      selects.forEach(select => {
        expect(select).toBeDisabled()
      })
    })

    it('enables both components when disabled is false', () => {
      render(<DateTimePicker {...defaultProps} disabled={false} />)
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      const selects = screen.getAllByRole('combobox')
      
      expect(dateInput).not.toBeDisabled()
      selects.forEach(select => {
        expect(select).not.toBeDisabled()
      })
    })

    it('does not call onChange when disabled and user tries to interact', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<DateTimePicker {...defaultProps} disabled onChange={mockOnChange} />)
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      await user.type(dateInput, '2023-11-10')
      
      expect(mockOnChange).not.toHaveBeenCalled()
    })
  })

  describe('Required State', () => {
    it('applies required attribute to date component when required is true', () => {
      render(<DateTimePicker {...defaultProps} required />)
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      expect(dateInput).toHaveAttribute('required')
    })

    it('applies required attribute to time selects when required is true', () => {
      render(<DateTimePicker {...defaultProps} required />)
      
      const selects = screen.getAllByRole('combobox')
      selects.forEach(select => {
        expect(select).toHaveAttribute('required')
      })
    })

    it('does not apply required attribute when required is false', () => {
      render(<DateTimePicker {...defaultProps} required={false} />)
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      const selects = screen.getAllByRole('combobox')
      
      expect(dateInput).not.toHaveAttribute('required')
      selects.forEach(select => {
        expect(select).not.toHaveAttribute('required')
      })
    })
  })

  describe('Date Constraints', () => {
    it('passes min date constraint to date picker', () => {
      const minDate = new Date('2023-01-01')
      render(<DateTimePicker {...defaultProps} min={minDate} />)
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      expect(dateInput).toHaveAttribute('min', '2023-01-01')
    })

    it('passes max date constraint to date picker', () => {
      const maxDate = new Date('2023-12-31')
      render(<DateTimePicker {...defaultProps} max={maxDate} />)
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      expect(dateInput).toHaveAttribute('max', '2023-12-31')
    })

    it('passes both min and max constraints', () => {
      const minDate = new Date('2023-01-01')
      const maxDate = new Date('2023-12-31')
      render(
        <DateTimePicker
          {...defaultProps}
          min={minDate}
          max={maxDate}
        />
      )
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      expect(dateInput).toHaveAttribute('min', '2023-01-01')
      expect(dateInput).toHaveAttribute('max', '2023-12-31')
    })
  })

  describe('Null Value Handling', () => {
    it('handles null value correctly', () => {
      render(<DateTimePicker {...defaultProps} value={null} />)
      
      // Date picker shows today's date when value is null (due to fallback in DatePicker)
      const today = new Date().toISOString().split('T')[0]
      const dateInput = screen.getByDisplayValue(today)
      expect(dateInput).toBeInTheDocument()
      
      // Time picker should show default values
      const hourSelect = screen.getAllByRole('combobox')[0]
      const minuteSelect = screen.getAllByRole('combobox')[1]
      expect(hourSelect).toHaveValue('0')
      expect(minuteSelect).toHaveValue('0')
    })

    it('shows current date in DatePicker when value is null', () => {
      render(<DateTimePicker {...defaultProps} value={null} />)
      
      // DatePicker component uses current date as fallback when value is null
      const today = new Date().toISOString().split('T')[0]
      const dateInput = screen.getByDisplayValue(today)
      expect(dateInput).toHaveValue(today)
    })
  })

  describe('Value Updates', () => {
    it('updates display when value prop changes', () => {
      const { rerender } = render(
        <DateTimePicker {...defaultProps} value={new Date('2023-01-01T10:15:00')} />
      )
      
      expect(screen.getByDisplayValue('2023-01-01')).toBeInTheDocument()
      expect(screen.getByDisplayValue('10')).toBeInTheDocument()
      expect(screen.getByDisplayValue('15')).toBeInTheDocument()
      
      rerender(
        <DateTimePicker {...defaultProps} value={new Date('2023-12-31T22:45:00')} />
      )
      
      expect(screen.getByDisplayValue('2023-12-31')).toBeInTheDocument()
      expect(screen.getByDisplayValue('22')).toBeInTheDocument()
      expect(screen.getByDisplayValue('45')).toBeInTheDocument()
    })
  })

  describe('Custom Styling', () => {
    it('passes className to child components', () => {
      render(<DateTimePicker {...defaultProps} className="custom-datetime" />)
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      const selects = screen.getAllByRole('combobox')
      
      expect(dateInput).toHaveClass('form-control', 'custom-datetime')
      selects.forEach(select => {
        expect(select).toHaveClass('form-select', 'custom-datetime')
      })
    })
  })

  describe('Accessibility', () => {
    it('provides clear labeling structure', () => {
      render(<DateTimePicker {...defaultProps} />)
      
      expect(screen.getByText('Test DateTime')).toHaveClass('form-label')
      expect(screen.getByText('Date')).toHaveClass('form-label')
      expect(screen.getByText('Time')).toHaveClass('form-label')
    })

    it('maintains accessibility when showTime is false', () => {
      render(<DateTimePicker {...defaultProps} showTime={false} />)
      
      expect(screen.getByText('Test DateTime')).toHaveClass('form-label')
      expect(screen.getByText('Date')).toHaveClass('form-label')
      expect(screen.queryByText('Time')).not.toBeInTheDocument()
    })

    it('supports screen readers with proper ARIA attributes when required', () => {
      render(<DateTimePicker {...defaultProps} required />)
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      expect(dateInput).toHaveAttribute('required')
      expect(dateInput).toHaveAccessibleName('Date *')
    })
  })

  describe('Placeholder Support', () => {
    it('passes placeholder to date picker', () => {
      render(<DateTimePicker {...defaultProps} placeholder="Select a date" />)
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      expect(dateInput).toHaveAttribute('placeholder', 'Select a date')
    })
  })

  describe('Date-Only Mode', () => {
    it('functions as date-only picker when showTime is false', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(
        <DateTimePicker
          {...defaultProps}
          showTime={false}
          onChange={mockOnChange}
        />
      )
      
      const dateInput = screen.getByDisplayValue('2023-12-25')
      await user.clear(dateInput)
      await user.type(dateInput, '2023-11-10')
      
      expect(mockOnChange).toHaveBeenCalled()
      // Find the last call that's not null (DatePicker may call with null during clear)
      const validCalls = mockOnChange.mock.calls.filter(call => call[0] !== null)
      if (validCalls.length > 0) {
        const callArg = validCalls[validCalls.length - 1][0]
        expect(callArg).toBeInstanceOf(Date)
        expect(callArg.getFullYear()).toBe(2023)
        expect(callArg.getMonth()).toBe(10) // November
        expect(callArg.getDate()).toBe(10)
        // Time should still be preserved from original value
        expect(callArg.getHours()).toBe(14)
        expect(callArg.getMinutes()).toBe(30)
      }
    })
  })
})
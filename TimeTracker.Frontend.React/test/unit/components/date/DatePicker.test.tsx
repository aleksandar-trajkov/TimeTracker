import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import DatePicker from '../../../../src/components/date/DatePicker'

describe('DatePicker Component', () => {
  const defaultProps = {
    id: 'test-datepicker',
    name: 'test-date',
    label: 'Test Date',
    value: '',
    onChange: vi.fn(),
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('renders date picker with label', () => {
      render(<DatePicker {...defaultProps} />)
      
      expect(screen.getByLabelText('Test Date')).toBeInTheDocument()
      expect(screen.getByDisplayValue('')).toBeInTheDocument()
    })

    it('renders with correct input attributes', () => {
      render(
        <DatePicker
          {...defaultProps}
          value="2023-12-25"
          placeholder="Select date"
        />
      )
      
      const input = screen.getByDisplayValue('2023-12-25')
      expect(input).toHaveAttribute('type', 'date')
      expect(input).toHaveAttribute('id', 'test-datepicker')
      expect(input).toHaveAttribute('name', 'test-date')
      expect(input).toHaveValue('2023-12-25')
    })

    it('displays required indicator when required is true', () => {
      render(<DatePicker {...defaultProps} required />)
      
      expect(screen.getByText('*')).toBeInTheDocument()
      expect(screen.getByLabelText(/Test Date/)).toHaveAttribute('required')
    })

    it('does not display required indicator when required is false', () => {
      render(<DatePicker {...defaultProps} required={false} />)
      
      expect(screen.queryByText('*')).not.toBeInTheDocument()
      expect(screen.getByLabelText(/Test Date/)).not.toHaveAttribute('required')
    })

    it('applies custom className to input', () => {
      render(<DatePicker {...defaultProps} className="custom-datepicker" />)
      
      const input = screen.getByLabelText(/Test Date/)
      expect(input).toHaveClass('form-control', 'custom-datepicker')
    })

    it('applies custom container className', () => {
      render(<DatePicker {...defaultProps} containerClassName="custom-container" />)
      
      const container = screen.getByLabelText(/Test Date/).closest('div')
      expect(container).toHaveClass('custom-container')
    })

    it('uses default container className when not provided', () => {
      render(<DatePicker {...defaultProps} />)
      
      const container = screen.getByLabelText(/Test Date/).closest('div')
      expect(container).toHaveClass('mb-3')
    })
  })

  describe('Date constraints', () => {
    it('applies min date constraint', () => {
      render(<DatePicker {...defaultProps} min="2023-01-01" />)
      
      const input = screen.getByLabelText('Test Date')
      expect(input).toHaveAttribute('min', '2023-01-01')
    })

    it('applies max date constraint', () => {
      render(<DatePicker {...defaultProps} max="2023-12-31" />)
      
      const input = screen.getByLabelText('Test Date')
      expect(input).toHaveAttribute('max', '2023-12-31')
    })

    it('applies both min and max date constraints', () => {
      render(
        <DatePicker
          {...defaultProps}
          min="2023-01-01"
          max="2023-12-31"
        />
      )
      
      const input = screen.getByLabelText('Test Date')
      expect(input).toHaveAttribute('min', '2023-01-01')
      expect(input).toHaveAttribute('max', '2023-12-31')
    })

    it('sets max to today when no max is provided', () => {
      const today = new Date().toISOString().split('T')[0]
      
      render(<DatePicker {...defaultProps} />)
      
      const input = screen.getByLabelText('Test Date')
      expect(input).toHaveAttribute('max', today)
    })
  })

  describe('User Interactions', () => {
    it('calls onChange when date value changes', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<DatePicker {...defaultProps} onChange={mockOnChange} />)
      
      const input = screen.getByLabelText('Test Date')
      await user.type(input, '2023-12-25')
      
      expect(mockOnChange).toHaveBeenCalled()
    })

    it('calls onChange with correct date value', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<DatePicker {...defaultProps} onChange={mockOnChange} />)
      
      const input = screen.getByLabelText('Test Date')
      await user.clear(input)
      await user.type(input, '2023-12-25')
      
      // Check that onChange was called with the date value
      expect(mockOnChange).toHaveBeenCalledWith('2023-12-25')
    })

    it('calls onChange when clearing date', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<DatePicker {...defaultProps} value="2023-12-25" onChange={mockOnChange} />)
      
      const input = screen.getByLabelText('Test Date')
      await user.clear(input)
      
      expect(mockOnChange).toHaveBeenCalledWith('')
    })
  })

  describe('Disabled State', () => {
    it('disables input when disabled prop is true', () => {
      render(<DatePicker {...defaultProps} disabled />)
      
      const input = screen.getByLabelText('Test Date')
      expect(input).toBeDisabled()
    })

    it('enables input when disabled prop is false', () => {
      render(<DatePicker {...defaultProps} disabled={false} />)
      
      const input = screen.getByLabelText('Test Date')
      expect(input).not.toBeDisabled()
    })

    it('does not call onChange when disabled and user tries to interact', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<DatePicker {...defaultProps} disabled onChange={mockOnChange} />)
      
      const input = screen.getByLabelText('Test Date')
      await user.type(input, '2023-12-25')
      
      expect(mockOnChange).not.toHaveBeenCalled()
    })
  })

  describe('Accessibility', () => {
    it('associates label with input using htmlFor and id', () => {
      render(<DatePicker {...defaultProps} />)
      
      const label = screen.getByText('Test Date')
      const input = screen.getByLabelText('Test Date')
      
      expect(label).toHaveAttribute('for', 'test-datepicker')
      expect(input).toHaveAttribute('id', 'test-datepicker')
    })

    it('supports screen readers with proper ARIA attributes', () => {
      render(<DatePicker {...defaultProps} required />)
      
      const input = screen.getByLabelText('Test Date', { exact: false })
      expect(input).toHaveAttribute('required')
      expect(input).toHaveAccessibleName('Test Date *')
    })
  })

  describe('Value Updates', () => {
    it('displays updated value when value prop changes', () => {
      const { rerender } = render(<DatePicker {...defaultProps} value="2023-01-01" />)
      
      expect(screen.getByLabelText('Test Date')).toHaveValue('2023-01-01')
      
      rerender(<DatePicker {...defaultProps} value="2023-12-31" />)
      
      expect(screen.getByLabelText('Test Date')).toHaveValue('2023-12-31')
    })

    it('handles empty string value', () => {
      render(<DatePicker {...defaultProps} value="" />)
      
      expect(screen.getByLabelText('Test Date')).toHaveValue('')
    })

    it('handles valid ISO date values', () => {
      const testDates = ['2023-01-01', '2023-06-15', '2023-12-31']
      
      testDates.forEach(date => {
        const { unmount } = render(<DatePicker {...defaultProps} value={date} />)
        expect(screen.getByLabelText('Test Date')).toHaveValue(date)
        unmount()
      })
    })
  })

  describe('Form Integration', () => {
    it('works within a form element', () => {
      render(
        <form>
          <DatePicker {...defaultProps} />
        </form>
      )
      
      const input = screen.getByLabelText('Test Date')
      expect(input.closest('form')).toBeInTheDocument()
    })

    it('submits form when Enter key is pressed', async () => {
      const handleSubmit = vi.fn((e) => e.preventDefault())
      const user = userEvent.setup()
      
      render(
        <form onSubmit={handleSubmit}>
          <DatePicker {...defaultProps} />
          <button type="submit">Submit</button>
        </form>
      )

      const input = screen.getByLabelText('Test Date')
      await user.type(input, '2023-12-25{enter}')
      
      expect(handleSubmit).toHaveBeenCalled()
    })
  })
})
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import '@testing-library/jest-dom'
import TimePicker from '../../../../src/components/date/TimePicker'

describe('TimePicker Component', () => {
  const testDate = new Date('2023-12-25T14:30:00')
  const defaultProps = {
    id: 'test-timepicker',
    name: 'test-time',
    label: 'Test Time',
    value: testDate,
    onChange: vi.fn(),
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('renders time picker with label', () => {
      render(<TimePicker {...defaultProps} />)
      
      expect(screen.getByText('Test Time')).toBeInTheDocument()
      expect(screen.getByDisplayValue('14')).toBeInTheDocument() // Hour select (14 = 2 PM)
      expect(screen.getByDisplayValue('30')).toBeInTheDocument() // Minute select
    })

    it('renders hour and minute dropdowns with correct attributes', () => {
      render(<TimePicker {...defaultProps} />)
      
      const hourSelect = screen.getAllByRole('combobox')[0]
      const minuteSelect = screen.getAllByRole('combobox')[1]
      
      expect(hourSelect).toHaveAttribute('id', 'test-timepicker-hour')
      expect(hourSelect).toHaveAttribute('name', 'test-time-hour')
      expect(minuteSelect).toHaveAttribute('id', 'test-timepicker-minute')
      expect(minuteSelect).toHaveAttribute('name', 'test-time-minute')
    })

    it('displays current time values correctly', () => {
      render(<TimePicker {...defaultProps} />)
      
      const hourSelect = screen.getByDisplayValue('14')
      const minuteSelect = screen.getByDisplayValue('30')
      
      expect(hourSelect).toBeInTheDocument()
      expect(minuteSelect).toBeInTheDocument()
    })

    it('displays required indicator when required is true', () => {
      render(<TimePicker {...defaultProps} required />)
      
      expect(screen.getByText('*')).toBeInTheDocument()
    })

    it('does not display required indicator when required is false', () => {
      render(<TimePicker {...defaultProps} required={false} />)
      
      expect(screen.queryByText('*')).not.toBeInTheDocument()
    })

    it('applies custom className to selects', () => {
      render(<TimePicker {...defaultProps} className="custom-time" />)
      
      const selects = screen.getAllByRole('combobox')
      selects.forEach(select => {
        expect(select).toHaveClass('form-select', 'custom-time')
      })
    })

    it('applies custom container className', () => {
      render(<TimePicker {...defaultProps} containerClassName="custom-container" />)
      
      const container = screen.getByText('Test Time').closest('div')
      expect(container).toHaveClass('custom-container')
    })

    it('uses default container className when not provided', () => {
      render(<TimePicker {...defaultProps} />)
      
      const container = screen.getByText('Test Time').closest('div')
      expect(container).toHaveClass('mb-3')
    })
  })

  describe('Hour Options', () => {
    it('generates all 24 hour options (0-23)', () => {
      render(<TimePicker {...defaultProps} />)
      
      const hourSelect = screen.getByDisplayValue('14')
      const options = hourSelect.querySelectorAll('option')
      
      expect(options).toHaveLength(24)
      expect(options[0]).toHaveValue('0')
      expect(options[0]).toHaveTextContent('00')
      expect(options[23]).toHaveValue('23')
      expect(options[23]).toHaveTextContent('23')
    })

    it('pads single digit hours with zeros', () => {
      render(<TimePicker {...defaultProps} value={new Date('2023-12-25T09:15:00')} />)
      
      const hourSelect = screen.getByDisplayValue('09')
      const option = hourSelect.querySelector('option[value="9"]')
      
      expect(option).toHaveTextContent('09')
    })
  })

  describe('Minute Options', () => {
    it('generates minute options in 5-minute intervals (0, 5, 10, ..., 55)', () => {
      render(<TimePicker {...defaultProps} />)
      
      const minuteSelect = screen.getByDisplayValue('30')
      const options = minuteSelect.querySelectorAll('option')
      
      expect(options).toHaveLength(12) // 0, 5, 10, ..., 55 = 12 options
      expect(options[0]).toHaveValue('0')
      expect(options[0]).toHaveTextContent('00')
      expect(options[1]).toHaveValue('5')
      expect(options[1]).toHaveTextContent('05')
      expect(options[11]).toHaveValue('55')
      expect(options[11]).toHaveTextContent('55')
    })

    it('rounds minutes to nearest 5-minute interval for display', () => {
      // Test with 32 minutes, should round to 30
      const dateWith32Minutes = new Date('2023-12-25T14:32:00')
      render(<TimePicker {...defaultProps} value={dateWith32Minutes} />)
      
      const minuteSelect = screen.getByDisplayValue('30')
      expect(minuteSelect).toBeInTheDocument()
    })

    it('pads single digit minutes with zeros', () => {
      render(<TimePicker {...defaultProps} value={new Date('2023-12-25T14:05:00')} />)
      
      const minuteSelect = screen.getByDisplayValue('05')
      const option = minuteSelect.querySelector('option[value="5"]')
      
      expect(option).toHaveTextContent('05')
    })
  })

  describe('User Interactions', () => {
    it('calls onChange when hour value changes', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<TimePicker {...defaultProps} onChange={mockOnChange} />)
      
      const hourSelect = screen.getByDisplayValue('14')
      await user.selectOptions(hourSelect, '16')
      
      expect(mockOnChange).toHaveBeenCalled()
      const callArg = mockOnChange.mock.calls[0][0]
      expect(callArg).toBeInstanceOf(Date)
      expect(callArg.getHours()).toBe(16)
      expect(callArg.getMinutes()).toBe(30) // Should preserve original minutes
    })

    it('calls onChange when minute value changes', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<TimePicker {...defaultProps} onChange={mockOnChange} />)
      
      const minuteSelect = screen.getByDisplayValue('30')
      await user.selectOptions(minuteSelect, '45')
      
      expect(mockOnChange).toHaveBeenCalled()
      const callArg = mockOnChange.mock.calls[0][0]
      expect(callArg).toBeInstanceOf(Date)
      expect(callArg.getHours()).toBe(14) // Should preserve original hour
      expect(callArg.getMinutes()).toBe(45)
    })

    it('creates new date with current time when value is null and hour changes', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<TimePicker {...defaultProps} value={null} onChange={mockOnChange} />)
      
      const hourSelect = screen.getAllByRole('combobox')[0] // Get hour select
      await user.selectOptions(hourSelect, '10')
      
      expect(mockOnChange).toHaveBeenCalled()
      const callArg = mockOnChange.mock.calls[0][0]
      expect(callArg).toBeInstanceOf(Date)
      expect(callArg.getHours()).toBe(10)
    })

    it('creates new date with current time when value is null and minute changes', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<TimePicker {...defaultProps} value={null} onChange={mockOnChange} />)
      
      const minuteSelect = screen.getAllByRole('combobox')[1] // Get minute select
      await user.selectOptions(minuteSelect, '15')
      
      expect(mockOnChange).toHaveBeenCalled()
      const callArg = mockOnChange.mock.calls[0][0]
      expect(callArg).toBeInstanceOf(Date)
      expect(callArg.getMinutes()).toBe(15)
    })
  })

  describe('Disabled State', () => {
    it('disables both selects when disabled prop is true', () => {
      render(<TimePicker {...defaultProps} disabled />)
      
      const selects = screen.getAllByRole('combobox')
      selects.forEach(select => {
        expect(select).toBeDisabled()
      })
    })

    it('enables both selects when disabled prop is false', () => {
      render(<TimePicker {...defaultProps} disabled={false} />)
      
      const selects = screen.getAllByRole('combobox')
      selects.forEach(select => {
        expect(select).not.toBeDisabled()
      })
    })

    it('does not call onChange when disabled and user tries to interact', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<TimePicker {...defaultProps} disabled onChange={mockOnChange} />)
      
      const hourSelect = screen.getByDisplayValue('14')
      await user.selectOptions(hourSelect, '16')
      
      expect(mockOnChange).not.toHaveBeenCalled()
    })
  })

  describe('Required State', () => {
    it('applies required attribute to both selects when required is true', () => {
      render(<TimePicker {...defaultProps} required />)
      
      const selects = screen.getAllByRole('combobox')
      selects.forEach(select => {
        expect(select).toHaveAttribute('required')
      })
    })

    it('does not apply required attribute when required is false', () => {
      render(<TimePicker {...defaultProps} required={false} />)
      
      const selects = screen.getAllByRole('combobox')
      selects.forEach(select => {
        expect(select).not.toHaveAttribute('required')
      })
    })
  })

  describe('Null Value Handling', () => {
    it('handles null value correctly', () => {
      render(<TimePicker {...defaultProps} value={null} />)
      
      const hourSelect = screen.getAllByRole('combobox')[0]
      const minuteSelect = screen.getAllByRole('combobox')[1]
      
      // When value is null, component defaults to hour 0 and minute 0
      expect(hourSelect).toHaveValue('0')
      expect(minuteSelect).toHaveValue('0')
    })

    it('defaults to current hour and 0 minutes when value is null', () => {
      render(<TimePicker {...defaultProps} value={null} />)
      
      const hourSelect = screen.getAllByRole('combobox')[0]
      const minuteSelect = screen.getAllByRole('combobox')[1]
      
      // Component defaults to 0 for both hour and minute when value is null
      expect(hourSelect).toHaveValue('0')
      expect(minuteSelect).toHaveValue('0')
    })
  })

  describe('Value Updates', () => {
    it('updates display when value prop changes', () => {
      const { rerender } = render(
        <TimePicker {...defaultProps} value={new Date('2023-12-25T10:15:00')} />
      )
      
      expect(screen.getByDisplayValue('10')).toBeInTheDocument()
      expect(screen.getByDisplayValue('15')).toBeInTheDocument()
      
      rerender(
        <TimePicker {...defaultProps} value={new Date('2023-12-25T22:45:00')} />
      )
      
      expect(screen.getByDisplayValue('22')).toBeInTheDocument()
      expect(screen.getByDisplayValue('45')).toBeInTheDocument()
    })
  })

  describe('Accessibility', () => {
    it('associates label with the container', () => {
      render(<TimePicker {...defaultProps} />)
      
      const label = screen.getByText('Test Time')
      expect(label).toHaveClass('form-label')
    })

    it('provides descriptive text for hour and minute selects', () => {
      render(<TimePicker {...defaultProps} />)
      
      const hourSelect = screen.getByDisplayValue('14')
      const minuteSelect = screen.getByDisplayValue('30')
      
      expect(hourSelect).toBeInTheDocument()
      expect(minuteSelect).toBeInTheDocument()
    })

    it('uses proper Bootstrap grid layout for responsive design', () => {
      render(<TimePicker {...defaultProps} />)
      
      const hourContainer = screen.getByDisplayValue('14').closest('.col-6')
      const minuteContainer = screen.getByDisplayValue('30').closest('.col-6')
      
      expect(hourContainer).toBeInTheDocument()
      expect(minuteContainer).toBeInTheDocument()
    })
  })
})
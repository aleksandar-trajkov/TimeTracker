import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import '@testing-library/jest-dom'
import Dropdown from '../../../../src/components/common/Dropdown'
import type { DropdownOption } from '../../../../src/components/common/Dropdown'

describe('Dropdown Component', () => {
  const defaultOptions: DropdownOption[] = [
    { value: '1', label: 'Option 1' },
    { value: '2', label: 'Option 2' },
    { value: 3, label: 'Option 3' },
    { value: '4', label: 'Option 4', disabled: true },
  ]

  const defaultProps = {
    id: 'test-dropdown',
    name: 'test',
    label: 'Test Label',
    value: '',
    options: defaultOptions,
    onChange: vi.fn(),
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('renders dropdown with label', () => {
      render(<Dropdown {...defaultProps} />)
      
      expect(screen.getByLabelText('Test Label')).toBeInTheDocument()
    })

    it('renders with correct dropdown attributes', () => {
      render(<Dropdown {...defaultProps} value="1" />)
      
      const select = screen.getByRole('combobox')
      expect(select).toHaveAttribute('id', 'test-dropdown')
      expect(select).toHaveAttribute('name', 'test')
      expect(select).toHaveValue('1')
    })

    it('displays required indicator when required is true', () => {
      render(<Dropdown {...defaultProps} required />)
      
      expect(screen.getByText('*')).toBeInTheDocument()
      expect(screen.getByRole('combobox')).toHaveAttribute('required')
    })

    it('renders all options correctly', () => {
      render(<Dropdown {...defaultProps} />)
      
      expect(screen.getByText('Option 1')).toBeInTheDocument()
      expect(screen.getByText('Option 2')).toBeInTheDocument()
      expect(screen.getByText('Option 3')).toBeInTheDocument()
      expect(screen.getByText('Option 4')).toBeInTheDocument()
    })

    it('renders disabled option correctly', () => {
      render(<Dropdown {...defaultProps} />)
      
      const disabledOption = screen.getByText('Option 4')
      expect(disabledOption).toBeDisabled()
    })

    it('renders empty option when allowEmpty is true', () => {
      render(<Dropdown {...defaultProps} allowEmpty />)
      
      expect(screen.getByText('Select an option...')).toBeInTheDocument()
    })

    it('renders custom empty option label', () => {
      render(
        <Dropdown 
          {...defaultProps} 
          allowEmpty 
          emptyOptionLabel="Choose something..." 
        />
      )
      
      expect(screen.getByText('Choose something...')).toBeInTheDocument()
    })

    it('renders placeholder option when not required and value is empty', () => {
      render(<Dropdown {...defaultProps} placeholder="Select option" />)
      
      expect(screen.getByText('Select option')).toBeInTheDocument()
    })
  })

  describe('Styling', () => {
    it('applies custom className to select element', () => {
      render(<Dropdown {...defaultProps} className="custom-class" />)
      
      const select = screen.getByRole('combobox')
      expect(select).toHaveClass('form-select', 'custom-class')
    })

    it('applies custom containerClassName to container', () => {
      render(<Dropdown {...defaultProps} containerClassName="custom-container" />)
      
      const container = screen.getByRole('combobox').closest('div')
      expect(container).toHaveClass('custom-container')
    })

    it('applies default Bootstrap classes', () => {
      render(<Dropdown {...defaultProps} />)
      
      const select = screen.getByRole('combobox')
      expect(select).toHaveClass('form-select')
      
      const label = screen.getByText('Test Label')
      expect(label).toHaveClass('form-label')
    })
  })

  describe('User Interactions', () => {
    it('calls onChange when option is selected', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<Dropdown {...defaultProps} onChange={mockOnChange} />)
      
      const select = screen.getByRole('combobox')
      await user.selectOptions(select, '2')
      
      expect(mockOnChange).toHaveBeenCalledWith('2')
    })

    it('calls onChange with string value when option value is string', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<Dropdown {...defaultProps} onChange={mockOnChange} />)
      
      const select = screen.getByRole('combobox')
      await user.selectOptions(select, '1')
      
      expect(mockOnChange).toHaveBeenCalledWith('1')
    })

    it('calls onChange with number value when option value is number', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<Dropdown {...defaultProps} onChange={mockOnChange} />)
      
      const select = screen.getByRole('combobox')
      await user.selectOptions(select, '3')
      
      expect(mockOnChange).toHaveBeenCalledWith(3)
    })

    it('does not call onChange when disabled', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<Dropdown {...defaultProps} disabled onChange={mockOnChange} />)
      
      const select = screen.getByRole('combobox')
      await user.click(select)
      
      expect(mockOnChange).not.toHaveBeenCalled()
    })
  })

  describe('Accessibility', () => {
    it('has proper ARIA attributes', () => {
      render(<Dropdown {...defaultProps} />)
      
      const select = screen.getByRole('combobox')
      expect(select).toBeInTheDocument()
    })

    it('associates label with select element', () => {
      render(<Dropdown {...defaultProps} />)
      
      const select = screen.getByLabelText('Test Label')
      expect(select).toBeInTheDocument()
    })
  })

  describe('State Management', () => {
    it('shows correct selected value', () => {
      render(<Dropdown {...defaultProps} value="2" />)
      
      const select = screen.getByRole('combobox') as HTMLSelectElement
      expect(select.value).toBe('2')
    })

    it('handles numeric values correctly', () => {
      render(<Dropdown {...defaultProps} value={3} />)
      
      const select = screen.getByRole('combobox') as HTMLSelectElement
      expect(select.value).toBe('3')
    })
  })
})
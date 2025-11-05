import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import '@testing-library/jest-dom'
import Input from '../../../../src/components/common/Input'

describe('Input Component', () => {
  const defaultProps = {
    id: 'test-input',
    name: 'test',
    type: 'text' as const,
    label: 'Test Label',
    value: '',
    onChange: vi.fn(),
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('renders input with label', () => {
      render(<Input {...defaultProps} placeholder="Test placeholder" />)
      
      expect(screen.getByLabelText('Test Label')).toBeInTheDocument()
      expect(screen.getByPlaceholderText('Test placeholder')).toBeInTheDocument()
    })

    it('renders with correct input attributes', () => {
      render(
        <Input
          {...defaultProps}
          type="email"
          value="test@example.com"
          placeholder="Enter email"
        />
      )
      
      const input = screen.getByRole('textbox')
      expect(input).toHaveAttribute('type', 'email')
      expect(input).toHaveAttribute('id', 'test-input')
      expect(input).toHaveAttribute('name', 'test')
      expect(input).toHaveValue('test@example.com')
      expect(input).toHaveAttribute('placeholder', 'Enter email')
    })

    it('displays required indicator when required is true', () => {
      render(<Input {...defaultProps} required />)
      
      expect(screen.getByText('*')).toBeInTheDocument()
      expect(screen.getByRole('textbox')).toHaveAttribute('required')
    })

    it('does not display required indicator when required is false', () => {
      render(<Input {...defaultProps} required={false} />)
      
      expect(screen.queryByText('*')).not.toBeInTheDocument()
      expect(screen.getByRole('textbox')).not.toHaveAttribute('required')
    })

    it('applies custom className to input', () => {
      render(<Input {...defaultProps} className="custom-input" />)
      
      const input = screen.getByRole('textbox')
      expect(input).toHaveClass('form-control', 'custom-input')
    })

    it('applies custom container className', () => {
      render(<Input {...defaultProps} containerClassName="custom-container" />)
      
      const container = screen.getByRole('textbox').closest('div')
      expect(container).toHaveClass('custom-container')
    })

    it('uses default container className when not provided', () => {
      render(<Input {...defaultProps} />)
      
      const container = screen.getByRole('textbox').closest('div')
      expect(container).toHaveClass('mb-3')
    })
  })

  describe('Input Types', () => {
    it('renders password input correctly', () => {
      render(<Input {...defaultProps} type="password" />)
      
      const input = screen.getByLabelText('Test Label')
      expect(input).toHaveAttribute('type', 'password')
    })

    it('renders email input correctly', () => {
      render(<Input {...defaultProps} type="email" />)
      
      const input = screen.getByRole('textbox')
      expect(input).toHaveAttribute('type', 'email')
    })

    it('renders number input correctly', () => {
      render(<Input {...defaultProps} type="number" />)
      
      const input = screen.getByRole('spinbutton')
      expect(input).toHaveAttribute('type', 'number')
    })

    it('renders tel input correctly', () => {
      render(<Input {...defaultProps} type="tel" />)
      
      const input = screen.getByRole('textbox')
      expect(input).toHaveAttribute('type', 'tel')
    })

    it('renders url input correctly', () => {
      render(<Input {...defaultProps} type="url" />)
      
      const input = screen.getByRole('textbox')
      expect(input).toHaveAttribute('type', 'url')
    })
  })

  describe('User Interactions', () => {
    it('calls onChange when input value changes', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<Input {...defaultProps} onChange={mockOnChange} />)
      
      const input = screen.getByRole('textbox')
      await user.type(input, 'test value')
      
      expect(mockOnChange).toHaveBeenCalled()
      expect(mockOnChange).toHaveBeenCalledWith('t')
      expect(mockOnChange).toHaveBeenCalledWith('e')
      expect(mockOnChange).toHaveBeenCalledWith('s')
      expect(mockOnChange).toHaveBeenCalledWith('t')
    })

    it('calls onChange with correct value on single character input', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<Input {...defaultProps} onChange={mockOnChange} />)
      
      const input = screen.getByRole('textbox')
      await user.type(input, 'a')
      
      expect(mockOnChange).toHaveBeenCalledWith('a')
    })

    it('calls onChange when clearing input', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<Input {...defaultProps} value="initial" onChange={mockOnChange} />)
      
      const input = screen.getByRole('textbox')
      await user.clear(input)
      
      expect(mockOnChange).toHaveBeenCalledWith('')
    })

    it('handles paste operations', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<Input {...defaultProps} onChange={mockOnChange} />)
      
      const input = screen.getByRole('textbox')
      await user.click(input)
      await user.paste('pasted text')
      
      expect(mockOnChange).toHaveBeenCalled()
    })
  })

  describe('Disabled State', () => {
    it('disables input when disabled prop is true', () => {
      render(<Input {...defaultProps} disabled />)
      
      const input = screen.getByRole('textbox')
      expect(input).toBeDisabled()
    })

    it('enables input when disabled prop is false', () => {
      render(<Input {...defaultProps} disabled={false} />)
      
      const input = screen.getByRole('textbox')
      expect(input).not.toBeDisabled()
    })

    it('does not call onChange when disabled and user tries to type', async () => {
      const mockOnChange = vi.fn()
      const user = userEvent.setup()
      
      render(<Input {...defaultProps} disabled onChange={mockOnChange} />)
      
      const input = screen.getByRole('textbox')
      await user.type(input, 'test')
      
      expect(mockOnChange).not.toHaveBeenCalled()
    })
  })

  describe('Accessibility', () => {
    it('associates label with input using htmlFor and id', () => {
      render(<Input {...defaultProps} />)
      
      const label = screen.getByText('Test Label')
      const input = screen.getByRole('textbox')
      
      expect(label).toHaveAttribute('for', 'test-input')
      expect(input).toHaveAttribute('id', 'test-input')
    })

    it('supports screen readers with proper ARIA attributes', () => {
      render(<Input {...defaultProps} required />)
      
      const input = screen.getByRole('textbox')
      expect(input).toHaveAttribute('required')
    })
  })

  describe('Value Updates', () => {
    it('displays updated value when value prop changes', () => {
      const { rerender } = render(<Input {...defaultProps} value="initial" />)
      
      expect(screen.getByRole('textbox')).toHaveValue('initial')
      
      rerender(<Input {...defaultProps} value="updated" />)
      
      expect(screen.getByRole('textbox')).toHaveValue('updated')
    })

    it('handles empty string value', () => {
      render(<Input {...defaultProps} value="" />)
      
      expect(screen.getByRole('textbox')).toHaveValue('')
    })

    it('handles whitespace values', () => {
      render(<Input {...defaultProps} value="   " />)
      
      expect(screen.getByRole('textbox')).toHaveValue('   ')
    })
  })

  describe('Form Integration', () => {
    it('works within a form element', () => {
      render(
        <form>
          <Input {...defaultProps} />
        </form>
      )
      
      const input = screen.getByRole('textbox')
      expect(input.closest('form')).toBeInTheDocument()
    })

    it('submits form when Enter key is pressed', async () => {
      const handleSubmit = vi.fn((e) => e.preventDefault())
      const user = userEvent.setup()
      
      render(
        <form onSubmit={handleSubmit}>
          <Input {...defaultProps} />
          <button type="submit">Submit</button>
        </form>
      )
      
      const input = screen.getByRole('textbox')
      await user.type(input, 'test{enter}')
      
      expect(handleSubmit).toHaveBeenCalled()
    })
  })
})
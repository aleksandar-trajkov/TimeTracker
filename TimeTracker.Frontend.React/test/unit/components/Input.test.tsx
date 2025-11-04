import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi } from 'vitest'
import Input from '../../../src/components/common/Input'

describe('Input Component', () => {
  it('renders input with label', () => {
    const mockOnChange = vi.fn()
    
    render(
      <Input
        id="test-input"
        name="test"
        type="text"
        label="Test Label"
        value=""
        onChange={mockOnChange}
        placeholder="Test placeholder"
      />
    )
    
    expect(screen.getByLabelText('Test Label')).toBeInTheDocument()
    expect(screen.getByPlaceholderText('Test placeholder')).toBeInTheDocument()
  })

  it('calls onChange when input value changes', async () => {
    const mockOnChange = vi.fn()
    const user = userEvent.setup()
    
    render(
      <Input
        id="test-input"
        name="test"
        type="text"
        label="Test Label"
        value=""
        onChange={mockOnChange}
        placeholder="Test placeholder"
      />
    )
    
    const input = screen.getByRole('textbox')
    await user.type(input, 'test value')
    
    expect(mockOnChange).toHaveBeenCalled()
  })
})
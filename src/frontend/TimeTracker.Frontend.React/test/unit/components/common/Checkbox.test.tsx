import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi } from 'vitest'
import Checkbox from '../../../../src/components/common/Checkbox'

describe('Checkbox Component', () => {
  it('renders checkbox with label', () => {
    const mockOnChange = vi.fn()
    
    render(
      <Checkbox
        id="test-checkbox"
        label="Test Checkbox"
        checked={false}
        onChange={mockOnChange}
      />
    )
    
    expect(screen.getByRole('checkbox')).toBeInTheDocument()
    expect(screen.getByLabelText('Test Checkbox')).toBeInTheDocument()
  })

  it('is checked when checked prop is true', () => {
    const mockOnChange = vi.fn()
    
    render(
      <Checkbox
        id="checked-checkbox"
        label="Checked Checkbox"
        checked={true}
        onChange={mockOnChange}
      />
    )
    
    const checkbox = screen.getByRole('checkbox')
    expect(checkbox).toBeChecked()
  })

  it('is not checked when checked prop is false', () => {
    const mockOnChange = vi.fn()
    
    render(
      <Checkbox
        id="unchecked-checkbox"
        label="Unchecked Checkbox"
        checked={false}
        onChange={mockOnChange}
      />
    )
    
    const checkbox = screen.getByRole('checkbox')
    expect(checkbox).not.toBeChecked()
  })

  it('calls onChange with true when clicked while unchecked', async () => {
    const mockOnChange = vi.fn()
    const user = userEvent.setup()
    
    render(
      <Checkbox
        id="toggle-checkbox"
        label="Toggle Checkbox"
        checked={false}
        onChange={mockOnChange}
      />
    )
    
    await user.click(screen.getByRole('checkbox'))
    expect(mockOnChange).toHaveBeenCalledWith(true)
  })

  it('calls onChange with false when clicked while checked', async () => {
    const mockOnChange = vi.fn()
    const user = userEvent.setup()
    
    render(
      <Checkbox
        id="toggle-checkbox"
        label="Toggle Checkbox"
        checked={true}
        onChange={mockOnChange}
      />
    )
    
    await user.click(screen.getByRole('checkbox'))
    expect(mockOnChange).toHaveBeenCalledWith(false)
  })

  it('can be clicked via label', async () => {
    const mockOnChange = vi.fn()
    const user = userEvent.setup()
    
    render(
      <Checkbox
        id="label-click-checkbox"
        label="Click Label"
        checked={false}
        onChange={mockOnChange}
      />
    )
    
    await user.click(screen.getByText('Click Label'))
    expect(mockOnChange).toHaveBeenCalledWith(true)
  })

  it('is disabled when disabled prop is true', () => {
    const mockOnChange = vi.fn()
    
    render(
      <Checkbox
        id="disabled-checkbox"
        label="Disabled Checkbox"
        checked={false}
        onChange={mockOnChange}
        disabled={true}
      />
    )
    
    const checkbox = screen.getByRole('checkbox')
    expect(checkbox).toBeDisabled()
  })

  it('does not call onChange when disabled and clicked', async () => {
    const mockOnChange = vi.fn()
    const user = userEvent.setup()
    
    render(
      <Checkbox
        id="disabled-checkbox"
        label="Disabled Checkbox"
        checked={false}
        onChange={mockOnChange}
        disabled={true}
      />
    )
    
    await user.click(screen.getByRole('checkbox'))
    expect(mockOnChange).not.toHaveBeenCalled()
  })

  it('applies custom className to checkbox input', () => {
    const mockOnChange = vi.fn()
    
    render(
      <Checkbox
        id="custom-class-checkbox"
        label="Custom Class"
        checked={false}
        onChange={mockOnChange}
        className="custom-checkbox-class"
      />
    )
    
    const checkbox = screen.getByRole('checkbox')
    expect(checkbox).toHaveClass('form-check-input', 'custom-checkbox-class')
  })

  it('applies custom containerClassName to wrapper div', () => {
    const mockOnChange = vi.fn()
    
    render(
      <Checkbox
        id="custom-container-checkbox"
        label="Custom Container"
        checked={false}
        onChange={mockOnChange}
        containerClassName="custom-container-class"
      />
    )
    
    const container = screen.getByRole('checkbox').closest('div')
    expect(container).toHaveClass('custom-container-class')
  })

  it('uses default containerClassName when not provided', () => {
    const mockOnChange = vi.fn()
    
    render(
      <Checkbox
        id="default-container-checkbox"
        label="Default Container"
        checked={false}
        onChange={mockOnChange}
      />
    )
    
    const container = screen.getByRole('checkbox').closest('div')
    expect(container).toHaveClass('mb-3', 'form-check')
  })

  it('applies name attribute when provided', () => {
    const mockOnChange = vi.fn()
    
    render(
      <Checkbox
        id="named-checkbox"
        name="test-name"
        label="Named Checkbox"
        checked={false}
        onChange={mockOnChange}
      />
    )
    
    const checkbox = screen.getByRole('checkbox')
    expect(checkbox).toHaveAttribute('name', 'test-name')
  })

  it('has correct id and htmlFor association', () => {
    const mockOnChange = vi.fn()
    
    render(
      <Checkbox
        id="associated-checkbox"
        label="Associated Label"
        checked={false}
        onChange={mockOnChange}
      />
    )
    
    const checkbox = screen.getByRole('checkbox')
    const label = screen.getByText('Associated Label')
    
    expect(checkbox).toHaveAttribute('id', 'associated-checkbox')
    expect(label).toHaveAttribute('for', 'associated-checkbox')
  })

  it('maintains checked state correctly during interactions', async () => {
    const mockOnChange = vi.fn()
    const user = userEvent.setup()
    
    const { rerender } = render(
      <Checkbox
        id="stateful-checkbox"
        label="Stateful Checkbox"
        checked={false}
        onChange={mockOnChange}
      />
    )
    
    const checkbox = screen.getByRole('checkbox')
    expect(checkbox).not.toBeChecked()
    
    await user.click(checkbox)
    expect(mockOnChange).toHaveBeenCalledWith(true)
    
    // Simulate parent component updating the checked state
    rerender(
      <Checkbox
        id="stateful-checkbox"
        label="Stateful Checkbox"
        checked={true}
        onChange={mockOnChange}
      />
    )
    
    expect(screen.getByRole('checkbox')).toBeChecked()
  })
})
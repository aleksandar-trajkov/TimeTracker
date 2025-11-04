import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi } from 'vitest'
import Button from '../../../src/components/common/Button'

describe('Button Component', () => {
  it('renders button with children text', () => {
    render(<Button>Click me</Button>)
    
    expect(screen.getByRole('button', { name: 'Click me' })).toBeInTheDocument()
  })

  it('applies default props correctly', () => {
    render(<Button>Default Button</Button>)
    
    const button = screen.getByRole('button')
    expect(button).toHaveAttribute('type', 'button')
    expect(button).toHaveClass('btn', 'btn-primary')
    expect(button).not.toBeDisabled()
  })

  it('applies custom variant class', () => {
    render(<Button variant="success">Success Button</Button>)
    
    const button = screen.getByRole('button')
    expect(button).toHaveClass('btn-success')
  })

  it('applies size class when provided', () => {
    render(<Button size="lg">Large Button</Button>)
    
    const button = screen.getByRole('button')
    expect(button).toHaveClass('btn-lg')
  })

  it('applies full width class when fullWidth is true', () => {
    render(<Button fullWidth>Full Width Button</Button>)
    
    const button = screen.getByRole('button')
    expect(button).toHaveClass('w-100')
  })

  it('applies custom className', () => {
    render(<Button className="custom-class">Custom Button</Button>)
    
    const button = screen.getByRole('button')
    expect(button).toHaveClass('custom-class')
  })

  it('calls onClick when clicked', async () => {
    const mockClick = vi.fn()
    const user = userEvent.setup()
    
    render(<Button onClick={mockClick}>Clickable Button</Button>)
    
    await user.click(screen.getByRole('button'))
    expect(mockClick).toHaveBeenCalledOnce()
  })

  it('is disabled when disabled prop is true', () => {
    const mockClick = vi.fn()
    
    render(<Button disabled onClick={mockClick}>Disabled Button</Button>)
    
    const button = screen.getByRole('button')
    expect(button).toBeDisabled()
  })

  it('does not call onClick when disabled', async () => {
    const mockClick = vi.fn()
    const user = userEvent.setup()
    
    render(<Button disabled onClick={mockClick}>Disabled Button</Button>)
    
    await user.click(screen.getByRole('button'))
    expect(mockClick).not.toHaveBeenCalled()
  })

  it('shows loading state correctly', () => {
    render(
      <Button loading loadingText="Processing...">
        Submit
      </Button>
    )
    
    const button = screen.getByRole('button')
    expect(button).toBeDisabled()
    expect(screen.getByText('Processing...')).toBeInTheDocument()
    expect(screen.queryByText('Submit')).not.toBeInTheDocument()
  })

  it('uses default loading text when loading but no loadingText provided', () => {
    render(<Button loading>Submit</Button>)
    
    expect(screen.getByText('Loading...')).toBeInTheDocument()
  })

  it('applies correct type attribute', () => {
    render(<Button type="submit">Submit Button</Button>)
    
    const button = screen.getByRole('button')
    expect(button).toHaveAttribute('type', 'submit')
  })

  it('does not call onClick when loading', async () => {
    const mockClick = vi.fn()
    const user = userEvent.setup()
    
    render(<Button loading onClick={mockClick}>Loading Button</Button>)
    
    await user.click(screen.getByRole('button'))
    expect(mockClick).not.toHaveBeenCalled()
  })

  it('renders with multiple classes correctly', () => {
    render(
      <Button 
        variant="outline-primary" 
        size="sm" 
        fullWidth 
        className="extra-class"
      >
        Multi-class Button
      </Button>
    )
    
    const button = screen.getByRole('button')
    expect(button).toHaveClass(
      'btn',
      'btn-outline-primary',
      'btn-sm',
      'w-100',
      'extra-class'
    )
  })
})
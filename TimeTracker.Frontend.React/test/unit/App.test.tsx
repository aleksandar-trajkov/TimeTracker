import { render, screen } from '@testing-library/react'
import { describe, it, expect, vi } from 'vitest'
import { BrowserRouter } from 'react-router-dom'
import App from '../../src/App'

// Mock the components that might cause issues during testing
vi.mock('../modules/auth/SignIn', () => ({
  default: () => <div>SignIn Component</div>
}))

describe('App', () => {
  it('renders login when starting', () => {
    render(
      <BrowserRouter>
        <App />
      </BrowserRouter>
    )
    expect(screen.getByRole('heading', { name: /Sign In/i })).toBeInTheDocument()
  })
})
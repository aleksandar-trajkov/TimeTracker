import { render, screen, waitFor } from '@testing-library/react'
import { describe, it, expect, vi } from 'vitest'
import App from '../../src/App'

// Mock the lazy-loaded components
vi.mock('../../src/modules/auth/SignIn', () => ({
  default: () => <div><h1>Sign In</h1></div>
}))

vi.mock('../../src/modules/timeEntries/TimeEntriesModule', () => ({
  default: () => <div>TimeEntries Module</div>
}))

vi.mock('../../src/modules/categories/CategoriesModule', () => ({
  default: () => <div>Categories Module</div>
}))

vi.mock('../../src/modules/settings/SettingsModule', () => ({
  default: () => <div>Settings Module</div>
}))

vi.mock('../../src/modules/reports/ReportsModule', () => ({
  default: () => <div>Reports Module</div>
}))

describe('App', () => {
  it('shows loading state initially', () => {
    render(<App />)
    
    // Should show loading component initially while lazy components load
    expect(screen.getByText('Loading...')).toBeInTheDocument()
  })

  it('renders login when starting', async () => {
    render(<App />)
    
    // Wait for the lazy-loaded SignIn component to load
    await waitFor(() => {
      expect(screen.getByRole('heading', { name: /Sign In/i })).toBeInTheDocument()
    })
  })
})
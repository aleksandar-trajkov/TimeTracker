import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import { MemoryRouter } from 'react-router-dom'
import '@testing-library/jest-dom'
import App from '../../src/App'
import React from 'react'

// Mock the lazy-loaded components
vi.mock('../../src/modules/auth/SignIn', () => ({
  default: ({ setIsSignedIn }: { setIsSignedIn: (value: boolean) => void }) => (
    <div>
      <h1>Sign In</h1>
      <button onClick={() => setIsSignedIn(true)} data-testid="signin-button">
        Sign In Button
      </button>
    </div>
  )
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
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Unauthenticated state', () => {
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

  describe('Authentication flow', () => {
    it('transitions from sign-in to TimeEntries module when sign-in button is clicked', async () => {
      const user = userEvent.setup()

      render(
        <MemoryRouter initialEntries={['/']}>
          <App />
        </MemoryRouter>)
      
      // Wait for SignIn component to load
      await waitFor(() => {
        expect(screen.getByRole('heading', { name: /Sign In/i })).toBeInTheDocument()
      })
      
      // Verify sign-in button is present
      const signInButton = screen.getByTestId('signin-button')
      expect(signInButton).toBeInTheDocument()
      
      // Click the sign-in button to trigger authentication
      await user.click(signInButton)
      
      // Wait for the authenticated state to show TimeEntries module
      await waitFor(() => {
        expect(screen.getByText('TimeEntries Module')).toBeInTheDocument()
      }, { timeout: 3000 })
      
      // Verify SignIn component is no longer shown
      expect(screen.queryByRole('heading', { name: /Sign In/i })).not.toBeInTheDocument()
    })
  })

  describe('Route-based module loading', () => {
    // Helper function to render authenticated app at a specific route
    const renderAuthenticatedApp = (route: string) => {
      // Mock useState to return authenticated state
      const mockUseState = vi.spyOn(React, 'useState')
      mockUseState.mockReturnValue([true, vi.fn()]) // signedIn = true
      
      const result = render(
        <MemoryRouter initialEntries={[route]}>
          <App />
        </MemoryRouter>
      )
      
      return { ...result, mockUseState }
    }

    it('loads TimeEntries module on default route (/)', async () => {
      const { mockUseState } = renderAuthenticatedApp('/')
      
      await waitFor(() => {
        expect(screen.getByText('TimeEntries Module')).toBeInTheDocument()
      }, { timeout: 3000 })

      mockUseState.mockRestore()
    })

    it('loads Categories module on /categories route', async () => {
      const { mockUseState } = renderAuthenticatedApp('/categories')
      
      await waitFor(() => {
        expect(screen.getByText('Categories Module')).toBeInTheDocument()
      }, { timeout: 3000 })

      mockUseState.mockRestore()
    })

    it('loads Settings module on /settings route', async () => {
      const { mockUseState } = renderAuthenticatedApp('/settings')
      
      await waitFor(() => {
        expect(screen.getByText('Settings Module')).toBeInTheDocument()
      }, { timeout: 3000 })

      mockUseState.mockRestore()
    })

    it('loads Reports module on /reports route', async () => {
      const { mockUseState } = renderAuthenticatedApp('/reports')
      
      await waitFor(() => {
        expect(screen.getByText('Reports Module')).toBeInTheDocument()
      }, { timeout: 3000 })

      mockUseState.mockRestore()
    })

    it('loads TimeEntries module as fallback for unknown routes', async () => {
      const { mockUseState } = renderAuthenticatedApp('/unknown-route')
      
      await waitFor(() => {
        expect(screen.getByText('TimeEntries Module')).toBeInTheDocument()
      }, { timeout: 3000 })

      mockUseState.mockRestore()
    })
  })

  describe('Module loading with suspense', () => {
    it('handles route changes correctly via navigation links', async () => {
      const user = userEvent.setup()
      const mockUseState = vi.spyOn(React, 'useState')
      mockUseState.mockReturnValue([true, vi.fn()])
      
      render(
        <MemoryRouter initialEntries={['/']}>
          <App />
        </MemoryRouter>
      )
      
      // Wait for initial TimeEntries module to load (default route)
      await waitFor(() => {
        expect(screen.getByText('TimeEntries Module')).toBeInTheDocument()
      })
      
      // Navigate to Categories by clicking the link
      const categoriesLink = screen.getByRole('link', { name: /Categories/i })
      await user.click(categoriesLink)
      
      // Wait for Categories module to load
      await waitFor(() => {
        expect(screen.getByText('Categories Module')).toBeInTheDocument()
      })
      
      // Navigate to Settings by clicking the link
      const settingsLink = screen.getByRole('link', { name: /Settings/i })
      await user.click(settingsLink)
      
      // Wait for Settings module to load
      await waitFor(() => {
        expect(screen.getByText('Settings Module')).toBeInTheDocument()
      })
      
      // Navigate to Reports by clicking the link
      const reportsLink = screen.getByRole('link', { name: /Reports/i })
      await user.click(reportsLink)
      
      // Wait for Reports module to load  
      await waitFor(() => {
        expect(screen.getByText('Reports Module')).toBeInTheDocument()
      })
      
      // Navigate back to Home by clicking the link
      const homeLink = screen.getByRole('link', { name: /Home/i })
      await user.click(homeLink)
      
      // Wait for TimeEntries module to load again
      await waitFor(() => {
        expect(screen.getByText('TimeEntries Module')).toBeInTheDocument()
      })
      
      mockUseState.mockRestore()
    })
  })
})
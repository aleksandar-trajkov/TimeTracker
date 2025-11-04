# Testing Configuration

This project is configured with **Vitest** for unit testing and **@testing-library/react** for component testing.

## Setup

### Dependencies
- `vitest` - Fast unit test framework
- `@testing-library/react` - React testing utilities
- `@testing-library/jest-dom` - Custom Jest matchers
- `@testing-library/user-event` - User interaction simulation
- `@vitest/coverage-v8` - Coverage reporting with V8
- `happy-dom` - Lightweight DOM implementation for testing

### Configuration
The test configuration is defined in `vite.config.ts`:

```typescript
test: {
  globals: true,                    // Enable global test APIs (describe, it, expect)
  environment: 'happy-dom',         // DOM environment for React components
  setupFiles: ['./src/test/setup.ts'], // Test setup file
  css: true,                        // Enable CSS processing in tests
  coverage: {
    provider: 'v8',                 // Use V8 coverage provider
    reporter: ['text', 'json', 'html'], // Coverage report formats
    exclude: [                      // Files to exclude from coverage
      'node_modules/',
      'dist/',
      'src/test/',
      '**/*.d.ts',
      '**/*.config.{js,ts}',
      '**/vite-env.d.ts',
      'src/main.tsx'
    ],
    thresholds: {                   // Coverage thresholds
      global: {
        branches: 80,
        functions: 80,
        lines: 80,
        statements: 80
      }
    }
  }
}
```

## Available Scripts

- `npm test` - Run tests in watch mode
- `npm run test:ui` - Run tests with UI interface
- `npm run test:coverage` - Run tests with coverage report

## Test Structure

Tests are organized in the `src/test/` directory:
```
src/test/
├── setup.ts              # Test environment setup
├── App.test.tsx          # App component tests
└── components/           # Component-specific tests
    └── Input.test.tsx    # Input component tests
```

## Writing Tests

### Basic Component Test
```typescript
import { render, screen } from '@testing-library/react'
import { describe, it, expect } from 'vitest'
import MyComponent from '../MyComponent'

describe('MyComponent', () => {
  it('renders correctly', () => {
    render(<MyComponent />)
    expect(screen.getByText('Expected Text')).toBeInTheDocument()
  })
})
```

### Testing User Interactions
```typescript
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi } from 'vitest'
import MyButton from '../MyButton'

describe('MyButton', () => {
  it('calls onClick when clicked', async () => {
    const mockClick = vi.fn()
    const user = userEvent.setup()
    
    render(<MyButton onClick={mockClick}>Click me</MyButton>)
    
    await user.click(screen.getByRole('button'))
    expect(mockClick).toHaveBeenCalledOnce()
  })
})
```

### Mocking Modules
```typescript
import { vi } from 'vitest'

// Mock a module
vi.mock('../api/auth', () => ({
  signIn: vi.fn(),
  signOut: vi.fn()
}))
```

## Coverage Reports

Coverage reports are generated in multiple formats:
- **Text**: Console output during test run
- **HTML**: Interactive coverage report in `coverage/` directory
- **JSON**: Machine-readable coverage data

## Best Practices

1. **Test Structure**: Use the AAA pattern (Arrange, Act, Assert)
2. **Test Names**: Describe what the test does in plain language
3. **Mock External Dependencies**: Use `vi.mock()` for external services
4. **Test User Behavior**: Focus on what users do, not implementation details
5. **Coverage Goals**: Aim for 80%+ coverage, but prioritize critical paths

## Troubleshooting

### Common Issues
- **Module not found**: Check import paths and ensure files exist
- **DOM not available**: Ensure `environment: 'happy-dom'` is set
- **Async tests failing**: Use `await` with user interactions and API calls
- **Coverage too low**: Add tests for uncovered branches and functions
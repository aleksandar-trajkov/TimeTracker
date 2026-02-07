import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import 'bootstrap/dist/css/bootstrap.min.css'
import './styles/style.scss'
import App from './src/App.tsx'
import { BrowserRouter } from 'react-router-dom'

// Set dark theme globally
document.documentElement.setAttribute('data-bs-theme', 'dark');

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
      <App />
    </BrowserRouter>
  </StrictMode>,
)

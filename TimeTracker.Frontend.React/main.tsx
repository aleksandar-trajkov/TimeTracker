import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './styles/style.scss'
import App from './src/App.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)

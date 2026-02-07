/// <reference types="vitest" />
import { defineConfig } from 'vitest/config'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  server: {
    port: 9621,
  },
  define: {
    BUILD_DATE: JSON.stringify(new Date().valueOf()),
  },
  plugins: [
    react()
  ],
  build: {
    rollupOptions: {
      output: {
        entryFileNames: `assets/script-[hash].js`,
        chunkFileNames: `assets/[name]-[hash].js`,
        assetFileNames: `assets/[name]-[hash].[ext]`
      }
    }
  },
  test: {
    globals: true,
    environment: 'happy-dom',
    setupFiles: ['./test/unit/setup.ts'],
    css: true,
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'json-summary', 'html'],
      reportOnFailure: true,
      include: [
        'src/modules/**',
        'src/components/**',
        'src/apiCalls/**',
        'src/helpers/**',
        'src/*.tsx'
      ],
      thresholds: {
        global: {
          branches: 80,
          functions: 80,
          lines: 80,
          statements: 80
        }
      }
    }
  }
})

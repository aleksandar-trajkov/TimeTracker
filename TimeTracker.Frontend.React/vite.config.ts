import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import subresourceIntegrity from 'vite-plugin-sri'

// https://vite.dev/config/
export default defineConfig({
  define: {
    BUILD_DATE: JSON.stringify(new Date().valueOf()),
  },
  plugins: [
	react(),
	subresourceIntegrity()
  ],
  build: {
    rollupOptions: {
      output: {
        entryFileNames: `assets/script-[hash].js`,
        chunkFileNames: `assets/[name]-[hash].js`,
        assetFileNames: `assets/[name]-[hash].[ext]`
      }
    }
  }
})

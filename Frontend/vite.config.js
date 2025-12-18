import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5203',  // Backend portu (launchSettings.json'dan)
        changeOrigin: true,
        secure: false
      },
      '/Departman': {
        target: 'http://localhost:5203',
        changeOrigin: true,
        secure: false
      },
      '/Stajers': {
        target: 'http://localhost:5203',
        changeOrigin: true,
        secure: false
      },
      '/Dashboard': {
        target: 'http://localhost:5203',
        changeOrigin: true,
        secure: false
      },
      '/UniversiteModels': {
        target: 'http://localhost:5203',
        changeOrigin: true,
        secure: false
      },
      '/DepartmanApi': {
        target: 'http://localhost:5203',
        changeOrigin: true,
        secure: false
      }
    }
  }
})
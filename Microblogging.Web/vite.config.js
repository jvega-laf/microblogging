// Microblogging.Web/vite.config.js
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// Configuración de Vite
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        rewrite: path => path.replace(/^\/api/, ''),
      },
    },
  },
  build: {
    outDir: 'dist',
    sourcemap: true, // Asegura que los source maps estén habilitados
  },
});

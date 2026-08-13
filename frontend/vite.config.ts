import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import path from 'path'

import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(),
    tailwindcss(),
  ],
   resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    proxy: {
// Forward all /api/* calls to the ASP.NET Core backend during development,
      // avoiding CORS entirely. The public client also keeps an absolute base
      // URL (see src/config/env.ts) so requests work without the proxy too.
      //
      // The backend runs UseHttpsRedirection() (see SchoolERP.Api/Program.cs),
      // so plain HTTP on :5053 answers with a 307 -> HTTPS on :7083. We proxy to
      // HTTPS directly and set `secure:false` because the ASP.NET Core dev cert
      // is self-signed; the browser trusts it after `dotnet dev-certs --trust`.
      "/api": {
        target: "https://localhost:7083",
        changeOrigin: true,
        secure: false,
      },
    },
  },
})

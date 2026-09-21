/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        ink: "#1c2431",
        slate: {
          925: "#141a24",
        },
        brand: {
          50: "#eef4ff",
          100: "#dbe7ff",
          400: "#5b8def",
          500: "#3866d6",
          600: "#2a4fb0",
          700: "#233f8c",
        },
      },
      fontFamily: {
        display: ["'Sora'", "system-ui", "sans-serif"],
        sans: ["'Inter'", "system-ui", "sans-serif"],
      },
    },
  },
  plugins: [],
};

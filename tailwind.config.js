/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        navy: {
          900: '#060B19',
          800: '#0B1528',
          700: '#112240',
          600: '#1D3557',
        },
        metallic: {
          light: '#E2E8F0',
          silver: '#CBD5E1',
          gold: '#F59E0B',
          bronze: '#B45309',
        },
        lottery: {
          red: '#DC2626',
          redGlow: '#EF4444',
          accent: '#38BDF8',
          gold: '#FBBF24',
        }
      },
      animation: {
        'spin-slow': 'spin 8s linear infinite',
        'pulse-fast': 'pulse 1s cubic-bezier(0.4, 0, 0.6, 1) infinite',
        'bounce-subtle': 'bounceSubtle 0.8s ease-in-out infinite',
      },
      keyframes: {
        bounceSubtle: {
          '0%, 100%': { transform: 'translateY(-3%)' },
          '50%': { transform: 'translateY(0)' },
        }
      }
    },
  },
  plugins: [],
}

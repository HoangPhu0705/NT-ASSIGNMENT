/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        "./wwwroot/**/*.html",
    ],
  theme: {
    extend: {},
  },
  plugins: [
      require('daisyui'),
  ], 
    daisyui: {
        themes: ["light"],
    },
}


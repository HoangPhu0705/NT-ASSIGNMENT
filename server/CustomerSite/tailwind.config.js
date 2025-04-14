module.exports = {
  content: [
    "./Pages/**/*.{cshtml,html}", // Include Razor Pages files
    "./wwwroot/**/*.{html,js}"    // Include static files in wwwroot
  ],
  theme: {
    extend: {
      fontFamily: {
        'sans': ['Brown', 'ui-sans-serif', 'system-ui', '-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'Roboto', 'Helvetica Neue', 'Arial', 'sans-serif'],
      },
      fontWeight: {
        thin: 100,
        light: 300,
        normal: 400,
        bold: 700,
      }
    },
  },
  plugins: [
    require('daisyui'),
  ],
  daisyui: {
    themes: ["light"],
  },
}
# City Weather API

A simple Node.js Express API that returns current weather for a given city. Uses external weather provider (OpenWeatherMap by default). Configure the API key and base URL via environment variables.

Endpoints:
- POST /api/weather - body: { city, units?, lang? }

Errors are logged in structured JSON format to the console.

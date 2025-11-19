# createweatherapp

This ASP.NET Core Web API returns weather data for a given city using the OpenWeatherMap API. The project follows a simple clean architecture with separation between controllers, services, and models.

How to use

1. Set your OpenWeatherMap API key in appsettings.json under OpenWeather:ApiKey.
2. Build and run the app (requires .NET 8 SDK):

 dotnet run

The application listens on the port configured in appsettings.json (BackendSystem:Port). By default it is 8080.

Endpoints

- GET /weather?city={city}
- GET /weather/{city}

Responses

Returns the raw JSON response returned by the configured weather provider.

Logging and errors

Errors are logged using the built-in logging infrastructure and will return a 400 for missing city or 500 for provider/errors.

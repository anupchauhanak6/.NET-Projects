using Microsoft.AspNetCore.Mvc;

namespace BasicAPIsPoints.Controllers
{
    public class WeatherForecast
    {
        // Unique identifier for each forecast.
        public int Id { get; set; }

        // The date for which the forecast is recorded.
        public DateTime Date { get; set; }

        // Temperature value in Celsius.
        public int TemperatureC { get; set; }

        // Optional description of the weather.
        public string? Summary { get; set; }
    }

    // Contains only optional fields so PATCH can update selected values.
    public class WeatherForecastPatch
    {
        public DateTime? Date { get; set; }
        public int? TemperatureC { get; set; }
        public string? Summary { get; set; }
    }

    // Enables API-specific behavior such as automatic model validation.
    [ApiController]

    // Creates routes such as /api/WeatherForecast.
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        // In-memory data store. Data will reset when the application restarts.
        private static readonly List<WeatherForecast> Forecasts = new()
        {
            new WeatherForecast
            {
                Id = 1,
                Date = DateTime.UtcNow.AddDays(1),
                TemperatureC = 10,
                Summary = "Cold"
            },
            new WeatherForecast
            {
                Id = 2,
                Date = DateTime.UtcNow.AddDays(2),
                TemperatureC = 30,
                Summary = "Warm"
            },
            new WeatherForecast
            {
                Id = 3,
                Date = DateTime.UtcNow.AddDays(3),
                TemperatureC = 25,
                Summary = "Mild"
            }
        };

        // Handles GET /api/WeatherForecast.
        [HttpGet]
        public IActionResult Get()
        {
            // Returns all forecasts.
            return Ok(Forecasts);
        }

        // Handles GET /api/WeatherForecast/{id}.
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            // Find one forecast by its unique ID.
            var forecast = Forecasts.FirstOrDefault(f => f.Id == id);

            // Return 404 when the requested ID does not exist; otherwise return the forecast.
            return forecast is null ? NotFound() : Ok(forecast);
        }

        // Handles POST /api/WeatherForecast and creates a new forecast.
        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecast forecast)
        {
            // Check whether the summary already exists, ignoring letter casing.
            // Simple Step 1: Check the summery is empty or not
            if (forecast.Summary != null && forecast.Summary.Trim() != "")
            {
                bool isDuplicateFound = false;

                // Simple Step 2: Search through loop
                foreach (var f in Forecasts)
                {
                    // Null safety + Case-insensitive match (both converted to lowercase)
                    if (f.Summary != null && f.Summary.ToLower() == forecast.Summary.ToLower())
                    {
                        isDuplicateFound = true;
                        break; // Summery mathed, break the loop
                    }
                }

                // Simple Step 3: handle the conflict
                if (isDuplicateFound)
                {
                    return Conflict(new { message = "This summary already exists." });
                }
            }
            // Method 2 for the checking duplicates.
            if (!string.IsNullOrWhiteSpace(forecast.Summary) && Forecasts.Any(f => string.Equals(f.Summary, forecast.Summary, StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict(new { message = "This summary already exists." });
            }

            // Generate the next ID automatically.
            forecast.Id = Forecasts.Count == 0
                ? 1
                : Forecasts.Max(f => f.Id) + 1;

            // Add the new forecast to the in-memory list.
            Forecasts.Add(forecast);

            // Return 201 and include the URL of the newly created forecast.
            return CreatedAtAction(
                nameof(GetById),
                new { id = forecast.Id },
                forecast
            );
        }

        // Handles PUT /api/WeatherForecast/{id} and replaces all editable fields.
        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] WeatherForecast forecast)
        {
            // Find the existing forecast before updating it.
            var existingForecast = Forecasts.FirstOrDefault(f => f.Id == id);

            if (existingForecast is null)
            {
                return NotFound();
            }

            // Replace every editable field with the values from the request body.
            existingForecast.Date = forecast.Date;
            existingForecast.TemperatureC = forecast.TemperatureC;
            existingForecast.Summary = forecast.Summary;

            // 204 means the update succeeded without returning a response body.
            return NoContent();
        }

        // Handles PATCH /api/WeatherForecast/{id} for partial updates.
        [HttpPatch("{id:int}")]
        public IActionResult Patch(int id, [FromBody] WeatherForecastPatch update)
        {
            // Find the forecast that should be partially updated.
            var existingForecast = Forecasts.FirstOrDefault(f => f.Id == id);

            if (existingForecast is null)
            {
                return NotFound();
            }

            // Update the date only when the request includes a date.
            if (update.Date.HasValue)
            {
                existingForecast.Date = update.Date.Value;
            }

            // Update the temperature only when the request includes a temperature.
            if (update.TemperatureC.HasValue)
            {
                existingForecast.TemperatureC = update.TemperatureC.Value;
            }

            // Update the summary only when the request includes a non-null summary.
            if (update.Summary is not null)
            {
                existingForecast.Summary = update.Summary;
            }

            // 204 means the partial update succeeded without returning a response body.
            return NoContent();
        }

        // Handles DELETE /api/WeatherForecast/{id}.
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            // Find the forecast that should be deleted.
            var forecast = Forecasts.FirstOrDefault(f => f.Id == id);

            if (forecast is null)
            {
                return NotFound();
            }

            // Remove the matching forecast from the in-memory list.
            Forecasts.Remove(forecast);

            // 204 means the delete succeeded without returning a response body.
            return NoContent();
        }
    }
}

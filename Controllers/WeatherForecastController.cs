using Microsoft.AspNetCore.Mvc;

namespace WebApiOgrenicem.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    class WeatherForecast
    {
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int TemperatureC { get; set; }
        public string City { get; set; }
    }
    
    private static readonly List<WeatherForecast> cityDegrees = new List<WeatherForecast>
        {
            new WeatherForecast { City = "Ankara", TemperatureC = 12 },
            new WeatherForecast { City = "Istanbul", TemperatureC = 18 },
            new WeatherForecast { City = "Izmir", TemperatureC = 23 },
            new WeatherForecast { City = "Antalya", TemperatureC = 25 },
            new WeatherForecast { City = "Adana", TemperatureC = 20 },
            new WeatherForecast { City = "Sivas", TemperatureC = 5 },
        };
    

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IActionResult Get()
    {
        return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
        })
        .ToArray());
    }
    
    [HttpGet("city-degrees")]
    public IEnumerable<object> GetCityDegrees()
    {
        return cityDegrees;
    }
    
    [HttpGet("ankara-degree")]
    public IActionResult GetAnkaraDegree()
    {
    var ankaraForecast = cityDegrees.FirstOrDefault(c => c.City == "Ankara");
    
    if (ankaraForecast != null)
    {
        return Ok(ankaraForecast.TemperatureC);
    }

    return NotFound(new { Message = "Ankara verisi bulunamadı." });
    }

    [HttpGet("degree/{city}")]
    public IActionResult GetCityDegree(string city)
    {
        var cityForecast = cityDegrees.FirstOrDefault(c => c.City == city);
        
        if (cityForecast != null)
        {
            return Ok(cityForecast.TemperatureC);
        }
        return NotFound(new { Message = $"{city} verisi bulunamadı." });
    }
}


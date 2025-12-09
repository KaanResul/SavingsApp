using Microsoft.AspNetCore.Mvc;

namespace WebApiOgrenicem.Controllers;

[ApiController]
[Route("[controller]")]
public class DovizController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public DovizController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    [HttpGet("dolar")]
    public async Task<IActionResult> GetDolarKuru()
    {
        var url = "https://api.frankfurter.app/latest?from=USD&to=TRY";
        try 
        {
            var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(url);
            
            if (response != null && response.Rates.ContainsKey("TRY"))
            {
                return Ok(response.Rates["TRY"]); 
            }
            
            return NotFound("Kur bilgisi bulunamadı.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Hata: {ex.Message}");
        }
    }
    
    public class ExchangeRateResponse
    {
        public double Amount { get; set; }
        public string Base { get; set; }
        public DateOnly Date { get; set; }
        public Dictionary<string, double> Rates { get; set; }
    }
}

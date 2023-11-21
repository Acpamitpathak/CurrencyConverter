// ConversionController.cs
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ConversionController : ControllerBase
{
    private readonly CurrencyConverterService converterService;

    public ConversionController(CurrencyConverterService converterService)
    {
        this.converterService = converterService;
    }

    [HttpGet("convert")]
    public IActionResult ConvertCurrency(string sourceCurrency, string targetCurrency, decimal amount)
    {
        try
        {
            var result = converterService.ConvertCurrency(sourceCurrency, targetCurrency, amount);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            // Handle invalid input or unsupported currencies
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            return StatusCode(500, "Internal Server Error");
        }
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class ExchangeRateOptions
{
    public Dictionary<string, decimal> ExchangeRates { get; set; }
}


public class CurrencyConverterService
{
    private readonly ILogger<CurrencyConverterService> logger;
    private readonly Dictionary<string, decimal> exchangeRates;
   
    public CurrencyConverterService(Dictionary<string, decimal> exchangeRates, ILogger<CurrencyConverterService> logger)
    {
        this.exchangeRates = exchangeRates ?? throw new ArgumentNullException(nameof(exchangeRates));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ConversionResult ConvertCurrency(string sourceCurrency, string targetCurrency, decimal amount)
    {
        if (!exchangeRates.ContainsKey($"{sourceCurrency}_TO_{targetCurrency}"))
        {
            // Handle unsupported currency pair
            logger.LogError($"Unsupported currency pair: {sourceCurrency} to {targetCurrency}");
            throw new ArgumentException("Unsupported currency pair");
        }

        if (amount < 0)
        {
            // Handle negative amount
            logger.LogError("Amount must be non-negative");
            throw new ArgumentException("Amount must be non-negative");
        }

        var exchangeRate = exchangeRates[$"{sourceCurrency}_TO_{targetCurrency}"];
        var convertedAmount = amount * exchangeRate;

        return new ConversionResult
        {
            ExchangeRate = exchangeRate,
            ConvertedAmount = convertedAmount
        };
    }
}
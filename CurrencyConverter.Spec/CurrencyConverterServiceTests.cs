using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class CurrencyConverterServiceTests
{

    public Mock<IOptionsMonitor<ExchangeRateOptions>> GetExchangeRateOptionsMock()
    {
        return exchangeRateOptionsMock;
    }

    [Fact]
    public void ConvertCurrency_ValidInput_ReturnsConversionResult()
    {
        // Arrange
        var exchangeRates = new Dictionary<string, decimal>
        {
            { "USD_TO_INR", 74.00m },
            { "INR_TO_USD", 0.013m },
            { "USD_TO_EUR", 0.85m }
        };

        var loggerMock = new Mock<ILogger<CurrencyConverterService>>();
        var exchangeRateOptionsMock = new Mock<IOptionsMonitor<ExchangeRateOptions>>();
        exchangeRateOptionsMock.Setup(x => x.CurrentValue.ExchangeRates).Returns(exchangeRates);

        var currencyConverterService = new CurrencyConverterService(exchangeRateOptionsMock.Object, loggerMock.Object);

        // Act
        var result = currencyConverterService.ConvertCurrency("USD", "INR", 100);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(74.00m, result.ExchangeRate);
        Assert.Equal(7400, result.ConvertedAmount);
    }

    [Fact]
    public void ConvertCurrency_UnsupportedCurrency_ThrowsArgumentException()
    {
        // Arrange
        var exchangeRates = new Dictionary<string, decimal>
        {
            { "USD_TO_INR", 74.00m },
            { "INR_TO_USD", 0.013m },
            { "USD_TO_EUR", 0.85m },
            // Add other exchange rates as needed
        };

        var loggerMock = new Mock<ILogger<CurrencyConverterService>>();
        var exchangeRateOptionsMock = new Mock<IOptionsMonitor<ExchangeRateOptions>>();
        exchangeRateOptionsMock.Setup(x => x.CurrentValue.ExchangeRates).Returns(exchangeRates);

        var currencyConverterService = new CurrencyConverterService(exchangeRateOptionsMock.Object, loggerMock.Object);

        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => currencyConverterService.ConvertCurrency("USD", "GBP", 100));
        Assert.Equal("Unsupported currency pair", exception.Message);
        loggerMock.Verify(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void ConvertCurrency_InvalidInput_ThrowsArgumentException()
    {
        // Arrange
        var exchangeRates = new Dictionary<string, decimal>
        {
            { "USD_TO_INR", 74.00m },
            { "INR_TO_USD", 0.013m },
            { "USD_TO_EUR", 0.85m }
        };

        var loggerMock = new Mock<ILogger<CurrencyConverterService>>();
        var exchangeRateOptionsMock = new Mock<IOptionsMonitor<ExchangeRateOptions>>();
        exchangeRateOptionsMock.Setup(x => x.CurrentValue).Returns(new ExchangeRateOptions { ExchangeRates = exchangeRates });

        var currencyConverterService = new CurrencyConverterService(exchangeRateOptionsMock.Object, loggerMock.Object);

        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => currencyConverterService.ConvertCurrency("USD", "INR", -100));
        Assert.Equal("Amount must be non-negative", exception.Message);
        loggerMock.Verify(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void ConvertCurrency_UnexpectedError_ThrowsException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<CurrencyConverterService>>();
        var exchangeRateOptionsMock = new Mock<IOptionsMonitor<ExchangeRateOptions>>();
        exchangeRateOptionsMock.Setup(x => x.CurrentValue.ExchangeRates).Throws(new Exception("Simulated unexpected error"));

        var currencyConverterService = new CurrencyConverterService(exchangeRateOptionsMock.Object, loggerMock.Object);

        // Act and Assert
        var exception = Assert.Throws<Exception>(() => currencyConverterService.ConvertCurrency("USD", "INR", 100));
        Assert.Equal("An unexpected error occurred", exception.Message);
        loggerMock.Verify(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
    }
}

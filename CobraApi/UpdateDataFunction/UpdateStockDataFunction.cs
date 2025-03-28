using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class UpdateStockDataFunction
{
    private readonly ILogger _logger;
    private static readonly HttpClient client = new HttpClient();

    public UpdateStockDataFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<UpdateStockDataFunction>();
    }

    [Function("UpdateStockDataFunction")]
    public async Task Run([TimerTrigger("0 0 */6 * * *")] object _)
    {
        _logger.LogInformation($"Running stock update at: {DateTime.Now}");

        var response = await client.PostAsync("https://cobraapiapi.azure-api.net/api/StockDatas/UpdateCurrentPrices", null);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Stock prices updated successfully.");
        }
        else
        {
            _logger.LogError($"Error updating stock prices: {response.StatusCode}");
        }
    }
}

public class OrdersService
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _httpClient;

    public OrdersService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<Order> GetOrder(int id)
    {
        await using var contentStream = await (await _httpClient.GetAsync($"{Unicorns.Server}/orders/{id}"))
            .EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<Order>(contentStream, _serializerOptions);
    }

    public async Task<IEnumerable<Order>> GetOrders()
    {
        await using var contentStream = await (await _httpClient.GetAsync($"{Unicorns.Server}/orders"))
            .EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<IEnumerable<Order>>(contentStream, _serializerOptions);
    }

    public async Task<Order> InsertOrder(Order order)
    {
        await using var contentStream = await (await _httpClient.PostAsJsonAsync($"{Unicorns.Server}/orders", order))
            .EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<Order>(contentStream, _serializerOptions);
    }

    public async Task<Order> UpdateOrder(Order order)
    {
        await using var contentStream = await (await _httpClient.PutAsJsonAsync($"{Unicorns.Server}/orders", order))
            .EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<Order>(contentStream, _serializerOptions);
    }

    public async Task ArchiveOldOrders(DateTime archiveDate)
        => (await _httpClient.SendAsync(new HttpRequestMessage(
                HttpMethod.Put,
                $"{Unicorns.Server}/orders/archive/{archiveDate:yyyy-M-d}")))
            .EnsureSuccessStatusCode();

    public async Task CancelOrdersForRegion(string region)
        => (await _httpClient.SendAsync(new HttpRequestMessage(
                HttpMethod.Put,
                $"{Unicorns.Server}/orders/cancel/{UrlEncoder.Default.Encode(region)}")))
            .EnsureSuccessStatusCode();
}

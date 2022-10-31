public class CustomersService
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _httpClient;

    public CustomersService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<Customer> GetCustomer(int id)
    {
        await using var contentStream = await (await _httpClient.GetAsync($"{Unicorns.Server}/customers/{id}"))
            .EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<Customer>(contentStream, _serializerOptions);
    }

    public async Task<IEnumerable<Customer>> GetCustomers()
    {
        await using var contentStream = await (await _httpClient.GetAsync($"{Unicorns.Server}/customers"))
            .EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<IEnumerable<Customer>>(contentStream, _serializerOptions);
    }

    public async Task<Customer> InsertCustomer(Customer customer)
    {
        await using var contentStream = await (await _httpClient.PostAsJsonAsync(
                $"{Unicorns.Server}/customers", customer))
            .EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<Customer>(contentStream, _serializerOptions);
    }
    
    public async Task<Customer> UpdateCustomer(Customer customer)
    {
        await using var contentStream = await (await _httpClient.PutAsJsonAsync(
                $"{Unicorns.Server}/customers", customer))
            .EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<Customer>(contentStream, _serializerOptions);
    }
}

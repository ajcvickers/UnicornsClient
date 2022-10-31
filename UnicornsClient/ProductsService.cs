public class ProductsService
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _httpClient;

    public ProductsService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<IEnumerable<Category>> GetProductsByCategory()
    {
        await using var contentStream = await (await _httpClient.GetAsync($"{Unicorns.Server}/products"))
            .EnsureSuccessStatusCode().Content.ReadAsStreamAsync();

        return await JsonSerializer.DeserializeAsync<IEnumerable<Category>>(contentStream, _serializerOptions);
    }
}

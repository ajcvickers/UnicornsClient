public class Benchmarks
{
    private static ProductsService ProductsService { get; }

    static Benchmarks()
    {
        var host = new HostBuilder()
            .ConfigureServices(services =>
            {
                services.AddHttpClient();
                services.AddTransient<ProductsService>();
            })
            .Build();

        ProductsService = host.Services.GetRequiredService<ProductsService>(); 
    }

    [Benchmark]
    public async Task GetProductsByCategory()
    {
        await ProductsService.GetProductsByCategory();
    }
    
    // [Benchmark]
    // public async Task GetProductsForCategory()
    // {
    //     await ProductsService.GetProducts(_getProductsForCategoryUrl);
    // }
    //
    // [Benchmark]
    // public async Task DiscontinueProducts()
    // {
    //     await ProductsService.DiscontinueProducts(_discontinueProductsUrl);
    // }
}

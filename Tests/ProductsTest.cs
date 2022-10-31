public class ProductsTest
{
    private readonly ProductsService _productsService;

    public ProductsTest()
    {
        _productsService = Unicorns.Host.Services.GetRequiredService<ProductsService>();
    }

    [Fact]
    public async Task Can_get_all_products()
    {
        var categories = (await _productsService.GetProductsByCategory()).ToList();

        Assert.Equal(3, categories.Count);
        Assert.Equal(new[] {6, 9, 12}, categories.Select(e => e.Products.Count));
    }
}

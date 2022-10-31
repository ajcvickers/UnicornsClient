public static class Unicorns
{
    // public static readonly string Server = "http://localhost:52070";
    public static readonly string Server = "http://localhost:9574";

    public static IHost Host { get; } = new HostBuilder()
        .ConfigureServices(services =>
        {
            services.AddHttpClient();
            services.AddTransient<ProductsService>();
            services.AddTransient<OrdersService>();
            services.AddTransient<CustomersService>();
        })
        .Build();
}

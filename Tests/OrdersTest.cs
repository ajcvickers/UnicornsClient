public class OrdersTest
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly OrdersService _ordersService;
    private readonly CustomersService _customersService;
    private readonly ProductsService _productsService;

    public OrdersTest()
    {
        _ordersService = Unicorns.Host.Services.GetRequiredService<OrdersService>();
        _customersService = Unicorns.Host.Services.GetRequiredService<CustomersService>();
        _productsService = Unicorns.Host.Services.GetRequiredService<ProductsService>();
    }

    [Fact]
    public async Task Can_get_all_orders()
    {
        var orders = (await _ordersService.GetOrders()).ToList();

        Assert.True(orders.Count >= 27);
        foreach (var order in orders.OrderBy(e => e.Id).Where(e => e.Id != 7).Take(27))
        {
            Assert.NotNull(order.Customer);
            Assert.Equal(12, order.OrderLines.Count);
            foreach (var orderLine in order.OrderLines)
            {
                Assert.NotNull(orderLine.Product);
            }
        }
    }

    [Fact]
    public async Task Can_get_order()
    {
        var order = await _ordersService.GetOrder(8);

        Assert.True(!order.DispatchedOn.HasValue || order.OrderedOn < order.DispatchedOn!.Value);
        Assert.True(!order.DeliveredOn.HasValue || order.DispatchedOn!.Value < order.DeliveredOn!.Value);
        
        Assert.Equal("Baxter", order.Customer.Name);
        Assert.Equal(12, order.OrderLines.Count);
    }

    [Fact]
    public async Task Can_create_new_order()
    {
        var customer = await _customersService.GetCustomer(2);
        
        var contactDetails =  JsonSerializer.Deserialize<ContactDetails>(customer.ContactDetails, _serializerOptions)!;
        
        var categories = (await _productsService.GetProductsByCategory()).ToList();
        var order = new Order
        {
            CustomerId = customer.Id,
            OrderedOn = DateTime.UtcNow,
            DeliveryAddress = JsonSerializer.Serialize(
                contactDetails.Addresses.Single(a => a.Primary), _serializerOptions),
            OrderLines =
            {
                new() {ProductId = categories[0].Products[2].Id, Quantity = 1},
                new() {ProductId = categories[1].Products[0].Id, Quantity = 7},
                new() {ProductId = categories[2].Products[1].Id, Quantity = 1},
            }
        };

        var insertedOrder = await _ordersService.InsertOrder(order);
        
        Assert.NotSame(order, insertedOrder);
        Assert.True(insertedOrder.Id > 0);
        Assert.Equal(3, insertedOrder.OrderLines.Count);
        foreach (var orderLine in insertedOrder.OrderLines)
        {
            Assert.True(orderLine.Id > 0);
        }
    }

    [Fact]
    public async Task Can_update_an_order()
    {
        var order = await _ordersService.GetOrder(9);
        order.DeliveredOn = DateTime.UtcNow;
        
        var updatedOrder = await _ordersService.UpdateOrder(order);

        AssertOrder(order, updatedOrder);
        AssertOrder(order, await _ordersService.GetOrder(9));
    }

    [Fact]
    public async Task Can_update_an_order_with_modified_order_lines()
    {
        var categories = (await _productsService.GetProductsByCategory()).ToList();

        var order = await _ordersService.GetOrder(7);

        order.OrderLines[2].Deleted = true;
        order.OrderLines[3].Deleted = true;
        order.OrderLines.AddRange(
            new OrderLine[]
            {
                new() {ProductId = categories[0].Products[2].Id, Quantity = 1},
                new() {ProductId = categories[1].Products[0].Id, Quantity = 7},
                new() {ProductId = categories[2].Products[1].Id, Quantity = 1},
            });
        
        var updatedOrder = await _ordersService.UpdateOrder(order);

        AssertOrder(order, updatedOrder);
        AssertOrder(order, await _ordersService.GetOrder(7));
    }
    
    private static void AssertOrder(Order order, Order updatedOrder)
    {
        Assert.NotSame(order, updatedOrder);

        Assert.Equal(order.Archived, updatedOrder.Archived);
        Assert.Equal(order.Cancelled, updatedOrder.Cancelled);
        Assert.Equal(order.OrderedOn, updatedOrder.OrderedOn);
        Assert.Equal(order.DispatchedOn, updatedOrder.DispatchedOn);
        Assert.Equal(order.DeliveredOn, updatedOrder.DeliveredOn);
        Assert.Equal(order.CustomerId, updatedOrder.CustomerId);
        
        Assert.True(updatedOrder.OrderLines.Count > 0);
        Assert.Equal(order.OrderLines.Count, updatedOrder.OrderLines.Count);
        for (var i = 0; i < order.OrderLines.Count; i++)
        {
            var orderLine = order.OrderLines[i];
            var updatedOrderLine = updatedOrder.OrderLines[i];
            
            if (orderLine.Id == 0)
            {
                Assert.True(updatedOrderLine.Id > 0);
            }
            else
            {
                Assert.Equal(orderLine.Id, updatedOrderLine.Id);
            }

            Assert.Equal(orderLine.Deleted, updatedOrderLine.Deleted);
        }
    }
    
    [Fact]
    public async Task Can_archive_old_orders()
    {
        var ordersBefore = (await _ordersService.GetOrders()).ToList();
        Assert.Equal(27, ordersBefore.Count(o => !o.Archived));

        await _ordersService.ArchiveOldOrders(DateTime.Today.AddDays(-10));

        var ordersAfter = (await _ordersService.GetOrders()).ToList();
        Assert.Equal(9, ordersAfter.Count(o => !o.Archived));
    }
    
    [Fact]
    public async Task Can_cancel_orders_for_region()
    {
        var ordersBefore = (await _ordersService.GetOrders()).ToList();
        Assert.Equal(0, ordersBefore.Count(o => o.Cancelled));

        await _ordersService.CancelOrdersForRegion("Europe");

        var ordersAfter = (await _ordersService.GetOrders()).ToList();
        Assert.Equal(9, ordersAfter.Count(o => o.Cancelled));
    }
}

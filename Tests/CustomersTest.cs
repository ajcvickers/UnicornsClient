public class CustomersTest
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly CustomersService _customersService;

    public CustomersTest()
    {
        _customersService = Unicorns.Host.Services.GetRequiredService<CustomersService>();
    }

    [Fact]
    public async Task Can_get_all_customers()
    {
        var customers = (await _customersService.GetCustomers()).ToList();

        Assert.True(customers.Count >= 9);
    }

    [Fact]
    public async Task Can_get_customer()
    {
        var customer = await _customersService.GetCustomer(2);

        Assert.Equal("Mac", customer.Name);
    }
    
    [Fact]
    public async Task Can_create_new_customer()
    {
        var contactDetails = new ContactDetails
        {
            Region = "Europe",
            Addresses =
            {
                new()
                {
                    Address1 = "2298 Main St",
                    City = "Ailsworth",
                    Country = "England",
                    PostalCode = "PE99 7RU",
                    Primary = true
                }
            },
            PhoneNumbers =
            {
                new()
                {
                    CountryCode = 44,
                    Number = "01632 12346",
                    Type = PhoneType.Mobile,
                    Primary = true
                }
            },
            EmailAddresses =
            {
                new()
                {
                    Address = "ivan@example.com",
                    Primary = true
                }
            }
        };
        
        var customer = new Customer
        {
            Name = "Ivan",
            ContactDetails = JsonSerializer.Serialize(contactDetails, _serializerOptions)
        };

        var insertedCustomer = await _customersService.InsertCustomer(customer);
        
        Assert.NotSame(customer, insertedCustomer);
        Assert.True(insertedCustomer.Id > 0);
    }

    [Fact]
    public async Task Can_update_a_customer()
    {
        var customer = await _customersService.GetCustomer(1);
        
        var contactDetails =  JsonSerializer.Deserialize<ContactDetails>(customer.ContactDetails, _serializerOptions)!;
        
        contactDetails.Addresses.Single(a => a.Primary).Primary = false;
        contactDetails.Addresses[2].Primary = true;

        customer.ContactDetails = JsonSerializer.Serialize(contactDetails, _serializerOptions);
        
        var updatedCustomer = await _customersService.UpdateCustomer(customer);

        Assert.NotSame(customer, updatedCustomer);
        Assert.Equal(1, updatedCustomer.Id);
        var updatedContactDetails =  JsonSerializer.Deserialize<ContactDetails>(customer.ContactDetails, _serializerOptions)!;
        Assert.False(updatedContactDetails.Addresses[0].Primary);
        Assert.False(updatedContactDetails.Addresses[1].Primary);
        Assert.True(updatedContactDetails.Addresses[2].Primary);
    }
}

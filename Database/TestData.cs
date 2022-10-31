﻿using UnicornSuppliesX;

public class TestData
{
    public static void RecreateDatabase(int productsPerCategory, int ordersPerCustomer)
    {
        using var context = new UnicornsContext();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var categories = CreateProducts(productsPerCategory).ToList();
        context.AddRange(categories);
        context.AddRange(CreateCustomers(categories, ordersPerCustomer));

        context.SaveChanges();
    }

    private static IList<Customer> CreateCustomers(IList<Category> categories, int ordersPerCustomer)
    {
        return new[]
        {
            CreateCustomer("Alice"),
            CreateCustomer("Mac"),
            CreateCustomer("Baxter"),
            CreateCustomer("Toast"),
            CreateCustomer("Boris"),
            CreateCustomer("Eeky Bear"),
            CreateCustomer("Mrs Pandy"),
            CreateCustomer("Fregs"),
            CreateCustomer("DJ Hazzy Smart")
        };

        Customer CreateCustomer(string name)
        {
            var customer = new Customer()
            {
                Name = name,
                ContactDetails = new ContactDetails
                {
                    Region = "Europe",
                    Addresses =
                    {
                        new()
                        {
                            Address1 = "Burns Cottage",
                            Address2 = "1145 Newlands",
                            City = "Bettyhill",
                            Country = "Scotland",
                            PostalCode = "KW99 7RU",
                            Primary = true
                        },
                        new()
                        {
                            Address1 = "2298 Main St",
                            City = "Ailsworth",
                            Country = "England",
                            PostalCode = "PE99 7RU"
                        },
                        new()
                        {
                            Address1 = "765 Meadow Drive",
                            City = "Healing",
                            Country = "England",
                            PostalCode = "DN99 7RU"
                        }
                    },
                    PhoneNumbers =
                    {
                        new()
                        {
                            CountryCode = 44,
                            Number = "01632 12345",
                            Type = PhoneType.Home
                        },
                        new()
                        {
                            CountryCode = 44,
                            Number = "01632 12346",
                            Type = PhoneType.Mobile,
                            Primary = true
                        },
                        new()
                        {
                            CountryCode = 44,
                            Number = "01632 12347",
                            Type = PhoneType.Work
                        }
                    },
                    EmailAddresses =
                    {
                        new()
                        {
                            Address = $"{name.ToLower()}@example.com",
                            Primary = true
                        },
                        new()
                        {
                            Address = $"{new string(name.ToLower().Reverse().ToArray())}@example.com",
                        }
                    }
                }
            };

            var random = new Random();
            var date = DateTime.UtcNow.AddDays(-1);
            for (var i = 0; i < ordersPerCustomer; i++)
            {
                var address = customer.ContactDetails.Addresses[random.Next(3)];
                var order = new Order
                {
                    OrderedOn = date,
                    DispatchedOn = date < DateTime.UtcNow.AddDays(-14) ? date.AddDays(random.Next(14)) : null,
                    DeliveredOn = date < DateTime.UtcNow.AddDays(-28) ? date.AddDays(14 + random.Next(14)) : null,
                    DeliveryAddress = new()
                    {
                        Address1 = address.Address1,
                        Address2 = address.Address2,
                        City = address.City,
                        Country = address.Country,
                        PostalCode = address.PostalCode,
                        Primary = address.Primary
                    }
                };

                for (var j = 0; j < 12; j++)
                {
                    var category = categories[random.Next(categories.Count)];
                    order.OrderLines.Add(new OrderLine
                    {
                        Product = category.Products[random.Next(category.Products.Count)],
                        Quantity = random.Next(1, 10)
                    });
                }
                
                customer.Orders.Add(order);

                date = date.AddDays(-14);
            }
            
            // Console.WriteLine($"Created customer '{customer.Name}' with {customer.Orders.Count} orders and {customer.Orders.SelectMany(o => o.OrderLines).Count()} total order lines.");

            return customer;
        }
    }

    private static IList<Category> CreateProducts(int productsPerCategory)
    {
        return new[]
        {
            CreateCategory(
                "Horns", 
                ("Full metal horn", 20m),
                ("Plastic horn", 2m)),
            CreateCategory(
                "Shoes",
                ("Steel hoof shoe", 8m),
                ("Flip-flops", 1m),
                ("Hoof loafers", 5m)),
            CreateCategory(
                "Treats",
                ("Boiled sweets (1lb)", 2m),
                ("Chocolate (equine safe)", 5m),
                ("Toffee apple", 1m),
                ("Pie", 3m))
        };
        
        Category CreateCategory(string categoryName, params (string Name, decimal BasePrice)[] items)
        {
            var category = new Category
            {
                Name = categoryName
            };

            var maxColor = Colors.Length;
            var random = new Random();
            foreach (var item in items)
            {
                for (var i = 0; i < productsPerCategory; i++)
                {
                    category.Products.Add(new Product
                    {
                        Name = item.Name,
                        Price = item.BasePrice + (decimal)random.Next((int)item.BasePrice * 100) / 200,
                        Color = Colors[random.Next(maxColor)],
                        Quality = (Quality)random.Next(3)
                    });
                }
            }

            // Console.WriteLine($"Create category '{category.Name}' with {category.Products.Count} products.");

            return category;
        }
    }

    private static readonly string[] Colors = 
    {
        "Air Force blue",
        "Alice blue",
        "Alizarin crimson",
        "Almond",
        "Amaranth",
        "Amber",
        "American rose",
        "Amethyst",
        "Android Green",
        "Anti-flash white",
        "Antique brass",
        "Antique fuchsia",
        "Antique white",
        "Ao",
        "Apple green",
        "Apricot",
        "Aqua",
        "Aquamarine",
        "Army green",
        "Arylide yellow",
        "Ash grey",
        "Asparagus",
        "Atomic tangerine",
        "Auburn",
        "Aureolin",
        "AuroMetalSaurus",
        "Awesome",
        "Azure",
        "Azure mist/web",
        "Baby blue",
        "Baby blue eyes",
        "Baby pink",
        "Ball Blue",
        "Banana Mania",
        "Banana yellow",
        "Battleship grey",
        "Bazaar",
        "Beau blue",
        "Beaver",
        "Beige",
        "Bisque",
        "Bistre",
        "Bittersweet",
        "Black",
        "Blanched Almond",
        "Bleu de France",
        "Blizzard Blue",
        "Blond",
        "Blue",
        "Blue Bell",
        "Blue Gray",
        "Blue green",
        "Blue purple",
        "Blue violet",
        "Blush",
        "Bole",
        "Bondi blue",
        "Bone",
        "Boston University Red",
        "Bottle green",
        "Boysenberry",
        "Brandeis blue",
        "Brass",
        "Brick red",
        "Bright cerulean",
        "Bright green",
        "Bright lavender",
        "Bright maroon",
        "Bright pink",
        "Bright turquoise",
        "Bright ube",
        "Brilliant lavender",
        "Brilliant rose",
        "Brink pink",
        "British racing green",
        "Bronze",
        "Brown",
        "Bubble gum",
        "Bubbles",
        "Buff",
        "Bulgarian rose",
        "Burgundy",
        "Burlywood",
        "Burnt orange",
        "Burnt sienna",
        "Burnt umber",
        "Byzantine",
        "Byzantium",
        "CG Blue",
        "CG Red",
        "Cadet",
        "Cadet blue",
        "Cadet grey",
        "Cadmium green",
        "Cadmium orange",
        "Cadmium red",
        "Cadmium yellow",
        "Café au lait",
        "Café noir",
        "Cal Poly Pomona green",
        "Cambridge Blue",
        "Camel",
        "Camouflage green",
        "Canary",
        "Canary yellow",
        "Candy apple red",
        "Candy pink",
        "Capri",
        "Caput mortuum",
        "Cardinal",
        "Caribbean green",
        "Carmine",
        "Carmine pink",
        "Carmine red",
        "Carnation pink",
        "Carnelian",
        "Carolina blue",
        "Carrot orange",
        "Celadon",
        "Celeste",
        "Celestial blue",
        "Cerise",
        "Cerise pink",
        "Cerulean",
        "Cerulean blue",
        "Chamoisee",
        "Champagne",
        "Charcoal",
        "Chartreuse",
        "Cherry",
        "Cherry blossom pink",
        "Chestnut",
        "Chocolate",
        "Chrome yellow",
        "Cinereous",
        "Cinnabar",
        "Cinnamon",
        "Citrine",
        "Classic rose",
        "Cobalt",
        "Cocoa brown",
        "Coffee",
        "Columbia blue",
        "Cool black",
        "Cool grey",
        "Copper",
        "Copper rose",
        "Coquelicot",
        "Coral",
        "Coral pink",
        "Coral red",
        "Cordovan",
        "Corn",
        "Cornell Red",
        "Cornflower",
        "Cornflower blue",
        "Cornsilk",
        "Cosmic latte",
        "Cotton candy",
        "Cream",
        "Crimson",
        "Crimson Red",
        "Crimson glory",
        "Cyan",
        "Daffodil",
        "Dandelion",
        "Dark blue",
        "Dark brown",
        "Dark byzantium",
        "Dark candy apple red",
        "Dark cerulean",
        "Dark chestnut",
        "Dark coral",
        "Dark cyan",
        "Dark electric blue",
        "Dark goldenrod",
        "Dark gray",
        "Dark green",
        "Dark jungle green",
        "Dark khaki",
        "Dark lava",
        "Dark lavender",
        "Dark magenta",
        "Dark midnight blue",
        "Dark olive green",
        "Dark orange",
        "Dark orchid",
        "Dark pastel blue",
        "Dark pastel green",
        "Dark pastel purple",
        "Dark pastel red",
        "Dark pink",
        "Dark powder blue",
        "Dark raspberry",
        "Dark red",
        "Dark salmon",
        "Dark scarlet",
        "Dark sea green",
        "Dark sienna",
        "Dark slate blue",
        "Dark slate gray",
        "Dark spring green",
        "Dark tan",
        "Dark tangerine",
        "Dark taupe",
        "Dark terra cotta",
        "Dark turquoise",
        "Dark violet",
        "Dartmouth green",
        "Davy grey",
        "Debian red",
        "Deep carmine",
        "Deep carmine pink",
        "Deep carrot orange",
        "Deep cerise",
        "Deep champagne",
        "Deep chestnut",
        "Deep coffee",
        "Deep fuchsia",
        "Deep jungle green",
        "Deep lilac",
        "Deep magenta",
        "Deep peach",
        "Deep pink",
        "Deep saffron",
        "Deep sky blue",
        "Denim",
        "Desert",
        "Desert sand",
        "Dim gray",
        "Dodger blue",
        "Dogwood rose",
        "Dollar bill",
        "Drab",
        "Duke blue",
        "Earth yellow",
        "Ecru",
        "Eggplant",
        "Eggshell",
        "Egyptian blue",
        "Electric blue",
        "Electric crimson",
        "Electric cyan",
        "Electric green",
        "Electric indigo",
        "Electric lavender",
        "Electric lime",
        "Electric purple",
        "Electric ultramarine",
        "Electric violet",
        "Electric yellow",
        "Emerald",
        "Eton blue",
        "Fallow",
        "Falu red",
        "Famous",
        "Fandango",
        "Fashion fuchsia",
        "Fawn",
        "Feldgrau",
        "Fern",
        "Fern green",
        "Ferrari Red",
        "Field drab",
        "Fire engine red",
        "Firebrick",
        "Flame",
        "Flamingo pink",
        "Flavescent",
        "Flax",
        "Floral white",
        "Fluorescent orange",
        "Fluorescent pink",
        "Fluorescent yellow",
        "Folly",
        "Forest green",
        "French beige",
        "French blue",
        "French lilac",
        "French rose",
        "Fuchsia",
        "Fuchsia pink",
        "Fulvous",
        "Fuzzy Wuzzy",
        "Gainsboro",
        "Gamboge",
        "Ghost white",
        "Ginger",
        "Glaucous",
        "Glitter",
        "Gold",
        "Golden brown",
        "Golden poppy",
        "Golden yellow",
        "Goldenrod",
        "Granny Smith Apple",
        "Gray",
        "Gray asparagus",
        "Green",
        "Green Blue",
        "Green yellow",
        "Grullo",
        "Guppie green",
        "Halayà úbe",
        "Han blue",
        "Han purple",
        "Hansa yellow",
        "Harlequin",
        "Harvard crimson",
        "Harvest Gold",
        "Heart Gold",
        "Heliotrope",
        "Hollywood cerise",
        "Honeydew",
        "Hooker green",
        "Hot magenta",
        "Hot pink",
        "Hunter green",
        "Icterine",
        "Inchworm",
        "India green",
        "Indian red",
        "Indian yellow",
        "Indigo",
        "International Klein Blue",
        "International orange",
        "Iris",
        "Isabelline",
        "Islamic green",
        "Ivory",
        "Jade",
        "Jasmine",
        "Jasper",
        "Jazzberry jam",
        "Jonquil",
        "June bud",
        "Jungle green",
        "KU Crimson",
        "Kelly green",
        "Khaki",
        "La Salle Green",
        "Languid lavender",
        "Lapis lazuli",
        "Laser Lemon",
        "Laurel green",
        "Lava",
        "Lavender",
        "Lavender blue",
        "Lavender blush",
        "Lavender gray",
        "Lavender indigo",
        "Lavender magenta",
        "Lavender mist",
        "Lavender pink",
        "Lavender purple",
        "Lavender rose",
        "Lawn green",
        "Lemon",
        "Lemon Yellow",
        "Lemon chiffon",
        "Lemon lime",
        "Light Crimson",
        "Light Thulian pink",
        "Light apricot",
        "Light blue",
        "Light brown",
        "Light carmine pink",
        "Light coral",
        "Light cornflower blue",
        "Light cyan",
        "Light fuchsia pink",
        "Light goldenrod yellow",
        "Light gray",
        "Light green",
        "Light khaki",
        "Light pastel purple",
        "Light pink",
        "Light salmon",
        "Light salmon pink",
        "Light sea green",
        "Light sky blue",
        "Light slate gray",
        "Light taupe",
        "Light yellow",
        "Lilac",
        "Lime",
        "Lime green",
        "Lincoln green",
        "Linen",
        "Lion",
        "Liver",
        "Lust",
        "MSU Green",
        "Macaroni and Cheese",
        "Magenta",
        "Magic mint",
        "Magnolia",
        "Mahogany",
        "Maize",
        "Majorelle Blue",
        "Malachite",
        "Manatee",
        "Mango Tango",
        "Mantis",
        "Maroon",
        "Mauve",
        "Mauve taupe",
        "Mauvelous",
        "Maya blue",
        "Meat brown",
        "Medium Persian blue",
        "Medium aquamarine",
        "Medium blue",
        "Medium candy apple red",
        "Medium carmine",
        "Medium champagne",
        "Medium electric blue",
        "Medium jungle green",
        "Medium lavender magenta",
        "Medium orchid",
        "Medium purple",
        "Medium red violet",
        "Medium sea green",
        "Medium slate blue",
        "Medium spring bud",
        "Medium spring green",
        "Medium taupe",
        "Medium teal blue",
        "Medium turquoise",
        "Medium violet red",
        "Melon",
        "Midnight blue",
        "Midnight green",
        "Mikado yellow",
        "Mint",
        "Mint cream",
        "Mint green",
        "Misty rose",
        "Moccasin",
        "Mode beige",
        "Moonstone blue",
        "Mordant red 19",
        "Moss green",
        "Mountain Meadow",
        "Mountbatten pink",
        "Mulberry",
        "Munsell",
        "Mustard",
        "Myrtle",
        "Nadeshiko pink",
        "Napier green",
        "Naples yellow",
        "Navajo white",
        "Navy blue",
        "Neon Carrot",
        "Neon fuchsia",
        "Neon green",
        "Non-photo blue",
        "North Texas Green",
        "Ocean Boat Blue",
        "Ochre",
        "Office green",
        "Old gold",
        "Old lace",
        "Old lavender",
        "Old mauve",
        "Old rose",
        "Olive",
        "Olive Drab",
        "Olive Green",
        "Olivine",
        "Onyx",
        "Opera mauve",
        "Orange",
        "Orange Yellow",
        "Orange peel",
        "Orange red",
        "Orchid",
        "Otter brown",
        "Outer Space",
        "Outrageous Orange",
        "Oxford Blue",
        "Pacific Blue",
        "Pakistan green",
        "Palatinate blue",
        "Palatinate purple",
        "Pale aqua",
        "Pale blue",
        "Pale brown",
        "Pale carmine",
        "Pale cerulean",
        "Pale chestnut",
        "Pale copper",
        "Pale cornflower blue",
        "Pale gold",
        "Pale goldenrod",
        "Pale green",
        "Pale lavender",
        "Pale magenta",
        "Pale pink",
        "Pale plum",
        "Pale red violet",
        "Pale robin egg blue",
        "Pale silver",
        "Pale spring bud",
        "Pale taupe",
        "Pale violet red",
        "Pansy purple",
        "Papaya whip",
        "Paris Green",
        "Pastel blue",
        "Pastel brown",
        "Pastel gray",
        "Pastel green",
        "Pastel magenta",
        "Pastel orange",
        "Pastel pink",
        "Pastel purple",
        "Pastel red",
        "Pastel violet",
        "Pastel yellow",
        "Patriarch",
        "Payne grey",
        "Peach",
        "Peach puff",
        "Peach yellow",
        "Pear",
        "Pearl",
        "Pearl Aqua",
        "Peridot",
        "Periwinkle",
        "Persian blue",
        "Persian indigo",
        "Persian orange",
        "Persian pink",
        "Persian plum",
        "Persian red",
        "Persian rose",
        "Phlox",
        "Phthalo blue",
        "Phthalo green",
        "Piggy pink",
        "Pine green",
        "Pink",
        "Pink Flamingo",
        "Pink Sherbet",
        "Pink pearl",
        "Pistachio",
        "Platinum",
        "Plum",
        "Portland Orange",
        "Powder blue",
        "Princeton orange",
        "Prussian blue",
        "Psychedelic purple",
        "Puce",
        "Pumpkin",
        "Purple",
        "Purple Heart",
        "Purple Mountain's Majesty",
        "Purple mountain majesty",
        "Purple pizzazz",
        "Purple taupe",
        "Rackley",
        "Radical Red",
        "Raspberry",
        "Raspberry glace",
        "Raspberry pink",
        "Raspberry rose",
        "Raw Sienna",
        "Razzle dazzle rose",
        "Razzmatazz",
        "Red",
        "Red Orange",
        "Red brown",
        "Red violet",
        "Rich black",
        "Rich carmine",
        "Rich electric blue",
        "Rich lilac",
        "Rich maroon",
        "Rifle green",
        "Robin's Egg Blue",
        "Rose",
        "Rose bonbon",
        "Rose ebony",
        "Rose gold",
        "Rose madder",
        "Rose pink",
        "Rose quartz",
        "Rose taupe",
        "Rose vale",
        "Rosewood",
        "Rosso corsa",
        "Rosy brown",
        "Royal azure",
        "Royal blue",
        "Royal fuchsia",
        "Royal purple",
        "Ruby",
        "Ruddy",
        "Ruddy brown",
        "Ruddy pink",
        "Rufous",
        "Russet",
        "Rust",
        "Sacramento State green",
        "Saddle brown",
        "Safety orange",
        "Saffron",
        "Saint Patrick Blue",
        "Salmon",
        "Salmon pink",
        "Sand",
        "Sand dune",
        "Sandstorm",
        "Sandy brown",
        "Sandy taupe",
        "Sap green",
        "Sapphire",
        "Satin sheen gold",
        "Scarlet",
        "School bus yellow",
        "Screamin Green",
        "Sea blue",
        "Sea green",
        "Seal brown",
        "Seashell",
        "Selective yellow",
        "Sepia",
        "Shadow",
        "Shamrock",
        "Shamrock green",
        "Shocking pink",
        "Sienna",
        "Silver",
        "Sinopia",
        "Skobeloff",
        "Sky blue",
        "Sky magenta",
        "Slate blue",
        "Slate gray",
        "Smalt",
        "Smokey topaz",
        "Smoky black",
        "Snow",
        "Spiro Disco Ball",
        "Spring bud",
        "Spring green",
        "Steel blue",
        "Stil de grain yellow",
        "Stizza",
        "Stormcloud",
        "Straw",
        "Sunglow",
        "Sunset",
        "Sunset Orange",
        "Tan",
        "Tangelo",
        "Tangerine",
        "Tangerine yellow",
        "Taupe",
        "Taupe gray",
        "Tawny",
        "Tea green",
        "Tea rose",
        "Teal",
        "Teal blue",
        "Teal green",
        "Terra cotta",
        "Thistle",
        "Thulian pink",
        "Tickle Me Pink",
        "Tiffany Blue",
        "Tiger eye",
        "Timberwolf",
        "Titanium yellow",
        "Tomato",
        "Toolbox",
        "Topaz",
        "Tractor red",
        "Trolley Grey",
        "Tropical rain forest",
        "True Blue",
        "Tufts Blue",
        "Tumbleweed",
        "Turkish rose",
        "Turquoise",
        "Turquoise blue",
        "Turquoise green",
        "Tuscan red",
        "Twilight lavender",
        "Tyrian purple",
        "UA blue",
        "UA red",
        "UCLA Blue",
        "UCLA Gold",
        "UFO Green",
        "UP Forest green",
        "UP Maroon",
        "USC Cardinal",
        "USC Gold",
        "Ube",
        "Ultra pink",
        "Ultramarine",
        "Ultramarine blue",
        "Umber",
        "United Nations blue",
        "University of California Gold",
        "Unmellow Yellow",
        "Upsdell red",
        "Urobilin",
        "Utah Crimson",
        "Vanilla",
        "Vegas gold",
        "Venetian red",
        "Verdigris",
        "Vermilion",
        "Veronica",
        "Violet",
        "Violet Blue",
        "Violet Red",
        "Viridian",
        "Vivid auburn",
        "Vivid burgundy",
        "Vivid cerise",
        "Vivid tangerine",
        "Vivid violet",
        "Warm black",
        "Waterspout",
        "Wenge",
        "Wheat",
        "White",
        "White smoke",
        "Wild Strawberry",
        "Wild Watermelon",
        "Wild blue yonder",
        "Wine",
        "Wisteria",
        "Xanadu",
        "Yale Blue",
        "Yellow",
        "Yellow Orange",
        "Yellow green",
        "Zaffre",
        "Zinnwaldite brown"
    };
}

using Microsoft.EntityFrameworkCore;

public class DataSeederService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DataSeederService> _logger;

    public DataSeederService(AppDbContext context, ILogger<DataSeederService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedDataAsync()
    {
        try
        {
            // Check if data already exists
            if (await _context.Categories.AnyAsync())
            {
                _logger.LogInformation("Database already seeded. Skipping seed operation.");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            // Seed categories and products
            var categories = CreateCategories();
            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} categories", categories.Count);

            // Seed subcategories and products
            var products = CreateProducts();
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} products", products.Count);

            // Seed inventory for each product
            var inventories = CreateInventories(products);
            await _context.Inventories.AddRangeAsync(inventories);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} inventory records", inventories.Count);

            _logger.LogInformation("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database seeding");
            throw;
        }
    }

    private List<Category> CreateCategories()
    {
        return new List<Category>
        {
            new Category { Id = 1, Name = "Groceries", Icon = "🛒", Description = "Fresh groceries and essentials", DisplayOrder = 1 },
            new Category { Id = 2, Name = "Fruits & Vegetables", Icon = "🥕", Description = "Fresh fruits and vegetables", DisplayOrder = 2 },
            new Category { Id = 3, Name = "Bakery", Icon = "🍞", Description = "Bread, cakes, and baked goods", DisplayOrder = 3 },
            new Category { Id = 4, Name = "Dairy & Eggs", Icon = "🥛", Description = "Milk, yogurt, eggs and dairy products", DisplayOrder = 4 },
            new Category { Id = 5, Name = "Snacks & Chips", Icon = "🍟", Description = "Snacks, chips and crispy items", DisplayOrder = 5 },
            new Category { Id = 6, Name = "Beverages", Icon = "🍷", Description = "Drinks, juices, and beverages", DisplayOrder = 6 },
            new Category { Id = 7, Name = "Meat & Fish", Icon = "🍗", Description = "Meat, chicken, and seafood", DisplayOrder = 7 },
            new Category { Id = 8, Name = "Spices & Seasonings", Icon = "🌶️", Description = "Spices and cooking seasonings", DisplayOrder = 8 },
            new Category { Id = 9, Name = "Beauty & Personal Care", Icon = "💄", Description = "Beauty products and personal care", DisplayOrder = 9 },
            new Category { Id = 10, Name = "Electronics", Icon = "📱", Description = "Electronics and gadgets", DisplayOrder = 10 },
            new Category { Id = 11, Name = "Home & Kitchen", Icon = "🏠", Description = "Home and kitchen products", DisplayOrder = 11 },
            new Category { Id = 12, Name = "Pet Supplies", Icon = "🐕", Description = "Pet food and supplies", DisplayOrder = 12 }
        };
    }

    private List<Product> CreateProducts()
    {
        var products = new List<Product>();
        int id = 1;

        // Groceries - 30 products
        products.AddRange(new[]
        {
            new Product { Id = id++, CategoryId = 1, Name = "Rice (1kg)", Price = 2.99m, OriginalPrice = 3.99m, Brand = "Basmati", Rating = 4.5, ReviewCount = 120, Sku = "RICE001", IntentTags = "grocery, daily, essentials, cooking", SearchKeywords = "rice, basmati, grain" },
            new Product { Id = id++, CategoryId = 1, Name = "Wheat Flour (5kg)", Price = 4.99m, Brand = "Chakki", Rating = 4.3, ReviewCount = 95, Sku = "FLOUR001", IntentTags = "grocery, daily, baking, cooking", SearchKeywords = "flour, wheat, baking" },
            new Product { Id = id++, CategoryId = 1, Name = "Sugar (1kg)", Price = 1.99m, Brand = "Sugarland", Rating = 4.2, ReviewCount = 80, Sku = "SUGAR001", IntentTags = "grocery, daily, kitchen", SearchKeywords = "sugar, sweet" },
            new Product { Id = id++, CategoryId = 1, Name = "Salt (1kg)", Price = 0.99m, Brand = "Iodized", Rating = 4.0, ReviewCount = 60, Sku = "SALT001", IntentTags = "grocery, daily, cooking", SearchKeywords = "salt, iodized" },
            new Product { Id = id++, CategoryId = 1, Name = "Oil (5L)", Price = 8.99m, OriginalPrice = 10.99m, Brand = "Sunflower", Rating = 4.4, ReviewCount = 150, Sku = "OIL001", IntentTags = "grocery, daily, cooking, essentials", SearchKeywords = "oil, sunflower, cooking" },
            new Product { Id = id++, CategoryId = 1, Name = "Lentils (1kg)", Price = 3.49m, Brand = "Premium", Rating = 4.6, ReviewCount = 110, Sku = "LENTIL001", IntentTags = "grocery, protein, cooking, vegan", SearchKeywords = "lentils, dal, pulse" },
            new Product { Id = id++, CategoryId = 1, Name = "Pasta (500g)", Price = 1.49m, Brand = "Primo", Rating = 4.1, ReviewCount = 75, Sku = "PASTA001", IntentTags = "grocery, cooking, dinner", SearchKeywords = "pasta, noodles, italian" },
            new Product { Id = id++, CategoryId = 1, Name = "Canned Beans (400g)", Price = 1.29m, Brand = "Valley Fresh", Rating = 4.0, ReviewCount = 50, Sku = "BEANS001", IntentTags = "grocery, protein, cooking", SearchKeywords = "beans, canned, legumes" },
            new Product { Id = id++, CategoryId = 1, Name = "Tomato Sauce (500ml)", Price = 2.49m, Brand = "Agrimark", Rating = 4.3, ReviewCount = 85, Sku = "SAUCE001", IntentTags = "grocery, cooking, italian", SearchKeywords = "tomato, sauce, condiment" },
            new Product { Id = id++, CategoryId = 1, Name = "Peanut Butter (400g)", Price = 3.99m, Brand = "NutLife", Rating = 4.7, ReviewCount = 95, Sku = "PEANUT001", IntentTags = "grocery, breakfast, protein", SearchKeywords = "peanut, butter, spread" },
        });

        // Fruits & Vegetables - 20 products
        products.AddRange(new[]
        {
            new Product { Id = id++, CategoryId = 2, Name = "Apples (1kg)", Price = 2.99m, Brand = "Kashmir", Rating = 4.6, ReviewCount = 120, Sku = "APPLE001", IntentTags = "fresh, fruit, healthy, party", SearchKeywords = "apple, fruit, fresh" },
            new Product { Id = id++, CategoryId = 2, Name = "Bananas (6pcs)", Price = 1.49m, Brand = "Local", Rating = 4.4, ReviewCount = 100, Sku = "BANANA001", IntentTags = "fresh, fruit, breakfast", SearchKeywords = "banana, fruit, potassium" },
            new Product { Id = id++, CategoryId = 2, Name = "Carrots (1kg)", Price = 1.99m, Brand = "Organic", Rating = 4.5, ReviewCount = 80, Sku = "CARROT001", IntentTags = "fresh, vegetable, healthy", SearchKeywords = "carrot, vegetable, root" },
            new Product { Id = id++, CategoryId = 2, Name = "Tomatoes (1kg)", Price = 2.49m, Brand = "Fresh Pick", Rating = 4.3, ReviewCount = 70, Sku = "TOMATO001", IntentTags = "fresh, vegetable, cooking", SearchKeywords = "tomato, vegetable" },
            new Product { Id = id++, CategoryId = 2, Name = "Onions (2kg)", Price = 1.79m, Brand = "Local", Rating = 4.2, ReviewCount = 65, Sku = "ONION001", IntentTags = "fresh, vegetable, cooking", SearchKeywords = "onion, bulb, cooking" },
            new Product { Id = id++, CategoryId = 2, Name = "Spinach (500g)", Price = 2.99m, Brand = "Organic", Rating = 4.7, ReviewCount = 85, Sku = "SPINACH001", IntentTags = "fresh, vegetable, healthy, organic", SearchKeywords = "spinach, leaves, greens" },
            new Product { Id = id++, CategoryId = 2, Name = "Broccoli (1pc)", Price = 3.49m, Brand = "Premium", Rating = 4.5, ReviewCount = 75, Sku = "BROCCOLI001", IntentTags = "fresh, vegetable, healthy", SearchKeywords = "broccoli, vegetable, greens" },
            new Product { Id = id++, CategoryId = 2, Name = "Lettuce (500g)", Price = 2.49m, Brand = "Fresh Green", Rating = 4.4, ReviewCount = 60, Sku = "LETTUCE001", IntentTags = "fresh, vegetable, salad", SearchKeywords = "lettuce, greens, salad" },
            new Product { Id = id++, CategoryId = 2, Name = "Bell Peppers (3pcs)", Price = 3.99m, Brand = "Colorful", Rating = 4.6, ReviewCount = 90, Sku = "PEPPER001", IntentTags = "fresh, vegetable, cooking, party", SearchKeywords = "pepper, capsicum, vegetable" },
            new Product { Id = id++, CategoryId = 2, Name = "Cucumbers (3pcs)", Price = 1.99m, Brand = "Fresh", Rating = 4.3, ReviewCount = 50, Sku = "CUCUMBER001", IntentTags = "fresh, vegetable, salad", SearchKeywords = "cucumber, vegetable, fresh" },
        });

        // Bakery - 15 products
        products.AddRange(new[]
        {
            new Product { Id = id++, CategoryId = 3, Name = "White Bread (500g)", Price = 1.99m, Brand = "Golden", Rating = 4.4, ReviewCount = 110, Sku = "BREAD001", IntentTags = "bakery, breakfast, party_lunch", SearchKeywords = "bread, white, baked" },
            new Product { Id = id++, CategoryId = 3, Name = "Whole Wheat Bread", Price = 2.49m, Brand = "HealthyLife", Rating = 4.5, ReviewCount = 95, Sku = "BREAD002", IntentTags = "bakery, breakfast, healthy", SearchKeywords = "bread, wheat, wholesome" },
            new Product { Id = id++, CategoryId = 3, Name = "Croissants (6pcs)", Price = 4.99m, Brand = "French Bakery", Rating = 4.7, ReviewCount = 120, Sku = "CROISSANT001", IntentTags = "bakery, breakfast, party, snacks", SearchKeywords = "croissant, pastry, french" },
            new Product { Id = id++, CategoryId = 3, Name = "Cookies (400g)", Price = 2.99m, Brand = "Sweet Treat", Rating = 4.6, ReviewCount = 140, Sku = "COOKIES001", IntentTags = "bakery, snacks, party, dessert", SearchKeywords = "cookies, chocolate, baked" },
            new Product { Id = id++, CategoryId = 3, Name = "Donut Box (6pcs)", Price = 3.99m, Brand = "Sweet Haven", Rating = 4.5, ReviewCount = 85, Sku = "DONUTS001", IntentTags = "bakery, snacks, party, dessert", SearchKeywords = "donut, pastry, sweet" },
            new Product { Id = id++, CategoryId = 3, Name = "Cheese Bread", Price = 2.49m, Brand = "Savory Bakes", Rating = 4.3, ReviewCount = 70, Sku = "CHEESEBREAD001", IntentTags = "bakery, party_lunch, snacks", SearchKeywords = "cheese, bread, savory" },
            new Product { Id = id++, CategoryId = 3, Name = "Muffins (4pcs)", Price = 3.49m, Brand = "Bakery Fresh", Rating = 4.4, ReviewCount = 80, Sku = "MUFFINS001", IntentTags = "bakery, breakfast, snacks", SearchKeywords = "muffins, baked, sweet" },
            new Product { Id = id++, CategoryId = 3, Name = "Cake (500g)", Price = 5.99m, Brand = "Cake Dreams", Rating = 4.8, ReviewCount = 110, Sku = "CAKE001", IntentTags = "bakery, party, celebration, dessert", SearchKeywords = "cake, dessert, celebration" },
        });

        // Dairy & Eggs - 15 products
        products.AddRange(new[]
        {
            new Product { Id = id++, CategoryId = 4, Name = "Milk (1L)", Price = 1.49m, Brand = "Fresh Dairy", Rating = 4.5, ReviewCount = 150, Sku = "MILK001", IntentTags = "dairy, breakfast, essentials", SearchKeywords = "milk, fresh, dairy" },
            new Product { Id = id++, CategoryId = 4, Name = "Yogurt (500g)", Price = 2.49m, Brand = "Creamy", Rating = 4.6, ReviewCount = 130, Sku = "YOGURT001", IntentTags = "dairy, breakfast, healthy", SearchKeywords = "yogurt, probiotic, health" },
            new Product { Id = id++, CategoryId = 4, Name = "Cheese (200g)", Price = 3.99m, Brand = "Milky", Rating = 4.4, ReviewCount = 90, Sku = "CHEESE001", IntentTags = "dairy, cooking, party", SearchKeywords = "cheese, dairy, food" },
            new Product { Id = id++, CategoryId = 4, Name = "Eggs (12pcs)", Price = 2.99m, Brand = "Fresh Farms", Rating = 4.7, ReviewCount = 180, Sku = "EGGS001", IntentTags = "eggs, breakfast, protein", SearchKeywords = "eggs, protein, breakfast" },
            new Product { Id = id++, CategoryId = 4, Name = "Butter (200g)", Price = 3.49m, Brand = "Premium Dairy", Rating = 4.5, ReviewCount = 75, Sku = "BUTTER001", IntentTags = "dairy, cooking, baking", SearchKeywords = "butter, cooking, dairy" },
            new Product { Id = id++, CategoryId = 4, Name = "Milk Powder (400g)", Price = 4.99m, Brand = "Nutrition Plus", Rating = 4.3, ReviewCount = 65, Sku = "MILKPOWDER001", IntentTags = "dairy, breakfast, nutrition", SearchKeywords = "milk powder, nutrition" },
            new Product { Id = id++, CategoryId = 4, Name = "Paneer (200g)", Price = 3.99m, Brand = "Fresh Paneer", Rating = 4.6, ReviewCount = 110, Sku = "PANEER001", IntentTags = "dairy, cooking, vegetarian", SearchKeywords = "paneer, cheese, vegetarian" },
        });

        // Snacks & Chips - 20 products
        products.AddRange(new[]
        {
            new Product { Id = id++, CategoryId = 5, Name = "Potato Chips (150g)", Price = 1.99m, OriginalPrice = 2.99m, Brand = "CrispyBites", Rating = 4.5, ReviewCount = 200, Sku = "CHIPS001", IntentTags = "snacks, party, chips, crunchy", SearchKeywords = "chips, potato, crispy" },
            new Product { Id = id++, CategoryId = 5, Name = "Cheese Nachos (200g)", Price = 2.49m, Brand = "NachoCheese", Rating = 4.6, ReviewCount = 160, Sku = "NACHOS001", IntentTags = "snacks, party, party_lunch, cheesy", SearchKeywords = "nachos, cheese, snacks" },
            new Product { Id = id++, CategoryId = 5, Name = "Mixed Nuts (300g)", Price = 4.99m, Brand = "NutHouse", Rating = 4.7, ReviewCount = 140, Sku = "NUTS001", IntentTags = "snacks, party, healthy, protein", SearchKeywords = "nuts, mixed, healthy" },
            new Product { Id = id++, CategoryId = 5, Name = "Pretzels (200g)", Price = 2.49m, Brand = "SaltyCrunch", Rating = 4.4, ReviewCount = 120, Sku = "PRETZELS001", IntentTags = "snacks, party, salty", SearchKeywords = "pretzels, salty, crispy" },
            new Product { Id = id++, CategoryId = 5, Name = "Popcorn (100g)", Price = 1.49m, Brand = "PopItUp", Rating = 4.3, ReviewCount = 95, Sku = "POPCORN001", IntentTags = "snacks, party, light", SearchKeywords = "popcorn, light, snack" },
            new Product { Id = id++, CategoryId = 5, Name = "Rava Wafers", Price = 1.99m, Brand = "Traditional", Rating = 4.2, ReviewCount = 80, Sku = "WAFERS001", IntentTags = "snacks, party_lunch, indian", SearchKeywords = "wafers, rava, crispy" },
            new Product { Id = id++, CategoryId = 5, Name = "Granola Bars (6pcs)", Price = 3.99m, Brand = "HealthyMunch", Rating = 4.6, ReviewCount = 110, Sku = "GRANOLA001", IntentTags = "snacks, breakfast, healthy, party", SearchKeywords = "granola, bars, energy" },
            new Product { Id = id++, CategoryId = 5, Name = "Choco Chips Cookies", Price = 2.99m, Brand = "SweetBites", Rating = 4.7, ReviewCount = 130, Sku = "CHOCOCOOKIES001", IntentTags = "snacks, dessert, party, chocolate", SearchKeywords = "chocolate, cookies, sweet" },
            new Product { Id = id++, CategoryId = 5, Name = "Bhujia Mix (250g)", Price = 2.49m, Brand = "Indian Delight", Rating = 4.3, ReviewCount = 90, Sku = "BHUJIA001", IntentTags = "snacks, party_lunch, indian, spicy", SearchKeywords = "bhujia, indian, spicy" },
            new Product { Id = id++, CategoryId = 5, Name = "Dry Fruits Mix", Price = 6.99m, Brand = "Premium Dry Fruits", Rating = 4.8, ReviewCount = 100, Sku = "DRYFRUITS001", IntentTags = "snacks, party, premium, healthy", SearchKeywords = "dry fruits, nuts, premium" },
        });

        // Beverages - 20 products
        products.AddRange(new[]
        {
            new Product { Id = id++, CategoryId = 6, Name = "Cola (2L)", Price = 2.49m, OriginalPrice = 3.49m, Brand = "Refresh", Rating = 4.4, ReviewCount = 180, Sku = "COLA001", IntentTags = "beverages, cold, drinks, party", SearchKeywords = "cola, soda, cold" },
            new Product { Id = id++, CategoryId = 6, Name = "Iced Tea (500ml)", Price = 1.99m, Brand = "ChillTea", Rating = 4.3, ReviewCount = 120, Sku = "ICEDTEA001", IntentTags = "beverages, cold, drinks, party", SearchKeywords = "tea, iced, cold" },
            new Product { Id = id++, CategoryId = 6, Name = "Lemonade (1L)", Price = 2.99m, Brand = "Fresh Squeeze", Rating = 4.5, ReviewCount = 110, Sku = "LEMONADE001", IntentTags = "beverages, cold, fresh, party", SearchKeywords = "lemonade, juice, fresh" },
            new Product { Id = id++, CategoryId = 6, Name = "Fruit Punch (2L)", Price = 3.99m, Brand = "Tropical Taste", Rating = 4.6, ReviewCount = 95, Sku = "PUNCH001", IntentTags = "beverages, cold, fruit, party", SearchKeywords = "punch, fruit, tropical" },
            new Product { Id = id++, CategoryId = 6, Name = "Coffee Instant (200g)", Price = 3.49m, Brand = "AromaBlend", Rating = 4.4, ReviewCount = 85, Sku = "COFFEE001", IntentTags = "beverages, breakfast, hot", SearchKeywords = "coffee, instant, morning" },
            new Product { Id = id++, CategoryId = 6, Name = "Tea Bags (25pcs)", Price = 1.49m, Brand = "Golden Brew", Rating = 4.3, ReviewCount = 100, Sku = "TEABAGS001", IntentTags = "beverages, breakfast, hot", SearchKeywords = "tea, bags, hot" },
            new Product { Id = id++, CategoryId = 6, Name = "Orange Juice (1L)", Price = 2.99m, Brand = "Fresh Orange", Rating = 4.5, ReviewCount = 90, Sku = "ORANGEJUICE001", IntentTags = "beverages, juice, healthy", SearchKeywords = "juice, orange, fresh" },
            new Product { Id = id++, CategoryId = 6, Name = "Mango Juice (1L)", Price = 3.49m, Brand = "Mango King", Rating = 4.6, ReviewCount = 80, Sku = "MANGOJUICE001", IntentTags = "beverages, juice, tropical", SearchKeywords = "juice, mango, tropical" },
            new Product { Id = id++, CategoryId = 6, Name = "Water Bottle (2L)", Price = 0.99m, Brand = "PureWater", Rating = 4.2, ReviewCount = 200, Sku = "WATER001", IntentTags = "beverages, hydration, essentials", SearchKeywords = "water, pure, hydration" },
            new Product { Id = id++, CategoryId = 6, Name = "Coconut Water (500ml)", Price = 1.99m, Brand = "TropicalNutrition", Rating = 4.4, ReviewCount = 105, Sku = "COCONUTWATER001", IntentTags = "beverages, healthy, tropical", SearchKeywords = "coconut, water, healthy" },
        });

        // Spices & Seasonings - 15 products
        products.AddRange(new[]
        {
            new Product { Id = id++, CategoryId = 8, Name = "Turmeric Powder (100g)", Price = 1.99m, Brand = "Pure Spice", Rating = 4.5, ReviewCount = 95, Sku = "TURMERIC001", IntentTags = "spices, cooking, indian", SearchKeywords = "turmeric, spice, golden" },
            new Product { Id = id++, CategoryId = 8, Name = "Chili Powder (100g)", Price = 1.49m, Brand = "Spicy Mix", Rating = 4.3, ReviewCount = 80, Sku = "CHILI001", IntentTags = "spices, cooking, indian, spicy", SearchKeywords = "chili, spice, red" },
            new Product { Id = id++, CategoryId = 8, Name = "Cumin Seeds (100g)", Price = 2.49m, Brand = "Aroma Spice", Rating = 4.4, ReviewCount = 75, Sku = "CUMIN001", IntentTags = "spices, cooking, indian", SearchKeywords = "cumin, seeds, spice" },
            new Product { Id = id++, CategoryId = 8, Name = "Garam Masala (50g)", Price = 2.99m, Brand = "Premium Blend", Rating = 4.6, ReviewCount = 110, Sku = "GARAMMASALA001", IntentTags = "spices, cooking, indian", SearchKeywords = "garam masala, spice, blend" },
            new Product { Id = id++, CategoryId = 8, Name = "Coriander Powder", Price = 1.99m, Brand = "Pure Grind", Rating = 4.3, ReviewCount = 70, Sku = "CORIANDER001", IntentTags = "spices, cooking, indian", SearchKeywords = "coriander, spice, powder" },
        });

        // Beauty & Personal Care - 15 products
        products.AddRange(new[]
        {
            new Product { Id = id++, CategoryId = 9, Name = "Face Wash (100ml)", Price = 2.99m, Brand = "ClearSkin", Rating = 4.5, ReviewCount = 140, Sku = "FACEWASH001", IntentTags = "beauty, skincare, daily", SearchKeywords = "face wash, skincare, cleansing" },
            new Product { Id = id++, CategoryId = 9, Name = "Shampoo (200ml)", Price = 2.49m, OriginalPrice = 3.49m, Brand = "ShimmerLocks", Rating = 4.4, ReviewCount = 160, Sku = "SHAMPOO001", IntentTags = "beauty, hair, daily", SearchKeywords = "shampoo, hair, care" },
            new Product { Id = id++, CategoryId = 9, Name = "Conditioner (200ml)", Price = 2.49m, Brand = "SilkyHair", Rating = 4.3, ReviewCount = 130, Sku = "CONDITIONER001", IntentTags = "beauty, hair, care", SearchKeywords = "conditioner, hair, smooth" },
            new Product { Id = id++, CategoryId = 9, Name = "Moisturizer (50ml)", Price = 3.99m, Brand = "GlowSkin", Rating = 4.6, ReviewCount = 115, Sku = "MOISTURIZER001", IntentTags = "beauty, skincare, daily", SearchKeywords = "moisturizer, moisturizing, skin" },
            new Product { Id = id++, CategoryId = 9, Name = "Toothpaste (100g)", Price = 1.49m, Brand = "BrightSmile", Rating = 4.5, ReviewCount = 180, Sku = "TOOTHPASTE001", IntentTags = "personal-care, daily, essentials", SearchKeywords = "toothpaste, dental, care" },
        });

        // Electronics - 15 products
        products.AddRange(new[]
        {
            new Product { Id = id++, CategoryId = 10, Name = "Phone Charger", Price = 9.99m, Brand = "TechPower", Rating = 4.4, ReviewCount = 150, Sku = "CHARGER001", IntentTags = "electronics, tech, essentials", SearchKeywords = "charger, phone, power" },
            new Product { Id = id++, CategoryId = 10, Name = "USB Cable (3m)", Price = 3.99m, Brand = "ConnectTech", Rating = 4.3, ReviewCount = 120, Sku = "Cable001", IntentTags = "electronics, tech, accessories", SearchKeywords = "cable, usb, connector" },
            new Product { Id = id++, CategoryId = 10, Name = "Wireless Earbuds", Price = 19.99m, Brand = "SoundMax", Rating = 4.6, ReviewCount = 95, Sku = "EARBUDS001", IntentTags = "electronics, audio, tech", SearchKeywords = "earbuds, wireless, audio" },
            new Product { Id = id++, CategoryId = 10, Name = "Power Bank (10000mAh)", Price = 14.99m, Brand = "PowerZone", Rating = 4.5, ReviewCount = 110, Sku = "POWERBANK001", IntentTags = "electronics, tech, essentials", SearchKeywords = "power bank, battery, portable" },
        });

        // Home & Kitchen - 15 products
        products.AddRange(new[]
        {
            new Product { Id = id++, CategoryId = 11, Name = "Non-Stick Pan", Price = 12.99m, Brand = "KitchenMaster", Rating = 4.4, ReviewCount = 95, Sku = "PAN001", IntentTags = "kitchen, cooking, essentials", SearchKeywords = "pan, non-stick, cooking" },
            new Product { Id = id++, CategoryId = 11, Name = "Cooking Spoon", Price = 2.99m, Brand = "HomeEssential", Rating = 4.3, ReviewCount = 70, Sku = "SPOON001", IntentTags = "kitchen, cooking, utensils", SearchKeywords = "spoon, utensil, cooking" },
            new Product { Id = id++, CategoryId = 11, Name = "Cutting Board", Price = 4.99m, Brand = "PrepWare", Rating = 4.5, ReviewCount = 85, Sku = "CUTTINGBOARD001", IntentTags = "kitchen, cooking, essentials", SearchKeywords = "cutting board, prep, kitchen" },
            new Product { Id = id++, CategoryId = 11, Name = "Trash Bin (10L)", Price = 7.99m, Brand = "CleanHome", Rating = 4.2, ReviewCount = 60, Sku = "TRASHBIN001", IntentTags = "home, essentials, organization", SearchKeywords = "trash, bin, waste" },
        });

        return products;
    }

    private List<ProductInventory> CreateInventories(List<Product> products)
    {
        return products.Select(p => new ProductInventory
        {
            ProductId = p.Id,
            Quantity = new Random().Next(5, 200),
            ReorderLevel = 10,
            LastRestockedAt = DateTime.Now.AddDays(-new Random().Next(1, 30))
        }).ToList();
    }
}

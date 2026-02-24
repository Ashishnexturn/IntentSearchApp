using Microsoft.Extensions.Logging;

public class ProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly List<Product> _products;

    public ProductService(ILogger<ProductService> logger)
    {
        _logger = logger;
        _products = InitializeProducts();
    }

    private List<Product> InitializeProducts()
    {
        return new List<Product>
        {
            // Party & Lunch Items
            new Product { Id = 1, Name = "Potato Chips", Category = "Snacks", Intent = "party", Description = "Crispy and delicious potato chips", Price = 2.99m, Quantity = 50 },
            new Product { Id = 2, Name = "Cheese Nachos", Category = "Snacks", Intent = "party", Description = "Cheesy nachos perfect for parties", Price = 4.99m, Quantity = 30 },
            new Product { Id = 3, Name = "Mixed Nuts", Category = "Snacks", Intent = "party", Description = "Healthy mixed nuts assortment", Price = 5.99m, Quantity = 25 },
            new Product { Id = 4, Name = "Pretzels", Category = "Snacks", Intent = "party", Description = "Crispy pretzel sticks", Price = 3.49m, Quantity = 40 },
            
            new Product { Id = 5, Name = "Cold Cola", Category = "Drinks", Intent = "party", Description = "Refreshing cold cola", Price = 1.99m, Quantity = 100 },
            new Product { Id = 6, Name = "Iced Tea", Category = "Drinks", Intent = "party", Description = "Chilled iced tea", Price = 2.49m, Quantity = 80 },
            new Product { Id = 7, Name = "Lemonade", Category = "Drinks", Intent = "party", Description = "Fresh lemonade", Price = 3.99m, Quantity = 60 },
            new Product { Id = 8, Name = "Fruit Punch", Category = "Drinks", Intent = "party", Description = "Tropical fruit punch", Price = 4.49m, Quantity = 45 },
            
            new Product { Id = 9, Name = "Sandwich Platter", Category = "Food", Intent = "lunch", Description = "Assorted sandwiches", Price = 12.99m, Quantity = 15 },
            new Product { Id = 10, Name = "Pasta Salad", Category = "Food", Intent = "lunch", Description = "Fresh pasta salad", Price = 8.99m, Quantity = 20 },
            new Product { Id = 11, Name = "Chicken Wrap", Category = "Food", Intent = "lunch", Description = "Grilled chicken wrap", Price = 7.99m, Quantity = 25 },
            new Product { Id = 12, Name = "Veggie Tray", Category = "Food", Intent = "lunch", Description = "Fresh vegetables with dip", Price = 9.99m, Quantity = 18 },
            
            // Party Lunch Combined
            new Product { Id = 13, Name = "Party Mix", Category = "Snacks", Intent = "party_lunch", Description = "Mix of chips and pretzels", Price = 6.99m, Quantity = 35 },
            new Product { Id = 14, Name = "Meatballs", Category = "Food", Intent = "party_lunch", Description = "Swedish meatballs", Price = 10.99m, Quantity = 20 },
            new Product { Id = 15, Name = "Mini Quiches", Category = "Food", Intent = "party_lunch", Description = "Assorted mini quiches", Price = 11.99m, Quantity = 12 },
            
            // General Products
            new Product { Id = 16, Name = "Coffee", Category = "Drinks", Intent = "general", Description = "Fresh brewed coffee", Price = 3.49m, Quantity = 90 },
            new Product { Id = 17, Name = "Water", Category = "Drinks", Intent = "general", Description = "Bottled water", Price = 1.49m, Quantity = 200 },
            new Product { Id = 18, Name = "Cookies", Category = "Snacks", Intent = "general", Description = "Chocolate chip cookies", Price = 2.49m, Quantity = 70 },
            new Product { Id = 19, Name = "Brownies", Category = "Snacks", Intent = "general", Description = "Fudgy brownies", Price = 3.99m, Quantity = 50 }
        };
    }

    public List<Product> GetProductsByIntent(string intent, string keyword)
    {
        _logger.LogInformation("Fetching products for intent: {Intent}, keyword: {Keyword}", intent, keyword);

        // Normalize intent and keyword
        var normalizedIntent = intent?.ToLower() ?? "";
        var normalizedKeyword = keyword?.ToLower() ?? "";

        var matchingProducts = _products
            .Where(p => 
            {
                // Match by exact intent
                if (p.Intent == normalizedIntent)
                    return true;

                // Match combined intent like "party_lunch"
                if (normalizedIntent.Contains("party") && normalizedIntent.Contains("lunch"))
                {
                    if (p.Intent == "party_lunch" || p.Intent == "party" || p.Intent == "lunch")
                        return true;
                }

                // Match by keywords in product name or category
                if (!string.IsNullOrEmpty(normalizedKeyword))
                {
                    return p.Name.ToLower().Contains(normalizedKeyword) ||
                           p.Category.ToLower().Contains(normalizedKeyword) ||
                           p.Description.ToLower().Contains(normalizedKeyword);
                }

                return false;
            })
            .OrderByDescending(p => p.Quantity > 0) // Show in-stock items first
            .ThenBy(p => p.Name)
            .ToList();

        _logger.LogInformation("Found {Count} products for intent: {Intent}", matchingProducts.Count, intent);
        return matchingProducts;
    }

    public List<Product> SearchByKeyword(string query)
    {
        _logger.LogInformation("Searching products by keyword: {Query}", query);

        if (string.IsNullOrEmpty(query))
            return new List<Product>();

        var normalizedQuery = query.ToLower();
        return _products
            .Where(p =>
                p.Name.ToLower().Contains(normalizedQuery) ||
                p.Category.ToLower().Contains(normalizedQuery) ||
                p.Description.ToLower().Contains(normalizedQuery) ||
                p.Intent.ToLower().Contains(normalizedQuery)
            )
            .OrderByDescending(p => p.Quantity > 0)
            .ThenBy(p => p.Name)
            .ToList();
    }

    public Product GetProductById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public List<Product> GetAllProducts()
    {
        return _products.OrderBy(p => p.Category).ThenBy(p => p.Name).ToList();
    }
}

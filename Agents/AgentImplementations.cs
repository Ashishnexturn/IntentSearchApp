public class IntentAgent : IIntentAgent
{
    private readonly IntentService _intentService;
    private readonly ILogger<IntentAgent> _logger;

    public IntentAgent(IntentService intentService, ILogger<IntentAgent> logger)
    {
        _intentService = intentService;
        _logger = logger;
    }

    public async Task<IntentResult> DetectIntentAsync(string query)
    {
        _logger.LogInformation("IntentAgent: Detecting intent for query: {Query}", query);
        
        var result = await _intentService.DetectIntent(query);
        
        _logger.LogInformation("IntentAgent: Detected intent={Intent}, keyword={Keyword}, category={Category}", 
            result.Intent, result.Keyword, result.Category);
        
        return result;
    }
}

public class KeywordAgent : IKeywordAgent
{
    private readonly ILogger<KeywordAgent> _logger;

    public KeywordAgent(ILogger<KeywordAgent> logger)
    {
        _logger = logger;
    }

    public async Task<List<string>> ExtractKeywordsAsync(string query)
    {
        _logger.LogInformation("KeywordAgent: Extracting keywords from: {Query}", query);
        
        // Extract keywords by splitting and filtering
        var keywords = query
            .Split(new[] { " ", ",", ".", ";", "-" }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length > 2)
            .Select(w => w.ToLower())
            .Distinct()
            .ToList();

        _logger.LogInformation("KeywordAgent: Extracted {Count} keywords", keywords.Count);
        return await Task.FromResult(keywords);
    }

    public async Task<List<string>> ExpandKeywordsAsync(List<string> keywords)
    {
        _logger.LogInformation("KeywordAgent: Expanding {Count} keywords", keywords.Count);
        
        // Keyword expansion mapping
        var expansions = new Dictionary<string, List<string>>
        {
            { "party", new List<string> { "celebration", "gathering", "event", "festive" } },
            { "lunch", new List<string> { "meal", "midday", "food", "eating" } },
            { "snacks", new List<string> { "chips", "nuts", "crisps", "bites" } },
            { "drinks", new List<string> { "beverage", "cold", "refreshing" } },
            { "grocery", new List<string> { "groceries", "essentials", "daily", "staples" } },
            { "kitchen", new List<string> { "cooking", "utensils", "appliances" } },
            { "beauty", new List<string> { "cosmetics", "skincare", "personal-care" } },
            { "electronics", new List<string> { "gadgets", "devices", "tech" } }
        };

        var expanded = new List<string>(keywords);
        foreach (var keyword in keywords)
        {
            if (expansions.ContainsKey(keyword))
            {
                expanded.AddRange(expansions[keyword]);
            }
        }

        var result = expanded.Distinct().ToList();
        _logger.LogInformation("KeywordAgent: Expanded to {Count} keywords", result.Count);
        return await Task.FromResult(result);
    }
}

public class CatalogAgent : ICatalogAgent
{
    private readonly IProductRepository _repository;
    private readonly ILogger<CatalogAgent> _logger;

    public CatalogAgent(IProductRepository repository, ILogger<CatalogAgent> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<Product>> SearchByCatalogAsync(string intentTag, List<string> keywords)
    {
        _logger.LogInformation("CatalogAgent: Searching catalog with intent={Intent}, keywords={KeywordCount}", 
            intentTag, keywords.Count);

        var results = new List<Product>();

        // First search by intent tags
        if (!string.IsNullOrEmpty(intentTag))
        {
            var byIntent = await _repository.GetProductsByIntentTagsAsync(intentTag);
            results.AddRange(byIntent);
            _logger.LogInformation("CatalogAgent: Found {Count} products by intent", byIntent.Count);
        }

        // Then search by keywords
        foreach (var keyword in keywords)
        {
            var byKeyword = await _repository.GetProductsBySearchKeywordsAsync(keyword);
            results.AddRange(byKeyword);
        }

        // Deduplicate and rank by rating
        var uniqueResults = results
            .GroupBy(p => p.Id)
            .Select(g => g.First())
            .OrderByDescending(p => p.Rating)
            .ThenByDescending(p => p.ReviewCount)
            .ToList();

        _logger.LogInformation("CatalogAgent: Returning {Count} unique products", uniqueResults.Count);
        return uniqueResults;
    }

    public async Task<List<Product>> GetRelatedProductsAsync(int productId)
    {
        _logger.LogInformation("CatalogAgent: Finding related products for product={ProductId}", productId);

        var product = await _repository.GetProductByIdAsync(productId);
        if (product == null)
            return new List<Product>();

        // Get products from same category
        var related = await _repository.GetProductsByCategoryAsync(product.CategoryId);
        
        // Filter out the current product and take top 6
        var result = related
            .Where(p => p.Id != productId)
            .OrderByDescending(p => p.Rating)
            .Take(6)
            .ToList();

        _logger.LogInformation("CatalogAgent: Found {Count} related products", result.Count);
        return result;
    }
}

public class InventoryAgent : IInventoryAgent
{
    private readonly IProductRepository _repository;
    private readonly ILogger<InventoryAgent> _logger;

    public InventoryAgent(IProductRepository repository, ILogger<InventoryAgent> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> CheckAvailabilityAsync(int productId)
    {
        var inStock = await _repository.IsProductInStockAsync(productId);
        _logger.LogInformation("InventoryAgent: Product={ProductId} InStock={InStock}", productId, inStock);
        return inStock;
    }

    public async Task<List<Product>> GetAvailableProductsAsync(List<Product> products)
    {
        _logger.LogInformation("InventoryAgent: Filtering {Count} products for availability", products.Count);

        var available = new List<Product>();
        foreach (var product in products)
        {
            if (product.Inventory?.Quantity > 0)
            {
                available.Add(product);
            }
        }

        _logger.LogInformation("InventoryAgent: {Count} products available in stock", available.Count);
        return await Task.FromResult(available);
    }

    public async Task<int> GetStockCountAsync(int productId)
    {
        var inventory = await _repository.GetInventoryByProductIdAsync(productId);
        return inventory?.Quantity ?? 0;
    }
}

public class SearchOrchestrator : ISearchOrchestrator
{
    private readonly IIntentAgent _intentAgent;
    private readonly IKeywordAgent _keywordAgent;
    private readonly ICatalogAgent _catalogAgent;
    private readonly IInventoryAgent _inventoryAgent;
    private readonly IProductRepository _repository;
    private readonly ILogger<SearchOrchestrator> _logger;

    public SearchOrchestrator(
        IIntentAgent intentAgent,
        IKeywordAgent keywordAgent,
        ICatalogAgent catalogAgent,
        IInventoryAgent inventoryAgent,
        IProductRepository repository,
        ILogger<SearchOrchestrator> logger)
    {
        _intentAgent = intentAgent;
        _keywordAgent = keywordAgent;
        _catalogAgent = catalogAgent;
        _inventoryAgent = inventoryAgent;
        _repository = repository;
        _logger = logger;
    }

    public async Task<SearchResultViewModel> SearchAsync(string query)
    {
        _logger.LogInformation("SearchOrchestrator: Starting orchestrated search for: {Query}", query);

        var viewModel = new SearchResultViewModel
        {
            SearchQuery = query,
            Intent = null,
            Products = new List<Product>()
        };

        try
        {
            // Step 1: Detect Intent
            _logger.LogInformation("SearchOrchestrator: Step 1 - Detecting intent");
            var intent = await _intentAgent.DetectIntentAsync(query);
            viewModel.Intent = intent;

            // Step 2: Extract and Expand Keywords
            _logger.LogInformation("SearchOrchestrator: Step 2 - Extracting keywords");
            var keywords = await _keywordAgent.ExtractKeywordsAsync(query);
            var expandedKeywords = await _keywordAgent.ExpandKeywordsAsync(keywords);

            // Step 3: Search Catalog
            _logger.LogInformation("SearchOrchestrator: Step 3 - Searching catalog");
            var products = await _catalogAgent.SearchByCatalogAsync(intent.Intent, expandedKeywords);

            // Step 4: Check Inventory
            _logger.LogInformation("SearchOrchestrator: Step 4 - Checking inventory");
            var availableProducts = await _inventoryAgent.GetAvailableProductsAsync(products);

            viewModel.Products = availableProducts;
            viewModel.TotalProducts = availableProducts.Count;

            // Step 5: Log Search
            _logger.LogInformation("SearchOrchestrator: Step 5 - Logging search");
            await _repository.LogSearchAsync(query, intent.Intent, intent.Keyword, availableProducts.Count);

            _logger.LogInformation("SearchOrchestrator: Search completed. Found {Count} products", availableProducts.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchOrchestrator: Error during orchestrated search");
            throw;
        }

        return viewModel;
    }

    public async Task<List<Product>> ExecuteIntentSearchAsync(string query)
    {
        _logger.LogInformation("SearchOrchestrator: Executing intent search for: {Query}", query);

        var intent = await _intentAgent.DetectIntentAsync(query);
        var keywords = await _keywordAgent.ExtractKeywordsAsync(query);
        var expandedKeywords = await _keywordAgent.ExpandKeywordsAsync(keywords);
        var products = await _catalogAgent.SearchByCatalogAsync(intent.Intent, expandedKeywords);
        var available = await _inventoryAgent.GetAvailableProductsAsync(products);

        return available;
    }

    public async Task<SearchResultViewModel> GetSearchResultsAsync(string query)
    {
        return await SearchAsync(query);
    }
}

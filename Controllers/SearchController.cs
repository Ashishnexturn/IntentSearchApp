using Microsoft.AspNetCore.Mvc;

namespace IntentSearchApp.Controllers;

public class SearchController : Controller
{
    private readonly IntentService _intentService;
    private readonly ProductService _productService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(IntentService intentService, ProductService productService, ILogger<SearchController> logger)
    {
        _intentService = intentService;
        _productService = productService;
        _logger = logger;
    }

    [HttpPost]
    [Route("/Search")]
    public async Task<IActionResult> Search(string query)
    {
        _logger.LogInformation("Search initiated with query: {Query}", query);

        var intent = await _intentService.DetectIntent(query);

        // Get products based on detected intent and keyword
        var products = _productService.GetProductsByIntent(intent.Intent, intent.Keyword);

        // If no products found by intent, try keyword search
        if (!products.Any())
        {
            _logger.LogInformation("No products found by intent, trying keyword search");
            products = _productService.SearchByKeyword(query);
        }

        var viewModel = new SearchResultViewModel
        {
            Intent = intent,
            Products = products,
            SearchQuery = query,
            TotalProducts = products.Count
        };

        _logger.LogInformation("Search completed. Found {Count} products for intent: {Intent}", products.Count, intent.Intent);

        // Route based on product results
        if (products.Any())
        {
            return View("ProductResults", viewModel);
        }
        else if (intent.Intent == "CategorySearch")
        {
            return View("CategoryResults", viewModel);
        }

        return View("GeneralResults", viewModel);
    }
}

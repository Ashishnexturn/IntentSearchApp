using Microsoft.AspNetCore.Mvc;

namespace IntentSearchApp.Controllers;

public class SearchController : Controller
{
    private readonly ISearchOrchestrator _searchOrchestrator;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ISearchOrchestrator searchOrchestrator, ILogger<SearchController> logger)
    {
        _searchOrchestrator = searchOrchestrator;
        _logger = logger;
    }

    [HttpPost]
    [Route("/Search")]
    public async Task<IActionResult> Search(string query)
    {
        _logger.LogInformation("SearchController: Search initiated with query: {Query}", query);

        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Search query cannot be empty");
        }

        try
        {
            // Use the orchestrator to perform the complete search
            var viewModel = await _searchOrchestrator.SearchAsync(query);

            _logger.LogInformation("SearchController: Search completed. Found {Count} products", viewModel.TotalProducts);

            // Display results
            return View("ProductResults", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SearchController: Error during search for query: {Query}", query);
            return View("GeneralResults", new SearchResultViewModel
            {
                SearchQuery = query,
                Products = new List<Product>(),
                Intent = new IntentResult()
            });
        }
    }

    [HttpGet]
    [Route("/Browse")]
    public async Task<IActionResult> Browse(int? categoryId)
    {
        _logger.LogInformation("SearchController: Browsing with categoryId: {CategoryId}", categoryId);
        
        return View("Browse");
    }
}


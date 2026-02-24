using Microsoft.AspNetCore.Mvc;

namespace IntentSearchApp.Controllers;

public class SearchController : Controller
{
    private readonly IntentService _intentService;

    public SearchController(IntentService intentService)
    {
        _intentService = intentService;
    }

    [HttpPost]
    [Route("Search/Search")]
    public async Task<IActionResult> Search(string query)
    {
        var intent = await _intentService.DetectIntent(query);

        // Route based on intent
        if (intent.Intent == "ProductSearch")
        {
            return View("ProductResults", intent);
        }
        else if (intent.Intent == "CategorySearch")
        {
            return View("CategoryResults", intent);
        }

        return View("GeneralResults", intent);
    }
}
// Agent interfaces for orchestrated search
public interface IIntentAgent
{
    Task<IntentResult> DetectIntentAsync(string query);
}

public interface IKeywordAgent
{
    Task<List<string>> ExtractKeywordsAsync(string query);
    Task<List<string>> ExpandKeywordsAsync(List<string> keywords);
}

public interface ICatalogAgent
{
    Task<List<Product>> SearchByCatalogAsync(string intentTag, List<string> keywords);
    Task<List<Product>> GetRelatedProductsAsync(int productId);
}

public interface IInventoryAgent
{
    Task<bool> CheckAvailabilityAsync(int productId);
    Task<List<Product>> GetAvailableProductsAsync(List<Product> products);
    Task<int> GetStockCountAsync(int productId);
}

public interface ISearchOrchestrator
{
    Task<SearchResultViewModel> SearchAsync(string query);
    Task<List<Product>> ExecuteIntentSearchAsync(string query);
    Task<SearchResultViewModel> GetSearchResultsAsync(string query);
}

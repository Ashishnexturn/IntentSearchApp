public class SearchResultViewModel
{
    public IntentResult Intent { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();
    public string SearchQuery { get; set; }
    public int TotalProducts { get; set; }
}

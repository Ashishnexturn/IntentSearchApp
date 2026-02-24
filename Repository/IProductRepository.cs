public interface IProductRepository
{
    // Query methods
    Task<List<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(int productId);
    Task<List<Product>> GetProductsByNameAsync(string name);
    Task<List<Product>> GetProductsByIntentTagsAsync(string intentTag);
    Task<List<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<List<Product>> GetProductsBySearchKeywordsAsync(string keywords);
    Task<List<Product>> GetFeaturedProductsAsync();
    Task<List<Product>> GetProductsByFiltersAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, double? minRating);
    
    // Category methods
    Task<List<Category>> GetAllCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(int categoryId);
    Task<Category> GetCategoryByNameAsync(string name);
    
    // Inventory methods
    Task<ProductInventory> GetInventoryByProductIdAsync(int productId);
    Task<bool> IsProductInStockAsync(int productId);
    
    // Review methods
    Task<List<ProductReview>> GetProductReviewsAsync(int productId);
    
    // Search logging
    Task LogSearchAsync(string query, string detectedIntent, string keyword, int resultCount);
    
    // Add/Update methods
    Task<Product> AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int productId);
    Task<Category> AddCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
}

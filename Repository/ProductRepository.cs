using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(AppDbContext context, ILogger<ProductRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Product> GetProductByIdAsync(int productId)
    {
        return await _context.Products
            .Where(p => p.IsActive && p.Id == productId)
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Product>> GetProductsByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new List<Product>();

        var searchTerm = name.ToLower();
        return await _context.Products
            .Where(p => p.IsActive && p.Name.ToLower().Contains(searchTerm))
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<List<Product>> GetProductsByIntentTagsAsync(string intentTag)
    {
        if (string.IsNullOrWhiteSpace(intentTag))
            return new List<Product>();

        var tagLower = intentTag.ToLower();
        return await _context.Products
            .Where(p => p.IsActive && p.IntentTags.ToLower().Contains(tagLower))
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .OrderByDescending(p => p.Rating)
            .ToListAsync();
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .Where(p => p.IsActive && p.CategoryId == categoryId)
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<List<Product>> GetProductsBySearchKeywordsAsync(string keywords)
    {
        if (string.IsNullOrWhiteSpace(keywords))
            return new List<Product>();

        var searchTerm = keywords.ToLower();
        return await _context.Products
            .Where(p => p.IsActive && (
                p.Name.ToLower().Contains(searchTerm) ||
                p.Description.ToLower().Contains(searchTerm) ||
                p.SearchKeywords.ToLower().Contains(searchTerm) ||
                p.IntentTags.ToLower().Contains(searchTerm)
            ))
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .OrderByDescending(p => p.Rating)
            .ToListAsync();
    }

    public async Task<List<Product>> GetFeaturedProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive && p.IsFeatured)
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .OrderByDescending(p => p.Rating)
            .Take(20)
            .ToListAsync();
    }

    public async Task<List<Product>> GetProductsByFiltersAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, double? minRating)
    {
        var query = _context.Products.Where(p => p.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice);

        if (minRating.HasValue)
            query = query.Where(p => p.Rating >= minRating);

        return await query
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .OrderByDescending(p => p.Rating)
            .ToListAsync();
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category> GetCategoryByIdAsync(int categoryId)
    {
        return await _context.Categories
            .Where(c => c.IsActive && c.Id == categoryId)
            .Include(c => c.Products.Where(p => p.IsActive))
            .FirstOrDefaultAsync();
    }

    public async Task<Category> GetCategoryByNameAsync(string name)
    {
        return await _context.Categories
            .Where(c => c.IsActive && c.Name.ToLower() == name.ToLower())
            .FirstOrDefaultAsync();
    }

    public async Task<ProductInventory> GetInventoryByProductIdAsync(int productId)
    {
        return await _context.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId);
    }

    public async Task<bool> IsProductInStockAsync(int productId)
    {
        var inventory = await GetInventoryByProductIdAsync(productId);
        return inventory?.Quantity > 0;
    }

    public async Task<List<ProductReview>> GetProductReviewsAsync(int productId)
    {
        return await _context.Reviews
            .Where(r => r.ProductId == productId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task LogSearchAsync(string query, string detectedIntent, string keyword, int resultCount)
    {
        var searchLog = new SearchLog
        {
            SearchQuery = query,
            DetectedIntent = detectedIntent,
            Keyword = keyword,
            ResultCount = resultCount,
            SearchedAt = DateTime.Now
        };

        _context.SearchLogs.Add(searchLog);
        await _context.SaveChangesAsync();
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task UpdateProductAsync(Product product)
    {
        product.UpdatedAt = DateTime.Now;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int productId)
    {
        var product = await GetProductByIdAsync(productId);
        if (product != null)
        {
            product.IsActive = false;
            await UpdateProductAsync(product);
        }
    }

    public async Task<Category> AddCategoryAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        category.UpdatedAt = DateTime.Now;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }
}

# Architecture Documentation

## System Design Overview

the IntentSearchApp uses a **Multi-Agent Orchestration Pattern** combined with **Clean Architecture** principles to create a scalable, maintainable, and extensible e-commerce search engine.

## Design Patterns Implemented

### 1. Multi-Agent Pattern
Each agent is responsible for a specific concern:
- **Separation of Concerns**: Each agent handles one responsibility
- **Pluggable Architecture**: Agents can be replaced or extended independently
- **Scalability**: New agents can be added for new features

### 2. Repository Pattern
Data access is abstracted through repositories:
- `IProductRepository` - Interface defines contract
- `ProductRepository` - Implementation uses Entity Framework
- Benefits: Testability, loose coupling, easy mocking

### 3. Dependency Injection
All services are registered in the container:
```csharp
// Program.cs
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISearchOrchestrator, SearchOrchestrator>();
builder.Services.AddScoped<IIntentAgent, IntentAgent>();
builder.Services.AddScoped<IKeywordAgent, KeywordAgent>();
builder.Services.AddScoped<ICatalogAgent, CatalogAgent>();
builder.Services.AddScoped<IInventoryAgent, InventoryAgent>();
```

### 4. MVC Pattern
Clean separation between Models, Views, and Controllers for maintainability

## Orchestration Flow

### Search Request Lifecycle

```
1. User Input
   ↓
2. SearchController.Search(query)
   ↓
3. ISearchOrchestrator.SearchAsync(query)
   ↓
4. Step 1: Intent Detection
   - IIntentAgent.DetectIntentAsync(query)
   - Uses Groq AI via IntentService
   - Returns: intentResult (intent, keyword, category)
   ↓
5. Step 2: Keyword Extraction & Expansion
   - IKeywordAgent.ExtractKeywordsAsync(query)
   - IKeywordAgent.ExpandKeywordsAsync(keywords)
   - Returns: expandedKeywords
   ↓
6. Step 3: Catalog Search
   - ICatalogAgent.SearchByCatalogAsync(intent, keywords)
   - Queries ProductRepository with multiple conditions
   - Returns: ranked list of products
   ↓
7. Step 4: Inventory Check
   - IInventoryAgent.GetAvailableProductsAsync(products)
   - Filters by stock availability
   - Returns: available products only
   ↓
8. Step 5: Analytics
   - IProductRepository.LogSearchAsync(...)
   - Logs search for future analysis
   ↓
9. Result
   - SearchResultViewModel with products
   - Returned to View for rendering
```

## Data Layer Architecture

### Entity Framework Core Setup

**DbContext Configuration:**
```csharp
public class AppDbContext : DbContext
{
    // DbSets
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductInventory> Inventories { get; set; }
    
    // Relationships configured in OnModelCreating
    // Indexes, constraints, and migrations handled automatically
}
```

**Relationships:**
- Category (1) → (Many) Product
- Category (1) → (Many) SubCategory
- Product (1) → (1) ProductInventory
- Product (1) → (Many) ProductReview

**Key Configurations:**
- Cascade delete where appropriate
- Unique constraints on natural keys (SKU)
- Indexes on frequently searched columns
- Default values for active status

### Repository Methods

**Query Operations:**
- `GetAllProductsAsync()` - All active products
- `GetProductsByIntentTagsAsync(tag)` - Intent-based search
- `GetProductsBySearchKeywordsAsync(keywords)` - Full-text search
- `GetProductsByFiltersAsync(...)` - Complex filtering
- `GetFeaturedProductsAsync()` - Promotional products

**Aggregation:**
- `GetAllCategoriesAsync()` - Organized categories
- `GetInventoryByProductIdAsync()` - Stock levels
- `GetProductReviewsAsync()` - Ratings and reviews

**Analytics:**
- `LogSearchAsync(...)` - Search tracking

## Query Optimization

### Eager Loading
```csharp
// Include related entities to avoid N+1 queries
await query
    .Include(p => p.Category)
    .Include(p => p.Inventory)
    .Include(p => p.Reviews)
    .ToListAsync();
```

### Indexing Strategy
```sql
-- Key indexes for performance
CREATE INDEX idx_Intent_Tags ON Products(IntentTags);
CREATE INDEX idx_Keywords ON Products(SearchKeywords);
CREATE INDEX idx_ProductId ON ProductInventory(ProductId);
CREATE INDEX idx_Category ON Products(CategoryId);
```

### Filtering & Sorting
- Filter by intent first (most restrictive)
- Then by keywords (broader search)
- Sort by relevance (rating, review count)
- Prioritize in-stock items

## Agent Implementation Details

### Intent Agent
```csharp
public class IntentAgent : IIntentAgent
{
    private readonly IntentService _intentService; // Calls Groq AI
    
    public async Task<IntentResult> DetectIntentAsync(string query)
    {
        // Refined AI prompt for shopping intent
        // Returns: intent classification + keywords
    }
}
```

**AI Prompt Structure:**
- Analyzes user query
- Maps to shopping intents
- Extracts meaningful keywords
- Suggests product category

### Keyword Agent
```csharp
public class KeywordAgent : IKeywordAgent
{
    public async Task<List<string>> ExtractKeywordsAsync(string query)
    {
        // Tokenize query
        // Remove stop words
        // Return meaningful terms
    }
    
    public async Task<List<string>> ExpandKeywordsAsync(List<string> keywords)
    {
        // Use synonym mapping
        // Expand with related terms
        // Example: "party" → ["celebration", "gathering", "event"]
    }
}
```

**Expansion Mappings:**
```csharp
var expansions = new Dictionary<string, List<string>>
{
    { "party", new[] { "celebration", "gathering", "event", "festive" } },
    { "lunch", new[] { "meal", "midday", "food" } },
    { "snacks", new[] { "chips", "nuts", "crisps", "bites" } },
    { "drinks", new[] { "beverage", "cold", "refreshing" } }
};
```

### Catalog Agent
```csharp
public class CatalogAgent : ICatalogAgent
{
    private readonly IProductRepository _repository;
    
    public async Task<List<Product>> SearchByCatalogAsync(
        string intentTag, 
        List<string> keywords)
    {
        // Search by intent tags first
        // Then by expanded keywords
        // Deduplicate and rank by relevance
    }
}
```

**Search Strategy:**
1. Intent-based search (specific)
2. Keyword search (broader)
3. Deduplication
4. Ranking by rating and reviews

### Inventory Agent
```csharp
public class InventoryAgent : IInventoryAgent
{
    public async Task<List<Product>> GetAvailableProductsAsync(
        List<Product> products)
    {
        // Filter out-of-stock items
        // Prioritize high-stock items
        // Return available products only
    }
}
```

**Inventory Logic:**
- Check `ProductInventory.Quantity > 0`
- Sort by quantity (descending)
- Handle reorder levels for alerts

### Search Orchestrator
```csharp
public class SearchOrchestrator : ISearchOrchestrator
{
    public async Task<SearchResultViewModel> SearchAsync(string query)
    {
        // Coordinate all agents in sequence
        // Handle errors gracefully
        // Log analytics
        // Return comprehensive results
    }
}
```

## Database Seeding

### DataSeederService
```csharp
public class DataSeederService
{
    public async Task SeedDataAsync()
    {
        // Check if already seeded
        // Seed categories (12 total)
        // Seed products (200+)
        // Seed inventory for each product
        // Seed relationships
    }
}
```

**Seeding Strategy:**
- Executed on application startup
- Idempotent (checks if data exists)
- Uses batch operations for performance
- Includes realistic data with:
  - Various price points
  - Ratings and review counts
  - Intent tags for searching
  - Random stock levels

## Logging & Debugging

### Structured Logging
```csharp
_logger.LogInformation("SearchOrchestrator: Starting orchestrated search for: {Query}", query);
_logger.LogInformation("IntentAgent: Detected intent={Intent}, keyword={Keyword}", 
    result.Intent, result.Keyword);
```

**Log Levels:**
- **Information**: User actions, search queries
- **Debug**: Detailed agent operations
- **Error**: Failures and exceptions

### Search Analytics
Every search is logged to database:
```csharp
public class SearchLog
{
    public string SearchQuery { get; set; }
    public string DetectedIntent { get; set; }
    public string Keyword { get; set; }
    public int ResultCount { get; set; }
    public DateTime SearchedAt { get; set; }
}
```

## Performance Considerations

### Database
- Connection pooling enabled
- Asynchronous queries
- Proper indexing strategy
- Include only necessary related entities

### Caching Strategy (Future)
```csharp
// Cache frequently searched intents
private static readonly IMemoryCache _cache;
var cacheKey = $"intent_{query.ToHash()}";
```

### Scalability
- Agents can be distributed across microservices
- Use message queues for async processing
- Add read replicas for reporting
- Implement CQRS for high-load scenarios

## Testing Strategy

### Unit Testing
- Test each agent independently
- Mock IProductRepository
- Verify orchestrator coordination

### Integration Testing
- Test with actual database
- Verify end-to-end search flow
- Test error handling

### Load Testing
- Simulate concurrent searches
- Monitor database performance
- Identify bottlenecks

## Security Implementation

### Input Validation
```csharp
if (string.IsNullOrWhiteSpace(query))
    return BadRequest("Search query cannot be empty");
```

### SQL Injection Prevention
- Entity Framework parameterized queries
- No string concatenation in queries

### API Security
- Groq API key from secure configuration
- HTTPS enforced
- CORS will be configured for APIs

## Error Handling

### Graceful Degradation
```csharp
try
{
    // Orchestrated search
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error during search");
    // Return empty results instead of crashing
    return new SearchResultViewModel { Products = new List<Product>() };
}
```

## Future Architecture Enhancements

### Microservices
- Intent Service (separate)
- Catalog Service (separate)
- Inventory Service (separate)

### Message Queue
- Search requests through queue
- Asynchronous processing
- Publish search events

### Machine Learning
- Improve intent detection
- Personalized recommendations
- User behavior analysis

### GraphQL
- Replace REST with GraphQL
- Better client control over fields
- Real-time subscriptions

---

**This architecture provides a solid foundation for scaling to millions of products and users while maintaining code quality and system reliability.**

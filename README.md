# Intent Search App - AI-Powered E-Commerce Search

A sophisticated, production-ready e-commerce search engine powered by AI and multi-agent orchestration architecture. Built with ASP.NET Core 8, Entity Framework Core, and Groq AI.

## 🎯 Project Overview

IntentSearchApp is a generic, intent-based search system that allows users to search for products using natural language queries. The application uses multiple intelligent agents working in orchestration to understand user intent, extract keywords, search catalogs, and check inventory in real-time.

### Key Features

- **🧠 AI-Powered Intent Detection** - Uses Groq AI to understand user intent and shopping context
- **🔍 Multi-Agent Orchestration** - Five specialized agents work together for accurate results
- **📦 E-Commerce Scale Database** - 200+ products across 12 categories (Blinkit-style)
- **⚡ Real-Time Inventory** - Live stock checking for all products
- **🏗️ Enterprise Architecture** - Repository pattern, dependency injection, clean code principles
- **📊 Search Analytics** - Logs all searches for analysis

## 🏛️ Architecture Overview

### Multi-Agent Orchestration System

The application uses a sophisticated orchestration pattern with 5 specialized agents:

```
User Query
    ↓
[Intent Agent] → Detects shopping intent using AI
    ↓
[Keyword Agent] → Extracts & expands keywords
    ↓
[Catalog Agent] → Searches product database
    ↓
[Inventory Agent] → Filters by availability
    ↓
[Products] → Ranked & returned to user
```

### Agent Responsibilities

1. **Intent Agent** (`IIntentAgent`)
   - Uses Groq AI to analyze user queries
   - Detects shopping context (party, lunch, daily shopping, etc.)
   - Returns intent classification and keywords

2. **Keyword Agent** (`IKeywordAgent`)
   - Extracts relevant keywords from queries
   - Expands keywords with synonyms
   - Maps keywords to product attributes

3. **Catalog Agent** (`ICatalogAgent`)
   - Searches products by intent tags
   - Full-text search across product names, descriptions, and keywords
   - Returns ranked results by rating and relevance

4. **Inventory Agent** (`IInventoryAgent`)
   - Checks real-time stock availability
   - Filters out-of-stock items
   - Prioritizes in-stock products

5. **Search Orchestrator** (`ISearchOrchestrator`)
   - Coordinates all agents
   - Manages the search workflow
   - Logs search analytics

## 💾 Database Schema

### Core Entities

- **Category** - Product categories with display ordering
- **SubCategory** - Sub-categories for better organization
- **Product** - Product details with pricing, ratings, and intent tags
- **ProductInventory** - Real-time stock levels
- **ProductReview** - Customer reviews and ratings
- **SearchLog** - Analytics on user searches

### Key Features

- Intent-based tagging for smart matching
- Full-text search keywords
- Price tracking with discounts
- Real-time inventory management
- SEO-friendly slugs

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or Express)
- Visual Studio or VS Code

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/Ashishnexturn/IntentSearchApp.git
cd IntentSearchApp
```

2. **Configure Database Connection**
Edit `appsettings.json` and set your connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=IntentSearchDb;Trusted_Connection=true;"
  }
}
```

3. **Set Groq API Key**
Add your Groq API key to `appsettings.json`:
```json
{
  "Groq": {
    "ApiKey": "your_api_key_here",
    "BaseUrl": "https://api.groq.com/openai/v1"
  }
}
```

4. **Run the Application**
```bash
dotnet run
```

The application will automatically:
- Create the database
- Run migrations
- Seed 200+ products across 12 categories

## 🔍 Usage Examples

### Search Queries

Try these searches in the application:

- **"party snacks"** - Returns party-appropriate snack foods
- **"healthy breakfast"** - Shows breakfast items and healthy options
- **"cold drinks for party"** - Returns cold beverages suitable for parties
- **"fresh vegetables"** - Shows fresh produce
- **"baking ingredients"** - Returns items needed for baking
- **"beauty products"** - Shows personal care and beauty items

### How It Works

1. User enters a natural language query
2. Intent Agent analyzes the query using AI
3. Keyword Agent extracts and expands search terms
4. Catalog Agent searches product database
5. Inventory Agent filters by availability
6. Results displayed with ratings, prices, and stock status

## 📊 Product Database

### Categories (12 Total)

1. **Groceries** - Essential food items
2. **Fruits & Vegetables** - Fresh produce
3. **Bakery** - Baked goods and pastries
4. **Dairy & Eggs** - Milk, yogurt, cheese, eggs
5. **Snacks & Chips** - Party snacks and chips
6. **Beverages** - Drinks, juices, coffee, tea
7. **Meat & Fish** - Protein sources
8. **Spices & Seasonings** - Cooking spices
9. **Beauty & Personal Care** - Cosmetics and care products
10. **Electronics** - Gadgets and tech
11. **Home & Kitchen** - Household items
12. **Pet Supplies** - Pet food and accessories

### Intent Tags

Each product is tagged with intent categories for smart matching:
- `grocery` - Daily grocery items
- `party` - Items suitable for parties
- `lunch` - Lunch-specific items
- `breakfast` - Morning items
- `healthy` - Health-conscious options
- `cooking` - Cooking ingredients
- `snacks` - Snack items
- `daily` - Everyday essentials

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0
- **Database**: SQL Server with Entity Framework Core
- **AI**: Groq API for intent detection
- **Architecture**: Multi-agent orchestration, Repository Pattern
- **Frontend**: Bootstrap 5, Razor Views
- **Logging**: Built-in .NET logging

## 📁 Project Structure

```
IntentSearchApp/
├── Agents/                 # Multi-agent orchestration
│   ├── AgentInterfaces.cs
│   └── AgentImplementations.cs
├── Controllers/            # MVC Controllers
│   ├── HomeController.cs
│   └── SearchController.cs
├── Data/                   # Database layer
│   ├── AppDbContext.cs
│   ├── Entities.cs
│   └── DataSeederService.cs
├── Models/                 # Data models
│   ├── IntentResult.cs
│   ├── Product.cs
│   ├── SearchResultViewModel.cs
│   └── ErrorViewModel.cs
├── Repository/             # Data access layer
│   ├── IProductRepository.cs
│   └── ProductRepository.cs
├── Services/               # Business logic
│   └── IntentService.cs
├── Views/                  # Razor views
│   ├── Home/
│   ├── Search/
│   └── Shared/
└── wwwroot/               # Static assets
```

## 🔑 Key Algorithms

### Intent Detection
- Uses Groq AI with refined prompts for shopping intent classification
- Supports multiple intent types: party, lunch, daily shopping, etc.

### Keyword Matching
- Extracts keywords from user queries
- Expands keywords with synonyms
- Maps to product attributes

### Product Ranking
- Ranked by relevance (intent tag match)
- Secondary sort by rating
- Stock availability prioritized

## 📈 Performance Features

- **Efficient Querying**: LINQ-to-SQL with proper indexing
- **Lazy Loading**: Related entities loaded on demand
- **Caching**: Search results can be cached
- **Async Operations**: All I/O operations are async
- **Connection Pooling**: SQL Server connection pooling enabled

## 🔒 Security Features

- ✅ SQL Injection Protection (Entity Framework parameterized queries)
- ✅ API Key Management (from configuration)
- ✅ Input Validation
- ✅ HTTPS Enforced
- ✅ CORS Configuration Ready

## 🧪 Testing Scenarios

### Intent Detection Tests
- Party theme searches
- Meal planning queries
- Dietary restriction searches
- Occasion-based searches

### Product Match Tests
- Single intent with multiple keywords
- Multi-intent searches
- Brand-specific searches
- Price range queries

### Inventory Tests
- Out-of-stock handling
- Low-stock alerts
- Real-time updates

## 🚀 Future Enhancements

- [ ] Shopping cart functionality
- [ ] User accounts and wishlists
- [ ] Advanced filtering (price, ratings, brand)
- [ ] Product recommendations
- [ ] Machine learning for intent improvement
- [ ] Multiple language support
- [ ] Voice search integration
- [ ] Analytics dashboard
- [ ] Admin management portal
- [ ] Payment gateway integration

## 📝 API Endpoints

### Search
- `POST /Search` - Execute intent-based search

### Browse
- `GET /Browse` - Browse products by category (TODO)

### Details
- `GET /Product/{id}` - Product details (TODO)

## 🤝 Contributing

Contributions are welcome! Please fork the repository and submit pull requests.

## 📄 License

This project is open source and available under the MIT License.

## 📧 Contact

For questions and feedback, please contact:
- **GitHub**: [@Ashishnexturn](https://github.com/Ashishnexturn)
- **Repository**: [IntentSearchApp](https://github.com/Ashishnexturn/IntentSearchApp)

## 🙏 Acknowledgments

- Groq AI for powerful language model access
- Entity Framework Core team for excellent ORM
- Bootstrap for responsive UI framework
- The .NET community for continuous support

---

**Built with ❤️ using cutting-edge .NET and AI technologies**
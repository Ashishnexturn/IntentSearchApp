using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [StringLength(50)]
    public string Icon { get; set; }

    [StringLength(255)]
    public string ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Navigation
    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [InverseProperty("Category")]
    public virtual ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}

public class SubCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    public bool IsActive { get; set; } = true;

    // Foreign Key
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; }

    // Navigation
    [InverseProperty("SubCategory")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public int? SubCategoryId { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; }

    [StringLength(1000)]
    public string Description { get; set; }

    [StringLength(255)]
    public string ImageUrl { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? OriginalPrice { get; set; }

    [StringLength(50)]
    public string Brand { get; set; }

    public double Rating { get; set; } = 0;

    public int ReviewCount { get; set; } = 0;

    [StringLength(50)]
    public string Sku { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsFeatured { get; set; } = false;

    // Intent-based tagging for search
    [StringLength(500)]
    public string IntentTags { get; set; } // Comma-separated: "grocery,daily,essentials,cooking"

    [StringLength(500)]
    public string SearchKeywords { get; set; } // Additional keywords for search

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // Foreign Keys
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; }

    [ForeignKey("SubCategoryId")]
    public virtual SubCategory SubCategory { get; set; }

    // Navigation
    [InverseProperty("Product")]
    public virtual ProductInventory Inventory { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
}

public class ProductInventory
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    public int Quantity { get; set; } = 0;

    public int ReorderLevel { get; set; } = 10;

    public DateTime LastRestockedAt { get; set; } = DateTime.Now;

    // Foreign Key
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
}

public class ProductReview
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [StringLength(100)]
    public string CustomerName { get; set; }

    [StringLength(1000)]
    public string ReviewText { get; set; }

    public int Rating { get; set; }

    public bool IsApproved { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Foreign Key
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
}

public class SearchLog
{
    [Key]
    public int Id { get; set; }

    [StringLength(500)]
    public string SearchQuery { get; set; }

    [StringLength(100)]
    public string DetectedIntent { get; set; }

    [StringLength(255)]
    public string Keyword { get; set; }

    public int ResultCount { get; set; }

    public DateTime SearchedAt { get; set; } = DateTime.Now;
}

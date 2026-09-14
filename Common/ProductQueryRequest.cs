using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Common;
public class ProductQueryRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? Search { get; set; }

    public CATEGORY? Category { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }

    public string? SortBy { get; set; }

    public bool SortDescending { get; set; } = false;
}
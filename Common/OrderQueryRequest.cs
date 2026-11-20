using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Common;

public class OrderQueryRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public STATUS? Status { get; set; }

    public string? SortBy { get; set; }

    public bool SortDescending { get; set; } = false;
}


using Inventory_Management_dotnet.Models.Entities;

namespace Inventory_Management_dotnet.Common;

public class UserQueryRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Name { get; set; }
    public GENDER? Gender { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}
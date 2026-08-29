using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_dotnet.Options;
public class JwtSettingsOptions 
{
    public const string SectionName = "JwtSettings";

    [Required(AllowEmptyStrings = false)]
    public string Key {get; set;} = string.Empty; // Can be placed in user-secrets
    
    [Required(AllowEmptyStrings = false)]
    public string Issuer {get; set;} = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string Audience {get; set;} = string.Empty;
    public int ExpiryInMinutes {get; set;}

    public int RefreshTokenExpiryInDays {get; set;}
}
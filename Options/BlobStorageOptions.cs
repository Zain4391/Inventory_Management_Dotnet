using System.ComponentModel.DataAnnotations;

namespace Inventory_Management_dotnet.Options;

public class BlobStorageOptions
{
    public const string SectionName = "BlobStorage";

    [Required(AllowEmptyStrings = false)]
    public string AzureStorageKey {get; set;} = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string ContainerName {get; set;} = string.Empty;
}
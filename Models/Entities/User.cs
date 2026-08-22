namespace Inventory_Management_dotnet.Models.Entities;

public enum ROLES
{
    staff,
    admin
}

public enum GENDER
{
    male,
    female
}

public class User
{
    public Guid Id {get; set;}

    public required string Name {get; set;}

    public ROLES Role {get; set;}

    public required string Email {get; set;}

    public int Age {get; set;}

    public GENDER Gender {get; set;}

    public required string PasswordHash {get; set;} // will be encrypted

    public string? ProfileImageUrl {get; set;}

    public ICollection<Order> Orders {get; set;} = [];
}
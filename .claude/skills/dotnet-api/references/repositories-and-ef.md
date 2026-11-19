# Repositories (R1–R5) and EF Core / entities (E1–E8)

## Repositories
- **R1** Return entity or null; never DTOs; never throw not-found (the service decides what null means).
- **R2** `SaveChangesAsync()` inside each write method (no Unit of Work).
- **R3** Reads use `AsNoTracking()` + `Include(...)`.
- **R4** List queries build the `IQueryable` step by step, count before paging, return `PageResult<T>` (computed `TotalPages`):
```csharp
IQueryable<Product> query = ctx.Products.AsNoTracking().Include(p => p.Images);
if (req.MinPrice.HasValue) query = query.Where(...);
query = req.SortBy?.ToLower() switch {
    "price" => req.SortDescending ? query.OrderByDescending(...) : query.OrderBy(...),
    _ => query.OrderBy(p => p.Name)
};
var total = await query.CountAsync();
var items = await query.Skip((Page-1)*PageSize).Take(PageSize).ToListAsync();
```
- **R5** One query object per listable resource (`XxxQueryRequest` in `Common/`) with defaults `Page = 1, PageSize = 10, SortDescending = false` and no annotations (bounds are checked in the service).

## DbContext
- **E1** Primary constructor; Identity with Guid keys:
```csharp
public class AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) :
    IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(dbContextOptions)
```
  `DbSet<T>` properties under a `// DB Sets = Tables in the Data Base` comment (Users comes from Identity; check the file for current sets). `OnModelCreating`: call `base.OnModelCreating(modelBuilder)` **first** (Identity needs it), then `ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly)`, so a new config class registers automatically. Provider is Npgsql, registered in `Program.cs` from the connection string.

## Config classes
- **E2** One `public sealed class XConfig : IEntityTypeConfiguration<X>` per entity in `Data/Configs/`. Builder param is always `b`. Fixed section order, each with a short comment: `// Table Name` → `// PK + Alt Key` → `// Properties` → `// Relationships` → `// Indexes`. Chained calls each on their own line at the **same indent as `b`** (not indented further):
```csharp
public sealed class ProductImageConfig : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> b)
    {
        // Table Name
        b.ToTable("Product_Images");

        // PK + Alt Key
        b.HasKey(pi => pi.Id);

        // Properties
        b.Property(pi => pi.ImageUrl)
        .IsRequired();

        // Relationships
        b.HasOne(pi => pi.Product)
        .WithMany(p => p.Images)
        .HasForeignKey(pi => pi.ProductId)
        .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        b.HasIndex(pi => pi.ImageUrl).IsUnique();
    }
}
```
- **E3** Table names explicit, plural, Snake_Case for multiple words (`"Products"`, `"Product_Images"`, `"Refresh_Tokens"`). Lambda parameter = abbreviation of the entity (`p`, `pi`, `rt`, `u`, `o`). PK is `b.HasKey(x => x.Id)` with a `Guid`.
- **E4** Strings `.IsRequired().HasMaxLength(n)` (Name 120, Description 500, Address 100). Enums `.HasConversion<string>().IsRequired()` with the comment `// DB will store enums as strings`. Money `.HasPrecision(10, 2).IsRequired()`.
- **E5** Relationships always fully written: `HasOne().WithMany().HasForeignKey().OnDelete(...)`. Child data (images, tokens) → `Cascade`; business records (orders → user) → `Restrict`.
- **E6** Alternate keys `b.HasAlternateKey(e => e.Name).HasName("AK_Product_Name")`; named indexes `HasDatabaseName("Entity_Column_Index")` (`"Order_Address_Index"`, `"User_NormalizedEmail_Index"`); unique lookups `.IsUnique()`.
- **E7** `AppUserConfig` adds only custom columns/indexes; never redefine the Identity key.

## Entities
- **E8** `Guid Id`; FK as raw `Guid`; `required string` for mandatory text; nullable for optional state; collections `ICollection<T> X {get; set;} = [];`; navigations `= null!;` preceded by `// for referencing`; `CreatedAt = DateTime.UtcNow`; enums defined in the same file as the entity that uses them, ALL-CAPS type names (`CATEGORY`, `GENDER`, `STATUS`), mixed-case members. **No data annotations on entities**: all DB rules live in config classes.
```csharp
public class RefreshToken
{
    public Guid Id {get; set;}
    public Guid UserId {get; set;}                         // FK as raw Guid
    public required string TokenHash {get; set;}           // required for mandatory strings
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime? RevokedAt {get; set;}                 // nullable = optional state
    // for referencing
    public AppUser User {get; set;} = null!;               // nav prop silenced with null!
}
```
- Identity: `AppUser : IdentityUser<Guid>`, with a block comment explaining the generics.

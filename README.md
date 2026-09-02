# Inventory Management .NET API

A simple ASP.NET Core Web API for managing inventory products and uploading product images to Azure Blob Storage.

The project currently stores product data in memory using a repository backed by `ConcurrentDictionary`, so products are reset whenever the application restarts. Product images are uploaded to Azure Blob Storage and their public URLs are attached to products.

## Features

- Create inventory products
- List all products
- Get a product by ID
- Upload product images
- List images for a product
- Azure Blob Storage integration
- OpenAPI endpoint available in development

## Tech Stack

- ASP.NET Core Web API
- .NET 10
- Azure Blob Storage
- In-memory product repository

## Project Structure

```text
Inventory_Management_dotnet/
├── Controllers/
│   └── ProductController.cs
├── Models/
│   ├── DTOs/
│   └── Entities/
├── Options/
├── Exceptions/
├── Repositories/
├── Services/
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── Inventory_Management_dotnet.csproj
```

## Prerequisites

Before running the project, make sure you have:

- .NET 10 SDK installed
- An Azure Storage account, or Azurite for local blob storage testing
- A blob storage connection string
- A terminal or IDE such as Visual Studio, Rider, or VS Code

## Configuration

The app expects the following configuration section:

```json
{
  "BlobStorage": {
    "AzureStorageKey": "<YOUR_AZURE_STORAGE_KEY>",
    "ContainerName": "product-images"
  }
}
```

`AzureStorageKey` should be your Azure Blob Storage connection string.

For development, you can configure it in `appsettings.Development.json`, but the recommended approach is to use .NET user secrets so sensitive values are not committed.

From the project directory, run:

```bash
dotnet user-secrets set "BlobStorage:AzureStorageKey" "<your-azure-storage-connection-string>"
dotnet user-secrets set "BlobStorage:ContainerName" "product-images"
```

For production, configure these values through your hosting environment, app service settings, or environment variables.

Example environment variables:

```bash
BlobStorage__AzureStorageKey="<your-azure-storage-connection-string>"
BlobStorage__ContainerName="product-images-prod"
```

## Setup

Clone or open the project, then move into the project directory:

```bash
cd D:\Inventory_Management_dotnet
```

Restore dependencies:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

Run the API:

```bash
dotnet run
```

By default, the development launch settings expose the app at:

```text
http://localhost:5142
https://localhost:7248
```

The HTTP profile uses:

```text
http://localhost:5142
```

## OpenAPI

When running in the Development environment, the OpenAPI document is available at:

```text
http://localhost:5142/openapi/v1.json
```

This project maps OpenAPI metadata, but it does not currently include a Swagger UI package.

## API Endpoints

Base route:

```text
/api/products
```

### Get All Products

```http
GET /api/products
```

Example:

```bash
curl http://localhost:5142/api/products
```

### Get Product By ID

```http
GET /api/products/{id}
```

Example:

```bash
curl http://localhost:5142/api/products/00000000-0000-0000-0000-000000000000
```

### Create Product

```http
POST /api/products
Content-Type: application/json
```

Request body:

```json
{
  "name": "Gaming Mouse",
  "description": "Wireless RGB gaming mouse",
  "category": "Electronics",
  "quantity": 25,
  "price": 49.99
}
```

Example:

```bash
curl -X POST http://localhost:5142/api/products \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Gaming Mouse\",\"description\":\"Wireless RGB gaming mouse\",\"category\":\"Electronics\",\"quantity\":25,\"price\":49.99}"
```

Valid product categories are:

```text
Electronics
Clothing
Food
VideoGames
Others
```

Validation rules:

- `name` is required
- `name` must be between 5 and 20 characters
- `description` can be up to 100 characters
- `category` is required
- `quantity` must be at least 1
- `price` must be between 0.01 and 999999999

### Upload Product Image

```http
POST /api/products/{id}/images
Content-Type: multipart/form-data
```

Form field:

```text
File
```

Example:

```bash
curl -X POST http://localhost:5142/api/products/{product-id}/images \
  -F "File=@C:\path\to\image.png"
```

Supported image formats:

```text
.jpg
.jpeg
.png
```

Upload limits:

- Maximum file size: 5 MB
- Maximum request size: 10 MB

The API validates both the file extension and file signature.

### Get Product Images

```http
GET /api/products/{id}/images
```

Example:

```bash
curl http://localhost:5142/api/products/{product-id}/images
```

## Data Storage

Products are stored in memory. This means:

- No database setup is required
- Data is lost when the application stops
- The repository is useful for development and testing, but should be replaced with persistent storage for production use

Images are uploaded to Azure Blob Storage. The blob container is created automatically if it does not already exist.

## Development Notes

Run the migrations from master/main branch only!

```bash
dotnet ef migrations add <name>
dotnet ef database update
```

JSON enum values are serialized as strings, so API requests should use category names such as `"Electronics"` or `"VideoGames"`.

## Common Commands

Restore packages:

```bash
dotnet restore
```

Build:

```bash
dotnet build
```

Run:

```bash
dotnet run
```

Run with the HTTP launch profile:

```bash
dotnet run --launch-profile http
```

Run with the HTTPS launch profile:

```bash
dotnet run --launch-profile https
```

## Troubleshooting

### The app fails on startup

Check that `BlobStorage:AzureStorageKey` and `BlobStorage:ContainerName` are configured. These options are validated when the app starts.

### Image upload fails with invalid extension

Only `.jpg`, `.jpeg`, and `.png` files are allowed.

### Image upload fails with file signature error

The file contents must match the file extension. For example, renaming another file type to `.png` will be rejected.

### Products disappear after restarting the API

This is expected. Products are currently stored in memory and are not persisted to a database.

using ApiPG.Models;
using ApiPG.DTOs;

namespace ApiPG.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int id);
    }

    public class ProductService : IProductService
    {
        // En un proyecto real, aquí inyectarías el DbContext o repositorio
        private static readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Laptop", Description = "Gaming laptop", Price = 1500m },
            new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse", Price = 50m },
            new Product { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard", Price = 120m }
        };

        public Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = _products.Where(p => p.IsActive)
                                  .Select(MapToDto)
                                  .AsEnumerable();
            return Task.FromResult(products);
        }

        public Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id && p.IsActive);
            return Task.FromResult(product != null ? MapToDto(product) : null);
        }

        public Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                Id = _products.Max(p => p.Id) + 1,
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _products.Add(product);
            return Task.FromResult(MapToDto(product));
        }

        public Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return Task.FromResult<ProductDto?>(null);

            if (!string.IsNullOrEmpty(updateProductDto.Name))
                product.Name = updateProductDto.Name;
            
            if (!string.IsNullOrEmpty(updateProductDto.Description))
                product.Description = updateProductDto.Description;
            
            if (updateProductDto.Price.HasValue)
                product.Price = updateProductDto.Price.Value;
            
            if (updateProductDto.IsActive.HasValue)
                product.IsActive = updateProductDto.IsActive.Value;

            return Task.FromResult<ProductDto?>(MapToDto(product));
        }

        public Task<bool> DeleteProductAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return Task.FromResult(false);

            product.IsActive = false; // Soft delete
            return Task.FromResult(true);
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt,
                IsActive = product.IsActive
            };
        }
    }
}

using ApiPG.Models;
using ApiPG.DTOs;

namespace ApiPG.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductoDto>> GetAllProductsAsync();
        Task<ProductoDto?> GetProductByIdAsync(int id);
        Task<ProductoDto> CreateProductAsync(CrearProductoDto createProductDto);
        Task<ProductoDto?> UpdateProductAsync(int id, ActualizarProductoDto updateProductDto);
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

        public Task<IEnumerable<ProductoDto>> GetAllProductsAsync()
        {
            var products = _products.Where(p => p.IsActive)
                                  .Select(MapToDto)
                                  .AsEnumerable();
            return Task.FromResult(products);
        }

        public Task<ProductoDto?> GetProductByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id && p.IsActive);
            return Task.FromResult(product != null ? MapToDto(product) : null);
        }

        public Task<ProductoDto> CreateProductAsync(CrearProductoDto createProductDto)
        {
            var product = new Product
            {
                Id = _products.Max(p => p.Id) + 1,
                Name = createProductDto.Nombre,
                Description = createProductDto.Descripcion,
                Price = createProductDto.Precio,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _products.Add(product);
            return Task.FromResult(MapToDto(product));
        }

        public Task<ProductoDto?> UpdateProductAsync(int id, ActualizarProductoDto updateProductDto)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return Task.FromResult<ProductoDto?>(null);

            if (!string.IsNullOrEmpty(updateProductDto.Nombre))
                product.Name = updateProductDto.Nombre;
            
            if (!string.IsNullOrEmpty(updateProductDto.Descripcion))
                product.Description = updateProductDto.Descripcion;
            
            if (updateProductDto.Precio.HasValue)
                product.Price = updateProductDto.Precio.Value;
            
            if (updateProductDto.EstaActivo.HasValue)
                product.IsActive = updateProductDto.EstaActivo.Value;

            return Task.FromResult<ProductoDto?>(MapToDto(product));
        }

        public Task<bool> DeleteProductAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return Task.FromResult(false);

            product.IsActive = false; // Soft delete
            return Task.FromResult(true);
        }

        private static ProductoDto MapToDto(Product product)
        {
            return new ProductoDto
            {
                Id = product.Id,
                Nombre = product.Name,
                Descripcion = product.Description,
                Precio = product.Price,
                CreadoEn = product.CreatedAt,
                EstaActivo = product.IsActive
            };
        }
    }
}

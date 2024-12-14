using BusinessLogic.DTOs;
using BusinessLogic.DTOs.Product;
using BusinessLogic.Utils.Implementation;
using DataAccess.Entities;

namespace BusinessLogic.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ReadProductDto>> GetAllProductsAsync();
        
        Task<ReadProductDto?> GetProductByIdAsync(int productId);
        Task<PaginatedList<ReadProductDto>> GetAllProductsAsync(QueryParameters queryParameters);
        Task<ReadProductDto> CreateProductAsync(CreateProductDto createDto);
        Task<bool> UpdateProductAsync(int productId, UpdateProductDto updateDto);
        Task<bool> DeleteProductAsync(int productId);
    }
}

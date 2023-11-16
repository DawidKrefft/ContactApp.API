using ContactApp.API.Models.Domain;
using ContactApp.API.Models.DTO;

namespace ContactApp.API.Repositories
{
    public interface ICategoryRepository
    {
        Task<PaginatedResult<CategoryDto>> GetAllAsync(int page, int pageSize);
        Task<CategoryDto> GetByIdAsync(int id);
        Task<Category> GetByNameAsync(string name);
        Task<Category> GetDomainModelByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CreateCategoryRequestDto request);
        Task<CategoryDto> UpdateAsync(int id, UpdateCategoryRequestDto request);
        Task<bool> DeleteAsync(int id);
    }
}

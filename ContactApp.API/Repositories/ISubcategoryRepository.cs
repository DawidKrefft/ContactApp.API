using ContactApp.API.Models.Domain;
using ContactApp.API.Models.DTO;

namespace ContactApp.API.Repositories
{
    public interface ISubcategoryRepository
    {
        Task<PaginatedResult<SubcategoryDto>> GetAllAsync(int page, int pageSize);
        Task<SubcategoryDto> GetByIdAsync(int id);
        Task<Subcategory> GetDomainModelByIdAsync(int id);
        Task<Subcategory> GetByNameAsync(string name);
        Task<SubcategoryDto> CreateAsync(CreateSubcategoryRequestDto request);
        Task<Subcategory> CreateAsyncDomain(Subcategory subcategory);
        Task<SubcategoryDto> UpdateAsync(int id, UpdateSubcategoryRequestDto request);
        Task<bool> DeleteAsync(int id);
    }
}

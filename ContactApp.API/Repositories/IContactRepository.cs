using ContactApp.API.Models.DTO;

namespace ContactApp.API.Repositories
{
    public interface IContactRepository
    {
        Task<PaginatedResult<ContactDto>> GetAllAsync(int page, int pageSize);
        Task<ContactDto> GetByIdAsync(int id);
        Task<ContactDto> CreateAsync(CreateContactRequestDto request);
        Task<ContactDto> UpdateAsync(int id, UpdateContactRequestDto request);
        Task<bool> DeleteAsync(int id);
    }
}

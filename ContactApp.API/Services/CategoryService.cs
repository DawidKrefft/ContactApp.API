using AutoMapper;
using ContactApp.API.Data;
using ContactApp.API.Models.Domain;
using ContactApp.API.Models.DTO;
using ContactApp.API.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ContactApp.API.Services
{
    public class CategoryService : ICategoryRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IValidator<CreateCategoryRequestDto> createCategoryValidator;
        private readonly IValidator<UpdateCategoryRequestDto> updateCategoryValidator;

        public CategoryService(
            ApplicationDbContext dbContext,
            IMapper mapper,
            IValidator<CreateCategoryRequestDto> createCategoryValidator,
            IValidator<UpdateCategoryRequestDto> updateCategoryValidator
        )
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.createCategoryValidator = createCategoryValidator;
            this.updateCategoryValidator = updateCategoryValidator;
        }

        public async Task<PaginatedResult<CategoryDto>> GetAllAsync(int page, int pageSize)
        {
            try
            {
                pageSize = Math.Min(pageSize, 10);

                var query = dbContext.Categories.AsNoTracking();
                var totalItems = await query.CountAsync();

                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var categories = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var categoriesDtos = mapper.Map<List<CategoryDto>>(categories);

                var result = new PaginatedResult<CategoryDto>
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Items = categoriesDtos
                };

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve categories.", ex);
            }
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            try
            {
                var existingCategory = await dbContext.Categories.FirstOrDefaultAsync(
                    c => c.Id == id
                );
                if (existingCategory != null)
                {
                    return mapper.Map<CategoryDto>(existingCategory);
                }
                else
                {
                    throw new InvalidOperationException("Category not found.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve a category by ID.", ex);
            }
        }

        public async Task<Category> GetDomainModelByIdAsync(int id)
        {
            try
            {
                return await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to retrieve a category domain model by ID.",
                    ex
                );
            }
        }

        public async Task<Category> GetByNameAsync(string name)
        {
            try
            {
                return await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == name);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to retrieve a category domain model by Name.",
                    ex
                );
            }
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryRequestDto request)
        {
            var validation = await createCategoryValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var validationErrors = string.Join(
                    ", ",
                    validation.Errors.Select(error => error.ErrorMessage)
                );
                throw new ValidationException(validationErrors);
            }
            try
            {
                var category = mapper.Map<Category>(request);
                await dbContext.Categories.AddAsync(category);
                await dbContext.SaveChangesAsync();

                return mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create the category.", ex);
            }
        }

        public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryRequestDto request)
        {
            var validation = await updateCategoryValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var validationErrors = string.Join(
                    ", ",
                    validation.Errors.Select(error => error.ErrorMessage)
                );
                throw new ValidationException(validationErrors);
            }
            try
            {
                var existingCategory = await dbContext.Categories.FirstOrDefaultAsync(
                    c => c.Id == id
                );

                if (existingCategory != null)
                {
                    mapper.Map(request, existingCategory);
                    await dbContext.SaveChangesAsync();
                    return mapper.Map<CategoryDto>(existingCategory);
                }
                else
                {
                    throw new InvalidOperationException("Category not found.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to update the category.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingCategory = await dbContext.Categories
                .Include(c => c.Contacts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory != null)
            {
                if (existingCategory.Contacts.Any())
                {
                    // Category is being used by at least one contact
                    throw new InvalidOperationException(
                        "Category is being used by one or more contacts and cannot be deleted."
                    );
                }

                dbContext.Categories.Remove(existingCategory);
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new InvalidOperationException("Category not found.");
            }
        }
    }
}

using AutoMapper;
using Azure.Core;
using ContactApp.API.Data;
using ContactApp.API.Models.Domain;
using ContactApp.API.Models.DTO;
using ContactApp.API.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ContactApp.API.Services
{
    public class SubcategoryService : ISubcategoryRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IValidator<CreateSubcategoryRequestDto> createSubcategoryValidator;
        private readonly IValidator<UpdateSubcategoryRequestDto> updateSubcategoryValidator;
        private readonly IValidator<Subcategory> subcategoryDomainValidator;

        public SubcategoryService(
            ApplicationDbContext dbContext,
            IMapper mapper,
            IValidator<CreateSubcategoryRequestDto> createSubcategoryValidator,
            IValidator<UpdateSubcategoryRequestDto> updateSubcategoryValidator,
            IValidator<Subcategory> subcategoryDomainValidator
        )
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.createSubcategoryValidator = createSubcategoryValidator;
            this.updateSubcategoryValidator = updateSubcategoryValidator;
            this.subcategoryDomainValidator = subcategoryDomainValidator;
        }

        public async Task<PaginatedResult<SubcategoryDto>> GetAllAsync(int page, int pageSize)
        {
            try
            {
                pageSize = Math.Min(pageSize, 10);

                var query = dbContext.Subcategories.AsNoTracking();
                var totalItems = await query.CountAsync();

                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var subcategories = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var categoriesDtos = mapper.Map<List<SubcategoryDto>>(subcategories);

                var result = new PaginatedResult<SubcategoryDto>
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
                throw new InvalidOperationException("Failed to retrieve subcategories.", ex);
            }
        }

        public async Task<SubcategoryDto> GetByIdAsync(int id)
        {
            try
            {
                var existingSubcategory = await dbContext.Subcategories.FirstOrDefaultAsync(
                    c => c.Id == id
                );
                if (existingSubcategory != null)
                {
                    return mapper.Map<SubcategoryDto>(existingSubcategory);
                }
                else
                {
                    throw new InvalidOperationException("Subcategory not found.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve a subcategory by ID.", ex);
            }
        }

        public async Task<Subcategory> GetDomainModelByIdAsync(int id)
        {
            try
            {
                return await dbContext.Subcategories.FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to retrieve a category domain model by ID.",
                    ex
                );
            }
        }

        public async Task<Subcategory> GetByNameAsync(string name)
        {
            try
            {
                return await dbContext.Subcategories.FirstOrDefaultAsync(s => s.Name == name);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to retrieve a subcategory domain model by Name.",
                    ex
                );
            }
        }

        public async Task<SubcategoryDto> CreateAsync(CreateSubcategoryRequestDto request)
        {
            var validation = await createSubcategoryValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var validationErrors = string.Join(
                    ", ",
                    validation.Errors.Select(error => error.ErrorMessage)
                );
                throw new ValidationException(validationErrors);
            }
            var isCategoryIdValid = await IsCategoryIdValidAsync(request.CategoryId);
            if (!isCategoryIdValid)
            {
                throw new InvalidOperationException("Invalid CategoryId specified.");
            }
            try
            {
                var subcategory = mapper.Map<Subcategory>(request);
                await dbContext.Subcategories.AddAsync(subcategory);
                await dbContext.SaveChangesAsync();

                return mapper.Map<SubcategoryDto>(subcategory);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create the category.", ex);
            }
        }

        public async Task<Subcategory> CreateAsyncDomain(Subcategory subcategory)
        {
            var validation = await subcategoryDomainValidator.ValidateAsync(subcategory);
            if (!validation.IsValid)
            {
                var validationErrors = string.Join(
                    ", ",
                    validation.Errors.Select(error => error.ErrorMessage)
                );
                throw new ValidationException(validationErrors);
            }
            var isCategoryIdValid = await IsCategoryIdValidAsync(subcategory.CategoryId);
            if (!isCategoryIdValid)
            {
                throw new InvalidOperationException("Invalid CategoryId specified.");
            }
            try
            {
                dbContext.Subcategories.Add(subcategory);
                await dbContext.SaveChangesAsync();
                return subcategory;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Failed to create a subcategory domain model.",
                    ex
                );
            }
        }

        public async Task<SubcategoryDto> UpdateAsync(int id, UpdateSubcategoryRequestDto request)
        {
            var validation = await updateSubcategoryValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var validationErrors = string.Join(
                    ", ",
                    validation.Errors.Select(error => error.ErrorMessage)
                );
                throw new ValidationException(validationErrors);
            }
            var isCategoryIdValid = await IsCategoryIdValidAsync(request.CategoryId);
            if (!isCategoryIdValid)
            {
                throw new InvalidOperationException("Invalid CategoryId specified.");
            }
            try
            {
                var existingSubcategory = await dbContext.Subcategories.FirstOrDefaultAsync(
                    c => c.Id == id
                );

                if (existingSubcategory != null)
                {
                    mapper.Map(request, existingSubcategory);
                    await dbContext.SaveChangesAsync();
                    return mapper.Map<SubcategoryDto>(existingSubcategory);
                }
                else
                {
                    throw new InvalidOperationException("Subcategory not found.");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to update the subcategory.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingSubcategory = await dbContext.Subcategories
                .Include(c => c.Contacts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingSubcategory != null)
            {
                if (existingSubcategory.Contacts.Any())
                {
                    throw new InvalidOperationException(
                        "Subcategory is being used by one or more contacts and cannot be deleted."
                    );
                }

                dbContext.Subcategories.Remove(existingSubcategory);
                await dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new InvalidOperationException("Subcategory not found.");
            }
        }

        private async Task<bool> IsCategoryIdValidAsync(int categoryId)
        {
            return await dbContext.Categories.AnyAsync(c => c.Id == categoryId)
                ? true
                : throw new InvalidOperationException("Invalid CategoryId specified.");
        }
    }
}

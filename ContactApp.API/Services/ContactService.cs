using AutoMapper;
using ContactApp.API.Data;
using ContactApp.API.Models.Domain;
using ContactApp.API.Models.DTO;
using ContactApp.API.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ContactApp.API.Services
{
    public class ContactService : IContactRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ICategoryRepository categoryRepository;
        private readonly ISubcategoryRepository subcategoryRepository;

        private readonly IValidator<CreateContactRequestDto> createContactValidator;
        private readonly IValidator<UpdateContactRequestDto> updateContactValidator;

        public ContactService(
            ApplicationDbContext dbContext,
            IMapper mapper,
            ICategoryRepository categoryRepository,
            ISubcategoryRepository subcategoryRepository,
            IValidator<CreateContactRequestDto> createContactValidator,
            IValidator<UpdateContactRequestDto> updateContactValidator
        )
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.categoryRepository = categoryRepository;
            this.subcategoryRepository = subcategoryRepository;
            this.createContactValidator = createContactValidator;
            this.updateContactValidator = updateContactValidator;
        }

        public async Task<PaginatedResult<ContactDto>> GetAllAsync(int page, int pageSize)
        {
            try
            {
                pageSize = Math.Min(pageSize, 10);

                var query = dbContext.Contacts
                    .Include(x => x.Category)
                    .Include(x => x.Subcategory)
                    .AsNoTracking();
                var totalItems = await query.CountAsync();

                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var contacts = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

                var contactDtos = mapper.Map<List<ContactDto>>(contacts);

                var result = new PaginatedResult<ContactDto>
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Items = contactDtos
                };

                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve categories.", ex);
            }
        }

        public async Task<ContactDto> GetByIdAsync(int id)
        {
            var contact = await dbContext.Contacts
                .Include(x => x.Category)
                .Include(x => x.Subcategory)
                .FirstOrDefaultAsync(x => x.Id == id);

            return contact != null
                ? mapper.Map<ContactDto>(contact)
                : throw new InvalidOperationException("Contact not found.");
        }

        public async Task<ContactDto> CreateAsync(CreateContactRequestDto request)
        {
            await ValidateCreateRequestAsync(request);

            Subcategory subcategory = await GetOrCreateSubcategoryAsync(request);
            bool isEmailUnique = await IsEmailUniqueAsync(request.Email);
            if (!isEmailUnique)
                throw new ValidationException("Email is not unique.");
            bool isPhoneUnique = await IsPhoneUniqueAsync(request.PhoneNumber);
            if (!isPhoneUnique)
                throw new ValidationException("Phone is not unique.");

            var category =
                await categoryRepository.GetByNameAsync(request.CategoryName)
                ?? throw new ValidationException($"Category '{request.CategoryName}' not found.");

            var contact = mapper.Map<Contact>(request);
            contact.CategoryId = category.Id;
            contact.SubcategoryId = subcategory?.Id;

            await dbContext.Contacts.AddAsync(contact);
            await dbContext.SaveChangesAsync();

            return mapper.Map<ContactDto>(contact);
        }

        public async Task<ContactDto> UpdateAsync(int id, UpdateContactRequestDto request)
        {
            var existingContact =
                await dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == id)
                ?? throw new InvalidOperationException("Contact not found.");

            await ValidateUpdateRequestAsync(request);

            Subcategory subcategory = await GetOrCreateSubcategoryAsync(request);
            var category =
                await categoryRepository.GetByNameAsync(request.CategoryName)
                ?? throw new ValidationException($"Category '{request.CategoryName}' not found.");

            mapper.Map(request, existingContact);
            existingContact.CategoryId = category.Id;
            existingContact.SubcategoryId = subcategory?.Id;

            await dbContext.SaveChangesAsync();
            return mapper.Map<ContactDto>(existingContact);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingContact = await dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            return existingContact != null
                ? (
                    dbContext.Contacts.Remove(existingContact) != null
                    && await dbContext.SaveChangesAsync() > 0
                )
                : throw new InvalidOperationException("Category not found.");
        }

        // Helper methods
        private async Task ValidateCreateRequestAsync(CreateContactRequestDto request)
        {
            var validation = await createContactValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var validationErrors = string.Join(
                    ", ",
                    validation.Errors.Select(error => error.ErrorMessage)
                );
                throw new ValidationException(validationErrors);
            }
        }

        private async Task ValidateUpdateRequestAsync(UpdateContactRequestDto request)
        {
            var validation = await updateContactValidator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                var validationErrors = string.Join(
                    ", ",
                    validation.Errors.Select(error => error.ErrorMessage)
                );
                throw new ValidationException(validationErrors);
            }
        }

        private async Task<Subcategory> GetOrCreateSubcategoryAsync(CreateContactRequestDto request)
        {
            Subcategory subcategory = null;
            var otherCategory = await categoryRepository.GetByNameAsync("other");
            var existingSubcategory = await subcategoryRepository.GetByNameAsync(
                request.SubcategoryName
            );
            var workCategory = await categoryRepository.GetByNameAsync("work");

            if (request.CategoryName == "work")
            {
                subcategory = await subcategoryRepository.GetByNameAsync(request.SubcategoryName);

                if (subcategory == null || subcategory.CategoryId != workCategory.Id)
                {
                    throw new ValidationException(
                        "Subcategory not found or doesn't have Id of work."
                    );
                }
            }
            else if (request.CategoryName == "other")
            {
                if (otherCategory == null)
                {
                    throw new ValidationException("Category 'other' not found.");
                }
                if (existingSubcategory != null)
                {
                    subcategory = existingSubcategory;
                }
                else
                {
                    var newSubcategory = new Subcategory
                    {
                        Name = request.SubcategoryName,
                        CategoryId = otherCategory.Id
                    };

                    subcategory = await subcategoryRepository.CreateAsyncDomain(newSubcategory);
                }
            }

            return subcategory;
        }

        private async Task<Subcategory> GetOrCreateSubcategoryAsync(UpdateContactRequestDto request)
        {
            Subcategory subcategory = null;
            var otherCategory = await categoryRepository.GetByNameAsync("other");
            var existingSubcategory = await subcategoryRepository.GetByNameAsync(
                request.SubcategoryName
            );
            var workCategory = await categoryRepository.GetByNameAsync("work");

            if (request.CategoryName == "work")
            {
                subcategory = await subcategoryRepository.GetByNameAsync(request.SubcategoryName);

                if (subcategory == null || subcategory.CategoryId != workCategory.Id)
                {
                    throw new ValidationException(
                        "Subcategory not found or doesn't have Id of work."
                    );
                }
            }
            else if (request.CategoryName == "other")
            {
                if (otherCategory == null)
                {
                    throw new ValidationException("Category 'other' not found.");
                }
                if (existingSubcategory != null)
                {
                    subcategory = existingSubcategory;
                }
                else
                {
                    var newSubcategory = new Subcategory
                    {
                        Name = request.SubcategoryName,
                        CategoryId = otherCategory.Id
                    };

                    subcategory = await subcategoryRepository.CreateAsyncDomain(newSubcategory);
                }
            }

            return subcategory;
        }

        private async Task<bool> IsEmailUniqueAsync(string email)
        {
            bool isUnique = !await dbContext.Contacts.AnyAsync(c => c.Email == email);
            return isUnique;
        }

        private async Task<bool> IsPhoneUniqueAsync(string phoneNumber)
        {
            bool isUnique = !await dbContext.Contacts.AnyAsync(c => c.PhoneNumber == phoneNumber);
            return isUnique;
        }
    }
}

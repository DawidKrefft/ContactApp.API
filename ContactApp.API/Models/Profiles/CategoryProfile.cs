using AutoMapper;
using ContactApp.API.Models.Domain;
using ContactApp.API.Models.DTO;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryRequestDto, Category>();
        CreateMap<UpdateCategoryRequestDto, Category>();
    }
}

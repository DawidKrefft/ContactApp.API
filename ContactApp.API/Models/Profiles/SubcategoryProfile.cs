using AutoMapper;
using ContactApp.API.Models.Domain;
using ContactApp.API.Models.DTO;

public class SubcategoryProfile : Profile
{
    public SubcategoryProfile()
    {
        CreateMap<Category, SubcategoryDto>();
        CreateMap<Subcategory, SubcategoryDto>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<CreateSubcategoryRequestDto, Subcategory>();
        CreateMap<UpdateSubcategoryRequestDto, Subcategory>();
    }
}

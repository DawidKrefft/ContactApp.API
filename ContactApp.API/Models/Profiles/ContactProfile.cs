using AutoMapper;
using ContactApp.API.Models.Domain;
using ContactApp.API.Models.DTO;

public class ContactProfile : Profile
{
    public ContactProfile()
    {
        CreateMap<Contact, ContactDto>();
        CreateMap<CreateContactRequestDto, Contact>();
        CreateMap<UpdateContactRequestDto, Contact>();
    }
}

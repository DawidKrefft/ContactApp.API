using AutoMapper;
using ContactApp.API.Models.DTO;
using Microsoft.AspNetCore.Identity;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<LoginRequestDto, IdentityUser>();
        CreateMap<RegisterRequestDto, IdentityUser>();
        CreateMap<IdentityUser, LoginResponseDto>();
    }
}

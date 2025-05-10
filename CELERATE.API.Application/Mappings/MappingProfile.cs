using AutoMapper;
using CELERATE.API.Application.DTOs;
using CELERATE.API.CORE.Entities;


namespace CELERATE.API.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType.ToString()))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole.ToString()));

            CreateMap<Card, CardDto>()
                .ForMember(dest => dest.Permissions, opt =>
                    opt.MapFrom(src => src.Permissions.Select(p => p.ToString()).ToList()));

            CreateMap<Branch, BranchDto>()
                .ForMember(dest => dest.CompanyType, opt => opt.MapFrom(src => src.CompanyType.ToString()))
                .ForMember(dest => dest.OperationType, opt => opt.MapFrom(src => src.OperationType.ToString()));

            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}

using AutoMapper;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Dto;
using Projekt_API_Models;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Helper
{
    public class MappingProfiles :Profile
    {
        public MappingProfiles()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Appointment,AppointmentDto>().ReverseMap();
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.AppointmentId))
                .ForMember(dest => dest.AppointmentTime, opt => opt.MapFrom(src => src.AppointmentTime))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.CustomerId))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.CustomerName))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer.CustomerEmail))
                .ForMember(dest => dest.CustomerPhoneNumber, opt => opt.MapFrom(src => src.Customer.CustomerPhoneNumber))
                .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Company.CompanyId))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName));
        }
    }
}

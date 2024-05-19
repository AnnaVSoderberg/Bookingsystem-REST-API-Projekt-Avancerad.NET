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
        }
    }
}

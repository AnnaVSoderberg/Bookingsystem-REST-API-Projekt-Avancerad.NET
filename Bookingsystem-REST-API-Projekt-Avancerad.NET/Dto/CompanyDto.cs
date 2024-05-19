using Projekt_API_Models;
using System.ComponentModel.DataAnnotations;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Dto
{
    public class CompanyDto
    {
        public int CompanyId { get; set; }
        [MaxLength]
        public string CompanyName { get; set; }

        //Many to one
        public ICollection<Appointment> Appointments { get; set; }
    }
}

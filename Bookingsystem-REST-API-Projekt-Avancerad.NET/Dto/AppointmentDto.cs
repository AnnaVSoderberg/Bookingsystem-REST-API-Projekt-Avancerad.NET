using Projekt_API_Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Dto
{
    public class AppointmentDto
    {
        [Key]
        public int AppointmentId { get; set; }
        public string AppointmentDiscription { get; set; }
        public DateTime CreatingDateAppointment { get; set; }

        public int CustomerId { get; set; } //FK
        //One To Many
        [JsonIgnore]
        public Customer Customer { get; set; }


        public int CompanyId { get; set; } //FK
        //One to Many
        [JsonIgnore]
        public Company Company { get; set; }

        public List<History> History { get; set; }
    }
}

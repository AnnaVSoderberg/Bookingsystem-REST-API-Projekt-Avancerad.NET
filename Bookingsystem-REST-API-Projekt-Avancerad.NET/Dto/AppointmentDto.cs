using Projekt_API_Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Dto
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentTime { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNumber { get; set; }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}

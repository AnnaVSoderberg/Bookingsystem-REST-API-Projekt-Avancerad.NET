using Projekt_API_Models;
using System.ComponentModel.DataAnnotations;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Dto
{
    public class CustomerDto
    {
        [Key]
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNumber { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Projekt_API_Models
{
    //[Serializable]
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        [Required]
        public DateTime AppointmentTime{ get; set; }

        public int CustomerId { get; set; } //FK
        //One To Many
        [JsonIgnore]
        public Customer Customer { get; set; }


        public int CompanyId { get; set; } //FK
        //One to Many
        [JsonIgnore]
        public Company Company { get; set; }

    }
}

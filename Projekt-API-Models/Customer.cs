using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_API_Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; }
        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; }
        [Required]
        [MaxLength(20)]
        public string CustomerPhoneNumber { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }

        //Many to One
        public ICollection<Appointment> Appointments { get; set; }


    }
}

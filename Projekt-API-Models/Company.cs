using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_API_Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
        [MaxLength]
        public string CompanyName { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        //Many to one
        public ICollection<Appointment> Appointments { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_API_Models
{
    public class LogInDetails
    {
        [Key]
        public int LoginId { get; set; }
        public int? CompanyId { get; set; }
        public int? CustomerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        // Navigation properties
        public Company Company { get; set; }
        public Customer Customer { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt_API_Models
{
    public class History
    {
        [Key]
        public int HistoryId { get; set; }
        public DateTime ChangeDate { get; set; }
        public string ReasonToChange { get; set; }
        public int AppointmentId { get; set; }

        public Appointment Appointment { get; set; }
    }
}

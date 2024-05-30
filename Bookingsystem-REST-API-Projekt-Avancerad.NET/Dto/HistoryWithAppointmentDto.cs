namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Dto
{
    public class HistoryWithAppointmentDto
    {
        public int HistoryId { get; set; }
        public int AppointmentId { get; set; }
        public string Action { get; set; }
        public string ReasonToChange { get; set; }
        public DateTime ChangeDate { get; set; }
        public AppointmentDto Appointment { get; set; }
    }
}

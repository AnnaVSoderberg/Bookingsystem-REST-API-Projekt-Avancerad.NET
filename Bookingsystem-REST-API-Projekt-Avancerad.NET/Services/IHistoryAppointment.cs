using Projekt_API_Models;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Services
{
    public interface IHistoryAppointment
    {
        Task Add(History history);
        Task<IEnumerable<History>> GetByAppointmentId(int appointmentId);
        Task<IEnumerable<History>> GetAll();
    }
}

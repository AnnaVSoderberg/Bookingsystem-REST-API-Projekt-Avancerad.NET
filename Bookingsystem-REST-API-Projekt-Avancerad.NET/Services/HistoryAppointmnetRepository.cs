using Bookingsystem_REST_API_Projekt_Avancerad.NET.Data;
using Microsoft.EntityFrameworkCore;
using Projekt_API_Models;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Services
{
    public class HistoryAppointmnetRepository : IHistoryAppointment
    {
        private readonly AppDbContext _appContext;

        public HistoryAppointmnetRepository(AppDbContext appContext)
        {
            _appContext = appContext;
            
        }

        public async Task Add(History history)
        {
            _appContext.History.Add(history);
            await _appContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<History>> GetAll()
        {
            return await _appContext.History.ToListAsync();
        }

        public async Task<IEnumerable<History>> GetByAppointmentId(int appointmentId)
        {
            return await _appContext.History.Where(h => h.AppointmentId == appointmentId).ToListAsync();
        }
    }
}

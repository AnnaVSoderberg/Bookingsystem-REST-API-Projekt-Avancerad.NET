using Bookingsystem_REST_API_Projekt_Avancerad.NET.Data;
using Microsoft.EntityFrameworkCore;
using Projekt_API_Models;
using System.Globalization;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Services
{
    public class AppointmentRepository : IBookingSystem<Appointment>, IAppointment
    {

        private AppDbContext _appContext;

        public AppointmentRepository(AppDbContext appContext)
        {
            _appContext = appContext;
        }

        public async Task<Appointment> Add(Appointment newEntity)  
        {
            var result = await _appContext.Appointments.AddAsync(newEntity);
            await _appContext.SaveChangesAsync();
            await AddBookingHistory(newEntity.CustomerId, "Appointment Created");
            return result.Entity;
        }

        public async Task AddBookingHistory(int appointmnetId, string NameOfChange)
        {
            var History = new History
            {
                AppointmentId = appointmnetId,
                ChangeDate = DateTime.Now,
                ReasonToChange = NameOfChange
            };

            _appContext.History.Add(History);
            await _appContext.SaveChangesAsync();
        }

        public async Task<Appointment> Delete(int id) 
        {
            var result = await _appContext.Appointments.FirstOrDefaultAsync(a => a.AppointmentId == id);
            if (result != null)
            {
                _appContext.Appointments.Remove(result);
                await _appContext.SaveChangesAsync();
                await AddBookingHistory(id, "Appointment Deleted");
                return result;
            }
            return null;
        }


        public async Task<IEnumerable<Appointment>> GetAll()
        {
            return await _appContext.Appointments.Include(a => a.Customer).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentMonth(int year, int month)
        {
            return await _appContext.Appointments.Where(t => t.CreatingDateAppointment.Year == year && t.CreatingDateAppointment.Month == month).ToListAsync(); 
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentWeek(int year, int week)
        {
            var appointment = await _appContext.Appointments.Where(a => a.CreatingDateAppointment.Year == year).ToListAsync();

            var calendar = CultureInfo.InvariantCulture.Calendar;
            var appointmentsInWeek = appointment.Where(a => calendar.GetWeekOfYear(a.CreatingDateAppointment, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) == week).ToList();

            return appointmentsInWeek;
        }


        public async Task<IEnumerable<History>> GetChangeHistory(int appointmentId)
        {
            return await _appContext.History.Where(h => h.AppointmentId == appointmentId).ToListAsync();
        }

        public async Task<Appointment> GetSingle(int id)
        {
           return await _appContext.Appointments.Include(a => a.Customer).FirstOrDefaultAsync(a => a.AppointmentId == id);
        }


        public async Task<Appointment> Update(Appointment entity)
        {
            var updateAppointment = await _appContext.Appointments.FirstOrDefaultAsync(o => o.AppointmentId == entity.AppointmentId);

            if(updateAppointment != null)
            {
                updateAppointment.AppointmentDiscription = entity.AppointmentDiscription;
                updateAppointment.CreatingDateAppointment = entity.CreatingDateAppointment;
                updateAppointment.CustomerId = entity.CustomerId;
                updateAppointment.CompanyId = entity.CompanyId;      
                await AddBookingHistory(entity.CustomerId, "Appointment Updated");
                await _appContext.SaveChangesAsync();
                return updateAppointment;
            }
            return null;
        }

    }
}

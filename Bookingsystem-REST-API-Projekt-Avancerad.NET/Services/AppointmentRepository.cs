using Bookingsystem_REST_API_Projekt_Avancerad.NET.Data;
using Microsoft.EntityFrameworkCore;
using Projekt_API_Models;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Services
{
    public class AppointmentRepository : IBookingSystem<Appointment>
    {

        private readonly AppDbContext _appContext;

        public AppointmentRepository(AppDbContext appContext)
        {
            _appContext = appContext;
        }

        public async Task<Appointment> Add(Appointment appointment)
        {
            var result = await _appContext.Appointments.AddAsync(appointment);
            await _appContext.SaveChangesAsync();
            return result.Entity;
        }


        public async Task<Appointment> Delete(int id)
        {
            var result = await _appContext.Appointments.FirstOrDefaultAsync(a => a.AppointmentId == id);
            if (result != null)
            {
                _appContext.Appointments.Remove(result);
                await _appContext.SaveChangesAsync();
                return result;
            }
            return null;
        }


        public async Task<IEnumerable<Appointment>> GetAll()
        {
            return await _appContext.Appointments.Include(a => a.Customer).Include(a => a.Company).ToListAsync(); //lagt till "Include(a => a.Company)." behövs?
        }


        public async Task<Appointment> GetSingle(int id)
        {
            return await _appContext.Appointments.Include(a => a.Customer).Include(a => a.Company).FirstOrDefaultAsync(a => a.AppointmentId == id); //lagt till "Include(a => a.Company)." behövs?
        }


        public async Task<Appointment> Update(Appointment appointment)
        {
            var updateAppointment = await _appContext.Appointments.FirstOrDefaultAsync(a => a.AppointmentId == appointment.AppointmentId);

            if (updateAppointment != null)
            {
                //VAD VILL JAG ÄNDRA HÄR?
                await _appContext.SaveChangesAsync();
                return updateAppointment;
            }
            return null;

        }

        public async Task<IEnumerable<Customer>> GetCustomersWithAppThisWeek()
        {
            var today = DateTime.Now;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var weekEnd = weekStart.AddDays(7);

            return await _appContext.Appointments.Where(a => a.AppointmentTime >= weekStart && a.AppointmentTime < weekEnd).Select(a => a.Customer).Distinct().ToListAsync();
        }

        public async Task<int> GetNumerOfAppThisWeek(int customerId, int weekOfYear)
        {
            var year = new DateTime(DateTime.Now.Year, 1, 1);
            var weekStart = year.AddDays((weekOfYear - 1) * 7);
            var weekEnd = weekStart.AddDays(7);

            return await _appContext.Appointments.Where(a => a.CustomerId == customerId && a.AppointmentTime >= weekStart && a.AppointmentTime < weekEnd).CountAsync();
        }
    }
}

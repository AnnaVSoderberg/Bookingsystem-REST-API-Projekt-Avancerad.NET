using AutoMapper;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Data;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Dto;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_API_Models;
using System;
using System.Globalization;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IBookingSystem<Appointment> _bookingSystemAppointment;
        private readonly IBookingSystem<Customer> _bookingSystemCustomer;
        private readonly IBookingSystem<Company> _bookingSystemCompany;
        private readonly IMapper _mapper;
        private readonly IHistoryAppointment _historyAppointment;
        private readonly AppDbContext _appDbContext;

        public AppointmentController(IBookingSystem<Appointment> bookingSystemAppointment, IBookingSystem<Company> bookingSystemCompany, IBookingSystem<Customer> bookingSystemCustomer, IHistoryAppointment historyAppointment, IMapper mapper, AppDbContext appDbContext)
        {
            _bookingSystemAppointment = bookingSystemAppointment;
            _bookingSystemCompany = bookingSystemCompany;
            _bookingSystemCustomer = bookingSystemCustomer;
            _mapper = mapper;
            _historyAppointment = historyAppointment;
            _appDbContext = appDbContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAppointments(string customerName, string companyName, DateTime? appointmentTime, string sortBy = "appointmentTime")
        {
            try
            {
                var appointments = await _bookingSystemAppointment.GetAll();

                if (!string.IsNullOrEmpty(customerName))
                {
                    appointments = appointments.Where(a => a.Customer.CustomerName.Contains(customerName));
                }

                if (!string.IsNullOrEmpty(companyName))
                {
                    appointments = appointments.Where(a => a.Company.CompanyName.Contains(companyName));
                }

                if (appointmentTime.HasValue)
                {
                    appointments = appointments.Where(a => a.AppointmentTime == appointmentTime.Value);
                }

                // Sortera bokningar
                appointments = sortBy.ToLower() switch
                {
                    "appointmentTime" => appointments.OrderByDescending(a => a.AppointmentTime),
                    "customerName" => appointments.OrderBy(a => a.Customer.CustomerName),
                    "companyName" => appointments.OrderBy(a => a.Company.CompanyName),
                    _ => appointments,
                };

                var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointments);
                return Ok(appointmentDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error to retrieve data from the database");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AppointmentDto>> GetAnAppointment(int id)
        {
            try
            {
                var getAnOppointment = await _bookingSystemAppointment.GetSingle(id);
                if (getAnOppointment == null)
                {
                    return NotFound();
                }
                var appointmnetDto = _mapper.Map<AppointmentDto>(getAnOppointment);
                return Ok(appointmnetDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error to retrive data from database");
            }
        }



        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> CreateNewAppointment(AppointmentDto appointmentDto)
        {
            try
            {
                if (appointmentDto == null)
                {
                    return BadRequest();
                }

                var newAppointment = _mapper.Map<Appointment>(appointmentDto); // Konvertera AppointmentDto till Appointment


                // Kontrollera om kunden redan finns
                if (newAppointment.CustomerId != 0)
                {
                    var existingCustomer = await _bookingSystemCustomer.GetSingle(newAppointment.CustomerId);
                    if (existingCustomer == null)
                    {
                        return BadRequest("Customer not found.");
                    }
                    newAppointment.Customer = existingCustomer;
                }

                // Kontrollera om företaget redan finns
                if (newAppointment.CompanyId != 0)
                {
                    var existingCompany = await _bookingSystemCompany.GetSingle(newAppointment.CompanyId);
                    if (existingCompany == null)
                    {
                        return BadRequest("Company not found.");
                    }
                    newAppointment.Company = existingCompany;
                }


                var createdAppointment = await _bookingSystemAppointment.Add(newAppointment);

                // Logga skapandet av bokningen
                var history = new History
                {
                    AppointmentId = createdAppointment.AppointmentId,
                    Action = "Created",
                    ReasonToChange = "Appointment created",
                    ChangeDate = DateTime.UtcNow
                };
                await _historyAppointment.Add(history);
                await _appDbContext.SaveChangesAsync(); 

                var createdAppointmentFromDb = await _bookingSystemAppointment.GetSingle(createdAppointment.AppointmentId); 

                
                var createdAppointmentDto = _mapper.Map<AppointmentDto>(createdAppointmentFromDb);

                return CreatedAtAction(nameof(GetAnAppointment), new { id = createdAppointmentDto.AppointmentId }, createdAppointmentDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error to post data to database");
            }
        }



        [HttpDelete("{id:int}")]  //Fungerar inte ändra till Cascade delete i database eller Soft delet med extra kolumn. 
        public async Task<ActionResult<Appointment>> DeleteAppointment(int id)
        {
            try
            {
                var appointmentToDelete = await _bookingSystemAppointment.GetSingle(id);
                if (appointmentToDelete == null)
                {
                    return NotFound($"Appointment with ID {id} not found.");
                }


                var deletedAppointment = await _bookingSystemAppointment.Delete(appointmentToDelete.AppointmentId);

  
                var history = new History
                {
                    AppointmentId = deletedAppointment.AppointmentId,
                    Action = "Deleted",
                    ReasonToChange = "Appointment deleted",
                    ChangeDate = DateTime.Now
                };
                await _historyAppointment.Add(history);

                return Ok(deletedAppointment);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error to delete data in the database: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
            }
        }



        [HttpPut("{id:int}")]
        public async Task<ActionResult<Appointment>> UpdateAppointment(int id, [FromBody] Appointment appointment)
        {
            try
            {
                if (id != appointment.AppointmentId)
                {
                    return BadRequest("Appointment ID doesn't match!");
                }

                var appointmentToUpdate = await _bookingSystemAppointment.GetSingle(id);
                if (appointmentToUpdate == null)
                {
                    return NotFound($"Appointment with ID {id} not found.");
                }

                // Uppdatera befintlig bokning med data från inkommande objekt
                appointmentToUpdate.AppointmentTime = appointment.AppointmentTime;
                appointmentToUpdate.CustomerId = appointment.CustomerId;
                appointmentToUpdate.CompanyId = appointment.CompanyId;

                // Uppdatera bokningen i databasen
                await _bookingSystemAppointment.Update(appointmentToUpdate);

                // Logga uppdateringen
                var history = new History
                {
                    AppointmentId = appointmentToUpdate.AppointmentId,
                    Action = "Updated",
                    ReasonToChange = "Appointment updated",
                    ChangeDate = DateTime.Now
                };
                await _historyAppointment.Add(history);

                // Hämta den uppdaterade bokningen
                var updatedAppointment = await _bookingSystemAppointment.GetSingle(appointmentToUpdate.AppointmentId);

                return Ok(updatedAppointment);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data in the database");
            }
        }


        [HttpGet("week/{year}/{week}/{companyId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByWeek(int year, int week, int companyId)
        {
            try
            {
                // Beräkna den första och sista dagen av veckan
                DateTime startOfWeek = FirstDateOfWeek(year, week);
                DateTime endOfWeek = startOfWeek.AddDays(7);

                // Hämta alla möten
                var appointments = await _bookingSystemAppointment.GetAll();

                // Filtrera mötena baserat på veckan och företaget
                var weeklyAppointments = appointments
                    .Where(a => a.AppointmentTime >= startOfWeek && a.AppointmentTime < endOfWeek && a.CompanyId == companyId)
                    .ToList();

                // Konvertera till DTOs
                var appointmentDtos = _mapper.Map<List<AppointmentDto>>(weeklyAppointments);

                return Ok(appointmentDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }


        [HttpGet("month/{year}/{month}/{companyId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsByMonth(int year, int month, int companyId)
        {
            try
            {
                // Beräkna den första och sista dagen av månaden
                DateTime startOfMonth = new DateTime(year, month, 1);
                DateTime endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);

                // Hämta alla möten
                var appointments = await _bookingSystemAppointment.GetAll();

                // Filtrera mötena baserat på månaden och företaget
                var monthlyAppointments = appointments
                    .Where(a => a.AppointmentTime >= startOfMonth && a.AppointmentTime <= endOfMonth && a.CompanyId == companyId)
                    .ToList();

                // Konvertera till DTOs
                var appointmentDtos = _mapper.Map<List<AppointmentDto>>(monthlyAppointments);

                return Ok(appointmentDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        // Hjälpmetod för att beräkna den första dagen av en viss vecka
        private static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

    }
}

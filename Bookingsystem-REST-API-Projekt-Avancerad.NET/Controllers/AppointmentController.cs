using AutoMapper;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Data;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Dto;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projekt_API_Models;
using System;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IBookingSystem<Appointment> _bookingSystem;
        private readonly IMapper _mapper;
        private readonly IAppointment _appointment;

        public AppointmentController(IBookingSystem<Appointment> bookingSystem, IAppointment appointmentService, IMapper mapper)
        {
            _bookingSystem = bookingSystem;
            _mapper = mapper;
            _appointment = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            try
            {
                var appointments = await _bookingSystem.GetAll();
                var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointments);
                return Ok(appointmentDtos);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to get data from database");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Appointment>> GetAnAppointment(int id)
        {
            try
            {
                var getAnOppointment = await _bookingSystem.GetSingle(id);
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

       
        [HttpGet("AppointmentByWeek/{year:int}/{week:int}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByWeek(int year, int week)
        {
            var appointmnets = await _appointment.GetAppointmentWeek(year, week);
            return Ok(appointmnets);
        }

        [HttpGet("AppointmentByMonth/{year:int}/{month:int}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByMonth(int year, int month)
        {
            var appointmnets = await _appointment.GetAppointmentMonth(year, month);
            return Ok(appointmnets);
        }

        [HttpGet("History/{id:int}")]
        public async Task<ActionResult<IEnumerable<History>>> GetHistory(int id)
        {

            var history = await _appointment.GetChangeHistory(id);
            return Ok(history);
        }


        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateNewAppointment(Appointment newAppointment)
        {
            try
            {
                if (newAppointment == null)
                {
                    return BadRequest();
                }
                var createdAppointment = await _bookingSystem.Add(newAppointment);
                return CreatedAtAction(nameof(GetAnAppointment), new { id = createdAppointment.AppointmentId }, createdAppointment);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error to post data to database");
            }
        }



        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Appointment>> DeleteAppointment(int id)
        {
            try
            {
                var AppointmentToDelete = await _bookingSystem.GetSingle(id);
                if (AppointmentToDelete == null)
                {
                    return NotFound($"Order with ID {id} not found to delete.");
                }
                return await _bookingSystem.Delete(id);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to delete data from database");
            }

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Appointment>> UpdateAppointment(int id, Appointment appointment)
        {
            try
            {
                if (id != appointment.AppointmentId)
                {
                    return BadRequest("Appointment is not matching");
                }
                var appointmentToUpdate = await _bookingSystem.GetSingle(id);
                if(appointmentToUpdate == null)
                {
                    return NotFound($"Order with ID: {id} was not found");
                }
                return await _bookingSystem.Update(appointment);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to update data from database");
            }
        }
    }
}

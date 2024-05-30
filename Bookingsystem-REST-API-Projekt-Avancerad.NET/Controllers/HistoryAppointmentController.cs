using AutoMapper;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Dto;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projekt_API_Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryAppointmentController : ControllerBase
    {
        private readonly IHistoryAppointment _historyAppointment;
        private readonly IBookingSystem<Appointment> _bookingSystemAppointment;
        private readonly IMapper _mapper;

        public HistoryAppointmentController(IHistoryAppointment historyAppointment, IBookingSystem<Appointment> bookingSystemAppointment, IMapper mapper)
        {
            _historyAppointment = historyAppointment;
            _bookingSystemAppointment = bookingSystemAppointment;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminCompanyPolicy")]
        public async Task<ActionResult<IEnumerable<HistoryWithAppointmentDto>>> GetAllHistory()
        {
            var history = await _historyAppointment.GetAll();

            var historyWithAppointments = new List<HistoryWithAppointmentDto>();

            foreach (var record in history)
            {
                var appointment = await _bookingSystemAppointment.GetSingle(record.AppointmentId);
                var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
                var historyWithAppointmentDto = new HistoryWithAppointmentDto
                {
                    HistoryId = record.HistoryId,
                    AppointmentId = record.AppointmentId,
                    Action = record.Action,
                    ReasonToChange = record.ReasonToChange,
                    ChangeDate = record.ChangeDate,
                    Appointment = appointmentDto
                };
                historyWithAppointments.Add(historyWithAppointmentDto);
            }

            return Ok(historyWithAppointments);
        }

        [HttpGet("{appId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminCompanyPolicy")]
        public async Task<ActionResult<IEnumerable<HistoryWithAppointmentDto>>> GetHistoryByAppointment(int appId)
        {
            var history = await _historyAppointment.GetByAppointmentId(appId);

            var historyWithAppointments = new List<HistoryWithAppointmentDto>();

            foreach (var record in history)
            {
                var appointment = await _bookingSystemAppointment.GetSingle(record.AppointmentId);
                var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
                var historyWithAppointmentDto = new HistoryWithAppointmentDto
                {
                    HistoryId = record.HistoryId,
                    AppointmentId = record.AppointmentId,
                    Action = record.Action,
                    ReasonToChange = record.ReasonToChange,
                    ChangeDate = record.ChangeDate,
                    Appointment = appointmentDto
                };
                historyWithAppointments.Add(historyWithAppointmentDto);
            }

            return Ok(historyWithAppointments);
        }
    }
}

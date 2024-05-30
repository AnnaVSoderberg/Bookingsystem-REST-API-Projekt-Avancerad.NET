using Bookingsystem_REST_API_Projekt_Avancerad.NET.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projekt_API_Models;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryAppointmentController : ControllerBase
    {
        private readonly IHistoryAppointment _historyAppointment;

        public HistoryAppointmentController(IHistoryAppointment historyAppointment)
        {
            _historyAppointment = historyAppointment;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminCompanyPolicy")]
        public async Task<ActionResult<History>> GetAllHistory() 
        {
            var history = await _historyAppointment.GetAll();
            return Ok(history);
        }

        [HttpGet("appId")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminCompanyPolicy")]
        public async Task<ActionResult<History>> GetHistoryByAppointment(int appId)
        {
            var history = await _historyAppointment.GetByAppointmentId(appId);
            return Ok(history);
        }
    }
}

using AutoMapper;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Data;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Dto;
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
    public class CompanyController : ControllerBase
    {
        private readonly IBookingSystem<Company> _bookingSystem;
        private readonly IMapper _mapper;
        private readonly AppDbContext _appDbContext;

        public CompanyController(IBookingSystem<Company> bookingSystem, IMapper mapper, AppDbContext appDbContext)
        {
            _bookingSystem = bookingSystem;
            _mapper = mapper;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminCompanyPolicy")]
        public async Task<IActionResult> GetAllCompanies(string name, string sortBy = "name")
        {
            try
            {
                var companies = await _bookingSystem.GetAll();

                if (!string.IsNullOrEmpty(name))
                {
                    companies = companies.Where(c => c.CompanyName.Contains(name));
                }

                // Sortera
                companies = sortBy.ToLower() switch
                {
                    "name" => companies.OrderBy(c => c.CompanyName),
                    _ => companies,
                };

                var companyDto = _mapper.Map<List<CompanyDto>>(companies);
                return Ok(companyDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error to retrieve data from the database");
            }
        }

        [HttpGet("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminCompanyPolicy")]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            try
            {
                var result = await _bookingSystem.GetSingle(id);
                if (result == null)
                {
                    return NotFound();
                }
                var companyDto = _mapper.Map<CompanyDto>(result);
                return Ok(companyDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error to retrive data from database....");
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminCompanyPolicy")]
        public async Task<ActionResult<CompanyDto>> CreateNewCompany(CompanyDto newCompanyDto)
        {
            try
            {
                if (newCompanyDto == null)
                    return BadRequest();



                var newCompany = _mapper.Map<Company>(newCompanyDto);
                var createdCompany = await _bookingSystem.Add(newCompany);

                var createdCompanyFromDb = await _bookingSystem.GetSingle(createdCompany.CompanyId);

                var createdCompanyDto = _mapper.Map<CompanyDto>(createdCompanyFromDb);

                return CreatedAtAction(nameof(GetCompany), new { id = createdCompany.CompanyId }, createdCompanyDto);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to create data in to the database....");
            }
        }




        [HttpDelete("{id:int}")] //Kan inte ta bort om det finns bokade appointments lägga in "OnCascade" ?
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        public async Task<ActionResult<Company>> DeleteCompany(int id)
        {
            try
            {
                var CompanyToDelete = await _bookingSystem.GetSingle(id);
                if (CompanyToDelete == null)
                {
                    return NotFound($"Company with Id: {id}, was not found to delete");
                }
                return await _bookingSystem.Delete(id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error to delete data in the database: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
            }
        }



        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminCompanyPolicy")]
        public async Task<ActionResult<CompanyDto>> UpdateCompany(int id, CompanyDto companyDto)
        {
            try
            {
                if (id != companyDto.CompanyId)
                {
                    return BadRequest($"Company with Id: {id}, does not match");
                }
                var companyToUpdate = await _bookingSystem.GetSingle(id);

                if (companyToUpdate == null)
                {
                    return NotFound($"Company with Id: {id} not found in database");
                }

                // Map CompanyDto to Company
                var company = _mapper.Map<Company>(companyDto);
                var updatedCompany = await _bookingSystem.Update(company);


                // Map updated Company to CompanyDto
                var updatedCompanyDto = _mapper.Map<CompanyDto>(updatedCompany);

                return Ok(updatedCompanyDto);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to update data in the database....");
            }
        }
    }
}

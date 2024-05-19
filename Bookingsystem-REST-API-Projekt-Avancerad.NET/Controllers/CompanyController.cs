using AutoMapper;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Dto;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Services;
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

        public CompanyController(IBookingSystem<Company> bookingSystem, IMapper mapper)
        {
            _bookingSystem = bookingSystem;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCompanies()
        {
            try
            {
                var companies = await _bookingSystem.GetAll();
                var companyDtos = _mapper.Map<List<CompanyDto>>(companies);
                return Ok(companyDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error to retrive data from database....");
            }
        }

        [HttpGet("{id:int}")]
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
        public async Task<ActionResult<Company>> CreateNewProduct(Company newCompany)
        {
            try
            {
                if(newCompany == null) 
                return BadRequest();

                var createdCompany = await _bookingSystem.Add(newCompany);
                return CreatedAtAction(nameof(GetCompany), new { id = createdCompany.CompanyId }, createdCompany);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to create data in to the database....");
            }
        }

        [HttpDelete ("{id:int}")]
        public async Task<ActionResult<Company>> DeleteCompany(int id)
        {
            try
            {
                var CompanyToDelete = await _bookingSystem.GetSingle(id);
                if(CompanyToDelete == null)
                {
                    return NotFound($"Company with Id: {id}, was not found to delete");
                }
                return await _bookingSystem.Delete(id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error to delete data in the database....");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Company>> UpdateCompany(int id, Company company)
        {
            try
            {
                if(id != company.CompanyId)
                {
                    return BadRequest($"Company with Id: {id}, does not match");
                }
                var companyToUpdate = await _bookingSystem.GetSingle(id);

                if (companyToUpdate == null)
                {
                    return NotFound($"Company with Id: {id} not found in database");
                }
                return await _bookingSystem.Update(company);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to update data in the database....");
            }
        }
    }
}

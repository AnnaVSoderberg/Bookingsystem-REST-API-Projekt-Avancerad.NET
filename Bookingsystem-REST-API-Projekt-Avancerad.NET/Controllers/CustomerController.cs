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
    public class CustomerController : ControllerBase
    {
        private readonly IBookingSystem<Customer> _bookingSystem;
        private readonly IMapper _mapper;
        private readonly AppDbContext _appDbContext;



        public CustomerController(IBookingSystem<Customer> bookingSystem, IMapper mapper, AppDbContext appDbContext)
        {
            _bookingSystem = bookingSystem;
            _mapper = mapper;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers(string name, string email, string phoneNumber, string sortBy = "name")
        {
            try
            {
                var customers = await _bookingSystem.GetAll();

                if (!string.IsNullOrEmpty(name))
                {
                    customers = customers.Where(c => c.CustomerName.Contains(name));
                }

                if (!string.IsNullOrEmpty(email))
                {
                    customers = customers.Where(c => c.CustomerEmail.Contains(email));
                }

                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    customers = customers.Where(c => c.CustomerPhoneNumber == phoneNumber);
                }

                // Sortera kunder
                customers = sortBy.ToLower() switch
                {
                    "name" => customers.OrderBy(c => c.CustomerName),
                    "email" => customers.OrderBy(c => c.CustomerEmail),
                    "phoneNumber" => customers.OrderBy(c => c.CustomerPhoneNumber),
                    _ => customers,
                };

                var customerDtos = _mapper.Map<List<CustomerDto>>(customers);
                return Ok(customerDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error to retrieve data from the database");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerDto>> GetACustomer(int id)
        {
            try
            {
                var customer = await _bookingSystem.GetSingle(id);
                if (customer == null)
                {
                    return NotFound();

                }
                var customerDto = _mapper.Map<CustomerDto>(customer);
                return Ok(customerDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error to retrive data from database....");
            }
        }

        [HttpGet("ACustomersAppointments/{id:int}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetACustomersAppointments(int id)
        {
            try
            {
                var customer = await _bookingSystem.GetSingle(id);
                if (customer == null)
                {
                    return NotFound();
                }
                var appointmentDto = _mapper.Map<IEnumerable<AppointmentDto>>(customer.Appointments);
                return Ok(appointmentDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error to retrive data from database");
            }
        }



        [HttpPost]
        public async Task<ActionResult<CustomerDto>> CreatNewCustomer(CustomerDto newCustomerDto)
        {
            try
            {
                if (newCustomerDto == null)
                    return BadRequest();

                var newCustomer =_mapper.Map<Customer>(newCustomerDto); 

                var createdCustomer = await _bookingSystem.Add(newCustomer);
                // Hämta den skapade kunden med alla uppdaterade fält, inklusive bokningar
                var createdCustomerFromDb = await _bookingSystem.GetSingle(createdCustomer.CustomerId);
                // Konvertera den skapade kunden till CustomerDto
                var createdCustomerDto = _mapper.Map<CustomerDto>(createdCustomerFromDb);


                return CreatedAtAction(nameof(GetACustomer), new { id = createdCustomerDto.CustomerId }, createdCustomerDto);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to create data in to the database");
            }
        }



        [HttpDelete("{id:int}")] //BEHÖVER TA BORT INLOG OCH APPOINTMEENTS OCKSÅ (lägg in onCascade)
        public async Task<ActionResult<Customer>> DeleteCustomer(int id) 
        {
            try
            {
                var CustomerToDelete = await _bookingSystem.GetSingle(id);
                if (CustomerToDelete == null)
                {
                    return NotFound($"Customer with Id: {id}, was not found to delete");
                }
                await _bookingSystem.Delete(id);
                return Ok(CustomerToDelete);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"Error to delete data in the database: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Customer>> UpdateCustomer(int id, CustomerDto customerDto)
        {
            try
            {
                if (id != customerDto.CustomerId)
                {
                    return BadRequest($"Customer with Id: {id}, does not match");
                }
                var customerToUpdate = await _bookingSystem.GetSingle(id);

                if (customerToUpdate == null)
                {
                    return NotFound($"Customer with Id: {id} not found in database");
                }
                var customer = _mapper.Map<Customer>(customerDto);
                return await _bookingSystem.Update(customer);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to update data in the database....");
            }

        }

    }

}

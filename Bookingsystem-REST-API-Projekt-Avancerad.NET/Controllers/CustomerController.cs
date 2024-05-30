using AutoMapper;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Data;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Dto;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IHistoryAppointment _historyAppointment;



        public CustomerController(IBookingSystem<Customer> bookingSystem, IMapper mapper, AppDbContext appDbContext, IHistoryAppointment historyAppointment)
        {
            _bookingSystem = bookingSystem;
            _mapper = mapper;
            _appDbContext = appDbContext;
            _historyAppointment = historyAppointment;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminUserPolicy")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminUserPolicy")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminUserPolicy")]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminUserPolicy")]
        public async Task<ActionResult<CustomerDto>> CreateNewCustomer(CustomerDto newCustomerDto)
        {
            try
            {
                if (newCustomerDto == null)
                    return BadRequest("Customer data is null.");

                Console.WriteLine("Creating a new customer");

                var newCustomer = _mapper.Map<Customer>(newCustomerDto);
                var createdCustomer = await _bookingSystem.Add(newCustomer);
                var createdCustomerFromDb = await _bookingSystem.GetSingle(createdCustomer.CustomerId);
                var createdCustomerDto = _mapper.Map<CustomerDto>(createdCustomerFromDb);

                return CreatedAtAction(nameof(GetACustomer), new { id = createdCustomerDto.CustomerId }, createdCustomerDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while creating a new customer: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error to create data in to the database: {ex.Message}");
            }
        }


        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
        public async Task<ActionResult<Customer>> DeleteCustomer(int id)
        {
            try
            {
                Console.WriteLine($"Deleting customer with ID {id}");

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
                Console.WriteLine($"Error occurred while deleting customer with ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error to delete data in the database: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
            }
        }


        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminUserPolicy")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminUserPolicy")]
        public async Task<ActionResult<Customer>> UpdateCustomer(int id, CustomerDto customerDto)
        {
            try
            {
                if (id != customerDto.CustomerId)
                {
                    return BadRequest($"Customer with Id: {id}, does not match");
                }

                Console.WriteLine($"Updating customer with ID {id}");

                var customerToUpdate = await _bookingSystem.GetSingle(id);
                if (customerToUpdate == null)
                {
                    return NotFound($"Customer with Id: {id} not found in database");
                }

                var customer = _mapper.Map<Customer>(customerDto);
                var updatedCustomer = await _bookingSystem.Update(customer);


                return Ok(updatedCustomer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while updating customer with ID {id}: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error to update data in the database: {ex.Message}");
            }
        }

    }

}

using AutoMapper;
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
        private readonly ICustomer _customer;
        private readonly IAppointment _appointment;


        public CustomerController(IBookingSystem<Customer> bookingSystem, IMapper mapper, ICustomer customer, IAppointment appointment)
        {
            _bookingSystem = bookingSystem;
            _mapper = mapper;
            _customer = customer;
            _appointment = appointment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _bookingSystem.GetAll();
                var customerDtos = _mapper.Map<List<CustomerDto>>(customers);
                return Ok (customerDtos);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to retrive data from database....");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Customer>> GetACustomer(int id)
        {
            try
            {
                var customer = await _bookingSystem.GetSingle(id);
                if(customer == null) 
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

        [HttpGet("CustomersAppointments/{id:int}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetACustomersAppointments(int id)
        {
            try
            {
                var customer = await _bookingSystem.GetSingle(id);
                if (customer == null)
                {
                    return NotFound();
                }
                return Ok(customer.Appointments);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error to retrive data from database");
            }
        }

        [HttpGet("Appointments/ThisWeek")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomerswithAppointmentsThisWeek()
        {
            try
            {
                var startOfWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                var customers = await _customer.GetCustomersWithAppointmentWeek(startOfWeek);
                if(customers == null || !customers.Any())
                {
                    return NotFound("No customers found with appointments this week");
                }
                return Ok(customers);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to retrive data from database");
            }

        }

        [HttpGet("{id}/numberOfAppointments/week/{startOfWeek}")]
        public async Task<ActionResult<int>> GetCountOfACustomersAppointmentsThisWeek(int id, DateTime startOfWeek)
        {
           
                var count = await _customer.GetCustomerAppointmentCountWeek(id, startOfWeek);
                return Ok(count);
        }

        [HttpGet ("SortAndFilter")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomersSortedAndFilterd(string sortField, string sortOrder, string filterField, string filterValue)
        {
            var customers = await _customer.GetCustomersSortedAndFiltered(sortField, sortOrder, filterField, filterValue);
            return Ok(customers);
        }


        [HttpPost]
        public async Task<ActionResult<Customer>> CreatNewCustomer(Customer newCustomer)
        {
            try
            {
                if(newCustomer == null)
                    return BadRequest();

                var createdCustomer = await _bookingSystem.Add(newCustomer);
                return CreatedAtAction(nameof(GetACustomer), new { id = createdCustomer.CustomerId }, createdCustomer);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to create data in to the database");
            }
        }

        [HttpDelete ("{id:int}")]
        public async Task<ActionResult<Customer>> DeleteCustomer(int id)
        {
            try
            {
                var CustomerToDelete = await _bookingSystem.GetSingle(id);
                if(CustomerToDelete == null)
                {
                    return NotFound($"Customer with Id: {id}, was not found to delete");
                }
                return await _bookingSystem.Delete(id);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to delete data in the database....");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Customer>> UpdateCustomer(int id, Customer customer)
        {
            try
            {
                if(id != customer.CustomerId)
                {
                    return BadRequest($"Customer with Id: {id}, does not match");
                }
                var customerToUpdate = await _bookingSystem.GetSingle(id);

                if(customerToUpdate == null)
                {
                    return NotFound($"Customer with Id: {id} not found in database");
                }
                return await _bookingSystem.Update(customer);
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error to update data in the database....");
            }

        }

    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-diff).Date;
        }
    }
}

using Bookingsystem_REST_API_Projekt_Avancerad.NET.Data;
using Microsoft.EntityFrameworkCore;
using Projekt_API_Models;
using System;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Services
{
    public class CustomerRepository : IBookingSystem<Customer>
    {

        private AppDbContext _appContext;

        public CustomerRepository(AppDbContext appContext)
        {
            _appContext = appContext;
        }
        public async Task<Customer> Add(Customer newEntity) //OK
        {
            var result = await _appContext.Customers.AddAsync(newEntity);
            await _appContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Customer> Delete(int id) //OK
        {
            var customerToDelete = await _appContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customerToDelete != null)
            {
                _appContext.Customers.Remove(customerToDelete);
                await _appContext.SaveChangesAsync();
                return customerToDelete;
            }
            return null;
        }

        public async Task<IEnumerable<Customer>> GetAll() //OK
        {
            return await _appContext.Customers.Include(c => c.Appointments).ToListAsync();
        }

        //public async Task<Customer> GetById(int id) //OK
        //{
        //    return await _appContext.Customers.Include(c => c.Appointments).FirstOrDefaultAsync(c => c.CustomerId == id);
        //}

        //public async Task<int> GetCustomerAppointmentCountWeek(int customerId, DateTime startOfWeek) //OK
        //{
        //    var endOfWeek = startOfWeek.AddDays(7);
        //    return await _appContext.Appointments.Where (a => a.CustomerId == customerId
        //                                                 && a.CreatingDateAppointment <= startOfWeek
        //                                                  && a.CreatingDateAppointment < endOfWeek).CountAsync();
        //}

        public async Task<IEnumerable<Customer>> GetCustomersSortedAndFiltered(string sortField, string sortOrder, string filterField, string filterValue) //OK
        {
            IQueryable<Customer> query = _appContext.Customers.Include(c => c.CustomerId);

            if(!string.IsNullOrEmpty(filterField) && !string.IsNullOrEmpty(filterValue))
                if (filterField == "Name")
                {
                    query = query.Where(q => q.CustomerName.Contains(filterValue));
                }
            if(!string.IsNullOrEmpty(filterField) && !string.IsNullOrEmpty(filterValue))
            {
                if (sortField == "Name" && sortOrder == "asc")
                {
                    query = query.OrderBy(q => q.CustomerName);
                }
                else if (sortField == "Name" && sortOrder == "desc")
                {
                    query = query.OrderByDescending(q => q.CustomerName);
                }
            }
            return await query.ToListAsync();
        }

        //public async Task<IEnumerable<Customer>> GetCustomersWithAppointmentWeek(DateTime startOfWeek) 
        //{
        //    var endOfWeek = startOfWeek.AddDays(7);
        //    var customerWithAppointmentThisWeek = await _appContext.Customers.Include(c => c.Appointments)
        //                                        .Where(c => c.Appointments.Any(d => d.CreatingDateAppointment >= startOfWeek && d.CreatingDateAppointment < endOfWeek))
        //                                        .ToListAsync();
        //    return customerWithAppointmentThisWeek;
        //}

        public async Task<Customer> GetSingle(int id)
        {
            return await _appContext.Customers.Include(c => c.Appointments).FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task<Customer> Update(Customer entity) //OK
        {
            var CustomerInfoToChange = await _appContext.Customers.FirstOrDefaultAsync(c => c.CustomerId == entity.CustomerId);

            if (CustomerInfoToChange != null)
            {
                CustomerInfoToChange.CustomerName = entity.CustomerName;
                CustomerInfoToChange.CustomerPhoneNumber = entity.CustomerPhoneNumber;
                CustomerInfoToChange.CustomerEmail = entity.CustomerEmail;
                await _appContext.SaveChangesAsync();
                return CustomerInfoToChange;
            }
            return null;
        }

        public async Task<Customer> GetCustomerWithAppointments(int id)
        {
            var customer = await _appContext.Customers.Include(c => c.Appointments).FirstOrDefaultAsync(c => c.CustomerId == id);
                return customer;
        }


    }
}

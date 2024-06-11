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
    }
}

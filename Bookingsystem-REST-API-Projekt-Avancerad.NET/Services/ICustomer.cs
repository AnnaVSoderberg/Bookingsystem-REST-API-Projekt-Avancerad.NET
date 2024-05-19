using Projekt_API_Models;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Services
{
    public interface ICustomer
    {
        Task<IEnumerable<Customer>> GetCustomersWithAppointmentWeek(DateTime startOfWeek);
        Task<int> GetCustomerAppointmentCountWeek(int customerId, DateTime startOfWeek);
        Task<IEnumerable<Customer>> GetCustomersSortedAndFiltered(string sortField, string sortOrder, string filterField, string filterValue);
    }
}

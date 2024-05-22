using Projekt_API_Models;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Services
{
    public interface ILogIn
    {
        Task<int?> GetCustomerIdByEmail(string email);
        Task<int?> GetCompanyIdByEmail(string email);
        Task<LogInDetails> GetByEmail(string email);
        Task<LogInDetails> GetByCustomerId(int customerId);

    }
}

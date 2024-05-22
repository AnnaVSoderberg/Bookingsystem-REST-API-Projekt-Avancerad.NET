using Bookingsystem_REST_API_Projekt_Avancerad.NET.Data;
using Projekt_API_Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Services
{
    public class LoginRepository: IBookingSystem<LogInDetails>, ILogIn
    {
        private AppDbContext _appContext;

        public LoginRepository(AppDbContext appContext)
        {
            _appContext = appContext;
        }

        public async Task<LogInDetails> Add(LogInDetails newEntity)
        {
            var result = await _appContext.LogInDetails.AddAsync(newEntity);
            await _appContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<LogInDetails> Delete(int id)
        {
            var loginToDelete = await _appContext.LogInDetails.FirstOrDefaultAsync(l => l.LoginId == id);
            if(loginToDelete != null)
            {
                _appContext.LogInDetails.Remove(loginToDelete);
                await _appContext.SaveChangesAsync();
                return loginToDelete;
            }
            return null;
        }

        public async Task<IEnumerable<LogInDetails>> GetAll()
        {
            return await _appContext.LogInDetails.ToListAsync();
        }

        public async Task<LogInDetails> GetByCustomerId(int customerId)
        {
            return await _appContext.LogInDetails.FirstOrDefaultAsync(l => l.CustomerId == customerId);
        }

        public async Task<LogInDetails> GetByEmail(string username)
        {
            return await _appContext.LogInDetails.FirstOrDefaultAsync(l => l.Username == username);
        }

        public async Task<int?> GetCompanyIdByEmail(string username)
        {
            var info = await _appContext.LogInDetails.FirstOrDefaultAsync(l => l.Username == username);
            return info.CompanyId;
        }

        public async Task<int?> GetCustomerIdByEmail(string username)
        {
            var info = await _appContext.LogInDetails.FirstOrDefaultAsync(l => l.Username == username);
            return info.CustomerId;
        }

        public async Task<LogInDetails> GetSingle(int id)
        {
            return await _appContext.LogInDetails.FirstOrDefaultAsync(l => l.LoginId == id);
        }

        public async Task<LogInDetails> Update(LogInDetails entity)
        {
            var loginToUpdate = await _appContext.LogInDetails.FirstOrDefaultAsync(l => l.LoginId == entity.LoginId);
            if(loginToUpdate != null)
            {
                loginToUpdate.Username = entity.Username;
                loginToUpdate.Role = entity.Role;
                loginToUpdate.CustomerId = entity.CustomerId;
                loginToUpdate.CompanyId = entity.CompanyId;
                loginToUpdate.Password = entity.Password;

                return loginToUpdate;
            }
            return null;
        }
    }
}

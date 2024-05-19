using Bookingsystem_REST_API_Projekt_Avancerad.NET.Data;
using Microsoft.EntityFrameworkCore;
using Projekt_API_Models;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Services
{
    public class CompanyRepository : IBookingSystem<Company>
    {

        private AppDbContext _appContext;

        public CompanyRepository(AppDbContext appContext)
        {
            _appContext = appContext;
        }

        public async Task<Company> Add(Company newEntity)
        {
            var result = await _appContext.Companies.AddAsync(newEntity);
            await _appContext.SaveChangesAsync();
            return result.Entity;

        }

        public async Task<Company> Delete(int id)
        {
            var companyToDelete = await _appContext.Companies.FirstOrDefaultAsync(c => c.CompanyId == id);
            if (companyToDelete != null)
            {
                _appContext.Companies.Remove(companyToDelete);
                await _appContext.SaveChangesAsync();
                return companyToDelete;
            }
            return null;

        }

        public async Task<IEnumerable<Company>> GetAll()
        {
            return await _appContext.Companies.ToListAsync();
        }

        public async Task<Company> GetSingle(int id)
        {
            return await _appContext.Companies.FirstOrDefaultAsync(c => c.CompanyId == id);
        }


        public async Task<Company> Update(Company entity)
        {
            var updateCompany = await _appContext.Companies.FirstOrDefaultAsync(c => c.CompanyId == entity.CompanyId);

            if(updateCompany != null)
            {
                updateCompany.CompanyName = entity.CompanyName;
                

                await _appContext.SaveChangesAsync();
                return updateCompany;
            }
            return null;
        }
    }
}

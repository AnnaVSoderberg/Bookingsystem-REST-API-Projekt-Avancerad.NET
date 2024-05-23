using Bookingsystem_REST_API_Projekt_Avancerad.NET.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Authentication
{
    public class AuthenticationServiceRepository : IMyAuthenticationService
    {
        private readonly AppDbContext _DbContext;

        public AuthenticationServiceRepository(AppDbContext DbContext)
        {
            _DbContext = DbContext;
        }

        public async Task<AuthenticateResult> AuthenticateAsync(string username, string password)
        {
            var user = await _DbContext.LogInDetails.SingleOrDefaultAsync(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                return AuthenticateResult.Fail("Invalid Username or Password");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, user.Role) // Include user's role as a claim
            };

            var identity = new ClaimsIdentity(claims, "custom");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "custom");

            return AuthenticateResult.Success(ticket);
        }

    }
}

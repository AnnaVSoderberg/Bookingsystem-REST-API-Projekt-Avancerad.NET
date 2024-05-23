using Microsoft.AspNetCore.Authentication;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET.Authentication
{
    public interface IMyAuthenticationService
    {
        Task<AuthenticateResult> AuthenticateAsync(string username, string password);
    }
}

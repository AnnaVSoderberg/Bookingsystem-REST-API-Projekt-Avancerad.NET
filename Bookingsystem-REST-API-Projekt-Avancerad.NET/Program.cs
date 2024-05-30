using Bookingsystem_REST_API_Projekt_Avancerad.NET.Data;
using Bookingsystem_REST_API_Projekt_Avancerad.NET.Services;
using Microsoft.EntityFrameworkCore;
using Projekt_API_Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace Bookingsystem_REST_API_Projekt_Avancerad.NET
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Hämta konfigurationsobjektet
            var configuration = builder.Configuration;

            // Lägg till tjänster till containern.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Lägg till databaskontext för Identity
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

            // Lägg till Identity-tjänster
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddSignInManager()
                .AddRoles<IdentityRole>();

            // Lägg till autentiseringstjänster
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Version = "v1" });

                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }, Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("AdminCompanyUserPolicy", o =>
                {
                    o.RequireAuthenticatedUser();
                    o.RequireRole("admin", "company", "user");
                })
                .AddPolicy("AdminCompanyPolicy", o =>
                {
                    o.RequireAuthenticatedUser();
                    o.RequireRole("admin", "company");
                })
                .AddPolicy("AdminUserPolicy", o =>
                {
                    o.RequireAuthenticatedUser();
                    o.RequireRole("admin", "user");
                })
                .AddPolicy("AdminPolicy", o =>
                {
                    o.RequireAuthenticatedUser();
                    o.RequireRole("admin");
                });

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddScoped<IBookingSystem<Company>, CompanyRepository>();
            builder.Services.AddScoped<IBookingSystem<Appointment>, AppointmentRepository>();
            builder.Services.AddScoped<IBookingSystem<Customer>, CustomerRepository>();
            builder.Services.AddScoped<IHistoryAppointment, HistoryAppointmnetRepository>();

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

            var app = builder.Build();

            // Konfigurera HTTP-begäran pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapPost("/account/create",
                async (string email, string password, string role, UserManager<IdentityUser> userManager) =>
                {
                    IdentityUser User = await userManager.FindByEmailAsync(email);
                    if (User != null) return Results.BadRequest(false);

                    IdentityUser user = new()
                    {
                        UserName = email,
                        PasswordHash = password,
                        Email = email
                    };
                    IdentityResult result = await userManager.CreateAsync(user, password);

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine(error.Description);
                        }
                        return Results.BadRequest(false);
                    }

                    Claim[] userClaims =
                    {
                        new Claim(ClaimTypes.Email,email),
                        new Claim(ClaimTypes.Role, role)
                    };
                    await userManager.AddClaimsAsync(user!, userClaims);
                    return Results.Ok(true);
                });

            app.MapPost("account/login", async (string email, string password, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration config) =>
            {
                IdentityUser User = await userManager.FindByEmailAsync(email);
                if (User == null) return Results.NotFound();

                SignInResult result = await signInManager.CheckPasswordSignInAsync(User!, password, false);
                if (!result.Succeeded) return Results.BadRequest(null);

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: config["Jwt:Issuer"],
                    audience: config["Jwt:Audience"],
                    claims: await userManager.GetClaimsAsync(User),
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );
                return Results.Ok(new JwtSecurityTokenHandler().WriteToken(token));
            });

            ////ExempelCode
            //app.MapGet("/list",
            //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminCompanyPolicy")]
            //() =>
            //{ return Results.Ok("Admin and Company has access"); });

            //app.MapGet("/single",
            //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminUserPolicy")]
            //() =>
            //{ return Results.Ok("Admin and User has access"); });


            //app.MapGet("/single1",
            //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminPolicy")]
            //() =>
            //{ return Results.Ok("Admin has access"); });


            //app.MapGet("/home",
            //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminCompanyUserPolicy")]
            //() =>
            //{ return Results.Ok("Welcome everyone"); });

            app.Run();
        }
    }
}
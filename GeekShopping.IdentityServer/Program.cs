using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connection = builder.Configuration["MySqlConnection:MySqlConnectionString"];

            builder.Services.AddDbContext<MySQLContext>(options => options.
                UseMySql(connection,
                    new MySqlServerVersion(new Version(10, 4, 27))));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<MySQLContext>()
                .AddDefaultTokenProviders();

            var builder1 = builder.Services.AddIdentityServer(optionos =>
            {
                optionos.Events.RaiseErrorEvents = true;
                optionos.Events.RaiseInformationEvents = true;
                optionos.Events.RaiseFailureEvents = true;
                optionos.Events.RaiseSuccessEvents = true;
                optionos.EmitStaticAudienceClaim = true;

            }).AddInMemoryIdentityResources(
                    IdentityConfiguration.IdentityResources)
                .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
                .AddInMemoryClients(IdentityConfiguration.Clients)
                .AddAspNetIdentity<ApplicationUser>();

            builder1.AddDeveloperSigningCredential();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
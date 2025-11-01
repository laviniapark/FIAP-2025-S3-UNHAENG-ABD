using ManagementApp.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace ManagementApp.Tests ;

    public class CustomWebAppFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptors = services.Where(d =>
                    d.ServiceType == typeof(DbContextOptions<ManagementDb>) ||
                    d.ServiceType == typeof(ManagementDb));

                foreach (var descriptor in dbContextDescriptors.ToArray())
                {
                    services.Remove(descriptor);
                }
                
                services.AddDbContext<ManagementDb>(options =>
                    options.UseInMemoryDatabase("ManagementDb"));
                
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ManagementDb>();
                db.Database.EnsureCreated();
            });
        }
    }
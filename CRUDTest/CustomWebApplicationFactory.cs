using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CRUDTest
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
                var dbContextOptionsDescriptor = services
                    .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (dbContextOptionsDescriptor != null)
                    services.Remove(dbContextOptionsDescriptor);


                var dbContextDescriptor = services
                    .SingleOrDefault(d => d.ServiceType == typeof(ApplicationDbContext));

                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("DatabaseForTesting");
                });

                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                db.Database.EnsureCreated();
            });
        }
    }
}

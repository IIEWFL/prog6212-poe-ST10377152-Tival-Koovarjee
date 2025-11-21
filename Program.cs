using ContractMonthlyClaimSystem.Data;
using ContractMonthlyClaimSystem.Middleware;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Services;



namespace ContractMonthlyClaimSystem
{
    public class Program
    {
        //code attribution
//        .NET Foundation(2024) ASP.NET Core MVC Framework(Version 9.0) [Software]. Available at: https://github.com/dotnet/aspnetcore [Accessed: 17 November 2025]
//Bootstrap Authors(2023) Bootstrap(Version 5.3.2) [Library]. Available at: https://getbootstrap.com/ [Accessed: 17 November 2025]
//OpenJS Foundation(2023) jQuery(Version 3.7.1) [Library]. Available at: https://jquery.com/ (Accessed: 15 March 2025)
//Moq Contributors(2024) Moq(Version 4.20.72) [Testing Library]. Available at: https://github.com/moq/moq [Accessed: 17 November 2025]
//Sommerville, I. (2016) Software Engineering. 10th edn. Harlow: Pearson Education.
//Pressman, R.S.and Maxim, B.R. (2020) Software Engineering: A Practitioner's Approach. 9th edn. New York: McGraw-Hill Education.


        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Configure Entity Framework with SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IClaimVerificationService, ClaimVerificationService>();

            var app = builder.Build();

            // Add detailed error pages in development
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); // This serves wwwroot files (CSS, JS, images)

            app.UseRouting();
            app.UseAuthorization();

            // Map controller routes
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

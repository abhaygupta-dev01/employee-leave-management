using LeaveManagementSystem;
using LeaveManagementSystem.Services;
using Microsoft.Extensions.Logging;

namespace LeaveManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession(); // ? Enables session support
            builder.Services.AddSingleton<EmailService>();


            var app = builder.Build();

            // Initialize database tables
            try
            {
                var logger = app.Services.GetRequiredService<ILogger<DatabaseInitializer>>();
                var configuration = app.Configuration;
                var initializer = new DatabaseInitializer(configuration, logger);
                initializer.InitializeDatabase();
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Failed to initialize database: {Message}", ex.Message);
                // Continue app startup even if initialization fails
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // ? Activates session middleware
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}"); // Optional: start at login

            app.Run();
        }
    }
}

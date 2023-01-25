using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Graph;
using TRMApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TRMApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


    }
}

//internal class Program
//{


//    private static void Main(string[] args)
//    {

//        var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

//        // Add services to the container.
//        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//        builder.Services.AddDbContext<ApplicationDbContext>(options =>
//            options.UseSqlServer(connectionString));
//        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//            .AddEntityFrameworkStores<ApplicationDbContext>();
//        builder.Services.AddControllersWithViews();

//        var app = builder.Build();

//        // Configure the HTTP request pipeline.
//        if (app.Environment.IsDevelopment())
//        {
//            app.UseMigrationsEndPoint();
//        }
//        else
//        {
//            app.UseExceptionHandler("/Home/Error");
//            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//            app.UseHsts();
//        }

//        app.UseHttpsRedirection();
//        app.UseStaticFiles();

//        app.UseRouting();

//        app.UseAuthentication();
//        app.UseAuthorization();

//        app.MapControllerRoute(
//            name: "default",
//            pattern: "{controller=Home}/{action=Index}/{id?}");
//        app.MapRazorPages();

//        app.Run();
//    }
//}
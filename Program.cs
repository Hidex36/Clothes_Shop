using Clothes_shop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Clothes_shop.Models;
using Clothes_shop.Repositories;

namespace Clothes_shop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            // Add DbContext and Identity services
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<Users, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddScoped<Cart>(sp => Cart.GetCart(sp));
            builder.Services.AddTransient<IOrderResponsitory, OrderResponsitory>();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
                var userManager = services.GetRequiredService<UserManager<Users>>();
                var config = services.GetRequiredService<IConfiguration>();

                var roles = new[] { "Admin", "Staff", "User" };
                foreach (var role in roles)
                {
                    var exists = roleManager.RoleExistsAsync(role).GetAwaiter().GetResult();
                    if (!exists)
                    {
                        roleManager.CreateAsync(new IdentityRole<int> { Name = role }).GetAwaiter().GetResult();
                    }
                }

                // Read admin credentials from configuration (recommended) or fallback to defaults
                var adminEmail = config["AdminUser:Email"] ?? "admin123@gmail.com";
                var adminPassword = config["AdminUser:Password"] ?? "Admin123!";

                var adminUser = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
                if (adminUser == null)
                {
                    adminUser = new Users
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        CreatedAt = DateTime.Now
                    };

                    var createResult = userManager.CreateAsync(adminUser, adminPassword).GetAwaiter().GetResult();

                    if (!createResult.Succeeded)
                    {
                        foreach (var error in createResult.Errors)
                        {
                            Console.WriteLine("ADMIN CREATE ERROR: " + error.Description);
                        }
                    }
                    else
                    {
                        var roleResult = userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();

                        if (!roleResult.Succeeded)
                        {
                            foreach (var error in roleResult.Errors)
                            {
                                Console.WriteLine("ADD ROLE ERROR: " + error.Description);
                            }
                        }
                    }
                }
                else
                {
                    // ensure existing user has Admin role
                    var inRole = userManager.IsInRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
                    if (!inRole)
                    {
                        userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
                    }
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

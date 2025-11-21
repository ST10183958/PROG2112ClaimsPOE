using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROG2112ClaimsPOE.Data;
using PROG2112ClaimsPOE.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PROG2112ClaimsPOE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add DbContext
            builder.Services.AddDbContext<ClaimDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ClaimDb")));

            builder.Services.AddScoped<IClaimVerificationService, ClaimVerificationService>();

            // FluentValidation registration (install FluentValidation.AspNetCore)
            builder.Services.AddControllersWithViews()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

            // Identity & authorization (sample skeleton; you must adapt to your identity setup)
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ClaimDbContext>()
            .AddDefaultTokenProviders();


            // policy for Manager role
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireManagerRole", policy =>
                    policy.RequireRole("Manager"));
            });

            builder.Services.AddDbContext<ClaimDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ClaimDb")));


            var app = builder.Build();

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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

using System;

using MedicalPoint.Data;
using MedicalPoint.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
    options =>
    {
        options.LoginPath = "/Home/Index";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });


builder.Services.AddHttpContextAccessor();


builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IVisitsService, VisitsService>();
builder.Services.AddScoped<IMedicinesService, MedicinesService>();
builder.Services.AddScoped<IPatientsService, PatientsService>();
builder.Services.AddScoped<IClinicsServices, ClinicsServices>();
builder.Services.AddScoped<IDegreesService, DegreesService>();
builder.Services.AddScoped<IVisitMedicinesService, VisitMedicinesService>();
builder.Services.AddScoped<IVisitImagesService, VisitImagesService>();
builder.Services.AddScoped<IVisitRestsService, VisitRestsService>();
builder.Services.AddScoped<IUnderObservationBedsService, UnderObservationBedsService>();
builder.Services.AddScoped<IMedicalPointUsersService, MedicalPointUsersService>();
builder.Services.AddScoped<IDepartmentsService, DepartmentsService>();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IReportsService, ReportsService>();

builder.Services.AddSingleton<CacheData>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
// Migrate latest database changes during startup
if(app.Configuration.GetValue<bool>("Properties:AutoMigrateOnStartup"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        // Here is the migration executed
        dbContext.Database.Migrate();
    }

}
app.UseStatusCodePagesWithReExecute("/Error/");
app.UseStaticFiles();
app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

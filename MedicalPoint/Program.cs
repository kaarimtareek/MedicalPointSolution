using MedicalPoint.Data;
using MedicalPoint.Services;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IVisitsService, VisitsService>();
builder.Services.AddScoped<IMedicinesService, MedicinesService>();
builder.Services.AddScoped<IPatientsService, PatientsService>();
builder.Services.AddScoped<IClinicsServices, ClinicsServices>();
builder.Services.AddScoped<IDegreesService, DegreesService>();
builder.Services.AddScoped<IVisitMedicinesService, VisitMedicinesService>();
builder.Services.AddScoped<IVisitImagesService, VisitImagesService>();
builder.Services.AddScoped<IVisitRestsService, VisitRestsService>();
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Patients}/{action=Index}/{id?}");

app.Run();

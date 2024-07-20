using Core;
using Core.Repository;
using Microsoft.EntityFrameworkCore;
using Repossitory.UnitOfWork;
using RingoMediaTask;
using Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IDepartmentServices, Services.Departments.DepartmentService>();
// Register the background service
builder.Services.AddHostedService<ReminderService>();
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
// Register AutoMapper with the dependency injection container
 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
 

app.Run();


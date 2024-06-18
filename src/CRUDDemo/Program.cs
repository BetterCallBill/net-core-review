using CRUDDemo.Entities;
using Microsoft.EntityFrameworkCore;
using CRUDDemo.Services;
using CRUDDemo.Interfaces;
using CRUDDemo.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
//add services into IoC container
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsService, PersonsService>();
builder.Services.AddDbContext<ApplicationDbContext>
(
  options =>
  {
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbContext"));
  }
);

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
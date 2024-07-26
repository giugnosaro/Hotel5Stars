using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Hotel5Stars.Services;
using Hotel5Stars.Models;

// Crea il builder dell'applicazione
var builder = WebApplication.CreateBuilder(args);

// Aggiungi i servizi al contenitore
builder.Services.AddControllersWithViews();

// Configura i servizi con la stringa di connessione
builder.Services.AddTransient<ICustomerService>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new CustomerService(connectionString);
});
builder.Services.AddTransient<IReservationService>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new ReservationService(connectionString);
});

// Costruisci l'applicazione
var app = builder.Build();

// Configura il middleware per l'applicazione
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Configura le route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

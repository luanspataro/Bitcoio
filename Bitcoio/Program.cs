using Bitcoio.Models;
using Bitcoio.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Bitcoio.Service;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos serviços (antigo ConfigureServices)
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<BitcoinService>();

var app = builder.Build();

// Configuração do pipeline de middleware (antigo Configure)
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Bitcoin}/{action=Index}/{id?}");

app.Run();

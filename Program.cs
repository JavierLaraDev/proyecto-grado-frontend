using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PersonalizacionProyectoGradoWASM;
using PersonalizacionProyectoGradoWASM.Helpers;
using PersonalizacionProyectoGradoWASM.Servicios;
using PersonalizacionProyectoGradoWASM.Servicios.IServicios;
using OfficeOpenXml;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient apuntando al backend en Render
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(Inicializar.UrlBaseApi)
    });

// Servicios
builder.Services.AddScoped<IPedidosServicio, PedidosServicio>();
builder.Services.AddScoped<IAccesoriosServicio, AccesoriosServicio>();
builder.Services.AddScoped<IUsuariosServicio, UsuariosServicio>();
builder.Services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Auth
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

await builder.Build().RunAsync();

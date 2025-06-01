using System;
using System.Net.Http;
using BlazorOfficina;
using BlazorOfficina.Data;
using BlazorOfficina.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using AutoMapper;                     // <-- aggiungi
using BlazorOfficina.Mapping;

var builder = WebApplication.CreateBuilder(args);

// 1) DbContext + ConnectionString (da appsettings.json)
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<VeicoloProfile>();
    // puoi aggiungere altri profili se ne crei altri
});

// 2) Servizio utente
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IVeicoloService, VeicoloService>();
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IRepairService, RepairService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<ISparePartService, SparePartService>();

// 2.1) Configuro i settings presi da appsettings.json (sezione "Smtp")
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));

// 2.2) Registro il sender concreto per l'invio email
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

// 3) Autenticazione con cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.Cookie.Name = "OfficinaAuth";
    });

// 4) Autorizzazione per ruoli
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
    options.AddPolicy("MechanicOnly", policy => policy.RequireRole("Mechanic"));
});

// 5) Aggiungi i controller API (AuthController)
builder.Services.AddControllers();

// 6) Razor Pages + Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// 7) HttpClient per NavMenu (o altri componenti) — base address dinamico
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
});

var app = builder.Build();

// 8) Applica le migrazioni al lancio
using (var scope = app.Services.CreateScope())
{
    var dbCtx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbCtx.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

// 9) Middleware di autenticazione/autorizzazione
app.UseAuthentication();
app.UseAuthorization();

// 10) Map delle rotte
app.MapControllers();               // per gli endpoint [ApiController]
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");   // catch-all per Blazor Server

app.Run();

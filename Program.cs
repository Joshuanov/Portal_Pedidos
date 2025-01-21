using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaTGU.Data;
using Serilog;
using SistemaTGU.Servicio;

var builder = WebApplication.CreateBuilder(args);


// Configurar límites de Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestLineSize = 16384; // Tamaño máximo de la línea de solicitud
    options.Limits.MaxRequestHeadersTotalSize = 65536; // Tamaño máximo de los encabezados
});

// Add services to the container.
builder.Services.AddControllersWithViews();

//Servicio de conexión a ERP
builder.Services.AddScoped<IConnectionErp, ConnectionErp>();


// Configurar el DbContext principal
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache(); // Necesario para manejar las sesiones en memoria
                                              // Habilita las sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Duración de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Necesario para GDPR, si es aplicable
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Identity Framework
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = false; // No requiere correo único
    options.SignIn.RequireConfirmedEmail = false; // No requiere confirmación de correo
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders();

//Redirect URL
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Home/Index");
    options.AccessDeniedPath = new PathString("/Error/AccesoNegado");
});

//Configración Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //options.Lockout.MaxFailedAccessAttempts = 3;
});


builder.Services.AddAuthorization();

// Configurar Serilog con el host del programa
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning() // Establece el nivel mínimo de registro en Warning
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Usar Serilog como proveedor de logs
builder.Host.UseSerilog();

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
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

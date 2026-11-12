
using InstructorInformationSystem.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. MVC SERVICES
// ==========================================

builder.Services.AddControllersWithViews();

// ==========================================
// 2. SQLITE DATABASE
// ==========================================

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
    {
        options.UseSqlite(connectionString);
    });

// ==========================================
// 3. COOKIE AUTHENTICATION
// ==========================================

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";

        options.AccessDeniedPath = "/Account/Login";

        options.Cookie.HttpOnly = true;

        options.Cookie.SameSite = SameSiteMode.Lax;

        options.Cookie.SecurePolicy =
            CookieSecurePolicy.SameAsRequest;

        options.ExpireTimeSpan = TimeSpan.FromHours(2);

        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// ==========================================
// 4. BUILD APPLICATION
// ==========================================

var app = builder.Build();

// ==========================================
// 5. ERROR HANDLING
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ==========================================
// 6. MIDDLEWARE
// ==========================================

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// AUTHENTICATION MUST COME BEFORE AUTHORIZATION
app.UseAuthentication();

app.UseAuthorization();

// ==========================================
// 7. ROUTING
// ==========================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ==========================================
// 8. RUN APPLICATION
// ==========================================

app.Run();

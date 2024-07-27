using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Motax.Models;
using Motax.Filters;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<UserAvatarFilter>();
});

// Config Sql Data
var connectionString = builder.Configuration.GetConnectionString("Motax");
builder.Services.AddDbContext<MotaxContext>(x => x.UseSqlServer(connectionString));

// Đăng ký UserAvatarFilter
builder.Services.AddScoped<UserAvatarFilter>();

//Cấu hình dịch vụ xác thực Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    options =>
    {
        options.LoginPath = "/Secure/Login";
        options.AccessDeniedPath = "/Secure/Denied";
    })
    .AddFacebook(options =>
    {
        options.AppId = "1685959258607865";
        options.AppSecret = "18cee327c0c9da060db8123289d7781d";
        options.Fields.Add("picture.type(large)"); // Add this line to get the picture
    })
    .AddGoogle(options =>
    {
        options.ClientId = "908460529684-mkhdgbf7uclr15gel8tm2sbuogi8pvo0.apps.googleusercontent.com";
        options.ClientSecret = "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";
        options.Scope.Add("profile"); // Add this line to get the profile information including the picture
    });

//Cấu hình phân quyền
builder.Services.AddAuthorization(
        options =>
        {
            options.AddPolicy("CheckAdmin", policy => policy.RequireClaim(ClaimTypes.Role, "admin"));
        }
    );

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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

using IdentityService.Authorization;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient("OurWebApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5170");
});

builder.Services.AddAuthentication(Constants.COOKIE_NAME).AddCookie(Constants.COOKIE_NAME, options =>
{
    options.Cookie.Name = Constants.COOKIE_NAME;
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "AdminOnly",
        policy => policy.RequireClaim("Admin")
        );
    options.AddPolicy(
        "MustBelongToHr", 
        policy => policy.RequireClaim("Department", "HR") 
        );
    
    options.AddPolicy(
        "HRManagerOnly", 
        policy => policy.RequireClaim("Department", "HR") 
        .RequireClaim("Manager")
        .Requirements.Add(new HRManagerProbationRequirement(3))
        );
});

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationRequirementHandler>();

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

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();

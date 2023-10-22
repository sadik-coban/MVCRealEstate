using Microsoft.EntityFrameworkCore;
using MVCRealEstate;
using MVCRealEstateData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(config =>
{
    config.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddIdentity<User, Role>(config =>
{
    config.SignIn.RequireConfirmedEmail = true;

    config.Password.RequireDigit = builder.Configuration.GetValue<bool>("Security:Password:RequireDigit");
    config.Password.RequiredLength = builder.Configuration.GetValue<int>("Security:Password:RequiredLength");
    config.Password.RequiredUniqueChars = builder.Configuration.GetValue<int>("Security:Password:RequiredUniqueChars");
    config.Password.RequireLowercase = builder.Configuration.GetValue<bool>("Security:Password:RequireLowercase");
    config.Password.RequireUppercase = builder.Configuration.GetValue<bool>("Security:Password:RequireUppercase");
    config.Password.RequireNonAlphanumeric = builder.Configuration.GetValue<bool>("Security:Password:RequireNonAlphanumeric");

    config.Lockout.MaxFailedAccessAttempts = builder.Configuration.GetValue<int>("Security:Lockout:MaxFailedAccessAttempts");
    config.Lockout.DefaultLockoutTimeSpan = builder.Configuration.GetValue<TimeSpan>("Security:Lockout:DefaultLockoutTimeSpan");
})
    .AddEntityFrameworkStores<AppDbContext>();


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

app.UseAuthentication();
app.UseAuthorization();

app.UseMVCRealEstate();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

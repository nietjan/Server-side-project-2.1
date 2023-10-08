using ApplicationServices;
using DomainServices;
using Infrastructure;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

////add session
//builder.Services.AddDistributedMemoryCache();

//builder.Services.AddSession(options => {
//    options.IdleTimeout = TimeSpan.FromHours(1);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

builder.Services.AddSingleton<IRepository, InMemoryRepository>();

builder.Services.AddTransient<SeedData>();

builder.Services.AddScoped<IUserSession, UserSessionIdentity>();

var connectionStringSql = builder.Configuration.GetConnectionString("Default");
var connectionStringSecurity = builder.Configuration.GetConnectionString("Security");

//db context
builder.Services.AddDbContext<PacketContext>(options => options.UseSqlServer(
    connectionStringSql
));

//Identity.
builder.Services.AddDbContext<SecurityContext>(options => options.UseSqlServer(
    connectionStringSecurity
));

//password requirements
builder.Services.AddIdentity<IdentityUser, IdentityRole>(config => {
    config.Password.RequireDigit = false;
    config.Password.RequiredLength = 6;
    config.Password.RequireLowercase = true;
    config.Password.RequireUppercase = true;
    config.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<SecurityContext>()
    .AddDefaultTokenProviders();

//identity paths
builder.Services.ConfigureApplicationCookie(config => {
    //config.Cookie.Expiration = TimeSpan.FromHours(1);
    config.Cookie.Name = "Identity";
    config.LoginPath = "/Home/Login";
    config.LogoutPath = "/Home/LogOut";
    config.AccessDeniedPath = "/home/index";
});

//identity policy an claims
builder.Services.AddAuthorization(config => {
    config.AddPolicy("Staff", policy => {
        policy.RequireClaim("userType", "canteenStaff");
    });

    config.AddPolicy("Student", policy => {
        policy.RequireClaim("userType", "student");
    });
});

var app = builder.Build();

//Seed date for PacketContext and SecurityContext
using (var scope = app.Services.CreateScope()) {
    var service = scope.ServiceProvider;
    var dataSeeder = service.GetService<SeedData>();
    dataSeeder?.SeedDatabase();
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "packet",
    pattern: "{controller=Packet}/{action=List}/{id?}",
    defaults: new { controller = "Packet", action = "List" });


app.Run();



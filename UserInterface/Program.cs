using DomainServices;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IRepository, InMemoryRepository>();

//db context
builder.Services.AddDbContext<PacketContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default")
));

//Identity
builder.Services.AddDbContext<SecurityContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Security")
));

//paswoord requirements
builder.Services.AddIdentity<IdentityUser, IdentityRole>(config => {
    config.Password.RequireDigit = false;
    config.Password.RequiredLength = 4;
    config.Password.RequireLowercase = false;
    config.Password.RequireUppercase = false;
    config.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<SecurityContext>()
    .AddDefaultTokenProviders();

//identity paths
builder.Services.ConfigureApplicationCookie(config => {
    config.Cookie.Name = "Identity";
    config.LoginPath = "/Home/Login";
    config.LogoutPath = "/Home/LogOut";
    config.AccessDeniedPath = "/home/index";
});

//identity policy an claims
builder.Services.AddAuthorization(config => {
    config.AddPolicy("Claim.test", policy => {
        policy.RequireClaim("UserName", "test", "test2");
    });
});

var app = builder.Build();

//Seed date for DBContext and securtityContext
using (var scope = app.Services.CreateScope()) {
    try {
        var dbContext = scope.ServiceProvider.GetRequiredService<PacketContext>();
        var identityContext = scope.ServiceProvider.GetRequiredService<SecurityContext>();

        if (!dbContext.canteen.Any()) {
            new SeedData(dbContext, identityContext).SeedDatabase();
        }

    } catch {
        throw;
    }
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "packet",
    pattern: "{controller=Packet}/{action=List}/{id?}",
    defaults: new { controller = "Packet", action = "List" });


app.Run();

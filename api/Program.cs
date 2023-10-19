using DomainServices;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using HotChocolate;
using api.Data;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//db context
builder.Services.AddDbContext<PacketContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default"),
    sqlServerOptionsAction: sqlOptions => {
        sqlOptions.EnableRetryOnFailure();
    }
), ServiceLifetime.Transient);
builder.Services.AddScoped<IRepository, InMemoryRepository>();

//identity
builder.Services.AddDbContext<SecurityContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Security"),
    sqlServerOptionsAction: sqlOptions => {
        sqlOptions.EnableRetryOnFailure();
    }
));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(config => {
    config.Password.RequireDigit = false;
    config.Password.RequiredLength = 6;
    config.Password.RequireLowercase = true;
    config.Password.RequireUppercase = true;
    config.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<SecurityContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        RequireExpirationTime = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = false,
        ValidateLifetime = false,
        IssuerSigningKey =
        new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value!)),
    };
});



builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGraphQL();

app.Run();

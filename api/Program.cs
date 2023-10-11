using DomainServices;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using HotChocolate;
using api.Data;
using NuGet.Protocol;
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
));

builder.Services.AddScoped<IRepository, SqlRepository>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddProjections()
    .AddFiltering()
    .AddSorting(); ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGraphQL();

app.UseAuthorization();

app.Run();

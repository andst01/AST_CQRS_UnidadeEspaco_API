using MongoDB.Bson.Serialization;
using System.Diagnostics.CodeAnalysis;


using UnidadeEspacoSrv.CrossCuting.Configuration;
using UnidadeEspacoSrv.Domain.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDIConfiguration();
builder.Services.AddAutoMapperConfiguration();
builder.Services.AddSwaggerConfiguration(builder.Configuration);
builder.Services.AddDatabaseConfiguration(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

BsonClassMap.RegisterClassMap<UnidadeNotification>(cm => {
    cm.AutoMap();
    cm.SetIsRootClass(true); // Define como a base da hierarquia
});

// Registra as classes filhas para o MongoDB saber quem são
BsonClassMap.RegisterClassMap<UnidadeCreateNotification>();
BsonClassMap.RegisterClassMap<UnidadeUpdateNotification>();


BsonClassMap.RegisterClassMap<EspacoNotification>(cm => {
    cm.AutoMap();
    cm.SetIsRootClass(true); // Define como a base da hierarquia
});

// Registra as classes filhas para o MongoDB saber quem são
BsonClassMap.RegisterClassMap<EspacoCreateNotification>();
BsonClassMap.RegisterClassMap<EspacoUpdateNotification>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

[ExcludeFromCodeCoverage]
public partial class Program { }

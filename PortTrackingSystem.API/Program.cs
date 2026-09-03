using Microsoft.EntityFrameworkCore;
using PortTrackingSystem.Core.Data;
using PortTrackingSystem.Core.Repositories;
using PortTrackingSystem.Core.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// builder.Services tanımlamaları başlangıcı
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// 1. Controller mimarisini projeye dahil ediyoruz
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IShipService, ShipService>();
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<IShipVisitService, ShipVisitService>();
builder.Services.AddScoped<IPortService, PortService>();
builder.Services.AddScoped<ICrewMemberService, CrewMemberService>();
builder.Services.AddScoped<IShipCrewAssignmentService, ShipCrewAssignmentService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}
);
builder.Services.AddControllers().AddJsonOptions(x =>x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. HTTP Request Pipeline ayarları
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAll");
app.UseHttpsRedirection();

// 3. Controller rotalarını eşleştiriyoruz
app.MapControllers();

app.Run();
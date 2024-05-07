using Microsoft.EntityFrameworkCore;
using CCA_DAL.DBContext;
using CCA_DAL.Interface;
using CCA_DAL;
using CCA_BAL.Interface;
using CCA_BAL.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<CCAContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbcs"));
});

builder.Services.AddScoped<IRideProvide, RideProvideRepository>();
builder.Services.AddScoped<ISmile, SmileRepository>();
builder.Services.AddScoped<IBill, BillRepository>();
builder.Services.AddScoped<IRideProvideService, RideProvideServices>();
builder.Services.AddScoped<IBillService, BillService>();
builder.Services.AddScoped<ISmileService, SmileService>();
builder.Services.AddScoped<ITripManage, TripManageRepository>();
builder.Services.AddScoped<ITripService,TripService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(option =>
{
    option.AddPolicy(name: "CORS", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("CORS");
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

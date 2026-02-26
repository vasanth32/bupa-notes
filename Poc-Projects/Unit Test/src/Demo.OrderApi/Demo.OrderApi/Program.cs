var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<Demo.OrderApi.Repositories.IOrderRepository, Demo.OrderApi.Repositories.FakeOrderRepository>();
builder.Services.AddScoped<Demo.OrderApi.Services.OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<Demo.OrderApi.Middleware.ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }

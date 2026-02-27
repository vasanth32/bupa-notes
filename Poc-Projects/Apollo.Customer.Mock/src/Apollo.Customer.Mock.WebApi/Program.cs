using Apollo.Customer.Mock.Application.Customers;
using Apollo.Customer.Mock.Infrastructure.Data;
using Apollo.Customer.Mock.Infrastructure.Repositories;
using Apollo.Customer.Mock.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApolloCustomerMockDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SqlServer");
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();

public partial class Program
{
}

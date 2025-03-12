using TestMcSonae.Services;
using TestMcSonae.Configurations;
using TestMcSonae.Repositories;
using TestMcSonae.Mapping;
using TestMcSonae.Middleware;
using TestMcSonae.Validation.Validators;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ReservationSettings>(options => 
{
    options.ReservationTimeoutSeconds = 15;
});

builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddSingleton<IStockMovementRepository, StockMovementRepository>();

builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<IStockService, StockService>();
builder.Services.AddSingleton<IOrderService, OrderService>();

builder.Services.AddSingleton<OrderMapper>();
builder.Services.AddSingleton<CreateOrderValidator>();

var app = builder.Build();

var stockService = app.Services.GetRequiredService<IStockService>();
stockService.InitializeExpirationTimer();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container


// 2. Register Controllers and JSON Serialization options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Preserve Pascal case for JSON properties
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// 3. Configure Swagger for API documentation (only in Development)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Configure CORS to allow requests from Angular app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Angular app URL
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline

// Enable Swagger in Development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS policy for Angular app
app.UseCors("AllowAngularApp");

app.UseHttpsRedirection();

app.UseAuthorization();

// Map the controllers (API endpoints)
app.MapControllers();

app.Run();

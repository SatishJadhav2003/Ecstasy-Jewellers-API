using Banner.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register EJ_BannerDataAccess as a Scoped Service
builder.Services.AddScoped<EJ_BannerDataAccess>();

// Register Controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Map the controllers
app.MapControllers();

app.Run();

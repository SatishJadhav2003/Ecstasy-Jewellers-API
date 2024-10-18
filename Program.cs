
using System.Text;
using ECSTASYJEWELS;
using ECSTASYJEWELS.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


// 1. Registaring repos
builder.Services.AddSingleton<ProductRepository>(sp =>
    new ProductRepository(builder.Configuration.GetConnectionString("DefaultConnection") ?? "")
);
builder.Services.AddSingleton<CategoryRepository>(sp =>
    new CategoryRepository(builder.Configuration.GetConnectionString("DefaultConnection") ?? "")
);
builder.Services.AddSingleton<BannerRepository>(sp =>
    new BannerRepository(builder.Configuration.GetConnectionString("DefaultConnection") ?? "")
);
builder.Services.AddSingleton<UserRepository>(sp =>
    new UserRepository(builder.Configuration.GetConnectionString("DefaultConnection") ?? "")
);

builder.Services.AddScoped<TokenService>();

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


// 5. JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]??""))
    };
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
app.UseAuthentication();


// Map the controllers (API endpoints)
app.MapControllers();

app.Run();

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pikmi.API.Data;
using Pikmi.API.Entities;
using Pikmi.API.Helpers;
using Pikmi.API.Mapping;
using Pikmi.API.Repositories.Implementations;
using Pikmi.API.Repositories.Interfaces;
using Pikmi.API.Services.Implementations;
using Pikmi.API.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});



// ============================================
// REGISTER REPOSITORIES
// ============================================
builder.Services.AddScoped<IRideRepository, RideRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();

// ============================================
// REGISTER SERVICES
// ============================================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, MailjetApiEmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRideService, RideService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IWalletService, WalletService>();


builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var issuer = builder.Configuration["JwtSettings:Issuer"];
var audience = builder.Configuration["JwtSettings:Audience"];
var secretKey = builder.Configuration["JwtSettings:SecretKey"];
var key = Encoding.UTF8.GetBytes(secretKey);

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
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseCors("AllowAll");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager); 
}

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

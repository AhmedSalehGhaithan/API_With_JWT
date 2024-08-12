using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Wep_API_JWT.Data;
using Swashbuckle.AspNetCore.Filters;
using Shared_ClassLibrary.Contracts;
using Wep_API_JWT.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(_option_ =>
{
    _option_.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string is not found"));
});

// add Identity & JWT authentication

builder.Services.AddIdentity<ApplicationUser,IdentityRole>()
       .AddEntityFrameworkStores<AppDbContext>()
       .AddSignInManager()
       .AddRoles<IdentityRole>();

// JWT 
builder.Services.AddAuthentication(_option_ =>
{
    _option_.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    _option_.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(_option_ =>
{
    _option_.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt :Issuer"],
        ValidAudience = builder.Configuration["Jwt :Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// add authentication to Swagger Ui

builder.Services.AddSwaggerGen(_option_ =>
{
    _option_.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    _option_.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddScoped<IUserAccount,AccountRepo>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

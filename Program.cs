using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore;
using Microsoft.OpenApi.Models;
using TicketApi.Data;
using TicketApi.DTO;
using TicketApi.Interface;
using TicketApi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TicketApi.Services;
using System.Reflection;
using TicketApi.Profiles;
using System.Security.Claims;
using TicketApi.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5126);
});

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new UserRoleConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ticket API",
        Version = "v1",
        Description = "API for managing support tickets"
    });
    
    // Add security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
});
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(
    typeof(AuthMappings),
    typeof(TicketMappings),
    typeof(NotificationMappings)
);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Add authentication (after AddControllers)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin"));
    options.AddPolicy("Supporter", policy =>
        policy.RequireRole("Supporter"));
    options.AddPolicy("User", policy =>
        policy.RequireRole("User"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticket API v1");
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Use(async (context, next) =>
{
    var user = context.User;
    Console.WriteLine($"User authenticated: {user.Identity?.IsAuthenticated}");
    Console.WriteLine($"Roles: {string.Join(",", user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))}");
    await next();
});

app.Run();



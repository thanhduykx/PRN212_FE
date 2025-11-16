using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository.Context;
using Repository.IRepositories;
using Repository.Repositories;
using Service.EmailConfirmation;
using Service.IServices;
using Service.Mapper;
using Service.Services;
using System.Text;
using Swashbuckle.AspNetCore.SwaggerGen;



var builder = WebApplication.CreateBuilder(args);
var smtpSettings = builder.Configuration.GetSection("SmtpSettings");
var connectionString = builder.Configuration.GetConnectionString("Database");
builder.Services.AddDbContext<EVSDbContext>(options =>
    options.UseSqlServer(connectionString));

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Add services to the container.
builder.Services.Configure<SmtpSettings>(smtpSettings);
builder.Services.AddSingleton<EmailService>();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    ////JWT Config
    option.DescribeAllParametersInCamelCase();
    option.ResolveConflictingActions(conf => conf.First());
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});




// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IRentalContactRepository, RentalContactRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IRentalLocationRepository, RentalLocationRepository>();
builder.Services.AddScoped<ICarRentalLocationRepository, CarRentalLocationRepository>();
builder.Services.AddScoped<ICitizenIdRepository, CitizenIdRepository>();
builder.Services.AddScoped<IDriverLicenseRepository, DriverLicenseRepository>();
builder.Services.AddScoped<IRentalOrderRepository, RentalOrderRepository>();
builder.Services.AddScoped<ICarDeliveryHistoryRepository, CarDeliveryHistoryRepository>();
builder.Services.AddScoped<ICarReturnHistoryRepository, CarReturnHistoryRepository>();


// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICarService, CarService>();
builder.Services.AddScoped<IRentalContactService, RentalContactService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IRentalLocationService, RentalLocationService>();
builder.Services.AddScoped<ICarRentalLocationService, CarRentalLocationService>();
builder.Services.AddScoped<ICitizenIdService, CitizenIdService>();
builder.Services.AddScoped<IDriverLicenseService, DriverLicenseService>();
builder.Services.AddScoped<IRentalOrderService, RentalOrderService>();
builder.Services.AddScoped<ICarDeliveryHistoryService, CarDeliveryHistoryService>();
builder.Services.AddScoped<ICarReturnHistoryService, CarReturnHistoryService>();
builder.Services.AddHttpClient<IAIService, AIService>();


//Others
builder.Services.Configure<SmtpSettings>(smtpSettings);
builder.Services.AddTransient<EmailService>();
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();


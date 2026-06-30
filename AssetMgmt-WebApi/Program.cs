using AssetMgmt_WebApi.Helpers;
using AssetMgmt_WebApi.Middleware;
using AssetMgmt_WebApi.Models;
using AssetMgmt_WebApi.Repositories;
using AssetMgmt_WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace AssetMgmt_WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. JSON formatting middleware
            builder.Services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                opt.JsonSerializerOptions.WriteIndented = true;
            });

            // 2. Database context
            builder.Services.AddDbContext<AssetMgmtContext>(
                options => options.UseSqlServer(
                    builder.Configuration.GetConnectionString("AssetMgmtDbConnection")));

            // 3. Repository / Service registrations (Module A - User Registration & Login)
            builder.Services.AddScoped<IUserRepository, UserRepositoryImpl>();
            builder.Services.AddScoped<IUserService, UserServiceImpl>();

            // Module E - Asset Creation
            builder.Services.AddScoped<IAssetMasterRepository, AssetMasterRepositoryImpl>();
            builder.Services.AddScoped<IAssetMasterService, AssetMasterServiceImpl>();


            builder.Services.AddScoped<IReferenceDataRepository, ReferenceDataRepositoryImpl>();
            builder.Services.AddScoped<IReferenceDataService, ReferenceDataServiceImpl>();

            builder.Services.AddSingleton<JwtTokenHelper>();

            // 4. Swagger / OpenAPI documentation (with JWT bearer support)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "XYZ Technologies - Asset Management System API",
                    Version = "v1",
                    Description = "Web API for Module A (User Registration & Login) and Module E (Asset Creation)."
                });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token here (without the word 'Bearer')."
                };
                opt.AddSecurityDefinition("Bearer", securityScheme);
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // 5. JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            // 6. Role based authorization
            builder.Services.AddAuthorization(opt =>
            {
                opt.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                opt.AddPolicy("PurchaseManagerOnly", policy => policy.RequireRole("Purchase Manager"));
            });

            // 7. CORS - allow the Angular dev server to call the API
            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("AngularClient", policy =>
                {
                    policy.WithOrigins(builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Global exception handling - first in the pipeline
            app.UseGlobalExceptionHandling();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(opt =>
                {
                    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Asset Management System API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseCors("AngularClient");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
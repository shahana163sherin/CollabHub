
using Asp.Versioning;
using CollabHub.Application.DependecyInjection;
using CollabHub.Infrastructure.Configuration;
using CollabHub.Infrastructure.DependencyInjection;
using CollabHub.Infrastructure.Persistence.Data;
using CollabHub.WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace CollabHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection("JWTSettings").Get<JWTSettings>();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    RoleClaimType = ClaimTypes.Role
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var identity = context.Principal.Identity as ClaimsIdentity;
                        if (identity == null)
                        {
                            context.Fail("Invalid token identity");
                            return;
                        }
                        var userIdCliam = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (string.IsNullOrEmpty(userIdCliam))
                        {
                            context.Fail("User id is missing");
                            return;
                        }

                        var tokenpwd = identity.FindFirst("pwd_changed")?.Value;
                        if (string.IsNullOrEmpty(tokenpwd))
                        {
                            context.Fail("Token missing password change info");
                            return;
                        }

                        long tokenTicks = long.Parse(tokenpwd);
                        int userId = int.Parse(userIdCliam);

                        var db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
                        var user = await db.Users.FindAsync(userId);

                        if(user == null)
                        {
                            context.Fail("User not found");
                            return;
                        }
                        if(user.IsDeleted || !user.IsActive) {
                            context.Fail("User revocked");
                            return;
                        }

                        var userTicks = (user.LastPasswordChangedAt ?? DateTime.MinValue).Ticks;
                        if (userTicks > tokenTicks)
                        {
                            context.Fail("Password chnaged");
                            return;
                        }
                        
                    }
                };


            });
            builder.Services.AddAuthorization();

            const string CorsPolicy= "AllowReact";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy, policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();

                });
            });


            builder.Services.AddInfrastructure();
            builder.Services.AddApplicationService();


           

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CollabHub API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat="JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseHttpsRedirection();
            app.UseCors(CorsPolicy);
          app.UseAuthentication();

            app.UseAuthorization();

          
            app.MapControllers();

            app.Run();
        }
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Services;

/**
 * Vitor Baraçal Guimarães - 40457125
 * João Vitor Pereira Borges -38146762
 * Cauã Tobias de Souza Proença - 40174255
 */

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("DefaultConn");

builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(conn, ServerVersion.AutoDetect(conn)));

builder.Services.AddCors(options =>{
    options.AddPolicy("FullAccess", policy =>
        policy.WithOrigins("https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>{
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);

        options.TokenValidationParameters = new TokenValidationParameters{
            ValidateIssuer = false,   
            ValidateAudience = false, 
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero 
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "Jwt"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {{
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddSingleton<TokenService>(); 

var app = builder.Build();

if (app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("FullAccess");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
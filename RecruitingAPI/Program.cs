using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RecruitingAPI.Context;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
/*builder.Services.AddSwaggerGen();*/

builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RecruitingAPI.App",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\nExample: Bearer 1safs"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// Add the SendGrid API key
/*var sendGridApiKey = builder.Configuration["SendGrid:ApiKey"];
builder.Services.AddSingleton(sendGridApiKey);*/

/*builder.Services.AddSendGrid(options =>
{
    options.ApiKey = builder.Configuration
    .GetSection("SendGridEmailSettings").GetValue<string>("APIKey");
});*/

builder.Services.AddSingleton<ISendGridClient>(new SendGridClient("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"));

using var hmac = new HMACSHA256();
var key = Convert.ToBase64String(hmac.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;//new added
}).AddJwtBearer(options =>
{
    options.SaveToken = true;//new
    options.RequireHttpsMetadata = false;//new
    options.TokenValidationParameters = new TokenValidationParameters()//() added
    {
        ValidateIssuer = true,//change it to false
        ValidateAudience = true,//change it to false
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero//added
    };

});//sixth

//Add Cors
var myOrigins = "_myOrigins";
builder.Services.AddCors(option =>
{
    option.AddPolicy(name: myOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    });
});//second


builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnStr"));
}); //first

builder.Services.Configure<FormOptions>(o =>
{
    o.ValueLengthLimit = int.MaxValue;
    o.MultipartBodyLengthLimit = int.MaxValue;
    o.MemoryBufferThreshold = int.MaxValue;
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(myOrigins); //third

app.UseAuthentication(); //fourth

app.UseAuthorization();

app.UseStaticFiles(); //fifth

app.MapControllers();

app.Run();

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Load config
var config = builder.Configuration;

// MongoDB
var mongoClient = new MongoClient(config["Mongo:ConnectionString"]);
var mongoDb = mongoClient.GetDatabase(config["Mongo:Database"]);
builder.Services.AddSingleton<IMongoDatabase>(mongoDb);

// Redis
var redis = ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

// Add controllers, SignalR, etc.
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IFileService, FileService>();

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000",   "https://filecollabapi.onrender.com")

              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // if using cookies/auth
    });
});

var app = builder.Build();
app.UseCors("AllowReactApp");
app.MapControllers();
app.MapHub<CollaborationHub>("/hub/collaboration");

app.Run();

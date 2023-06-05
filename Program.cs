using apidemoproj.Data;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//uses the connection string in apidemocontext.cs 
//builder.Services.AddDbContext<apidemoContext>();
//var app = builder.Build();

//Use Default Connection String in Project User secrets or App settings.json 
//var connString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<apidemoContext>(o => o.UseSqlServer(connString));
//var app = builder.Build();

//Use KeyVault connection for the stored Secret. Key vault url is defined in appsettings.json
var keyVaultEndpoint = new Uri(builder.Configuration["VaultKey"]);
var secretClient = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());
KeyVaultSecret kvs = secretClient.GetSecret("apidemoappsecret");
builder.Services.AddDbContext<apidemoContext>(o => o.UseSqlServer(kvs.Value));
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

//Map Get to retrieve the data from then models
app.MapGet("api/", async ([FromServices] apidemoContext db) =>
{ return await db.Product.ToListAsync(); });

app.Run();
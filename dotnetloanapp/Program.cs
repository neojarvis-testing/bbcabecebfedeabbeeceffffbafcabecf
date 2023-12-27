using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BookStoreDBFirst.Models;
using BookStoreDBFirst.Repository;
using BookStoreDBFirst.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configure the application configuration
builder.Configuration.AddJsonFile("appsettings.json");

// Check if Key Vault URL is provided in appsettings.json
var keyVaultUrl = builder.Configuration["KeyVaultConfiguration:URL"];
if (keyVaultUrl != null)
{
    // Add Key Vault to configuration sources
    builder.Configuration.AddAzureKeyVault(keyVaultUrl);
}

// Retrieve SQL connection string from configuration
var sqlConnectionString = builder.Configuration.GetConnectionString("ConnectionStrings:SqlConnectionString");
Console.WriteLine(sqlConnectionString);

// Retrieve Blob connection string from configuration
var blobConnectionString = builder.Configuration.GetConnectionString("ConnectionStrings:BlobConnectionString");

builder.Services.AddDbContext<LoanApplicationDbContext>(options =>
    options.UseSqlServer(sqlConnectionString),
    ServiceLifetime.Transient
);

builder.Services.AddScoped<IAzureStorage>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<AzureStorage>>();
    var containerName = builder.Configuration.GetValue<string>("BlobContainerName");

    return new AzureStorage(blobConnectionString, containerName, logger);
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<LoanApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Create roles if they don't exist
    if (!await roleManager.RoleExistsAsync("admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("admin"));
    }

    if (!await roleManager.RoleExistsAsync("user"))
    {
        await roleManager.CreateAsync(new IdentityRole("user"));
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LoginAuth v1")
    );
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
Console.WriteLine("bye");

app.Run();
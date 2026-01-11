using ECommerce.API.Extensions;
using ECommerce.Infrastructure.DependencyInjection;
using ECommerce.Application.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//Add Infrastructure (Db, Identity, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Application Services
builder.Services.AddApplication();


// Add API Services (Controllers, Swagger, JWT, CORS)
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// Configure Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ECommerce.Infrastructure.Persistence.AppDbContext>();
        var userManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ECommerce.Infrastructure.Identity.ApplicationUser>>();
        var roleManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
        
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Starting Database Seeding...");
        
        await ECommerce.Infrastructure.Persistence.DbInitializer.SeedAsync(userManager, roleManager, context);
        
        logger.LogInformation("Database Seeding Completed Successfully.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();

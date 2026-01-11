using ECommerce.API.Extensions;
using ECommerce.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//Add Infrastructure (Db, Identity, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);


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

app.Run();

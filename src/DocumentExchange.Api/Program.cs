using DocumentExchange.Api.Data;
using DocumentExchange.Api.Hosted;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

builder.Services.AddInMemoryPatientDatabase();

if (builder.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("SeedDatabase"))
{
    builder.Services.AddDevPatientSeeder();
}

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

using DocumentExchange.Api.Data;
using DocumentExchange.Api.Endpoints;
using DocumentExchange.Api.Hosted;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddValidation();

// Return 400 instead of throwing when receiving malformed JSON. Used with testing.
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);

builder.Services.AddInMemoryPatientDatabase();
builder.Services.AddInMemoryReferralDatabase();

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

app.MapReferralEndpoints();
app.MapPatientEndpoints();

app.Run();

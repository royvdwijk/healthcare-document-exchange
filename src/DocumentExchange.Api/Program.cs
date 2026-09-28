using DocumentExchange.Api.Data;
using DocumentExchange.Api.Endpoints;
using DocumentExchange.Api.Hosted;
using DocumentExchange.Api.Identification;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, logger) =>
{
    logger.ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();

    var logFilePath = builder.Configuration["LogFilePath"];
    if (!string.IsNullOrWhiteSpace(logFilePath))
    {
        logger.WriteTo.File(
            new CompactJsonFormatter(),
            Path.Combine(builder.Environment.ContentRootPath, logFilePath),
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7);
    }
});

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

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} from {Identity} ({ClientIp}) responded {StatusCode} in {Elapsed:0} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("Identity", IdentityHeader.Find(httpContext.Request) ?? "unknown");
        diagnosticContext.Set("ClientIp", httpContext.Connection.RemoteIpAddress?.ToString());
    };
});

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

using System;
using System.Net.Mime;
using FluentValidation;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.FileProviders;
using Minio;
using Minio.AspNetCore.HealthChecks;
using Rentals.Api.Contracts;
using Rentals.Api.Swagger;
using Rentals.Application.Abstractions;
using Rentals.Application.Abstractions.Messaging;
using Rentals.Application.Behaviors;
using Rentals.Application.Motorcycles;
using Rentals.Application.Motorcycles.CreateMotorcycle;
using Rentals.Application.Services;
using Rentals.Domain.Services;
using Rentals.Infrastructure.Messaging.Consumers;
using Rentals.Infrastructure.Messaging.Definition;
using Rentals.Infrastructure.Persistence;
using Rentals.Infrastructure.Persistence.Repositories;
using Rentals.Infrastructure.Storage;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);


string? BuildPgFromEnv()
{
    var host = Environment.GetEnvironmentVariable("PGHOST");
    if (string.IsNullOrWhiteSpace(host)) return null;
    var port = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";
    var db = Environment.GetEnvironmentVariable("PGDATABASE") ?? "rentalsdb";
    var user = Environment.GetEnvironmentVariable("PGUSER") ?? "app";
    var pwd = Environment.GetEnvironmentVariable("PGPASSWORD") ?? "app";
    return $"Host={host};Port={port};Database={db};Username={user};Password={pwd}";
}


builder.Host.UseSerilog();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services
    .AddControllers()
    .AddJsonOptions(o => {  });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateMotorcycleCommandValidator>();

builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection("Minio"));
builder.Services.Configure<LocalStorageOptions>(builder.Configuration.GetSection("Storage"));



// MediatR (varre o assembly da Application)
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssemblyContaining<CreateMotorcycleCommand>();
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// DbContext (Postgres)
var cs =
    builder.Configuration.GetConnectionString("Default")              
    ?? BuildPgFromEnv()                                            
    ?? "Host=localhost;Port=5432;Database=rentals;Username=postgres;Password=452313"; 

         ;
builder.Services.AddDbContext<RentalsDbContext>(opt =>
    opt.UseNpgsql(cs)
       .ConfigureWarnings(warnings =>
           warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MotorcycleRegisteredConsumer, MotorcycleRegisteredConsumerDefinition>();

    x.AddEntityFrameworkOutbox<RentalsDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(1);
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((ctx, cfg) =>
    {
        var host = builder.Configuration["RabbitMQ:Host"] ?? "rabbitmq";
        var user = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var pass = builder.Configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host(host, "/", h => { h.Username(user); h.Password(pass); });

        // NADA de ReceiveEndpoint aqui; a Definition cuida do nome/opções
        cfg.ConfigureEndpoints(ctx);
    });
});
builder.Services.AddSingleton<IPricingService, PricingService>();

builder.Services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICourierRepository, CourierRepository>();

builder.Services.AddSingleton<IStorageService, MinioStorageService>();

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.ExampleFilters(); });
builder.Services.AddSwaggerExamplesFromAssemblyOf<ErroResponseExample>();
builder.Services.AddSwaggerExamplesFromAssemblyOf<ChangePlateRequestExample>();

builder.Services.AddHealthChecks()
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("Default")!,
        name: "postgres")
    .AddRabbitMQ(serviceProvider =>
    {
        var connectionString = $"amqp://{builder.Configuration["RabbitMQ:Username"]}:{builder.Configuration["RabbitMQ:Password"]}@{builder.Configuration["RabbitMQ:Host"]}:5672";
        return new RabbitMQ.Client.ConnectionFactory
        {
            Uri = new Uri(connectionString)
        }.CreateConnectionAsync();
    }, name: "rabbitmq")
    .AddMinio(serviceProvider =>
    {
        var endpoint = builder.Configuration["Minio:Endpoint"]!;
        var accessKey = builder.Configuration["Minio:AccessKey"]!;
        var secretKey = builder.Configuration["Minio:SecretKey"]!;
        var cleanEndpoint = endpoint.Replace("http://", "").Replace("https://", "");
        var withSSL = endpoint.StartsWith("https", StringComparison.OrdinalIgnoreCase);

        return new Minio.MinioClient()
            .WithEndpoint(cleanEndpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(withSSL)
            .Build();
    }, name: "minio");


var app = builder.Build();

app.UseExceptionHandler(errApp =>
{
    errApp.Run(async ctx =>
    {
        var feature = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var ex = feature?.Error;

        var (status, message) = ex switch
        {
            Rentals.Application.Exceptions.MotorcycleNotFoundException => (StatusCodes.Status404NotFound, "Moto não encontrada"),
            Rentals.Application.Exceptions.BadRequestException => (StatusCodes.Status400BadRequest, "Request mal formada"),
            Rentals.Application.Exceptions.MotorcycleInvalidDataException => (StatusCodes.Status400BadRequest, "Dados inválidos"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Moto não encontrada"),
            ValidationException => (StatusCodes.Status400BadRequest, "Request mal formada"),
            _ => (StatusCodes.Status400BadRequest, "Dados inválidos")
        };

        var errorResponse = new ErrorResponse(message);

        ctx.Response.ContentType = "application/json";
        ctx.Response.StatusCode = status;
        await ctx.Response.WriteAsJsonAsync(errorResponse);
    });
});



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (ctx, next) =>
{
    var cid = ctx.Request.Headers.TryGetValue("X-Correlation-ID", out var h) && !string.IsNullOrWhiteSpace(h)
        ? h.ToString()
        : ctx.TraceIdentifier;

    using (Serilog.Context.LogContext.PushProperty("CorrelationId", cid))
    using (Serilog.Context.LogContext.PushProperty("ClientIP", ctx.Connection.RemoteIpAddress?.ToString()))
    {
        await next();
    }
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false 
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.UseSerilogRequestLogging(opts =>
{
    opts.IncludeQueryInRequestPath = true;
    opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<RentalsDbContext>();

    var retries = 5;
    while (retries > 0)
    {
        try
        {
            context.Database.EnsureCreated(); 
            break;
        }
        catch (Exception ex)
        {
            retries--;
            if (retries == 0) throw;

            Console.WriteLine($"Database not ready, retrying in 5 seconds... ({ex.Message})");
            await Task.Delay(5000);
        }
    }
}
app.MapControllers();



app.Run();
using System.Text.Json.Serialization;

using BuildingBlocks.API.Http.Middlewares;
using BuildingBlocks.Infrastructure.Configuration;

using GradingModule.Application;
using GradingModule.Application.Commands;
using GradingModule.Domain;
using GradingModule.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.WithModuleName(nameof(GradingModule));

builder.AddConfiguration();

var secrets = builder.Configuration.GetSection("Secrets").Get<Secrets>()!;
builder.Services.AddSingleton(secrets);

builder.Services.AddAppDbContext<GradingContext>(secrets.Database.ConnectionString);

builder.Services.AddQuartzScheduler();

builder.AddSwagger();
builder.AddLogger();

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        {
            var enumConverter = new JsonStringEnumConverter();
            opts.JsonSerializerOptions.Converters.Add(enumConverter);
        }
    );

Application.init();
builder.Services.AddMediatr();
builder.Services.AutoRegister();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();
app.Services.ExecuteStartupActions();

app.MapControllers();

app.UseRouting();
app.UseExceptionHandler();
app.AddSwagger();

app.Run();
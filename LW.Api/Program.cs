using FluentValidation;
using FluentValidation.AspNetCore;
using LW.Api;
using LW.Api.Configs;
using LW.Api.Data;
using LW.Api.Validators;
using LW.Application.Features.Translation;
using LW.Application.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

//Auth config
builder.Services.AddDbContext<IdentityDbContext>(options =>
    // Saving in memory for simple db managmenet.
    options.UseInMemoryDatabase("InMemoryIdentityDb")
);
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<IdentityDbContext>();

//DI and config
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.Configure<LanguageSettings>(options => builder.Configuration.Bind("LanguageSettings", options));

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RawContentValidator>();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddCors(options => options.AddPolicy("lwpolicy",
    policy => policy.WithOrigins(builder.Configuration["BackendUrl"] ?? "",
    builder.Configuration["FrontendUrl"] ?? "")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    ));

//App
var app = builder.Build();
app.MapIdentityApi<IdentityUser>();

app.UseCors("lwpolicy");

//For internal usage, we could private the swagger.
//if (app.Environment.IsDevelopment())
app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();
app.Run();

using HotelAPI.Core.Configurations;
using HotelAPI.Core.Contracts;
using HotelAPI.Core.Middleeware;
using HotelAPI.Core.Repository;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Drawing.Imaging;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("HotelDbConnectionString") ?? "";
builder.Services.AddDbContext<HotelDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
#region Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    //doc
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Hotel API",
        Description = "An ASP.NET Core Web API for managing ToDo items",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Tim Developer",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Tim Developer",
            Url = new Uri("https://example.com/license")
        }
    });
    
    //jwt => put token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
    });

    //jwt requirement => requirement icon show 
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "0auth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        x => x.AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowAnyMethod());
});
builder.Services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate();
builder.Services.AddCertificateForwarding(options =>
{
    options.CertificateHeader = "ssl-client-cert";

    options.HeaderConverter = (headerValue) =>
    {
        X509Certificate2? clientCertificate = null;

        if (!string.IsNullOrWhiteSpace(headerValue))
        {
            clientCertificate = X509Certificate2.CreateFromPem(
                WebUtility.UrlDecode(headerValue));
        }

        return clientCertificate!;
    };
});
builder.Services.AddCertificateForwarding(options =>
{
    options.CertificateHeader = "X-SSL-CERT";

    options.HeaderConverter = headerValue =>
    {
        X509Certificate2? clientCertificate = null;

        if (!string.IsNullOrWhiteSpace(headerValue))
        {
            clientCertificate = new X509Certificate2(StringToByteArray(headerValue));
        }

        return clientCertificate!;

        static byte[] StringToByteArray(string hex)
        {
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];

            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }
    };
});
builder.Host.UseSerilog((context, logger) => logger.WriteTo.Console().ReadFrom.Configuration(context.Configuration));
#endregion

//Auto map
builder.Services.AddAutoMapper(typeof(MapperConfig));

//DI
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(ICountriesRepository), typeof(CountriesRepository));
builder.Services.AddScoped(typeof(IHotelsRepository), typeof(HotelsRepository));
builder.Services.AddScoped(typeof(IAuthManager), typeof(AuthManager));

//role
builder.Services.AddIdentityCore<ApiUser>().AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<ApiUser>>("HotelAPI")
    .AddEntityFrameworkStores<HotelDbContext>()
    .AddDefaultTokenProviders();

//authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Bearer
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters 
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])),
    };
});

//versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new QueryStringApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver")
        );
});
builder.Services.AddVersionedApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

//caching
builder.Services.AddResponseCaching(
    options => {
        options.MaximumBodySize = 1024;
        options.UseCaseSensitivePaths = true;
    });

//headlth check
//sqlserver health check
builder.Services.AddHealthChecks()
    .AddCheck<CustomHealthCheck>(
    "Custom health check",
    failureStatus: HealthStatus.Degraded,
    tags: new[] { "custom" })
    .AddSqlServer(connectionString, tags: new[] {"database"})
    .AddDbContextCheck<HotelDbContext>(tags: new[] {"database"});

//OData
builder.Services.AddControllers().AddOData( 
    options =>
    {
        options.Select().Filter().OrderBy();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/healthcheck", new HealthCheckOptions
{
    Predicate = healthcheck => healthcheck.Tags.Contains("custom"),
    ResultStatusCodes = 
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status500InternalServerError,
        [HealthStatus.Degraded] = StatusCodes.Status502BadGateway,
    },
    ResponseWriter = WriteResponse
    
});

app.MapHealthChecks("/databasehealthcheck", new HealthCheckOptions
{
    Predicate = healthcheck => healthcheck.Tags.Contains("database"),
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status500InternalServerError,
        [HealthStatus.Degraded] = StatusCodes.Status502BadGateway,
    },
    ResponseWriter = WriteResponse

});

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseResponseCaching();
app.Use(async (context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl =
    new Microsoft.Net.Http.Headers.CacheControlHeaderValue
    {
        Public = true,
        MaxAge = TimeSpan.FromSeconds(10),
    };
    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] = new string[] { "Accept-Encoding" };

    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static Task WriteResponse(HttpContext context, HealthReport healthReport)
{
    var options = new JsonWriterOptions { Indented = true };

    using var memoryStream = new MemoryStream();
    using (var jsonWriter = new Utf8JsonWriter(memoryStream, options))
    {
        jsonWriter.WriteStartObject();
        jsonWriter.WriteString("status", healthReport.Status.ToString());
        jsonWriter.WriteStartObject("results");

        foreach (var healthReportEntry in healthReport.Entries)
        {
            jsonWriter.WriteStartObject(healthReportEntry.Key);
            jsonWriter.WriteString("status", healthReportEntry.Value.Status.ToString());
            jsonWriter.WriteString("description", healthReportEntry.Value.Description.ToString());
            jsonWriter.WriteStartObject("data");

            foreach (var item in healthReportEntry.Value.Data)
            {
                jsonWriter.WritePropertyName(item.Key);

                JsonSerializer.Serialize(jsonWriter, item.Value, item.Value?.GetType() ?? typeof(object));
            }
            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }
        jsonWriter.WriteEndObject();
        jsonWriter.WriteEndObject();
    }
    return context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));    
}

class CustomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;

        if(isHealthy)
        {
            return Task.FromResult(HealthCheckResult.Healthy("All systems are looking good ^^"));
        }
        return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, "System Unhealthy"));
    }
}



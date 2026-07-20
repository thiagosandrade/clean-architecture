using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Extensions;

namespace SharedKernel.Extensions;

public static class CorsExtensions
{
    public static WebApplicationBuilder AllowCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return builder;
    }

    public static WebApplication UseCors(this WebApplication app)
    {
        app.UseCors("AllowFrontend");

        return app;
    }
}

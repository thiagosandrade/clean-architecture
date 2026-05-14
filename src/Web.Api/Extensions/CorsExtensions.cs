using Web.Api.Middleware;

namespace Web.Api.Extensions;

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

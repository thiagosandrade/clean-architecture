using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;

namespace Application.OpenAI.Extensions;

public static class AddOpenAIExtensions
{
    public static void AddOpenAI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new OpenAIClient(
                    configuration["AIConfig:OpenAIKey"]
                ));
    }
}

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Core.Api.Swagger;

public static class SwaggerConfigurationExtention
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });

        return services;
    }
    
}
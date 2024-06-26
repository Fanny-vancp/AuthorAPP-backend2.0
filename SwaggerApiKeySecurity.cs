﻿using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UniverseCreation.API
{
    public static class SwaggerApiKeySecurity
    {
        public static void AddSwaggerApiKeySecurity(this SwaggerGenOptions c)
        {
            c.AddSecurityDefinition("ApiKey", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = "ApiKey must appear in header",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Name = "XApiKey",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Scheme = "ApiKeyScheme"
            });

            var key = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header,
            };

            var requirement = new OpenApiSecurityRequirement { { key, new List<string>() } };
            c.AddSecurityRequirement(requirement);
        }
    }
}

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Elara.API
{
    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
            {
                var responseKey = responseType.IsDefaultResponse
                    ? "default"
                    : responseType.StatusCode.ToString();
                if (!operation.Responses.TryGetValue(responseKey, out var response))
                    continue;

                foreach (var contentType in response.Content.Keys.ToList())
                {
                    if (!responseType.ApiResponseFormats.Any(x => x.MediaType == contentType))
                        response.Content.Remove(contentType);
                }
            }

            if (operation.Parameters == null)
                return;

            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions
                    .FirstOrDefault(p => p.Name == parameter.Name);
                if (description == null)
                    continue;

                parameter.Description ??= description.ModelMetadata?.Description;

                if (parameter.Schema.Default == null && description.DefaultValue != null)
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(
                        description.DefaultValue,
                        description.ModelMetadata?.ModelType ?? typeof(object));
                    parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
                }

                parameter.Required |= description.IsRequired;
            }
        }
    }
}

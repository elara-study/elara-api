using System.Text.Json.Serialization;

namespace Elara.Application.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        // Success response factory
        public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        // Error response factory
        public static ApiResponse<T> ErrorResponse(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default
            };
        }

        // Convert to ErrorResponse for API responses
        public ErrorResponse ToErrorResponse()
        {
            return new ErrorResponse
            {
                Success = this.Success,
                Message = this.Message ?? string.Empty
            };
        }
    }
}


using Elara.Domain.Constants;

namespace Elara.Application.Models
{
    public class ErrorResponse
    {
        public string Status { get; set; } = ResponseStatus.Error;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }

        public static ErrorResponse Create(string message, string status = ResponseStatus.Error, object? data = null)
        {
            return new ErrorResponse
            {
                Status = status,
                Message = message,
                Data = data
            };
        }
    }
}

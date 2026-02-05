namespace Elara.Application.Models
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;

        public static ErrorResponse Create(string message)
        {
            return new ErrorResponse
            {
                Message = message
            };
        }
    }
}

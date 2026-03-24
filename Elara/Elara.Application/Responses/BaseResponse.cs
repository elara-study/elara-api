namespace Elara.Application.Responses
{
    public class BaseResponse<T>
    {
        public string Status { get; set; } = "Success";
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}

namespace TestMcSonae.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> Create(bool success, T data = default, string message = "", int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = success,
                Message = message,
                StatusCode = statusCode,
                Data = data
            };
        }
    }
}
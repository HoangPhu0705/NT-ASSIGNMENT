namespace SharedViewModels.Shared
{
    public class ApiResponse<T>
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse(int code, string message, T data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        public ApiResponse(int code, string message)
        {
            Code = code;
            Message = message;
            Data = default;
        }

        public static ApiResponse<T> Success(T data, string message = "Success")
            => new ApiResponse<T>(200, message, data);

        public static ApiResponse<T> Created(T data, string message = "Resource created successfully")
            => new ApiResponse<T>(201, message, data);

        public static ApiResponse<T> Error(string message, int code = 400)
            => new ApiResponse<T>(code, message);

        public static ApiResponse<T> NotFound(string message = "Resource not found")
            => new ApiResponse<T>(404, message);

        public static ApiResponse<T> Unauthorized(string message = "Unauthorized access")
            => new ApiResponse<T>(401, message);

        public static ApiResponse<T> Forbidden(string message = "Access forbidden")
            => new ApiResponse<T>(403, message);

        public static ApiResponse<T> InternalServerError(string message = "An unexpected error occurred")
            => new ApiResponse<T>(500, message);
    }
}
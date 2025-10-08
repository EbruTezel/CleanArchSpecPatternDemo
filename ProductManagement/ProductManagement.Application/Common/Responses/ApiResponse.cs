namespace ProductManagement.Application.Common.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }
        public Dictionary<string, string[]>? Errors { get; init; }

        public static ApiResponse<T> Ok(T data, string? message = null)
            => new ApiResponse<T> { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(Dictionary<string, string[]> errors, string? message = null)
            => new ApiResponse<T> { Success = false, Errors = errors, Message = message };
    }
}


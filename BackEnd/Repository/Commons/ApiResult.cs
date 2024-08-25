namespace Repository.Commons
{
    public record ApiResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }

        public static ApiResult<T> Succeed(T? data, string message)
        {
            return new ApiResult<T> { Success = true, Data = data, Message = message };
        }

        public static ApiResult<T> Error(T? data, string message)
        {
            return new ApiResult<T> { Success = false, Data = data, Message = message };
        }

        public static ApiResult<T> Fail(Exception ex)
        {
            return new ApiResult<T>
            {
                Success = false,
                Data = default,
                Message = ex.Message
            };
        }
    }
}
namespace ProjectHX.Models.Data
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static Result<T> Ok(T data, string? message = null)
        {
            return new Result<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static Result<T> Failure(string message)
        {
            return new Result<T>
            {
                Success = false,
                Message = message,
                Data = default
            };
        }
    }
}

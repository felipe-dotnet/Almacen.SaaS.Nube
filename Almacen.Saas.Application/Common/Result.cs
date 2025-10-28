namespace Almacen.Saas.Application.Common;

public class Result<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = [];

    public static Result<T> SuccessResult(T data, string message = "Operación exitosa")
    {
        return new Result<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static Result<T> FailureResult(string message, List<string>? errors = null)
    {
        return new Result<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

public class Result
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();

    public static Result SuccessResult(string message = "Operación exitosa")
    {
        return new Result
        {
            Success = true,
            Message = message
        };
    }

    public static Result FailureResult(string message, List<string>? errors = null)
    {
        return new Result
        {
            Success = false,
            Message = message,
            Errors = errors ?? []
        };
    }
}




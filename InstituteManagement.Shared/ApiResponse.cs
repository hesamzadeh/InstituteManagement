namespace InstituteManagement.Shared.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public List<ApiError>? Errors { get; set; }

    public static ApiResponse<T> Ok(T data) => new() { Success = true, Data = data };
    public static ApiResponse<T> Fail(params ApiError[] errors) =>
        new() { Success = false, Errors = errors.ToList() };
}
public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
    public string[]? Errors { get; set; }
    public string? Details { get; set; }
}

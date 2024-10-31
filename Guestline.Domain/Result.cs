namespace Guestline.Domain;

public class Result<T>
{
    private T? _value;
    public T Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException("Cannot retrieve value from a failed result");
            return _value!;
        }
        private set => _value = value;
    }

    public Exception? Exception { get; }
    public string? Error { get; }
    public bool IsSuccess => Exception == null && Error == null;
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Exception e) => new(e);
    public static Result<T> Failure(string error) => new(error);
    public static implicit operator Result<T>(T v) => Success(v);
    public static implicit operator Result<T>(Exception e) => Failure(e);

    private Result(T value)
    {
        Value = value;
    }

    private Result(Exception error)
    {
        Exception = error;
    }
    
    private Result(string error)
    {
        Error = error;
    }
}
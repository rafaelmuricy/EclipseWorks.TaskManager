public class Result<T>
{
    public T? Value { get; }
    public string? ErrorMessage { get; }

    private Result(T? value)
    {
        Value = value;
    }

    private Result(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public static Result<T> Error(string errorMessage)
    {
        return new Result<T>(errorMessage);
    }

    public static Result<T> ValidationError(List<string> errors)
    {
        string errorMessage = "Campos Necessários: " + string.Join(", ", errors.ToArray());

        return new Result<T>(errorMessage);
    }

    public static implicit operator Result<T>(T value) => new Result<T>(value);

}

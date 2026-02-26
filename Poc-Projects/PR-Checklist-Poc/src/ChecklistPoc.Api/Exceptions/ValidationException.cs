namespace ChecklistPoc.Api.Exceptions;

public sealed class ValidationException : ChecklistPocException
{
    public ValidationException(string message, IReadOnlyDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}


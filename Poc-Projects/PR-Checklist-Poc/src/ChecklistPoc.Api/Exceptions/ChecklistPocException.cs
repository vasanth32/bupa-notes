namespace ChecklistPoc.Api.Exceptions;

public abstract class ChecklistPocException : Exception
{
    protected ChecklistPocException(string message) : base(message)
    {
    }
}


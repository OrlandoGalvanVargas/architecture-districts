namespace FacilityOS.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Resource '{name}' with ID ({key}) was not found.") { }
}
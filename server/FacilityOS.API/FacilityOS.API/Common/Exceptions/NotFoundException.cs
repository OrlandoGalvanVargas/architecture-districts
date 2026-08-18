namespace FacilityOS.API.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"El recurso '{name}' con ID ({key}) no fue encontrado.") { }

        public NotFoundException(string message)
            : base(message) { }
    }
}
namespace HotelManagement.Aplicacion.Exceptions
{
    public class NotFoundException : Exception
    {
        public string Field { get; }
        
        public NotFoundException(string message, string field = "") : base(message) 
        { 
            Field = field;
        }
    }

    public class BadRequestException : Exception
    {
        public string Field { get; }
        
        public BadRequestException(string message, string field = "") : base(message) 
        { 
            Field = field;
        }
    }

    public class ValidationException : Exception
    {
        public Dictionary<string, List<string>> Errors { get; }

        public ValidationException(string field, string message) : base("Errores de validación")
        {
            Errors = new Dictionary<string, List<string>>
            {
                { field, new List<string> { message } }
            };
        }

        public ValidationException(Dictionary<string, List<string>> errors) : base(BuildMessage(errors))
        {
            Errors = errors;
        }

        private static string BuildMessage(Dictionary<string, List<string>> errors)
        {
            if (!errors.Any())
                return "Errores de validación";

            // Construir mensaje con los errores específicos
            var errorMessages = new List<string> { "Errores de validación:" };
            foreach (var kvp in errors)
            {
                foreach (var msg in kvp.Value)
                {
                    errorMessages.Add($" - {msg}");
                }
            }
            return string.Join(" ", errorMessages);
        }
    }

    public class ConflictException : Exception
    {
        public string Field { get; }
        
        public ConflictException(string message, string field = "") : base(message) 
        { 
            Field = field;
        }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "No autorizado") : base(message) { }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "Acceso prohibido") : base(message) { }
    }
}

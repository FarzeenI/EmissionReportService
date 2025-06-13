namespace EmissionReportService
{
    [Serializable]
    // Base custom exception (optional, good for grouping)
    public class EmissionException : Exception
    {
        public EmissionException() { }

        public EmissionException(string message) : base(message) { }

        public EmissionException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    // Thrown when no emission data is found
    public class EmissionNotFoundException : EmissionException
    {
        public EmissionNotFoundException(string message) : base(message) { }
    }

    // Thrown for validation-related issues
    public class EmissionValidationException : EmissionException
    {
        public EmissionValidationException(string message) : base(message) { }
    }
}
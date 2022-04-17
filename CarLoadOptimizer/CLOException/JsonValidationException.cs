namespace CarLoadOptimizer.CLOException
{
    /// <summary>
    /// Exception thrown when there is a JSON validation error
    /// </summary>
    public class JsonValidationException : Exception
    {
        public JsonValidationException() { }

        public JsonValidationException(string message) : base(message) { }
    }
}

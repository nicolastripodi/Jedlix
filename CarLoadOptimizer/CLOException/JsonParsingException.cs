namespace CarLoadOptimizer.CLOException
{
    /// <summary>
    /// Exception thrown when there is a JSON parsing error
    /// </summary>
    public class JsonParsingException : Exception
    {
        public JsonParsingException() { }

        public JsonParsingException(string message) : base(message) { }
    }
}

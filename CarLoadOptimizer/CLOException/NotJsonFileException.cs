namespace CarLoadOptimizer.CLOException
{
    /// <summary>
    /// Exception thrown when the specified file is not of JSON type
    /// </summary>
   public  class NotJsonFileException : Exception
    {
        public NotJsonFileException() { }

        public NotJsonFileException(string message) : base(message) { }
    }
}

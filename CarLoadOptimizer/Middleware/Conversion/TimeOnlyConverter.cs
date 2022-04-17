using CarLoadOptimizer.CLOException;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CarLoadOptimizer.Middleware.Conversion
{
    /// <summary>
    /// JSON TimeOnly converter
    /// Used to parse a JSON string as a TimeOnly and a TimeOnly to a JSON string
    /// </summary>
    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        // Current string format (In this case we use 'HH:mm')
        private readonly string serializationFormat;

        /// <summary>
        /// Inherited constructor extension
        /// </summary>
        public TimeOnlyConverter() : this(null) {}

        /// <summary>
        /// Inherited constructor extension
        /// </summary>
        /// <param name="serializationFormat"></param>
        public TimeOnlyConverter(string? serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "HH:mm";
        }

        /// <summary>
        /// Override of the read method
        /// Allows to parse a JSON string to TimeOnly
        /// </summary>
        /// <param name="reader">Current reader</param>
        /// <param name="typeToConvert">Type to convert</param>
        /// <param name="options">Serialization option</param>
        /// <returns>TimeOnly parsed as a string</returns>
        /// <exception cref="JsonParsingException">Throws exception on parsing error</exception>
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                var value = reader.GetString();
                return TimeOnly.Parse(value!);
            }
            catch (Exception ex) {
                throw new JsonParsingException("Could not parse string to TimeOnly: " + ex.Message);
            }
            
        }

        /// <summary>
        /// Override of the write method
        /// Parse a TimeOnly to a JSON string
        /// </summary>
        /// <param name="writer">Writer to write to</param>
        /// <param name="value">Value to convert to JSON</param>
        /// <param name="options">Serialization option</param>
        /// <exception cref="JsonParsingException">Throws exception on parsing error</exception>
        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            try
            {
                writer.WriteStringValue(value.ToString(serializationFormat));
            }
            catch (Exception ex)
            {
                throw new JsonParsingException("Could not parse TimeOnly to string: " + ex.Message);
            }
        }
    }
}

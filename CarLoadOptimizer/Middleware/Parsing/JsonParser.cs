using CarLoadOptimizer.CLOException;
using CarLoadOptimizer.Data.Input;
using CarLoadOptimizer.Data.Output;
using CarLoadOptimizer.Middleware.Conversion;
using System.Text.Json;

namespace CarLoadOptimizer.Middleware.Parsing
{
    /// <summary>
    /// JSON parser
    /// Used to read / write JSON file
    /// </summary>
    public class JsonParser
    {

        /// <summary>
        /// Read specified input file, and parse its data as a ChargingQuery
        /// </summary>
        /// <param name="inputFile">File to read</param>
        /// <returns>Charging query data from the file</returns>
        /// <exception cref="NotJsonFileException">Throws exception if file is not of JSON format</exception>
        /// <exception cref="JsonParsingException">Throws exception on parsing error</exception>
        public static ChargingQuery ParseInputFile(String inputFile)
        {
            if (!inputFile.EndsWith(".json")) 
                throw new NotJsonFileException("Input file is not of Json type.");

            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new TimeOnlyConverter()); // We add the TimeOnly converter to allow support for parsing TimeOnly

            string text = File.ReadAllText(inputFile); // Read the file
            ChargingQuery query = JsonSerializer.Deserialize<ChargingQuery>(text, options); // Parse data into the query

            if (query == null)
                throw new JsonParsingException("Could not parse charging query.");

            return query;
        }

        /// <summary>
        /// Write charging periods to specified output file as a JSON format
        /// </summary>
        /// <param name="chargingPeriods">Charging period to write to file</param>
        /// <param name="outputFile">File to write into</param>
        /// <exception cref="NotJsonFileException">Throws exception if file is not of JSON format</exception>
        public static void WriteChargingPeriodsToOutputFile(List<ChargingPeriod> chargingPeriods, String outputFile)
        {
            if (!outputFile.EndsWith(".json"))
                throw new NotJsonFileException("Input file is not of Json type.");

            String jsonContent = JsonSerializer.Serialize(chargingPeriods); // Convert to JSON string

            if (!File.Exists(outputFile)) // Create file if it doesnt exist
                File.Create(outputFile);

            File.WriteAllText(outputFile, jsonContent); // Write to file
        }

    }
}

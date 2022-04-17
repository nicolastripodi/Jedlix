using CarLoadOptimizer.CLOException;
using CarLoadOptimizer.Data.Input;
using CarLoadOptimizer.Data.Output;
using CarLoadOptimizer.Middleware.Parsing;
using CarLoadOptimizer.Middleware.LoadOptimization;

/// <summary>
/// Car Load Optimizer
/// Version 1.0.0
/// Parse a json file containing car information and user settings,
/// then returns a json file with the optimal charging plan for this car
/// </summary>
class CarLoadOptimizerMain
{
    /// <summary>
    /// Main method, run on program start
    /// Parse the input json file into an object
    /// Check if all the data is correct (no input errors)
    /// Calculate the optimal charging plan, then returns it in the json output file
    /// </summary>
    /// <param name="args">args[0]: json file to input. args[1]: json file to output.</param>
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Incorrect input. first argument: json input file. second argument: json output file");
        }
        else
        {
            String jsonInput = args[0]; // Input file
            String jsonOutput = args[1]; // Output file

            try
            {
                ChargingQuery query = JsonParser.ParseInputFile(jsonInput);
                LoadOptimizer loadOptimizer = new LoadOptimizer(query);
                List<ChargingPeriod> chargingPeriods = loadOptimizer.OptimizeCarLoad();
                JsonParser.WriteChargingPeriodsToOutputFile(chargingPeriods, jsonOutput);
                Console.WriteLine("Result written in " + jsonOutput);
            }
            catch (NotJsonFileException e)
            {
                Console.WriteLine("Json file exception: " + e.Message);
            }
            catch (JsonParsingException e)
            {
                Console.WriteLine("Json parsing exception: " + e.Message);
            }
            catch (JsonValidationException e)
            {
                Console.WriteLine("Json validation exception: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an issue during the execution of the program: " + e.Message);
            }
        }
    }
}
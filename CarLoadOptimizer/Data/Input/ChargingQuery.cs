using System.Text.Json.Serialization;

namespace CarLoadOptimizer.Data.Input
{
    /// <summary>
    /// Charging query, contains info about the starting date and time, user settings, and car data
    /// </summary>
    public class ChargingQuery
    {
        // Starting date and time
        [JsonPropertyName("startingTime")]
        public DateTime StartingTime { get; set; }

        // User settings (tariff, end time, and charging percentages)
        [JsonPropertyName("userSettings")]
        public UserSettings UserSettings { get; set; }

        // Car charging data
        [JsonPropertyName("carData")]
        public CarData CarData { get; set; }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="startingTime">Query start time</param>
        /// <param name="userSettings">User settings</param>
        /// <param name="carData">Car daa</param>
        public ChargingQuery(DateTime startingTime, UserSettings userSettings, CarData carData)
        {
            StartingTime = startingTime;
            UserSettings = userSettings;
            CarData = carData;
        }
    }
}

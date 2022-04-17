using System.Text.Json.Serialization;

namespace CarLoadOptimizer.Data.Input
{
    /// <summary>
    /// Represents the user settings (charge state + tariffs)
    /// </summary>
    public class UserSettings 
    {
        // The desired battery percentage of the battery of the car at Leaving Time
        [JsonPropertyName("desiredStateOfCharge")]
        public int DesiredStateOfCharge { get; set; }

        // Leaving time of the user
        [JsonPropertyName("leavingTime")]
        public TimeOnly LeavingTime { get; set; }

        // Minimum percentage of the battery we will always charge directly
        [JsonPropertyName("directChargingPercentage")]
        public int DirectChargingPercentage { get; set; }

        // List of energy prices
        [JsonPropertyName("tariffs")]
        public List<Tariff> Tariffs { get; set; }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="desiredStateOfCharge">Desired battery percentage</param>
        /// <param name="leavingTime">Car leaving time</param>
        /// <param name="directChargingPercentage">Direct battery charging percentage</param>
        /// <param name="tariffs">List of tariffs</param>
        public UserSettings(int desiredStateOfCharge, TimeOnly leavingTime, int directChargingPercentage, List<Tariff> tariffs)
        {
            DesiredStateOfCharge = desiredStateOfCharge;
            LeavingTime = leavingTime;
            DirectChargingPercentage = directChargingPercentage;
            Tariffs = tariffs;
        }
    }
}

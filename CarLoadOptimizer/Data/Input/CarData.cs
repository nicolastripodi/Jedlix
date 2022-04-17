using System.Text.Json.Serialization;

namespace CarLoadOptimizer.Data.Input
{
    /// <summary>
    /// Represent the car charging data
    /// </summary>
    public class CarData
    {
        // The power in kW (kilowatt) used when when charging (100% efficiency assumed)
        [JsonPropertyName("chargePower")]
        public decimal ChargePower { get; set; }

        // The amount of energy in kWh (kilowatt hour) that the battery can store.
        [JsonPropertyName("batteryCapacity")]
        public decimal BatteryCapacity { get; set; }

        // The level of charge in kWh the car battery at any one time
        [JsonPropertyName("currentBatteryLevel")]
        public decimal CurrentBatteryLevel { get; set; }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="chargePower">Power used when charging</param>
        /// <param name="batteryCapacity">Total battery capacity</param>
        /// <param name="currentBatteryLevel">Current battery level</param>
        public CarData(decimal chargePower, decimal batteryCapacity, decimal currentBatteryLevel)
        {
            ChargePower = chargePower;
            BatteryCapacity = batteryCapacity;
            CurrentBatteryLevel = currentBatteryLevel;
        }
    }
}

using System.Text.Json.Serialization;

namespace CarLoadOptimizer.Data.Output
{
    /// <summary>
    /// Output class, representing a charging period, and if the car is charging during this period
    /// </summary>
    public class ChargingPeriod
    {
        // Charging period start date and time
        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; }

        // Charging period end date and time
        [JsonPropertyName("endTime")]
        public DateTime EndTime { get; set; }

        // If the car is charging during the period
        [JsonPropertyName("isCharging")]
        public Boolean IsCharging { get; set; }

        /// <summary>
        /// Create a charging period from parameters
        /// </summary>
        /// <param name="startTime">Charging period start</param>
        /// <param name="endTime">Charging period end</param>
        /// <param name="isCharging">If the car is charging during the period</param>
        public ChargingPeriod(DateTime startTime, DateTime endTime, bool isCharging)
        {
            StartTime = startTime;
            EndTime = endTime;
            IsCharging = isCharging;
        }

        /// <summary>
        /// Combined time period with similar start / end date and charging status
        /// </summary>
        /// <param name="periods">Charging periods to combine</param>
        /// <returns>Combined charging periods</returns>
        public static List<ChargingPeriod> GetCombinedPeriods(List<ChargingPeriod> periods)
        {
            List<ChargingPeriod> combinedChargingPeriod = new List<ChargingPeriod>();

            if (periods != null)
            {
                if (periods.Count > 1)
                {
                    periods = periods.OrderBy(x => x.StartTime).ToList();
                    ChargingPeriod currentPeriod = periods.First();
                    periods.RemoveAt(0);

                    while (periods.Count > 0)
                    {
                        if (currentPeriod.EndTime == periods.First().StartTime && currentPeriod.IsCharging == periods.First().IsCharging)  // we regroup period with similar start and end date and same charging status
                            currentPeriod.EndTime = periods.First().EndTime;
                        else // Otherwise added as individual period
                        {
                            combinedChargingPeriod.Add(currentPeriod);
                            currentPeriod = periods.First();
                        }

                        periods.RemoveAt(0);
                    }

                    combinedChargingPeriod.Add(currentPeriod);
                }
                else if (periods.Count == 1)
                    return periods;
            }

            return combinedChargingPeriod;
        }

        /// <summary>
        /// Equals method override
        /// </summary>
        /// <param name="obj">Object to equals with</param>
        /// <returns>bool: true if equals, false if not</returns>
        public override bool Equals(object? obj)
        {
            return obj is ChargingPeriod period &&
                   StartTime == period.StartTime &&
                   EndTime == period.EndTime &&
                   IsCharging == period.IsCharging;
        }

        /// <summary>
        /// GetHashCode method override
        /// </summary>
        /// <returns>int: object hashcode</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(StartTime, EndTime, IsCharging);
        }
    }
}

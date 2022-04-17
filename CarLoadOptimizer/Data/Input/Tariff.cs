using System.Text.Json.Serialization;

namespace CarLoadOptimizer.Data.Input
{
    /// <summary>
    /// Represent a tariff
    /// </summary>
    public class Tariff
    {
        // Tariff start time
        [JsonPropertyName("startTime")]
        public TimeOnly StartTime { get; set; }

        // Tariff end time
        [JsonPropertyName("endTime")]
        public TimeOnly EndTime { get; set; }

        // Tariff energy price
        [JsonPropertyName("energyPrice")]
        public decimal EnergyPrice { get; set; }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="startTime">Tariff start time</param>
        /// <param name="endTime">Tariff end time</param>
        /// <param name="energyPrice">Current energy price for tariff</param>
        public Tariff(TimeOnly startTime, TimeOnly endTime, decimal energyPrice)
        {
            StartTime = startTime;
            EndTime = endTime;
            EnergyPrice = energyPrice;
        }


        /// <summary>
        /// Check if the current tariff intersects with the specified tariff
        /// </summary>
        /// <param name="t">Tariff to check intersection with</param>
        /// <returns>bool : true if tariffs intersect, false if they do not</returns>
        public bool Interesects(Tariff t)
        {
            if (t == null)
                return false;

            if (t.StartTime == StartTime && t.EndTime == EndTime) // Same period covered
                return true;

            bool isCurrentTariffOvernight = StartTime > EndTime; // Check if its overnight (EG from 23:00 to 1:00)

            bool doesOtherTariffEmcompassCurrentTariff = (t.StartTime < StartTime && EndTime > t.EndTime); // If current tariff overlaps the specified tariff

            if (isCurrentTariffOvernight)
            {
                bool doesStartTimeInterects = (StartTime < t.StartTime && t.StartTime <= TimeOnly.MaxValue || TimeOnly.MinValue <= t.StartTime && t.StartTime < EndTime); // Start time is contained in tariff
                bool doesEndTimeInterects = (StartTime < t.EndTime && t.EndTime <= TimeOnly.MaxValue || TimeOnly.MinValue <= t.EndTime && t.EndTime < EndTime); // End time is contained in tariff
                return doesStartTimeInterects || doesEndTimeInterects || doesOtherTariffEmcompassCurrentTariff;
            }
            else
            {
                bool doesStartTimeInterects = (StartTime < t.StartTime && t.StartTime < EndTime); // Start time is contained in tariff
                bool doesEndTimeInterects = (StartTime < t.EndTime && t.EndTime < EndTime); // End time is contained in tariff
                return doesStartTimeInterects || doesEndTimeInterects || doesOtherTariffEmcompassCurrentTariff;
            }
        }

        /// <summary>
        /// Check if the specified TimeOnly is included in the tariff period
        /// </summary>
        /// <param name="t">Time to check if its included</param>
        /// <returns>bool: true if the tariff period contains the specified TimeOnly, false if not</returns>
        public bool PeriodContains(TimeOnly t)
        {
            if (StartTime == EndTime)
                return true;

            bool isCurrentTariffOvernight = StartTime > EndTime; // Check if its overnight (EG from 23:00 to 1:00)

            if (isCurrentTariffOvernight)
                return (StartTime < t && t <= TimeOnly.MaxValue || TimeOnly.MinValue <= t && t < EndTime);

            else
                return (StartTime < t && t < EndTime);

        }
    }
}

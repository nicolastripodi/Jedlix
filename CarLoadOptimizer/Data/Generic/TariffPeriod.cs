namespace CarLoadOptimizer.Data.Generic
{
    /// <summary>
    /// Represent a tariff period with datetimes
    /// </summary>
    public class TariffPeriod
    {
        // Date and time of tariff start
        public DateTime Start { get; set; }

        // Date and time of tariff end
        public DateTime End { get; set; }

        // Energy price for tariff period
        public decimal EnergyPrice { get; set; }

        /// <summary>
        /// Crreates a tariff period
        /// </summary>
        /// <param name="start">start date and time</param>
        /// <param name="end">end date and time</param>
        /// <param name="energyPrice">energy price for period</param>
        public TariffPeriod(DateTime start, DateTime end, decimal energyPrice)
        { 
            Start = start;
            End = end;
            EnergyPrice = energyPrice;
        }

        /// <summary>
        /// Check if the specified DateTime is included in the tariff period
        /// </summary>
        /// <param name="t">DateTime to check if its included</param>
        /// <returns>bool: true if the tariff period contains the specified DateTime, false if not</returns>
        public bool PeriodContains(DateTime date) 
        { 
            return Start < date && date < End;
        }

        /// <summary>
        /// Returns the current period as hours
        /// </summary>
        /// <returns>How many hours there is in the period</returns>
        public double GetPeriodInHours()
        {
            return (End - Start).TotalHours;
        }
    }
}

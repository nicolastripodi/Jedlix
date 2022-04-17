namespace CarLoadOptimizer.Data.Generic
{
    /// <summary>
    /// Represents a time period represented by TimeOnly start and end
    /// </summary>
    public class TimePeriod
    {
        // Period start time
        public TimeOnly Start { get; set; }
        
        // Period end time
        public TimeOnly End { get; set; }

        /// <summary>
        /// Creates a TimePeriod from pamaters
        /// </summary>
        /// <param name="start">Start time</param>
        /// <param name="end">End time</param>
        public TimePeriod(TimeOnly start, TimeOnly end)
        { 
            Start = start;
            End = end;
        }

        /// <summary>
        /// Check if the current time period encompass the specified time period
        /// </summary>
        /// <param name="t">Time period to check if its included in current time period</param>
        /// <returns>bool: true if it does encompass, false if it does not</returns>
        public bool Encompass(TimePeriod t) 
        {
            if (t == null)
                return false;

            if (Start == End)
                return true;

            return (Start <= t.Start && End >= t.End);
        }
    }
}

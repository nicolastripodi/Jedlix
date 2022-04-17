using CarLoadOptimizer.Data.Generic;
using CarLoadOptimizer.Data.Input;
using CarLoadOptimizer.Data.Output;
using CarLoadOptimizer.Middleware.DataValidation;

namespace CarLoadOptimizer.Middleware.LoadOptimization
{
    /// <summary>
    /// Optimizer for charging query
    /// </summary>
    public class LoadOptimizer
    {

        // Current charging query to optimize
        private readonly ChargingQuery query;

        // List of charging period that encompass the query
        private List<ChargingPeriod> chargingPeriods;

        // List of tariff period that encompass the query
        private List<TariffPeriod> tariffPeriods;

        // If the query has already been processed
        private bool hasQueryAlreadyBeenProcessed;

        /// <summary>
        /// Creates a load optimizer
        /// </summary>
        /// <param name="query"></param>
        public LoadOptimizer(ChargingQuery query)
        {
            this.query = query;
            chargingPeriods = new List<ChargingPeriod>(); // List of charging periods
            tariffPeriods = new List<TariffPeriod>();
            hasQueryAlreadyBeenProcessed = false;
        }

        /// <summary>
        /// Optimize a charging query
        /// </summary>
        /// <returns>List<ChargingPeriod>: list of when the car is charging and when its not charging ordered by date</returns>
        public List<ChargingPeriod> OptimizeCarLoad()
        {
            if (!hasQueryAlreadyBeenProcessed) // Only process the query on first call, return cached value otherwise
            {
                ChargingQueryValidator.ValidateChargingQuery(query); // Check that the query is valid first

                if (DoesCarNeedCharge()) // Car needs to be charged
                {
                    GetTariffPeriodFromQuery();
                    if (DoesCarNeedDirectCharge()) // Minimum charge
                        chargingPeriods.AddRange(GetPeriodTillPercentageReached(ChargeType.Minimum)); // Adds periods to reach minimum charge

                    if (DoesCarNeedCharge()) // Desired charge
                        chargingPeriods.AddRange(GetPeriodTillPercentageReached(ChargeType.Desired)); // Adds periods to reach desired charge

                    chargingPeriods = ChargingPeriod.GetCombinedPeriods(chargingPeriods);

                }
                else // Car doesn't need to be charged, return the default charge
                    chargingPeriods.Add(GetFullPeriodWithNoCharge());

                hasQueryAlreadyBeenProcessed = true;
            }

            return chargingPeriods;
        }

        /// <summary>
        /// Checks if the car did reach its desired state of charge
        /// </summary>
        /// <returns>bool: true if the car needs to still be charged, false if not</returns>
        private bool DoesCarNeedCharge()
        {
            return (query.CarData.CurrentBatteryLevel / query.CarData.BatteryCapacity * 100) < (Decimal)query.UserSettings.DesiredStateOfCharge;
        }

        /// <summary>
        /// Checks if the car did reach its minimum state of charge
        /// </summary>
        /// <returns>bool: true if the car needs to still be charged, false if not</returns>
        private bool DoesCarNeedDirectCharge()
        {
            return (query.CarData.CurrentBatteryLevel / query.CarData.BatteryCapacity * 100) < (Decimal)query.UserSettings.DirectChargingPercentage;
        }

        /// <summary>
        /// Returns a default answer when the car doesnt need to be charged at all
        /// </summary>
        /// <returns>A charging period that starts when user start to load its car and end when he leaves, with a isCharging status set to false</returns>
        private ChargingPeriod GetFullPeriodWithNoCharge()
        {
            TimeOnly startTime = new TimeOnly(query.StartingTime.Hour, query.StartingTime.Minute);
            DateTime endDate = new DateTime(query.StartingTime.Year, query.StartingTime.Month, query.StartingTime.Day, query.UserSettings.LeavingTime.Hour, query.UserSettings.LeavingTime.Minute, query.UserSettings.LeavingTime.Second);
            if (startTime > query.UserSettings.LeavingTime)
                endDate = endDate.AddDays(1);
            return new ChargingPeriod(query.StartingTime, endDate, false);
        }

        /// <summary>
        /// Get the list of tariff period from the query
        /// </summary>
        /// <exception cref="InvalidDataException">Throws an exception if the tariff data is invalid</exception>
        private void GetTariffPeriodFromQuery()
        {
            TimeOnly startTime = new TimeOnly(query.StartingTime.Hour, query.StartingTime.Minute);
            TimeOnly endTime = new TimeOnly(query.UserSettings.LeavingTime.Hour, query.UserSettings.LeavingTime.Minute);
            bool sameDayCharge = endTime > startTime;

            // First we shift the tariff until we get the tariff containing the start time first
            List<Tariff> tariffsOrdered = query.UserSettings.Tariffs.OrderBy(x => x.EndTime).ToList();
            int tariffCount = tariffsOrdered.Count;
            while (tariffCount > 0)
            {
                Tariff tariffToShift = tariffsOrdered.First();
                if (tariffToShift.PeriodContains(startTime))
                {
                    break;
                }

                tariffsOrdered.RemoveAt(0);
                tariffsOrdered.Add(tariffToShift);
                tariffCount--;
            }
            if (tariffCount <= 0)
                throw new InvalidDataException("Tariff does not intersect with start time");
            DateTime startingPeriod = query.StartingTime;

            // Then we create the tariff period from the start date to the end date
            tariffCount = tariffsOrdered.Count + 1; // We do one more occurence in case the end date and start date are in the same tariff period
            while (tariffCount >= 0)
            {
                Tariff tariffToShift = tariffsOrdered.First();
                DateTime endingPeriod = new DateTime(query.StartingTime.Year, query.StartingTime.Month, query.StartingTime.Day, tariffToShift.EndTime.Hour, tariffToShift.EndTime.Minute, tariffToShift.EndTime.Second);

                if (tariffToShift.PeriodContains(endTime) && (tariffCount < tariffsOrdered.Count + 1 || sameDayCharge)) // if end date and start date are in the same tariff period, skip the first time to include the other tariff too, except for same day charge
                {
                    endingPeriod = new DateTime(query.StartingTime.Year, query.StartingTime.Month, query.StartingTime.Day, endTime.Hour, endTime.Minute, endTime.Second);
                    if (endingPeriod < startingPeriod)
                        endingPeriod = endingPeriod.AddDays(1);

                    tariffPeriods.Add(new TariffPeriod(startingPeriod, endingPeriod, tariffToShift.EnergyPrice));
                    break;
                }
  
                if (endingPeriod < startingPeriod)
                    endingPeriod = endingPeriod.AddDays(1);

                tariffPeriods.Add(new TariffPeriod(startingPeriod, endingPeriod, tariffToShift.EnergyPrice));
                tariffsOrdered.RemoveAt(0);
                tariffsOrdered.Add(tariffToShift);
                startingPeriod = endingPeriod;
                tariffCount--;
            }

            if (tariffCount <= 0)
                throw new InvalidDataException("Tariff does not intersect with end time");
        }

        /// <summary>
        /// Get the list of charging period to reach specified charge type
        /// </summary>
        /// <param name="chargeType">Type of charge: minimum(direct charging) / desired (desired charging)</param>
        /// <returns>List<ChargingPeriod>: List of charging period to reach specified charge type</returns>
        private List<ChargingPeriod> GetPeriodTillPercentageReached(ChargeType chargeType) {
            List<ChargingPeriod> chargingPeriods = new List<ChargingPeriod>();
            tariffPeriods = chargeType == ChargeType.Minimum ? tariffPeriods.OrderBy(x => x.Start).ToList() : tariffPeriods.OrderBy(x => x.EnergyPrice).ThenBy(x => x.Start).ToList();
            decimal percentChargeToReach = chargeType == ChargeType.Minimum ? query.UserSettings.DirectChargingPercentage : query.UserSettings.DesiredStateOfCharge;
            decimal chargeToReach = percentChargeToReach / 100 * query.CarData.BatteryCapacity;
           
            while (tariffPeriods.Count > 0) // Until we reach the end of the caring periods
            {
                TariffPeriod currentTariff = tariffPeriods.First();
                decimal missingCharge = chargeToReach - query.CarData.CurrentBatteryLevel;
                decimal timeToReachMissingCharge = missingCharge / query.CarData.ChargePower;
                decimal currentTariffPeriodInHours = (decimal)currentTariff.GetPeriodInHours();

                if (timeToReachMissingCharge <= currentTariffPeriodInHours) // Current period is enough to charge car
                {
                    // Update the starting time of the tariff period
                    chargingPeriods.Add(new ChargingPeriod(currentTariff.Start, currentTariff.Start.AddHours(Decimal.ToDouble(timeToReachMissingCharge)), true));
                    currentTariff.Start = currentTariff.Start.AddHours(Decimal.ToDouble(timeToReachMissingCharge));
                    query.CarData.CurrentBatteryLevel += missingCharge;
                    break; // then we break as charge is reached
                }

                // Current period is not enough to charge car
                chargingPeriods.Add(new ChargingPeriod(currentTariff.Start, currentTariff.End, true));
                decimal chargeForCurrentTariffPeriod = currentTariffPeriodInHours * query.CarData.ChargePower;
                query.CarData.CurrentBatteryLevel += chargeForCurrentTariffPeriod;
                tariffPeriods.RemoveAt(0);
            }

            if (chargeType == ChargeType.Desired) // Add the rest of the period when the car is not loading
            {
                while (tariffPeriods.Count > 0)
                {
                    TariffPeriod currentTariff = tariffPeriods.First();
                    chargingPeriods.Add(new ChargingPeriod(currentTariff.Start, currentTariff.End, false));
                    tariffPeriods.RemoveAt(0);
                }
            }
            
            return chargingPeriods;
        }
    }
}

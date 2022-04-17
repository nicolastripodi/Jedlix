using CarLoadOptimizer.CLOException;
using CarLoadOptimizer.Data.Generic;
using CarLoadOptimizer.Data.Input;

namespace CarLoadOptimizer.Middleware.DataValidation
{
    /// <summary>
    /// Validator for charging query
    /// Checks that all the data is valid and consistend
    /// </summary>
    public static class ChargingQueryValidator
    {
        /// <summary>
        /// Validate the charging query
        /// </summary>
        /// <param name="chargingQuery">Query to validate</param>
        /// <exception cref="JsonValidationException">Throws an error if the data is invalid</exception>
        public static void ValidateChargingQuery(ChargingQuery chargingQuery) 
        {
            if (chargingQuery == null)
                throw new JsonValidationException("Charging query doesn't contain data.");

            if (chargingQuery.StartingTime < DateTime.Now)
                throw new JsonValidationException("Starting charge time cannot be in the past.");

            ValidateUserSettings(chargingQuery.UserSettings);
            ValidateCarData(chargingQuery.CarData);
            CheckIfTariffsCoverChargingPeriod(chargingQuery);
        }

        /// <summary>
        /// Validate user settings
        /// </summary>
        /// <param name="userSettings">User settings to validate</param>
        /// <exception cref="JsonValidationException">Throws an error if the data is invalid</exception>
        public static void ValidateUserSettings(UserSettings userSettings) 
        {
            if (userSettings == null)
                throw new JsonValidationException("User settings unspecified.");

            CheckValidIntValue(userSettings.DesiredStateOfCharge, "Desired state of charge");
            CheckValidIntValue(userSettings.DirectChargingPercentage, "Direct charging percentage");

            if (userSettings.DirectChargingPercentage > userSettings.DesiredStateOfCharge)
                throw new JsonValidationException("Direct charging percentage cannot exceed desired state of charge.");

            if (userSettings.Tariffs == null || userSettings.Tariffs.Count == 0)
                throw new JsonValidationException("Tariffs unspecified.");

            foreach (Tariff t in userSettings.Tariffs)
            {
                ValidateTariff(t);
            }
        }

        /// <summary>
        /// Validate car data
        /// </summary>
        /// <param name="carData">Car data to validate</param>
        /// <exception cref="JsonValidationException">Throws an error if the data is invalid</exception>
        public static void ValidateCarData(CarData carData) 
        {
            if (carData == null)
                throw new JsonValidationException("Car data unspecified.");

            CheckValidDecimalValue(carData.ChargePower, "Charge power", false);
            CheckValidDecimalValue(carData.BatteryCapacity, "Battery capacity", false);
            CheckValidDecimalValue(carData.CurrentBatteryLevel, "Current battery level", true);

            if (carData.CurrentBatteryLevel > carData.BatteryCapacity)
                throw new JsonValidationException("Current battery level cannot exceed max battery capacity.");
        }

        /// <summary>
        /// Validate tariff
        /// </summary>
        /// <param name="t">Tariff to validate</param>
        /// <exception cref="JsonValidationException">Throws an error if the data is invalid</exception>
        public static void ValidateTariff(Tariff t)
        {
            if (t == null)
                throw new JsonValidationException("Tariff unspecified.");

            CheckValidDecimalValue(t.EnergyPrice, "Energy price", false);
        }

        /// <summary>
        /// Checks if an integer value is valid
        /// </summary>
        /// <param name="i">Value to check validity</param>
        /// <param name="valueName">Name of the value</param>
        /// <exception cref="JsonValidationException">Throws an error if the data is invalid</exception>
        private static void CheckValidIntValue(int i, string valueName)
        {
            if (i < 0)
                throw new JsonValidationException(valueName + " cannot be below 0.");
        }

        /// <summary>
        /// Checks if a decimal value is valid
        /// </summary>
        /// <param name="dec">Value to check validity</param>
        /// <param name="valueName">Name of the value</param>
        /// <exception cref="JsonValidationException">Throws an error if the data is invalid</exception>
        private static void CheckValidDecimalValue(decimal dec, string valueName, bool allowZero)
        {
            if ((!allowZero && dec <= 0) || (allowZero && dec < 0))
                throw new JsonValidationException(valueName + " cannot be" + (!allowZero ? " equal to or" : "") + " below 0.");
        }

        /// <summary>
        /// Check if tariff cover the charging period
        /// </summary>
        /// <param name="chargingQuery">Query to check coverage of</param>
        /// <exception cref="JsonValidationException">Throws an error if there is a missing tariff</exception>
        private static void CheckIfTariffsCoverChargingPeriod(ChargingQuery chargingQuery) 
        {
            TimeOnly startChargingTime = new TimeOnly(chargingQuery.StartingTime.Hour, chargingQuery.StartingTime.Minute);
            TimeOnly endChargingTime = chargingQuery.UserSettings.LeavingTime;
            TimePeriod chargingPeriod = new TimePeriod(startChargingTime, endChargingTime); // Car charing period
            Tariff[] currentTariffs = chargingQuery.UserSettings.Tariffs.OrderBy(x => x.EndTime).ToArray();

            // Check for tariff overlap
            for (var i = 0; i < currentTariffs.Length; i++) {
                for (var j = i+1; j < currentTariffs.Length; j++)
                {
                    if (currentTariffs[i].Interesects(currentTariffs[j]))
                        throw new JsonValidationException("Tariff intersection detected.");
                }
            }

            // Then we check if tariff encompass charging period: 
            // We create a list of continuous charging perid
            List<TimePeriod> continuousChargingPeriods = new List<TimePeriod>();
            continuousChargingPeriods.Add(new TimePeriod(currentTariffs[0].StartTime, currentTariffs[0].EndTime));

            for (var i = 1; i < currentTariffs.Length; i++)
            {
                if (continuousChargingPeriods.Last().End == currentTariffs[i].StartTime)
                    continuousChargingPeriods.Last().End = currentTariffs[i].EndTime;
                else
                    continuousChargingPeriods.Add(new TimePeriod(currentTariffs[i].StartTime, currentTariffs[i].EndTime));
            }

            // Then we check if any of this continuous charging period encompass the car charging period
            bool encompass = false;
            foreach (var continousChargingPeriod in continuousChargingPeriods)
            {
                if (continousChargingPeriod.Encompass(chargingPeriod))
                {
                    encompass = true;
                    break;
                }
            }

            if (!encompass)
                throw new JsonValidationException("Tariff does not encompass the charging period.");
        }

    }
}

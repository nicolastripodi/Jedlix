using CarLoadOptimizer.Data.Input;
using NUnit.Framework;
using System.Collections.Generic;
using System;
using CarLoadOptimizer.Middleware.DataValidation;
using CarLoadOptimizer.CLOException;

namespace CarLoadOptimizerTest
{
    public class ChargingQueryValidatorTests
    {
        private ChargingQuery defaultQuery;
        private CarData defaultCarData;  
        private List<Tariff> defaultTariffs;
        private UserSettings defaultSettings;

        [SetUp]
        public void Setup()
        {
            DateTime nextDay = DateTime.Now.AddDays(1);
            DateTime startTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 23, 0 ,0);
            TimeOnly defaultTime = new TimeOnly(0, 0);
            TimeOnly defaultTimeOfLeave = new TimeOnly(7, 0);

            defaultCarData = new CarData(1, 100, 0);
            defaultTariffs = new List<Tariff>();
            defaultTariffs.Add(new Tariff(defaultTime, defaultTime, 0.2m));
            defaultSettings = new UserSettings(90, defaultTimeOfLeave, 20, defaultTariffs);
            defaultQuery = new ChargingQuery(startTime, defaultSettings, defaultCarData);
        }

        #region Valid data testing

        [Test]
        public void ValidateDefaultTariff()
        {
            foreach (Tariff t in defaultQuery.UserSettings.Tariffs)
            {
                ChargingQueryValidator.ValidateTariff(t);
            }

            Assert.Pass();
        }

        [Test]
        public void ValidateDefaultCarData()
        {
            ChargingQueryValidator.ValidateCarData(defaultCarData);
            Assert.Pass();
        }

        [Test]
        public void ValidateDefaultUserSettings()
        {
            ChargingQueryValidator.ValidateUserSettings(defaultSettings);
            Assert.Pass();
        }

        [Test]
        public void ValidateDefaultQuery()
        {
            ChargingQueryValidator.ValidateChargingQuery(defaultQuery);
            Assert.Pass();
        }

        [Test]
        public void CheckQueryWithCustomTariffCoverage()
        {
            List<Tariff> customTariff = new List<Tariff>();
            customTariff.Add(new Tariff(new TimeOnly(1, 0), new TimeOnly(3, 0), 0.2m));
            customTariff.Add(new Tariff(new TimeOnly(3, 0), new TimeOnly(7, 0), 0.25m));
            DateTime startDate = new DateTime(DateTime.Now.Year + 1, 1, 1, 1, 0, 0);
            UserSettings settings = new UserSettings(90, new TimeOnly(7, 0), 5, customTariff);
            ChargingQuery query = new ChargingQuery(startDate, settings, defaultCarData);
            ChargingQueryValidator.ValidateChargingQuery(query);
            Assert.Pass();
        }

        #endregion

        #region Invalid data testing

        #region Tariff

        [Test]
        public void CheckNullTariffIsInvalid()
        {
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateTariff(null));
            Assert.That(ex.Message, Is.EqualTo("Tariff unspecified."));
        }

        [Test]
        public void CheckTariffWithInvalidEnergyPrice()
        {
            Tariff invalidTariff = new Tariff(TimeOnly.MinValue, TimeOnly.MinValue, 0);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateTariff(invalidTariff));
            Assert.That(ex.Message, Is.EqualTo("Energy price cannot be equal to or below 0."));
        }

        #endregion

        #region Car Data

        [Test]
        public void CheckNullCarDataIsInvalid()
        {
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateCarData(null));
            Assert.That(ex.Message, Is.EqualTo("Car data unspecified."));
        }

        [Test]
        public void CheckCarDataWithInvalidChargePower()
        {
            CarData carData = new CarData(0, 90, 5);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateCarData(carData));
            Assert.That(ex.Message, Is.EqualTo("Charge power cannot be equal to or below 0."));
        }

        [Test]
        public void CheckCarDataWithInvalidBatteryCapacity()
        {
            CarData carData = new CarData(1, 0, 5);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateCarData(carData));
            Assert.That(ex.Message, Is.EqualTo("Battery capacity cannot be equal to or below 0."));

            carData = new CarData(1, 10, 15);
            ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateCarData(carData));
            Assert.That(ex.Message, Is.EqualTo("Current battery level cannot exceed max battery capacity."));
        }

        [Test]
        public void CheckCarDataWithInvalidBatterylevel()
        {
            CarData carData = new CarData(1, 10, -1);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateCarData(carData));
            Assert.That(ex.Message, Is.EqualTo("Current battery level cannot be below 0."));
        }

        #endregion

        #region User settings

        [Test]
        public void CheckNullUserSettingsIsInvalid()
        {
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateUserSettings(null));
            Assert.That(ex.Message, Is.EqualTo("User settings unspecified."));
        }

        [Test]
        public void CheckUserSettingsWithInvalidDesiredStateOfCharge()
        {
            UserSettings userSettings = new UserSettings(-1, TimeOnly.MinValue, 5, defaultTariffs);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateUserSettings(userSettings));
            Assert.That(ex.Message, Is.EqualTo("Desired state of charge cannot be below 0."));
        }

        [Test]
        public void CheckUserSettingsWithInvalidDirectChargingPercentage()
        {
            UserSettings userSettings = new UserSettings(90, TimeOnly.MinValue, -1, defaultTariffs);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateUserSettings(userSettings));
            Assert.That(ex.Message, Is.EqualTo("Direct charging percentage cannot be below 0."));
        }

        [Test]
        public void CheckUserSettingsWithInvalidDirectChargingPercentageAboveDesiredStateOfCharge()
        {
            UserSettings userSettings = new UserSettings(50, TimeOnly.MinValue, 90, defaultTariffs);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateUserSettings(userSettings));
            Assert.That(ex.Message, Is.EqualTo("Direct charging percentage cannot exceed desired state of charge."));
        }

        [Test]
        public void CheckUserSettingsWithInvalidTariff()
        {
            UserSettings userSettings = new UserSettings(90, TimeOnly.MinValue, 50, null);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateUserSettings(userSettings));
            Assert.That(ex.Message, Is.EqualTo("Tariffs unspecified."));

            userSettings = new UserSettings(90, TimeOnly.MinValue, 50, new List<Tariff>());
            ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateUserSettings(userSettings));
            Assert.That(ex.Message, Is.EqualTo("Tariffs unspecified."));
        }

        #endregion

        #region Charging query

        [Test]
        public void CheckNullChargingQueryIsInvalid()
        {
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateChargingQuery(null));
            Assert.That(ex.Message, Is.EqualTo("Charging query doesn't contain data."));
        }

        [Test]
        public void CheckChargingQueryWithInvalidStartTime()
        {
            ChargingQuery query = new ChargingQuery(DateTime.Now.AddMinutes(-1), defaultSettings, defaultCarData);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateChargingQuery(query));
            Assert.That(ex.Message, Is.EqualTo("Starting charge time cannot be in the past."));
        }

        [Test]
        public void CheckChargingQueryWithInvalidUserSettings()
        {
            ChargingQuery query = new ChargingQuery(DateTime.Now.AddMinutes(1), null, defaultCarData);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateChargingQuery(query));
            Assert.That(ex.Message, Is.EqualTo("User settings unspecified."));
        }

        [Test]
        public void CheckChargingQueryWithInvalidCarData()
        {
            ChargingQuery query = new ChargingQuery(DateTime.Now.AddMinutes(1), defaultSettings, null);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateChargingQuery(query));
            Assert.That(ex.Message, Is.EqualTo("Car data unspecified."));
        }

        [Test]
        public void CheckQueryWithSameTariff()
        {
            List<Tariff> uncoveredTariff = new List<Tariff>();
            uncoveredTariff.Add(new Tariff(new TimeOnly(1, 0), new TimeOnly(7, 0), 0.2m));
            uncoveredTariff.Add(new Tariff(new TimeOnly(1, 0), new TimeOnly(7, 0), 0.25m));
            DateTime startDate = new DateTime(DateTime.Now.Year + 1, 1, 1, 1, 0, 0);
            UserSettings settings = new UserSettings(90, new TimeOnly(7, 0), 5, uncoveredTariff);
            ChargingQuery query = new ChargingQuery(startDate, settings, defaultCarData);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateChargingQuery(query));
            Assert.That(ex.Message, Is.EqualTo("Tariff intersection detected."));
        }

        [Test]
        public void CheckQueryWithInvalidTariffIntersection()
        {
            List<Tariff> uncoveredTariff = new List<Tariff>();
            uncoveredTariff.Add(new Tariff(new TimeOnly(1, 0), new TimeOnly(2, 0), 0.2m));
            uncoveredTariff.Add(new Tariff(new TimeOnly(1, 30), new TimeOnly(7, 0), 0.25m));
            DateTime startDate = new DateTime(DateTime.Now.Year + 1, 1, 1, 1, 0, 0);
            UserSettings settings = new UserSettings(90, new TimeOnly(7, 0), 5, uncoveredTariff);
            ChargingQuery query = new ChargingQuery(startDate, settings, defaultCarData);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateChargingQuery(query));
            Assert.That(ex.Message, Is.EqualTo("Tariff intersection detected."));
        }

        [Test]
        public void CheckQueryWithInvalidTariffCoverage()
        {
            List<Tariff> uncoveredTariff = new List<Tariff>();
            uncoveredTariff.Add(new Tariff(new TimeOnly(1, 0), new TimeOnly(2, 0), 0.2m));
            uncoveredTariff.Add(new Tariff(new TimeOnly(3, 0), new TimeOnly(7, 0), 0.25m));
            DateTime startDate = new DateTime(DateTime.Now.Year + 1, 1, 1, 1, 0, 0);
            UserSettings settings = new UserSettings(90, new TimeOnly(7, 0), 5, uncoveredTariff);
            ChargingQuery query = new ChargingQuery(startDate, settings, defaultCarData);
            var ex = Assert.Throws<JsonValidationException>(() => ChargingQueryValidator.ValidateChargingQuery(query));
            Assert.That(ex.Message, Is.EqualTo("Tariff does not encompass the charging period."));
        }

        #endregion

        #endregion
    }
}
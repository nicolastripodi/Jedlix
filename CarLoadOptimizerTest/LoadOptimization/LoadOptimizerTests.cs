using CarLoadOptimizer.CLOException;
using CarLoadOptimizer.Data.Input;
using CarLoadOptimizer.Data.Output;
using CarLoadOptimizer.Middleware.LoadOptimization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarLoadOptimizerTest
{
    public class LoadOptimizerTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CheckLoadOptimizationWhenCannotReachDesiredCapacity()
        {
            DateTime nextDay = DateTime.Now.AddDays(1);
            DateTime startTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 23, 0, 0);

            TimeOnly defaultTime = new TimeOnly(0, 0);
            TimeOnly timeOfLeave = new TimeOnly(7, 0);
            nextDay = nextDay.AddDays(1);
            DateTime endTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 7, 0, 0);

            List<Tariff> tariffs = new List<Tariff>();
            tariffs.Add(new Tariff(defaultTime, defaultTime, 0.2m));

            CarData carData = new CarData(1, 100, 0);

            UserSettings settings = new UserSettings(90, timeOfLeave, 20, tariffs);

            ChargingQuery query = new ChargingQuery(startTime, settings, carData);

            LoadOptimizer optimizer = new LoadOptimizer(query);
            List<ChargingPeriod> chargingPeriods = optimizer.OptimizeCarLoad();
            List<ChargingPeriod> expectedResults = new List<ChargingPeriod>() {
                new ChargingPeriod(startTime, endTime, true)
            };

            Assert.That(chargingPeriods.SequenceEqual(expectedResults));
        }

        [Test]
        public void CheckLoadOptimizationWithNoMinimuButDesiredReachable()
        {
            DateTime nextDay = DateTime.Now.AddDays(1);
            DateTime startTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 23, 0, 0);

            TimeOnly defaultTime = new TimeOnly(0, 0);
            TimeOnly timeOfLeave = new TimeOnly(7, 0);
            nextDay = nextDay.AddDays(1);
            DateTime endTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 7, 0, 0);

            List<Tariff> tariffs = new List<Tariff>();
            tariffs.Add(new Tariff(defaultTime, defaultTime, 0.2m));

            CarData carData = new CarData(45, 200, 0);

            UserSettings settings = new UserSettings(90, timeOfLeave, 0, tariffs);

            ChargingQuery query = new ChargingQuery(startTime, settings, carData);

            LoadOptimizer optimizer = new LoadOptimizer(query);
            List<ChargingPeriod> chargingPeriods = optimizer.OptimizeCarLoad();
            List<ChargingPeriod> expectedResults = new List<ChargingPeriod>() {
                new ChargingPeriod(startTime, startTime.AddHours(4), true),
                new ChargingPeriod(startTime.AddHours(4), endTime, false)
            };

            Assert.That(chargingPeriods.SequenceEqual(expectedResults));
        }

        [Test]
        public void CheckLoadOptimizationWithMinimuAndDesiredReachable()
        {
            DateTime nextDay = DateTime.Now.AddDays(1);
            DateTime startTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 23, 0, 0);

            TimeOnly defaultTime = new TimeOnly(0, 0);
            TimeOnly timeOfLeave = new TimeOnly(7, 0);
            nextDay = nextDay.AddDays(1);
            DateTime endTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, timeOfLeave.Hour, timeOfLeave.Minute, 0);

            List<Tariff> tariffs = new List<Tariff>();
            tariffs.Add(new Tariff(defaultTime, defaultTime, 0.2m));

            CarData carData = new CarData(45, 200, 0);

            UserSettings settings = new UserSettings(90, timeOfLeave, 20, tariffs);

            ChargingQuery query = new ChargingQuery(startTime, settings, carData);

            LoadOptimizer optimizer = new LoadOptimizer(query);
            List<ChargingPeriod> chargingPeriods = optimizer.OptimizeCarLoad();
            List<ChargingPeriod> expectedResults = new List<ChargingPeriod>() {
                new ChargingPeriod(startTime, startTime.AddHours(4), true),
                new ChargingPeriod(startTime.AddHours(4), endTime, false)
            };

            Assert.That(chargingPeriods.SequenceEqual(expectedResults));
        }

        [Test]
        public void CheckLoadOptimizationWithNoMinimuButDesiredReachableAndDifferentTariffs()
        {
            DateTime nextDay = DateTime.Now.AddDays(1);
            DateTime startTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 23, 0, 0);

            TimeOnly defaultTime = new TimeOnly(0, 0);
            TimeOnly noon = new TimeOnly(12, 0);

            TimeOnly timeOfLeave = new TimeOnly(14, 0);
            nextDay = nextDay.AddDays(1);
            DateTime endTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, timeOfLeave.Hour, timeOfLeave.Minute, 0);

            List<Tariff> tariffs = new List<Tariff>();
            tariffs.Add(new Tariff(defaultTime, noon, 0.2m));
            tariffs.Add(new Tariff(noon, defaultTime, 0.15m));

            CarData carData = new CarData(45, 200, 0);

            UserSettings settings = new UserSettings(90, timeOfLeave, 0, tariffs);

            ChargingQuery query = new ChargingQuery(startTime, settings, carData);

            LoadOptimizer optimizer = new LoadOptimizer(query);
            List<ChargingPeriod> chargingPeriods = optimizer.OptimizeCarLoad();
            List<ChargingPeriod> expectedResults = new List<ChargingPeriod>() {
                new ChargingPeriod(startTime, startTime.AddHours(2), true),
                new ChargingPeriod(startTime.AddHours(2), endTime.AddHours(-2), false),
                new ChargingPeriod(endTime.AddHours(-2), endTime, true)
            };

            Assert.That(chargingPeriods.SequenceEqual(expectedResults));
        }

        [Test]
        public void CheckLoadOptimizationWithMinimuButDesiredReachableAndDifferentTariffs()
        {
            DateTime nextDay = DateTime.Now.AddDays(1);
            DateTime startTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 19, 0, 0);

            TimeOnly defaultTime = new TimeOnly(0, 0);
            TimeOnly noon = new TimeOnly(12, 0);

            TimeOnly timeOfLeave = new TimeOnly(14, 0);
            nextDay = nextDay.AddDays(1);
            DateTime midnightTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0);
            DateTime endTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, timeOfLeave.Hour, timeOfLeave.Minute, 0);

            List<Tariff> tariffs = new List<Tariff>();
            tariffs.Add(new Tariff(defaultTime, noon, 0.15m));
            tariffs.Add(new Tariff(noon, defaultTime, 0.2m));

            CarData carData = new CarData(40, 200, 0);

            UserSettings settings = new UserSettings(80, timeOfLeave, 20, tariffs);

            ChargingQuery query = new ChargingQuery(startTime, settings, carData);

            LoadOptimizer optimizer = new LoadOptimizer(query);
            List<ChargingPeriod> chargingPeriods = optimizer.OptimizeCarLoad();
            List<ChargingPeriod> expectedResults = new List<ChargingPeriod>() {
                new ChargingPeriod(startTime, startTime.AddHours(1), true),
                new ChargingPeriod(startTime.AddHours(1), midnightTime, false),
                new ChargingPeriod(midnightTime, midnightTime.AddHours(3), true),
                new ChargingPeriod(midnightTime.AddHours(3), endTime, false),
            };

            Assert.That(chargingPeriods.SequenceEqual(expectedResults));
        }

        [Test]
        public void CheckLoadOptimizationInvalidQuery()
        {
            LoadOptimizer optimizer = new LoadOptimizer(null);
            var ex = Assert.Throws<JsonValidationException>(() => optimizer.OptimizeCarLoad());
            Assert.That(ex.Message, Is.EqualTo("Charging query doesn't contain data."));
        }
    }
}
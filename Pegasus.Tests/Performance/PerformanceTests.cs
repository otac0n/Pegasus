// Copyright © John Gietzen. All Rights Reserved. This source is subject to the MIT license. Please see license.md for more information.

namespace Pegasus.Tests.Performance
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using NUnit.Framework;

    public static class PerformanceTests
    {
        private static decimal[] tDistribution = new decimal[]
        {
            0.00m, 0.00m, 12.71m, 4.30m, 3.18m, 2.78m, 2.57m, 2.45m, 2.36m, 2.31m,
            2.26m, 2.23m, 2.20m, 2.18m, 2.16m, 2.14m, 2.13m, 2.12m, 2.11m, 2.10m,
            2.09m, 2.09m, 2.08m, 2.07m, 2.07m, 2.06m, 2.06m, 2.06m, 2.05m, 2.05m,
            2.05m, 2.04m, 2.04m, 2.04m, 2.03m, 2.03m, 2.03m, 2.03m, 2.03m, 2.02m,
            2.02m, 2.02m, 2.02m, 2.02m, 2.02m, 2.02m, 2.01m, 2.01m, 2.01m, 2.01m,
            2.01m, 2.01m, 2.01m, 2.01m, 2.01m, 2.00m, 2.00m, 2.00m, 2.00m, 2.00m,
            2.00m, 2.00m, 2.00m, 2.00m, 2.00m, 2.00m, 2.00m, 2.00m, 2.00m, 2.00m,
            1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m,
            1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m,
            1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.99m, 1.98m, 1.98m, 1.98m,
            1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m,
            1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m,
            1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m,
            1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m,
            1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m,
            1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m, 1.98m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m, 1.97m,
            1.97m, 1.97m, 1.97m, 1.97m, 1.96m
        };

        private static TimeSpan TestTargetTime => TimeSpan.FromSeconds(2);

        private static TimeSpan WarmupTargetTime => TimeSpan.FromSeconds(0.1);

        [TestCaseSource("Methods")]
        public static void Evaluate(Action action)
        {
            var measure = new Func<int, RunningStat>(samples =>
            {
                var runningStat = new RunningStat();
                var sw = new Stopwatch();

                while (samples-- > 0)
                {
                    sw.Restart();
                    action();
                    runningStat.Push((decimal)sw.Elapsed.TotalMilliseconds);
                }

                return runningStat;
            });

            var initialTime = measure(1);
            var baseTime = measure(1);

            var warmupSamples = Math.Max(1, (int)Math.Round(WarmupTargetTime.TotalMilliseconds / (double)baseTime.Mean));
            var warmupTime = measure(warmupSamples);

            var testSamples = Math.Max(30, (int)Math.Round(TestTargetTime.TotalMilliseconds / (double)warmupTime.Mean));
            var testTime = measure(testSamples);

            PublishResults(initialTime.Mean, baseTime.Mean, warmupSamples, warmupTime.Mean, warmupTime.StandardDeviation, testSamples, testTime.Mean, testTime.StandardDeviation);
        }

        private static string FormatTime(int count, decimal mean, decimal stdDev = 0)
        {
            string suffix;
            decimal rounded;

            if (count > 1 && stdDev != 0)
            {
                var interval = tDistribution[Math.Min(count, tDistribution.Length - 1)] * stdDev;
                var intervalScale = GetScale(interval);

                suffix = "±" + Math.Round(interval, 3 - intervalScale) + "ms";

                var scale = Math.Min(intervalScale, GetScale(mean));

                rounded = Math.Round(mean, 3 - scale);
            }
            else
            {
                suffix = string.Empty;
                rounded = Math.Round(mean, 3 - GetScale(mean));
            }

            return rounded + "ms" + suffix;
        }

        private static int GetScale(decimal d)
        {
            return d == 0 ? 0 : (int)Math.Floor(Math.Log10((double)Math.Abs(d))) + 1;
        }

        private static Action MakeAction(object fixture, MethodInfo method)
        {
            return (Action)Expression.Lambda(Expression.Call(Expression.Constant(fixture), method)).Compile();
        }

        private static void PublishResults(decimal initialTime, decimal baseTime, int warmupSamples, decimal warmupMean, decimal warmupStandardDeviation, int testSamples, decimal testMean, decimal testStandardDeviation)
        {
            Trace.WriteLine($"initialTime: {FormatTime(1, initialTime)}:");
            Trace.WriteLine($"baseTime: {FormatTime(1, baseTime)}:");
            Trace.WriteLine($"warmupSamples: {warmupSamples}");
            Trace.WriteLine($"warmupMean: {FormatTime(warmupSamples, warmupMean, warmupStandardDeviation)}:");
            Trace.WriteLine($"testSamples: {testSamples}");
            Trace.WriteLine($"testMean: {FormatTime(testSamples, testMean, testStandardDeviation)}:");

            var testName = TestContext.CurrentContext.Test.FullName;
            var resultsFolder = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                "performance");
            var outputPath = Path.Combine(
                resultsFolder,
                testName + ".csv");

            var columns = "date,testSamples,testMean,testStandardDeviation,warmupSamples,warmupMean,warmupStandardDeviation,initialTime,baseTime,machine";

            if (File.Exists(outputPath))
            {
                var lines = File.ReadAllLines(outputPath);
                Assume.That(lines.Length, Is.GreaterThanOrEqualTo(1));
                Assume.That(lines[0], Is.EqualTo(columns));
            }
            else
            {
                if (!Directory.Exists(resultsFolder))
                {
                    Directory.CreateDirectory(resultsFolder);
                }

                File.WriteAllLines(outputPath, new[]
                {
                    columns
                });
            }

            var data = new[] { testSamples, testMean, testStandardDeviation, warmupSamples, warmupMean, warmupStandardDeviation, initialTime, baseTime }.Select(d => d.ToString(CultureInfo.InvariantCulture)).ToList();
            data.Insert(0, DateTime.UtcNow.ToString("O"));
            data.Insert(9, Environment.MachineName);
            File.AppendAllLines(outputPath, new[]
            {
                string.Join(",", data),
            });
        }

        private class RunningStat
        {
            private int count = 0;
            private decimal mean;
            private decimal s;

            public int Count => this.count;

            public decimal Mean => this.count > 0 ? this.mean : 0;

            public decimal StandardDeviation => (decimal)Math.Sqrt((double)this.Variance);

            public decimal Variance => this.count > 1 ? this.s / (this.count - 1) : 0;

            public void Clear()
            {
                this.count = 0;
            }

            public void Push(decimal value)
            {
                this.count++;
                if (this.count == 1)
                {
                    this.mean = value;
                }
                else
                {
                    var a = value - this.mean;
                    this.mean = this.mean + (value - this.mean) / this.count;
                    var b = value - this.mean;
                    this.s += a * b;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MergePowerData.CIAFdata;

namespace MergePowerData.IntelMath
{
    /// <summary>
    /// </summary>
    public class StatCollector
    {
        private readonly IDictionary<string, Statistic> _stats = new Dictionary<string, Statistic>();
        public readonly double PLimit;

        public StatCollector(double minGdpPp)
        {
            PLimit = minGdpPp;
        }

        public int Count => _stats.Count;

        public double CalcX(string statName, double yValue)
        {
            return yValue / _stats[statName].Slope();
        }

        public double Stand(string statName, double xValue, double yValue)
        {
            return (xValue - CalcX(statName, yValue)) / _stats[statName].Qx();
        }

        public double Stand(string statName, Country c)
        {
            var xy = statName.Split('_');
            return Stand(statName, IntelCore.XValue(xy[0], c), IntelCore.XValue(xy[1], c));
        }

        public void Add(Country c)
        {
            // Limit countries by minimum GDP
            if (c.PurchasePower.value / IntelCore.Giga < PLimit) return;

            //     do not add aggregation-country entries to stats
            if (Regex.IsMatch(c.Name, @"world|European", RegexOptions.IgnoreCase)) return;

            AddCrossTableRegressions(c);
        }

        /// <summary>
        /// </summary>
        /// <param name="c"></param>
        public void AddCrossTableRegressions(Country c)
        {
            IDictionary<string, int> exclude = new Dictionary<string, int>();

            var x = IntelCore.ColumnConfigs.Keys.ToArray();

            for (var a = 0; a < x.Length; a++)
                for (var b = 0; b < x.Length; b++)
                    if (a != b)
                    {
                        var statName = $"{x[a]}_{x[b]}";
                        var reverse = $"{x[b]}_{x[a]}";

                        if (!exclude.ContainsKey(statName))
                        {
                            var m = IntelCore.XValue(x[a], c);
                            var n = IntelCore.XValue(x[b], c);

                            if (!_stats.ContainsKey(statName))
                                _stats.Add(statName, new Statistic());
                            //Console.WriteLine($"AddStat: {statName}");

                            if (!(m is double.NaN || n is double.NaN))
                                _stats[statName].Add(m, n);

                            exclude.Add(reverse, 0);
                        }
                    }
        }


        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var result = new StringBuilder(base.ToString() + "\n");

            foreach (var item in _stats.OrderByDescending(r => r.Value.Correlation()))
                result.Append($"{item.Key}: {item.Value}\n");

            return result.ToString();
        }
    

        Dictionary<string, int> ReportSet = new Dictionary<string, int>();

        /// <summary>
        /// </summary>
        /// <param name="dv"></param>
        /// <param name="gdpAbove"></param>
        /// <param name="gdpBelow"></param>
        /// <returns></returns>
        public string ToReport(string dv, double gdpAbove, double gdpBelow)
        {
            var result = new StringBuilder($"Independent(X){dv}vs Dependent(Y){dv}Correlation{dv}MeanX{dv}Slope\n");

            foreach (var item in _stats.OrderByDescending(r => r.Value.Correlation()))
            {
                var xyNames = IntelCore.GetNames(item.Key.Split('_'));

                if (item.Value.Correlation() > .95)
                    foreach (var key in item.Key.Split('_'))
                        if (ReportSet.ContainsKey(key))
                            ReportSet[key]++;
                        else
                            ReportSet.Add(key, 0);

                var xyUnits = IntelCore.GetUnits(item.Key.Split('_'));

                var stat = item.Value;
                if (item.Value.Correlation() > gdpAbove || item.Value.Correlation() < gdpBelow)
                    result.Append(
                            $"{xyNames[0]}{dv}vs {xyNames[1]}{dv}{stat.Correlation():F3}{dv}{stat.MeanX():F1}{dv}{stat.Slope():F1} {xyUnits[0]}/{xyUnits[1]}\n");
            }

            //Console.WriteLine(string.Join(", ", ReportSet.Keys));
            return result.ToString();
        }

        public string ToReport(string dv, string filter)
        {
            Console.WriteLine($"Filter by: {filter}");
            var result = new StringBuilder($"Independent(X){dv}vs Dependent(Y){dv}Correlation{dv}MeanX{dv}Slope\n");

            foreach (var item in _stats.OrderByDescending(r => r.Value.Correlation()))
            {
                var xyNames = IntelCore.GetNames(item.Key.Split('_'));
                var xyUnits = IntelCore.GetUnits(item.Key.Split('_'));

                var stat = item.Value;

                if (Regex.IsMatch(item.Key, filter, RegexOptions.IgnoreCase))
                    result.Append(
                        $"{xyNames[0]}{dv}vs {xyNames[1]}{dv}{stat.Correlation():F3}{dv}{stat.MeanX():F1}{dv}{stat.Slope():F1} {xyUnits[0]}/{xyUnits[1]}\n");
            }

            return result.ToString();
        }
    }
}
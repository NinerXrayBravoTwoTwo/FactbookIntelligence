using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MergePowerData.CIAFdata;
using Newtonsoft.Json;

namespace MergePowerData.IntelMath
{
    /// <summary>
    /// 
    /// </summary>
    public class StatCollector
    {
        public readonly double PLimit;
        private static readonly string ConfigFile = Environment.CurrentDirectory + "/Columns.json";

        public static Dictionary<string, ColumnConfig> ColumnConfigs =>
            JsonConvert.DeserializeObject<Dictionary<string, ColumnConfig>>(File.ReadAllText(ConfigFile));

        public int Count => _stats.Count;
        private readonly IDictionary<string, Statistic> _stats = new Dictionary<string, Statistic>();

        public StatCollector(double minGdpPp)
        {
            PLimit = minGdpPp;
        }

        public double CalcX(string statName, double yValue)
        {
            return yValue / _stats[statName].Slope();
        }

        public double Stand(string statName, double xValue, double yValue)
        {
            return (xValue - CalcX(statName, yValue)) / _stats[statName].Qx();
        }

        public void Add(Country c)
        {
            // Limit countries by minimum GDP
            if (c.PurchasePower.value / Intel.Giga < PLimit) return;

            //     do not add aggregation-country entries to stats
            if (Regex.IsMatch(c.Name, @"world|European", RegexOptions.IgnoreCase)) return;

            AddCrossTableRegressions(c);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        public void AddCrossTableRegressions(Country c)
        {
            IDictionary<string, int> exclude = new Dictionary<string, int>();

            var x = ColumnConfigs.Keys.ToArray();

            for (var a = 0; a < x.Length; a++)
                for (var b = 0; b < x.Length; b++)
                    if (a != b)
                    {
                        var statName = $"{x[a]}_{x[b]}";
                        var reverse = $"{x[b]}_{x[a]}";

                        if (!exclude.ContainsKey(statName))
                        {
                            var m = XValue(x[a], c);
                            var n = XValue(x[b], c);

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
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double XValue(string name, Country c)
        {
            double result;

            var igc = c.Electric.Electricity.installed_generating_capacity;
            var elcity = c.Electric.Electricity;

            switch (name)
            {
                case "emission":
                    result = c.Electric.TtonCo2; break;
                case "gdp":
                    result = c.PurchasePower.value / Intel.Giga; break;
                case "growth":
                    result = c.GrowthRate.value; break;
                case "pop":
                    result = c.Pop / Intel.Mega; break;
                case "kwpop":
                    result = c.Electric.ProdKWh / c.Pop; break;
                case "eprod":
                    result = c.Electric.ProdTWh * Intel.TWh2kg; break;
                case "econs":
                    result = c.Electric.ConsTWh * Intel.TWh2kg; break;
                case "eprodtw":
                    result = c.Electric.ProdTWh; break;
                case "econstw":
                    result = c.Electric.ConsTWh; break;
                case "eimport":
                    result = elcity.imports != null ? elcity.imports.TWh * Intel.TWh2kg : double.NaN;
                    break;
                case "eexport":
                    result = elcity.exports != null ? elcity.exports.TWh * Intel.TWh2kg : double.NaN;
                    break;
                case "pcff":
                    result = igc != null
                        ? igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.fossil_fuels.percent) * Intel.TWh2kg
                        : double.NaN;
                    break;
                case "pcnuke":
                    result = igc != null
                        ? igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.nuclear_fuels.percent) * Intel.TWh2kg
                        : double.NaN;
                    break;
                case "pchydro":
                    result = igc != null
                        ? igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.hydroelectric_plants.percent) * Intel.TWh2kg
                        : double.NaN;
                    break;
                case "pcrenew":
                    result = igc != null
                        ? igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.other_renewable_sources.percent) * Intel.TWh2kg
                        : double.NaN;
                    break;
                case "ffrefineprod":
                    result = c.FossilFuelDetail.RefinedPetroleum.Production.Value / Intel.Mega;
                    break;
                case "ffrefinecons":
                    result = c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / Intel.Mega;
                    break;
                case "ffrefineimport":
                    result = c.FossilFuelDetail.RefinedPetroleum.Imports.Value / Intel.Mega;
                    break;
                case "ffrefineexport":
                    result = c.FossilFuelDetail.RefinedPetroleum.Exports.Value / Intel.Mega;
                    break;

                case "ffnatgasprod":
                    result = c.FossilFuelDetail.NaturalGas.Production.Value / Intel.Giga;
                    break;
                case "ffnatgascons":
                    result = c.FossilFuelDetail.NaturalGas.Consumption.Value / Intel.Giga;
                    break;
                case "ffnatgasimport":
                    result = c.FossilFuelDetail.NaturalGas.Imports.Value / Intel.Giga;
                    break;
                case "ffnatgasexport":
                    result = c.FossilFuelDetail.NaturalGas.Exports.Value / Intel.Giga;
                    break;
                case "ffnatgasreserv":
                    result = c.FossilFuelDetail.NaturalGas.ProvedReserves.Value / Intel.Giga;
                    break;

                case "ffcrudereserv":
                    result = c.FossilFuelDetail.CrudeOil.ProvedReserves.Value / Intel.Mega;
                    break;
                case "ffcrudeprod":
                    result = c.FossilFuelDetail.CrudeOil.Production.Value / Intel.Mega;
                    break;
                case "ffcrudeimport":
                    result = c.FossilFuelDetail.CrudeOil.Imports.Value / Intel.Mega;
                    break;
                case "ffcrudeexport":
                    result = c.FossilFuelDetail.CrudeOil.Exports.Value / Intel.Mega;
                    break;
                case "pcoilgdp":
                    result = (c.FossilFuelDetail.CrudeOil.Production.Value * 50 * 365.242198781) /
                             c.PurchasePower.value;
                    break;
                default:
                    throw new ArgumentException($"Undefined {name}");
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var result = new StringBuilder(base.ToString() + "\n");

            foreach (var item in _stats.OrderByDescending(r => r.Value.Correlation()))
                result.Append($"{item.Key}: {item.Value}\n");

            return result.ToString();
        }

        /// <summary>
        /// 
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
                var xy = item.Key.Split('_');

                var stat = item.Value;
                if (item.Value.Correlation() > gdpAbove || item.Value.Correlation() < gdpBelow)
                    result.Append($"{ColumnConfigs[xy[0]].Name}\tvs {ColumnConfigs[xy[1]].Name}{dv}{stat.Correlation():F3}{dv}{stat.MeanX():F1}{dv}{stat.Slope():F1}\n");
            }

            return result.ToString();
        }

        public string ToReport(string dv, string filter)
        {
            Console.WriteLine($"Filter by: {filter}");
            var result = new StringBuilder($"Independent(X){dv}vs Dependent(Y){dv}Correlation{dv}MeanX{dv}Slope\n");

            foreach (var item in _stats.OrderByDescending(r => r.Value.Correlation()))
            {
                var xy = item.Key.Split('_');

                var stat = item.Value;
                if (Regex.IsMatch(item.Key, filter, RegexOptions.IgnoreCase))
                    result.Append($"{ColumnConfigs[xy[0]].Name}\tvs {ColumnConfigs[xy[1]].Name}{dv}{stat.Correlation():F3}{dv}{stat.MeanX():F1}{dv}{stat.Slope():F1}\n");
            }

            return result.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ColumnConfig
    {
        public string Short { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
    }
}
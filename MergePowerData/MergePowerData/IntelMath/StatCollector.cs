using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MergePowerData.CIAFdata;

namespace MergePowerData.IntelMath
{
    public class StatCollector
    {
        readonly IDictionary<string, Statistic> _stats = new Dictionary<string, Statistic>();

        public readonly double PLimit;

        public StatCollector(double minGdpPp)
        {
            PLimit = minGdpPp;

            _stats.Add("refinefuelcons", new Statistic());
            _stats.Add("refinefuelprod", new Statistic());
            _stats.Add("refinefuelexport", new Statistic());
            _stats.Add("refinefuelimport", new Statistic());

            _stats.Add("natgascons", new Statistic());
            _stats.Add("natgasprod", new Statistic());
            _stats.Add("natgasexport", new Statistic());
            _stats.Add("natgasimport", new Statistic());
        }

        public double CalcX(string statName, double yValue)
        {
            return yValue / _stats[statName].Slope(); //+ _stats[statName].YIntercept();
        }

        public double Stand(string statName, double xValue, double yValue)
        {
            return ((xValue - CalcX(statName, yValue)) / _stats[statName].Qx());
        }

        public void Add(Country c)
        {
            //947.3   N: 46 slope: 104.516 Yint: 540.545
            if (c.PurchasePower.value / Intel.Giga < PLimit) return;

            //     do not add aggregation country entries to stats
            if (Regex.IsMatch(c.Name, @"world|European", RegexOptions.IgnoreCase)) return;

            _stats["refinefuelcons"].Add(c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / Intel.Mega, c.PurchasePower.value / Intel.Giga);
            _stats["refinefuelprod"].Add(c.FossilFuelDetail.RefinedPetroleum.Production.Value / Intel.Mega, c.PurchasePower.value / Intel.Giga);
            _stats["refinefuelexport"].Add(c.FossilFuelDetail.RefinedPetroleum.Exports.Value / Intel.Mega, c.PurchasePower.value / Intel.Giga);
            _stats["refinefuelimport"].Add(c.FossilFuelDetail.RefinedPetroleum.Imports.Value / Intel.Mega, c.PurchasePower.value / Intel.Giga);

            _stats["natgascons"].Add(c.FossilFuelDetail.NaturalGas.Consumption.Value / Intel.Giga, c.PurchasePower.value / Intel.Giga);
            _stats["natgasprod"].Add(c.FossilFuelDetail.NaturalGas.Production.Value / Intel.Giga, c.PurchasePower.value / Intel.Giga);
            _stats["natgasexport"].Add(c.FossilFuelDetail.NaturalGas.Exports.Value / Intel.Giga, c.PurchasePower.value / Intel.Giga);
            _stats["natgasimport"].Add(c.FossilFuelDetail.NaturalGas.Imports.Value / Intel.Giga, c.PurchasePower.value / Intel.Giga);

            AddCrossTableRegressions(c);
        }

        public void AddCrossTableRegressions(Country c)
        {
            IDictionary<string, int> exclude = new Dictionary<string, int>();

            string[] x = { "eprod", "econs", "eimport", "eexport", "pcff", "pcnuke", "pchydro", "pcrenew",
                    "ffrefineprod", "ffrefinecons","ffrefineimport","ffrefineexport",
                    "ffnatgasprod","ffnatgascons","ffnatgasimport","ffnatgasexport","ffnatgasreserv",
                    "ffcrudereserv","ffcrudeprod","ffcrudeimport","ffcrudeexport",
                    "gdp", "growth",
                    "emission", "pop" };

            for (int a = 0; a < x.Length; a++)
                for (int b = 0; b < x.Length; b++)
                    if (a != b)
                    {
                        var statName = $"{x[a]}_{x[b]}";
                        var reverse = $"{x[b]}_{x[a]}";

                        if (!exclude.ContainsKey(statName))
                        {
                            var m = XValue(x[a], c);
                            var n = XValue(x[b], c);

                            if (!_stats.ContainsKey(statName))
                            {
                                _stats.Add(statName, new Statistic());
                                Console.WriteLine($"AddStat: {statName}");
                            }

                            if (!(m is double.NaN || n is double.NaN))
                                _stats[statName].Add(m, n);
                            // Console.WriteLine($"No Data for country: {c.Name} - {statName}");


                            exclude.Add(reverse, 0);
                        }
                    }
        }

        public double XValue(string name, Country c)
        {
            double result;

            var igc = c.Electric.Electricity.installed_generating_capacity;
            var elcity = c.Electric.Electricity;

            switch (name)
            {
                case "emission": result = c.Electric.TtonCo2; break;
                case "gdp": result = c.PurchasePower.value / Intel.Giga; break;
                case "growth": result = c.GrowthRate.value; break;
                case "pop": result = c.Pop / Intel.Mega; break;
                case "eprod": result = c.Electric.ProdTWh * Intel.TWh2kg; break;
                case "econs": result = c.Electric.ConsTWh * Intel.TWh2kg; break;
                case "eimport": result = elcity.imports != null ? elcity.imports.TWh * Intel.TWh2kg : double.NaN; break;
                case "eexport": result = elcity.exports != null ? elcity.exports.TWh * Intel.TWh2kg : double.NaN; break;
                case "pcff":
                    result = igc != null ? (igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.fossil_fuels.percent) * Intel.TWh2kg) : double.NaN;
                    break;
                case "pcnuke":
                    result = igc != null ? (igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.nuclear_fuels.percent) * Intel.TWh2kg) : double.NaN;
                    break;
                case "pchydro":
                    result = igc != null ? (igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.hydroelectric_plants.percent) * Intel.TWh2kg) : double.NaN;
                    break;
                case "pcrenew":
                    result = igc != null ? (igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.other_renewable_sources.percent) * Intel.TWh2kg) : double.NaN;
                    break;
                case "ffrefineprod": result = c.FossilFuelDetail.RefinedPetroleum.Production.Value / Intel.Mega; break;
                case "ffrefinecons": result = c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / Intel.Mega; break;
                case "ffrefineimport": result = c.FossilFuelDetail.RefinedPetroleum.Imports.Value / Intel.Mega; break;
                case "ffrefineexport": result = c.FossilFuelDetail.RefinedPetroleum.Exports.Value / Intel.Mega; break;

                case "ffnatgasprod": result = c.FossilFuelDetail.NaturalGas.Production.Value / Intel.Giga; break;
                case "ffnatgascons": result = c.FossilFuelDetail.NaturalGas.Consumption.Value / Intel.Giga; break;
                case "ffnatgasimport": result = c.FossilFuelDetail.NaturalGas.Imports.Value / Intel.Giga; break;
                case "ffnatgasexport": result = c.FossilFuelDetail.NaturalGas.Exports.Value / Intel.Giga; break;
                case "ffnatgasreserv": result = c.FossilFuelDetail.NaturalGas.ProvedReserves.Value / Intel.Giga; break;

                case "ffcrudereserv": result = c.FossilFuelDetail.CrudeOil.ProvedReserves.Value / Intel.Giga; break;
                case "ffcrudeprod": result = c.FossilFuelDetail.CrudeOil.Production.Value / Intel.Giga; break;
                case "ffcrudeimport": result = c.FossilFuelDetail.CrudeOil.Imports.Value / Intel.Giga; break;
                case "ffcrudeexport": result = c.FossilFuelDetail.CrudeOil.Exports.Value / Intel.Giga; break;

                default:
                    throw new ArgumentException($"Undefined {name}");

            }

            return result;

        }

        public override string ToString()
        {
            var result = new StringBuilder(base.ToString() + "\n");

            foreach (var item in _stats.OrderByDescending(r => r.Value.Correlation()))
                result.Append($"{item.Key}: {item.Value}\n");

            return result.ToString();
        }
    }
}
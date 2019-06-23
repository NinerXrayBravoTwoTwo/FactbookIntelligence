using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MergePowerData.CIAFdata;

namespace MergePowerData.Report
{
    public class StatCollector
    {
        readonly IDictionary<string, Statistic> _stats = new Dictionary<string, Statistic> { };

        public readonly double PLimit;

        public StatCollector(double minGdpPp)
        {
            PLimit = minGdpPp;

            _stats.Add("elecimport", new Statistic());
            _stats.Add("elecexport", new Statistic());
            _stats.Add("elecprod", new Statistic());
            _stats.Add("eleccons", new Statistic());

            _stats.Add("refinefuelcons", new Statistic());
            _stats.Add("refinefuelprod", new Statistic());
            _stats.Add("refinefuelexport", new Statistic());
            _stats.Add("refinefuelimport", new Statistic());

            _stats.Add("natgascons", new Statistic());
            _stats.Add("natgasprod", new Statistic());
            _stats.Add("natgasexport", new Statistic());
            _stats.Add("natgasimport", new Statistic());

            _stats.Add("prcntelecff", new Statistic());
            _stats.Add("prcntelecnuke", new Statistic());
            _stats.Add("prcntelechydro", new Statistic());
            _stats.Add("prcntelecrenew", new Statistic());

            _stats.Add("emission", new Statistic());
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

            // do not add aggregation country entries to stats
            if (Regex.IsMatch(c.Name, @"world|European", RegexOptions.IgnoreCase))
                return;

            // TODO: Can I pump in a bunch of anon methods here so I can use anon methods to define actions and make this class more generic?
            if (c.Electric.Electricity.exports != null)
            {
                _stats["elecimport"].Add(c.Electric.Electricity.imports.TWh * Intel.TWh2kg * Intel.TWh2kg,c.PurchasePower.value / Intel.Giga);
                _stats["elecexport"].Add(c.Electric.Electricity.exports.TWh * Intel.TWh2kg * Intel.TWh2kg,c.PurchasePower.value / Intel.Giga);
            }

            _stats["elecprod"].Add(c.Electric.ProdTWh * Intel.TWh2kg, c.PurchasePower.value / Intel.Giga);
            _stats["eleccons"].Add(c.Electric.ConsTWh * Intel.TWh2kg, c.PurchasePower.value / Intel.Giga);

            _stats["refinefuelcons"].Add(c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / Intel.Mega, c.PurchasePower.value / Intel.Giga);
            _stats["refinefuelprod"].Add(c.FossilFuelDetail.RefinedPetroleum.Production.Value / Intel.Mega, c.PurchasePower.value / Intel.Giga);
            _stats["refinefuelexport"].Add(c.FossilFuelDetail.RefinedPetroleum.Exports.Value / Intel.Mega, c.PurchasePower.value / Intel.Giga);
            _stats["refinefuelimport"].Add(c.FossilFuelDetail.RefinedPetroleum.Imports.Value / Intel.Mega, c.PurchasePower.value / Intel.Giga);

            _stats["natgascons"].Add(c.FossilFuelDetail.NaturalGas.Consumption.Value / Intel.Giga, c.PurchasePower.value / Intel.Giga);
            _stats["natgasprod"].Add(c.FossilFuelDetail.NaturalGas.Production.Value / Intel.Giga, c.PurchasePower.value / Intel.Giga);
            _stats["natgasexport"].Add(c.FossilFuelDetail.NaturalGas.Exports.Value / Intel.Giga, c.PurchasePower.value / Intel.Giga);
            _stats["natgasimport"].Add(c.FossilFuelDetail.NaturalGas.Imports.Value / Intel.Giga, c.PurchasePower.value / Intel.Giga);

            var igc = c.Electric.Electricity.installed_generating_capacity;

            _stats["prcntelecff"].Add(igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.fossil_fuels.percent) *Intel.TWh2kg, c.PurchasePower.value / Intel.Giga);
            _stats["prcntelecnuke"].Add(igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.nuclear_fuels.percent), c.PurchasePower.value / Intel.Giga);
            _stats["prcntelechydro"].Add(igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.hydroelectric_plants.percent), c.PurchasePower.value / Intel.Giga);
            _stats["prcntelecrenew"].Add(igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.other_renewable_sources.percent), c.PurchasePower.value / Intel.Giga);

            _stats["emission"].Add(c.Electric.TtonCo2, c.PurchasePower.value / Intel.Giga);
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
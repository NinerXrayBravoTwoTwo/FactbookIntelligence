using System.Collections.Generic;
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

            _stats.Add("elecprod", new Statistic());
            _stats.Add("eleccons", new Statistic());
            _stats.Add("fuel", new Statistic());
            _stats.Add("natgas", new Statistic());
            _stats.Add("ff", new Statistic());
            _stats.Add("emission", new Statistic());
            _stats.Add("growth", new Statistic());
        }

        public double CalcX(string statName, double yValue)
        {
            return yValue / _stats[statName].Slope();
        }

        public double Stand(string statName, double xValue, double yValue)
        {
            return /**Math.Abs*/(xValue - CalcX(statName, yValue)) / _stats[statName].Qx();
        }

        public void Add(Country c)
        {
            //947.3   N: 46 slope: 104.516 Yint: 540.545
            if (c.PurchasePower.value / Intel.Giga < PLimit)
                return;

            // do not add aggregation country entries to stats
            if (Regex.IsMatch(c.Name, @"world|European", RegexOptions.IgnoreCase))
                return;

            // TODO: Can I pump in a bunch of anon methods here so I can use anon methods to define actions and make this class more generic?
            _stats["elecprod"].Add(c.Electric.ProdTWh * Intel.TWh2kg, c.PurchasePower.value / Intel.Giga);
            _stats["eleccons"].Add(c.Electric.ConsTWh * Intel.TWh2kg, c.PurchasePower.value / Intel.Giga);
            _stats["fuel"].Add(c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / Intel.Mega, c.PurchasePower.value / Intel.Giga);
            _stats["natgas"].Add(c.FossilFuelDetail.NaturalGas.Consumption.Value / Intel.Giga, c.PurchasePower.value / Intel.Giga);
            _stats["ff"].Add(c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh * Intel.TWh2kg, c.PurchasePower.value / Intel.Giga);
            _stats["emission"].Add(c.Electric.TtonCo2, c.PurchasePower.value / Intel.Giga);
            _stats["growth"].Add(c.GrowthRate.value, c.PurchasePower.value / Intel.Giga);
           // TODO:  _stats["energystd2gdpdelta"].Add( double stand = fn( find energy Qx by country.name), c.GrowthRate); // or should this be growthRate / energy Qx ?
        }

        public override string ToString()
        {
            var result = new StringBuilder(base.ToString()+"\n");

            foreach (var item in _stats)
                result.Append($"{item.Key}: {item.Value}\n");

            return result.ToString();
        }
    }
}
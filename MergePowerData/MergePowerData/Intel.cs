using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MergePowerData.CIAFdata;
using MergePowerData.Report;

namespace MergePowerData
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Intel
    {
        public const double Tera = 1.0e12;
        public const double Giga = 1.0e9;
        public const double Mega = 1.0e6;
        public const double Kilo = 1.0e3;

        protected const double kgU245perkWh = 24.0e6;

        public const double TWh2kg = 0.040055402; // TWh = 0.040055402 kg
        public static double windMillTWh = 2.0e-6 * InstalledGeneratingCapacity.HoursPerYear * 0.4;  // 2MW wind mill 40% eff

        // CIAF is a collection of data by country.  To analyze a specific set of data into a report we must extract a relevant subset 
        protected readonly List<Country> _countries = new List<Country>();
        protected StatCollector _stats = new StatCollector(1000);

        protected const double TwentyMtonTnt = 0.93106557; //
                                                           // 20 Mton_e = 0.93106557 kg, ~1 kg
        protected const double TWh2MTonTnt = 0.86042065; // TWh = 0.86042 M ton Tnt
        protected const double TWh2PJ = 3.6; // TWh = 3.6 PJ (Peta Joule's)
        protected Country _world;

        public void Add(string name, Electric electric, FossilFuelDetail ff, Gdp gdp, long pop)
        {
            if (name.Equals("World"))
                _world = new Country(name, electric, ff, gdp, pop);

            var country = new Country(name, electric, ff, gdp, pop);

            _countries.Add(country);
            _stats.Add(country);
        }

        public void CsvReport()
        {
            var dv = "\t";
            // header
            Console.WriteLine(
                //"ProdTWh{dv}"-
                $"ekg{dv}"
                + $"dev_e{dv}"
                + $"eFFkg{dv}"
                + $"dev_FF{dv}"
                + $"FuelMbl{dv}"
                + $"NatGasGcm{dv}"
                + $"Co2Tton{dv}"
                ////+ "maxFF_kWh"
                ////+ "kw/pop{dv}"
                ////+ "pop_M{dv}"
                ////+ "kWh/pop{dv}"
                ////+ "Prod_FF_TWh{dv}"
                ////+ "Capacity_TWh/y{dv}"
                ////+ "$Growth{dv}"
                + $"$G PP{dv}"
                // $"WindMillRepl{dv}"
                // + $"$PP/TWh{dv}"
                + "Country");

            foreach (var c in _countries.OrderByDescending(d => d.Electric.ProdTWh))
            {
                var kgEfossil = c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh * TWh2kg;
                var wrldKgEfossil = _world.Electric.Electricity.by_source.fossil_fuels.percent / 100 * _world.Electric.ProdTWh * TWh2kg;

                var igc = c.Electric.Electricity.installed_generating_capacity;
                if (igc == null) continue;

                var currentCap = igc.YearCapacityTWhr;
                var futureCap50Y = igc.YearCapacityTWhr * 2;

                if (c.PurchasePower.value / Giga < _stats.PLimit)
                    continue;
                
                // Understanding Country wealth relative to use of FF and electricity.
                Console.WriteLine(
                    // $"{c.Electric.ProdTWh}{dv}"
                    $"{c.Electric.ProdTWh * TWh2kg:F1}{dv}"
                    //+ $"{_stats.Equation("elecprod", c.Electric.ProdTWh)}{dv}"
//                    + $"{_stats.CalcX("elecprod", c.PurchasePower.value / Giga):F3}\t"
                    + $"{_stats.Stand("elecprod", c.Electric.ProdTWh * TWh2kg, c.PurchasePower.value/Giga):F3}{dv}"
                    + $"{c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh * TWh2kg:F1}{dv}"
                    + $"{_stats.Stand("ff", c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh * Intel.TWh2kg, c.PurchasePower.value / Giga):F3}{dv}"
//                    + $"{c.Electric.Electricity.by_source.nuclear_fuels.percent / 100 * c.Electric.ProdKWh / kgU245perkWh * 1.6:F1}{dv}" // 1.6 is power xfer loss estimate
                    + $"{c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / Mega:F2}{dv}"
                    + $"{c.FossilFuelDetail.NaturalGas.Consumption.Value / Giga:F1}{dv}"
                    //+ $"{c.Pop / 1e6:F0}{dv}"
                    //+ $"{c.Electric.ProdKWh / c.Pop:F0}{dv}"
                    //+ $"{c.Electric.ProdTWh / _world.Electric.ProdTWh:F3}{dv}"
                    //+ $"{kgEfossil / wrldKgEfossil:F3}{dv}"
                    //$"{c.Electric.MtonCo2 / c.Pop:F2}{dv}"
                    //$"{c.Electric.TtonCo2 / _world.Electric.TtonCo2:F3}{dv}" // want amtCo2 per TW consumed Fossil
                    //  Emissions Co2
                    + $"{c.Electric.TtonCo2:F0}{dv}"
                    //+ $"{c.Electric.Electricity.by_source.nuclear_fuels.percent}{dv}"
                    //+ $"{c.Electric.Electricity.by_source.hydroelectric_plants.percent}{dv}"
                    //+ $"{c.Electric.Electricity.by_source.fossil_fuels.percent}{dv}"
                    //+ $"{c.Electric.Electricity.by_source.other_renewable_sources.percent}{dv}"
                    //+ $"{igc.YearCapacityTWhr:F0}{dv}"
                    //+ $"{igc.YearCapacityTWhr / c.Electric.ProdTWh:F2}{dv}"
                    + $"{c.PurchasePower.value / Giga:F0}{dv}"
                    //+ $"{c.GrowthRate.value:F1}{dv}"
                    //+ $"{c.GrowthRate.date}{dv}"
                    //+ $"{c.PurchasePower.value / _world.PurchasePower.value:F3}{dv}"
                    //+ $"{c.PurchasePower.value}{dv}"
                    //+ $"{c.PurchasePower.value / c.Electric.ProdTWh:F2}{dv}"
                    //+ $"{c.FossilFuelDetail.RefinedPetroleum.Consumption.Value:F0}{dv}"
                    // + $"{c.FossilFuelDetail.RefinedPetroleum.Production.Value:F0}{dv}"
                    //+ $"{c.FossilFuelDetail.CrudeOil.Exports.Value / c.FossilFuelDetail.CrudeOil.Production.Value:F2}{dv}"
                    //+ $"{c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / _world.FossilFuelDetail.RefinedPetroleum.Consumption.Value:F5}{dv}"
                    //+$"{c.FossilFuelDetail.RefinedPetroleum.Consumption.Date}{dv}"
                    // + $"{c.Electric.ProdTWh / windMillTWh:F0}\t"
                    + $"{c.Name}");
            }

            Console.WriteLine(_stats.ToString());
        }

        public void PdfReport()
        {
            var pdf = new PowerPdf(_world, _countries);

            var path = Environment.CurrentDirectory;

            var stream = new FileStream(path + "/EnergyUseReport.pdf", FileMode.Create);

            pdf.Create(stream);

            Process.Start(path + "/EnergyUseReport.pdf");
        }
    }

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
            return (xValue - CalcX(statName, yValue)) / _stats[statName].Qx();
        }

        public void Add(Country c)
        {
            //947.3   N: 46 slope: 104.516 Yint: 540.545
            if (c.PurchasePower.value / Intel.Giga < PLimit)
                return;

            // do not add aggregation country entries to stats
            if (Regex.IsMatch(c.Name, @"world|European", RegexOptions.IgnoreCase))
                return;

            _stats["elecprod"].Add(c.Electric.ProdTWh * Intel.TWh2kg, c.PurchasePower.value / Intel.Giga);
            _stats["eleccons"].Add(c.Electric.ConsTWh * Intel.TWh2kg, c.PurchasePower.value / Intel.Giga);
            _stats["fuel"].Add(c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / Intel.Mega, c.PurchasePower.value / Intel.Giga);
            _stats["natgas"].Add(c.FossilFuelDetail.NaturalGas.Consumption.Value / Intel.Giga, c.PurchasePower.value / Intel.Giga);
            _stats["ff"].Add(c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh * Intel.TWh2kg, c.PurchasePower.value / Intel.Giga);
            _stats["emission"].Add(c.Electric.TtonCo2, c.PurchasePower.value / Intel.Giga);
            _stats["growth"].Add(c.GrowthRate.value, c.PurchasePower.value / Intel.Giga);
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
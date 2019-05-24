using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MergePowerData.CIAFData;

namespace MergePowerData
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Intel
    {
        private const double TwentyMtonTnt = 0.93106557; // 20 Mton_e = 0.93106557 kg, ~1 kg
        private const double TWh2kg = 0.040055402; // TWh = 0.040055402 kg
        private const double TWh2MTonTnt = 0.86042065; // TWh = 0.86042 M ton Tnt
        private const double TWh2PJ = 3.6; // TWh = 3.6 PJ (Peta Joule's)
        private Country _world;

        // CIAF is a collection of data by country.  To analyze a specific set of data into a report we must extract a relevant subset 
        private readonly List<Country> _data = new List<Country>();

        public void Add(string name, Electric electric, Gdp gdp, long pop)
        {
            if (name.Equals("World"))
                _world = new Country(name, electric, gdp, pop);

            _data.Add(new Country(name, electric, gdp, pop));
        }

        public void CsvReport()
        {
            // header
            Console.WriteLine(
                "ProdTWh\t"
                + "kg\t"
                + "max%FF\t"
                //+ "maxFF_kWh"
                //+ "kw/pop\t"
                //+ "pop_M\t"
                //+ "kWh/pop\t"
                //+ "Prod_FF_TWh/y\t"
                //+ "Capacity_TWh/y\t"
                + "$Growth\t"
                + "$PP\t"
                + "$PP/TWh\t"
                + "Country");

            foreach (var c in _data.OrderByDescending(d => d.Electric.ProdTWh))
            {
                var kgEfossil = c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh *
                                TWh2kg;
                var wrldKgEfossil = _world.Electric.Electricity.by_source.fossil_fuels.percent / 100 *
                                    _world.Electric.ProdTWh * TWh2kg;

                var igc = c.Electric.Electricity.installed_generating_capacity;

                if (igc == null) continue;

                var currentCap = igc.YearCapacityTWhr();
                var futureCap50Y = igc.YearCapacityTWhr() * 2;

                // Note; Working on making sense of Economics relative to use of FF and electricity.
                // Note; Need to bring in oil usage numbers
                Console.WriteLine(
                    $"{c.Electric.ProdTWh}\t"
                    + $"{c.Electric.ProdTWh * TWh2kg:F1}\t"
                    //+ $"{c.Pop / 1e6:F0}\t"
                    //+ $"{c.Electric.ProdKWh / c.Pop:F0}\t"
                    //+ $"{c.Electric.ProdTWh / _world.Electric.ProdTWh:F3}\t"
                    //+ $"{kgEfossil / wrldKgEfossil:F3}\t"
                    //+ $"{c.Electric.TtonCo2 / _world.Electric.TtonCo2:F3}\t" // want amtCo2 per TW consumed Fossil
                    //+ $"{c.Electric.Electricity.by_source.nuclear_fuels.percent}\t"
                    //+ $"{c.Electric.Electricity.by_source.hydroelectric_plants.percent}\t"
                    + $"{c.Electric.Electricity.by_source.fossil_fuels.percent}\t"
                    //+ $"{c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh:F0}\t"
                    //+ $"{c.Electric.Electricity.by_source.other_renewable_sources.percent}\t"
                    //+ $"{igc.YearCapacityTWhr():F0}\t"
                    //+ $"{igc.YearCapacityTWhr() / c.Electric.ProdTWh:F2}\t"
                    + $"{c.GrowthRate.value:F1}\t"
                    //+ $"{c.PurchasePower.value / _world.PurchasePower.value:F3}\t"
                    + $"{c.PurchasePower.value}\t"
                    + $"{c.PurchasePower.value / c.Electric.ProdTWh:F2}\t"
                    + $"{c.Name}");
            }
        }

        public void PdfReport()
        {
            var pdf = new PowerPdf(_world, _data);
        }
    }

}
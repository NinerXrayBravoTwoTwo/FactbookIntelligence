﻿
using System;

using System.Collections.Generic;
using System.Linq;

namespace MergePowerData
{
    public class Intel
    {
        private const double TwentyMtonTnt = 0.93106557; // 20 Mton_e = 0.93106557 kg, ~1 kg
        private const double TWh2kg = 0.040055402; // TWh = 0.040055402 kg
        private const double TWh2MTonTnt = 0.86042065; // TWh = 0.86042 M ton Tnt
        private const double TWh2PJ = 3.6; // TWh = 3.6 PJ (Peta Joule's)

        private List<Country> data = new List<Country>();
        private Country _world;

        public void Add(string name, Electric electric, AnnualValue3 purchasingPower, long pop)
        {
            if (name.Equals("World"))
                _world = new Country(name, electric, purchasingPower, pop);


            data.Add(new Country(name, electric, purchasingPower, pop));
        }

        internal void Report()
        {
            foreach (var c in data.OrderByDescending(d => d.electric.ProdTWh))
            {
                var kgEfossil = c.electric.Electricity.by_source.fossil_fuels.percent / 100 * c.electric.ProdTWh * TWh2kg;
                var wrldKgEfossil = _world.electric.Electricity.by_source.fossil_fuels.percent / 100 * _world.electric.ProdTWh * TWh2kg;

                var igc = c.electric.Electricity.installed_generating_capacity;

                if (igc == null) continue;

                var currentCap = igc.YearCapacityTWhr();
                var futureCap50y = igc.YearCapacityTWhr() * 2;

                Console.WriteLine(
                $"{c.electric.ProdTWh}\t"
                + $"{c.electric.ProdTWh * TWh2kg:F1}\t"
                + $"{c.pop / 1e6:F0}\t"
                + $"{c.electric.ProdKWh / c.pop:F0}\t"
                + $"{c.electric.ProdTWh / _world.electric.ProdTWh:F3}\t"
                + $"{kgEfossil / wrldKgEfossil:F3}\t"
                + $"{c.electric.TtonCo2 / _world.electric.TtonCo2:F3}\t" // want amtCo2 per TW consumed Fossil
                                                                         //+ $"{growthRate.value:F1}\t"
                                                                         //+ $"{purchasingPower.value / worldPP.value:F3}\t"
                + $"{c.electric.Electricity.by_source.nuclear_fuels.percent}\t"
                + $"{c.electric.Electricity.by_source.hydroelectric_plants.percent}\t"
                + $"{c.electric.Electricity.by_source.fossil_fuels.percent}%\t"
                + $"{c.electric.Electricity.by_source.fossil_fuels.percent / 100 * c.electric.ProdTWh:F0}\t"
                + $"{c.electric.Electricity.by_source.other_renewable_sources.percent}\t"
                + $"{igc.YearCapacityTWhr():F0}\t"
                + $"{igc.YearCapacityTWhr() / c.electric.ProdTWh:F2}\t"
                + $"{c.electric.ProdTWh / _world.electric.ProdTWh:F3}\t"
                + $"{kgEfossil / wrldKgEfossil:F3}\t"
                + $"{c.electric.TtonCo2 / _world.electric.TtonCo2:F3}\t" // want amtCo2 per TW consumed Fossil
                                                                         //+ $"{growthRate.value:F1}\t"
                                                                         //+ $"{purchasingPower.value / worldPP.value:F3}\t"
                + $"{c.name}");
            }
        }
    }

    internal class Country
    {
        public readonly string name;
        public readonly Electric electric;
        public readonly AnnualValue3 purchasingPower;
        public readonly long pop;

        public Country(string name, Electric electric, AnnualValue3 purchasingPower, long pop)
        {
            this.name = name;
            this.electric = electric;
            this.purchasingPower = purchasingPower;
            this.pop = pop;
        }
    }
}

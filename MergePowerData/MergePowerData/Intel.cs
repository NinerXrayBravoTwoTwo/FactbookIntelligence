
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

        public void Add(string name, Electric electric, Gdp gdp, long pop)
        {
            if (name.Equals("World"))
                _world = new Country(name, electric, gdp, pop);

            data.Add(new Country(name, electric, gdp, pop));
        }

        internal void Report()
        {
            // header
            Console.WriteLine(
            "ProdTWh\t"
            + "kg_e\t"
            //+ "pop_M\t"
            //+ "kWh/pop\t"
            + "Prod_FF_TWh/y\t"
            + "Capacity_TWh/y\t"
            + "Country");

            foreach (var c in data.OrderByDescending(d => d.Electric.ProdTWh))
            {
                var kgEfossil = c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh * TWh2kg;
                var wrldKgEfossil = _world.Electric.Electricity.by_source.fossil_fuels.percent / 100 * _world.Electric.ProdTWh * TWh2kg;

                var igc = c.Electric.Electricity.installed_generating_capacity;

                if (igc == null) continue;

                var currentCap = igc.YearCapacityTWhr();
                var futureCap50Y = igc.YearCapacityTWhr() * 2;

                Console.WriteLine(
                $"{c.Electric.ProdTWh}\t"
                + $"{c.Electric.ProdTWh * TWh2kg:F1}\t"
                + $"{c.Pop / 1e6:F0}\t"
                + $"{c.Electric.ProdKWh / c.Pop:F0}\t"
                + $"{c.Electric.ProdTWh / _world.Electric.ProdTWh:F3}\t"
                + $"{kgEfossil / wrldKgEfossil:F3}\t"
                + $"{c.Electric.TtonCo2 / _world.Electric.TtonCo2:F3}\t" // want amtCo2 per TW consumed Fossil
                + $"{c.GrowthRate.value:F1}\t"
                + $"{c.PurchasePower.value / _world.PurchasePower.value:F3}\t"
                + $"{c.Electric.Electricity.by_source.nuclear_fuels.percent}\t"
                + $"{c.Electric.Electricity.by_source.hydroelectric_plants.percent}\t"
                + $"{c.Electric.Electricity.by_source.fossil_fuels.percent}%\t"
                + $"{c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh:F0}\t"
                + $"{c.Electric.Electricity.by_source.other_renewable_sources.percent}\t"
                + $"{igc.YearCapacityTWhr():F0}\t"
                + $"{igc.YearCapacityTWhr() / c.Electric.ProdTWh:F2}\t"
                + $"{c.Name}");
            }
        }
    }

    internal class Country
    {
        public readonly string Name;
        public readonly Electric Electric;
        public readonly Gdp Gdp;
        public readonly long Pop;
        public readonly AnnualValue2 GrowthRate;
        public readonly AnnualValue3 PurchasePower;

        public Country(string name, Electric electric, Gdp gdp, long pop)
        {
            this.Name = name;
            this.Electric = electric;
            this.Gdp = gdp;
            this.Pop = pop;

            this.PurchasePower = EconBase(gdp, out GrowthRate);
        }

        /// <summary>
        /// </summary>
        /// <param name="gdp"></param>
        /// <param name="growthRate"></param>
        /// <returns></returns>
        private static AnnualValue3 EconBase(Gdp gdp, out AnnualValue2 growthRate)
        {
            var gr = gdp.real_growth_rate ?? new RealGrowthRate();

            if (gr.annual_values == null)
                gr.annual_values = new List<AnnualValue2>
                    {new AnnualValue2 {date = "2000", units = "USD", value = double.NaN}};

            growthRate = gr.annual_values.OrderBy(av => av.date).LastOrDefault();

            AnnualValue3 purchasingPower;
            if (gdp.per_capita_purchasing_power_parity == null)
            {
                purchasingPower = new AnnualValue3 { date = "2000", units = "USD", value = double.NaN };
            }
            else
            {
                var pp = gdp.per_capita_purchasing_power_parity.annual_values;

                purchasingPower = pp != null ? pp.OrderBy(p => p.date).LastOrDefault() : new AnnualValue3 { date = "1990", value = 0, units = "USD" };
            }

            return purchasingPower;
        }
    }

}

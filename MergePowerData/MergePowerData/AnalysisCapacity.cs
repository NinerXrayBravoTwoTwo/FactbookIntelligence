using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MergePowerData
{
    internal class AnalysisCapacity
    {
        private const double TwentyMtonTnt = 0.93106557; // 20 Mton_e = 0.93106557 kg, ~1 kg
        private const double TWh2kg = 0.040055402; // TWh = 0.040055402 kg
        private const double TWh2MTonTnt = 0.86042065; // TWh = 0.86042 M ton Tnt
        private const double TWh2PJ = 3.6; // TWh = 3.6 PJ (Peta Joule's)

        private readonly JObject _fact;


        public AnalysisCapacity(string fileName)
        {
            _fact = JObject.Parse(File.ReadAllText(fileName));
        }

        public void ElectricReport()
        {
            Console.WriteLine(
                "ProdTWh\t"
                //+ "kg_e\t"
                //+ "pop_M\t"
                //+ "kWh/pop\t"
                + "Prod_FF_TWh/y\t"
                + "Capacity_TWh/y\t"
                + "Country");

            Electric eWorld = null;
            AnnualValue2 worldGR = null;
            AnnualValue3 worldPP = null;
            InstalledGeneratingCapacity worldIgc = null;

            var countries =
                _fact.SelectTokens(
                    ".countries$..$.data$"); //["data"];//.Where(t => t.SelectToken("name").Value<string>().Equals("World"));

            //foreach (var country in _fact.SelectTokens("countries$..$data"))
            foreach (var item in _fact["countries"])
            foreach (var country in item)
            {
                var data = country["data"];
                var name = data["name"].Value<string>();
                //var name2 = data.SelectToken("name").Value<string>();


                var energy = data["energy"];

                // population
                var pop = Population(data);

                if (energy == null) continue;

                var e = energy["electricity"] != null ? new Electric(energy) : null;

                var gdp = Gdp(data);

                AnnualValue2 growthRate;
                var purchasingPower = EconBase(gdp, out growthRate);

                var igc = e?.Electricity.installed_generating_capacity;

                if (igc == null) continue;

                if (name.Equals("World"))
                {
                    eWorld = e;
                    worldGR = growthRate;
                    worldPP = purchasingPower;
                    worldIgc = igc;
                }

                var kgEfossil = e.Electricity.by_source.fossil_fuels.percent / 100 * e.ProdTWh * TWh2kg;
                var wrldKgEfossil = eWorld.Electricity.by_source.fossil_fuels.percent / 100 * eWorld.ProdTWh * TWh2kg;

                var currentCap = igc.YearCapacityTWhr();
                var futureCap50y = igc.YearCapacityTWhr() * 2;


                Console.WriteLine(
                    $"{e.ProdTWh}\t"
                    + $"{e.ProdTWh * TWh2kg:F1}\t"
                    //+ $"{pop / 1e6:F0}\t"
                    //+ $"{e.ProdKWh / pop:F0}\t"
                    //$"{e.ProdTWh / eWorld.ProdTWh:F3}\t"
                    //+ $"{kgEfossil / wrldKgEfossil:F3}\t"
                    // $"{e.TtonCo2 / eWorld.TtonCo2:F3}\t" // want amtCo2 per TW consumed Fossil
                    //+ $"{growthRate.value:F1}\t"
                    //+ $"{purchasingPower.value / worldPP.value:F3}\t"
                    //+ $"{e.Electricity.by_source.nuclear_fuels.percent}\t"
                    //+ $"{e.Electricity.by_source.hydroelectric_plants.percent}\t"
                    // + $"{e.Electricity.by_source.fossil_fuels.percent}%\t"
                    + $"{e.Electricity.by_source.fossil_fuels.percent / 100 * e.ProdTWh:F0}\t"
                    //+ $"{e.Electricity.by_source.other_renewable_sources.percent}\t"
                    + $"{igc.YearCapacityTWhr():F0}\t"
                    //+ $"{igc.YearCapacityTWhr() / e.ProdTWh:F2}\t"
                    //$"{e.ProdTWh / eWorld.ProdTWh:F3}\t"
                    //+ $"{kgEfossil / wrldKgEfossil:F3}\t"
                    // $"{e.TtonCo2 / eWorld.TtonCo2:F3}\t" // want amtCo2 per TW consumed Fossil
                    //+ $"{growthRate.value:F1}\t"
                    //+ $"{purchasingPower.value / worldPP.value:F3}\t"
                    + $"{name}");
            }
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
                purchasingPower = new AnnualValue3 {date = "2000", units = "USD", value = double.NaN};
            }
            else
            {
                var pp = gdp.per_capita_purchasing_power_parity.annual_values;

                if (pp != null)
                    purchasingPower = pp.OrderBy(p => p.date).LastOrDefault();
                else
                    purchasingPower = new AnnualValue3 {date = "1990", value = 0, units = "USD"};
            }

            return purchasingPower;
        }

        // People
        /// <summary>
        ///     Population data -> people
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static long Population(JToken data)
        {
            return data["people"]?["population"]["total"].Value<long>() ?? 0;
        }

        /// <summary>
        ///     Gross domestic product
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static Gdp Gdp(JToken data)
        {
            var gdp = data["economy"]["gdp"];
            return gdp != null ? JsonConvert.DeserializeObject<Gdp>(gdp.ToString()) : null;
        }
    }
}
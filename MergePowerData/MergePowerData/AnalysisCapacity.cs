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
        private readonly JObject _fact;
        
        public AnalysisCapacity(string fileName)
        {
            _fact = JObject.Parse(File.ReadAllText(fileName));
        }

        public void ElectricReport()
        {
            Console.WriteLine(
                "ProdTWh\t"
                + "kg_e\t"
                //+ "pop_M\t"
                //+ "kWh/pop\t"
                + "Prod_FF_TWh/y\t"
                + "Capacity_TWh/y\t"
                + "Country");

            var Intel = new Intel();

            foreach (var item in _fact["countries"])
                foreach (var country in item)
                {
                    var data = country["data"];
                    var name = data["name"].Value<string>();
                    var energy = data["energy"];
                    
                    if (energy == null) continue;

                    var electric = energy["electricity"] != null ? new Electric(energy) : null;
                    if (electric == null) continue;

                    //var purchasingPower = EconBase(Gdp(data), out var growthRate);

                    Intel.Add(name, electric, Gdp(data), Population(data));
                }

            Intel.Report();
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
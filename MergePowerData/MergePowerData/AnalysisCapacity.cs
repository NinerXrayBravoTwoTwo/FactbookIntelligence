﻿using System;
using System.IO;
using MergePowerData.CIAFdata;
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
            var intel = new Intel();

            foreach (var item in _fact["countries"])
                foreach (var country in item)
                {
                    var data = country["data"];
                    var name = data["name"].Value<string>();
                    var energy = data["energy"];

                    var electric = energy?["electricity"] != null ? new Electric(energy) : null;

                    if (electric == null) continue;

                    var ff = new FossilFuelDetail(energy);

                  // Console.WriteLine(value: $"{ff.CrudeOil.Production.Value}  {ff.CrudeOil.Exports.Value} ");

                    intel.Add(name, electric, ff, Gdp(data), Population(data));
                }

            intel.CsvReport();
        }

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
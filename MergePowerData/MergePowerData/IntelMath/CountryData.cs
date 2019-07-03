using System.IO;
using System.Collections.Generic;
using MergePowerData.CIAFdata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace MergePowerData.IntelMath
{
    public class CountryData
    {
        private readonly JObject _fact;

        public readonly List<Country> Countries = new List<Country>();
        public Country World { get; protected set; }
        protected Statistic _energyDeviationRelativeToGrowth;
        protected Statistic _ffDevRelativeToGrowth;
        public StatCollector Stats = new StatCollector();

        public readonly double GdpMinimum;
        public readonly double GdpMaximum;

        public CountryData(string fileName, double gdpLower, double gdpUpper = double.NaN)
        {
            _fact = JObject.Parse(File.ReadAllText(fileName));
            GdpMinimum = gdpLower;
            GdpMaximum = gdpUpper;

            foreach (var item in _fact["countries"])
                foreach (var country in item)
                {
                    var data = country["data"];
                    var name = data["name"].Value<string>();
                    var energy = data["energy"];

                    var electric = energy?["electricity"] != null ? new Electric(energy) : null;

                    if (electric == null) continue;

                    var ff = new FossilFuelDetail(energy);

                    Add(name, electric, ff, Gdp(data), Population(data));
                }
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

        /// <summary>
        ///     Countries added here will be part of report.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="electric"></param>
        /// <param name="ff"></param>
        /// <param name="gdp"></param>
        /// <param name="pop"></param>
        public void Add(string name, Electric electric, FossilFuelDetail ff, Gdp gdp, long pop)
        {
            if (name.Equals("World"))
                World = new Country(name, electric, ff, gdp, pop);

            var country = new Country(name, electric, ff, gdp, pop);

            var gdpGiga = country.PurchasePower.value / IntelCore.Giga;
            if (!double.IsNaN(GdpMaximum)
                &&
                gdpGiga < GdpMinimum || gdpGiga >= GdpMaximum)
                return;
            else
                if (gdpGiga < GdpMinimum) return;
                       
            Countries.Add(country);
            Stats.Add(country);
        }

    }
}

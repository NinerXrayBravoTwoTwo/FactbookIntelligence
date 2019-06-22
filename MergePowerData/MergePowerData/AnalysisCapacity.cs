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
        /// <summary>
        /// Produce report
        /// </summary>
        /// <param name="grossDomesticProductLowerLimit"></param>
        public void ElectricReport( double grossDomesticProductLowerLimit)
        {
            /* * IMPORTANT; Key variable that limits how many countries are processed in Linear regressions AND included in report **/

            var intel = new Intel(grossDomesticProductLowerLimit); 


            foreach (var item in _fact["countries"])
            foreach (var country in item)
            {
                var data = country["data"];
                var name = data["name"].Value<string>();
                var energy = data["energy"];

                var electric = energy?["electricity"] != null ? new Electric(energy) : null;

                if (electric == null) continue;

                var ff = new FossilFuelDetail(energy);

                intel.Add(name, electric, ff, Gdp(data), Population(data));
            }

            intel.CsvReport();
            intel.PdfReport();
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
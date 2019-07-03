using System.IO;
using Newtonsoft.Json.Linq;
using MergePowerData.IntelMath;

namespace MergePowerData
{
    internal class AnalysisCapacity
    {
        private readonly JObject _fact;
        private readonly string _fileName;

        public AnalysisCapacity(string fileName)
        {
            _fileName = fileName;
            _fact = JObject.Parse(File.ReadAllText(fileName));
        }
        /// <summary>
        /// Produce report
        /// </summary>
        /// <param name="grossDomesticProductLowerLimit"></param>
        /// <param name="filter">Regular expression match string to identify linear regression report targets</param>
        public void ElectricReport(string filter, double gdpLower, double gdpUpper= double.NaN)
        {
            /** IMPORTANT; Key variable that limits how many countries are processed in Linear regressions AND included in report **/

            var data = new CountryData(_fileName, gdpLower, gdpUpper);
            var intel = new Intel(data,  filter);

            intel.CsvReport();
        }

    }
}
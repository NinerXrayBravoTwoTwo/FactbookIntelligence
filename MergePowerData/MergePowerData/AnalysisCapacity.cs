using MergePowerData.IntelMath;
using Newtonsoft.Json.Linq;
using System.IO;

namespace MergePowerData
{
    internal class AnalysisCapacity
    {
        private readonly string _fileName;

        public AnalysisCapacity(string fileName)
        {
            _fileName = fileName;
            JObject.Parse(File.ReadAllText(fileName));
        }

        /// <summary>
        /// Produce report
        /// </summary>
        /// <param name="filter">Regular expression match string to identify linear regression report targets</param>
        /// <param name="gdpLower"></param>
        /// <param name="gdpUpper"></param>
        public void ElectricReport(string filter, double gdpLower, double gdpUpper = double.NaN)
        {
            /* IMPORTANT; Key variable that limits how many countries are processed in Linear regressions AND included in report */

            var data = new CountryData(_fileName, gdpLower, gdpUpper);
            var intel = new Intel(data, filter);

            intel.CsvReport();
        }
    }
}
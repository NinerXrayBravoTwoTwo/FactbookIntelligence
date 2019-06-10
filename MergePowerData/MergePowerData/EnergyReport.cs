using System;
using System.IO;
using MergePowerData.CIAFdata;
using Newtonsoft.Json.Linq;

namespace MergePowerData
{
    // Obsolete  - May 24, 2019, represents simplest report
    internal class EnergyReport
    {
        // ReSharper disable once InconsistentNaming
        private const double TWh2kg = 0.040055402;
        private readonly JObject _fact;

        public EnergyReport(string fileName)
        {
            _fact = JObject.Parse(File.ReadAllText(fileName));
        }

        public void ElectricReport()
        {
            Console.WriteLine(
                "Country\tPopulation\tProduction TWh\tConsumption TWh\tKW per person\tkg Prod\tco2\tkWnuke\tkWhydro\tkWfossil\tkWrenew");

            foreach (var item in _fact["countries"])
            foreach (var country in item)
            {
                var data = country["data"];
                var name = data["name"].Value<string>();

                var people = data["people"];

                long pop = 0;
                if (people != null)
                    pop = data["people"]["population"]["total"].Value<long>();

                var energy = data["energy"];

                var e = energy?["electricity"] != null ? new Electric(energy) : null;

                if (e == null) continue;

                Console.WriteLine(
                    $"{name}\t{pop}\t{e.ProdTWh}\t{e.ConsTWh}\t{e.ProdKWh / pop}\t{e.ProdTWh * TWh2kg}\t{e.TtonCo2}\t{e.KWnuke}\t{e.KWhydro}\t{e.KWfossil}\t{e.KWrenew}");
            }
        }
    }
}
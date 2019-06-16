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
            var dv = "\t";
            Console.WriteLine(
               $"Country{dv}Population{dv}Production TWh{dv}Consumption TWh{dv}KW per person{dv}kg Prod{dv}co2{dv}kWnuke{dv}kWhydro{dv}kWfossil{dv}kWrenew");

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
                    $"{name}{dv}{pop}{dv}{e.ProdTWh}{dv}{e.ConsTWh}{dv}{e.ProdKWh / pop}{dv}{e.ProdTWh * TWh2kg}{dv}{e.TtonCo2}{dv}{e.KWnuke}{dv}{e.KWhydro}{dv}{e.KWfossil}{dv}{e.KWrenew}");
            }
        }
    }
}
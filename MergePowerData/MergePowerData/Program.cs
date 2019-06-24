using System;
using System.Text.RegularExpressions;

namespace MergePowerData
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Currently getting factbook data from Ian's Factbook project
            // https://github.com/iancoleman/cia_world_factbook_api

            double gdpLowerLimit = 1000;
            if (args.Length > 0)
            {
                var argString = string.Join(" ", args);
                var match = Regex.Match(argString, @"\s[-]*(mingdp|gdp|pp|dollar|\$)=(\d+)$", RegexOptions.IgnoreCase);

                if (match.Success)
                    gdpLowerLimit = double.Parse(match.Groups[2].Value);

                match = Regex.Match(argString, @"[-]*(help|help|h)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    Console.WriteLine("MinGdp=nn | dollar=nn | gdp=nn are all acceptable parameters.");
                    return;
                }
            }

            var report = new AnalysisCapacity("../../../../../cia_world_factbook_api/data/" + "factbook.json");
            report.ElectricReport(gdpLowerLimit);  // GDP in Billion $ ( Giga $)
        }
    }
}
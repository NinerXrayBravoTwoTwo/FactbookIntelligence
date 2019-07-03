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
            double gdpUpperLimit = double.NaN;
            string filter = string.Empty;

            if (args.Length > 0)
            {
                var argString = string.Join(" ", args);

                //
                var match = Regex.Match(argString, @"[-]*(mingdp|gdp\-)=(\d+)", RegexOptions.IgnoreCase);
                if (match.Success) gdpLowerLimit = double.Parse(match.Groups[2].Value);

                //
                 match = Regex.Match(argString, @"[-]*(maxgdp|gdp\+)=(\d+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    gdpUpperLimit = double.Parse(match.Groups[2].Value);
                    if (gdpUpperLimit <= gdpLowerLimit)
                    {
                        Console.WriteLine("Error; Max gdp must be greater than Min gdp.");
                        return;
                    }
                }


                //
                match = Regex.Match(argString, @"[-]*(filter)=([\w|\|]+)", RegexOptions.IgnoreCase);
                if (match.Success) filter = match.Groups[2].Value;

                //
                match = Regex.Match(argString, @"[-]*(help)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    Console.WriteLine("minGdp=nn | maxGdp=nn | filter=string; are all acceptable parameters.");
                    return;
                }
            }

            var report = new AnalysisCapacity("../../../../../cia_world_factbook_api/data/" + "factbook.json");
            report.ElectricReport(filter, gdpLowerLimit, gdpUpperLimit);  // GDP in Billion $ ( Giga $)
        }
    }
}
using System;
using MergePowerData.IntelMath;
using System.Linq;
using System.Collections.Generic;
using MergePowerData.CIAFdata;

namespace MergePowerData
{
    internal class ElectricSourceUtilizationAdjustment
    {
        public ElectricSourceUtilizationAdjustment(Dictionary<string, Statistic> stats, List<Country> countries)
        {
            /*
                eutilization_pctcapnuclear:     Electric Utilization    vs Nuclear Capacity     0.294   0.4     0.42883 n%/u%
                eutilization_pctcapfossil:      Electric Utilization    vs Fossil Capacity      0.149   0.4     0.44520 f%/u%
                eutilization_pctcaphydro:       Electric Utilization    vs Hydro Capacity       0.050   0.4     0.10781 h%/u%
                eutilization_pctcaprenew:       Electric Utilization    vs Renewable Capacity  -0.543   0.4    -0.96077 r%/u%
             */

            // Estimate Utilization

            foreach (var item in stats.OrderBy(k => k.Key))
            {
                Console.WriteLine($"{item.Key}:\t{item.Value.Slope():F4}\n{item.Value.ToString()}");
            }

            var target = countries.FirstOrDefault<Country>(c => c.Name.Equals("United States"));
            if (target is null)
            {
                Console.WriteLine("United States NOT found in country set");
                return;
            }

            var source = target.Electric.Electricity.by_source;

            Console.WriteLine(
                $"{target.Name} PowerTWh: {IntelCore.XValue("eprodtwh", target):F3} Utilization%: {IntelCore.XValue("eutilization", target):F3} "
                + $"f: {IntelCore.XValue("pctcapfossil", target)} "
                + $"h: {IntelCore.XValue("pctcaphydro", target)} "
                + $"n: {IntelCore.XValue("pctcapnuclear", target)} "
                + $"r: {IntelCore.XValue("pctcaprenew", target)}"
                );

            // Utiization starts at .25 for nuclear, fossil, hydro, renew
            // sum of utilizations always = 1 (100%) just like sum of capacity's
            // Total utilization % for a country is ePowerGen total / Capacity total ==> c.Electric.ProdTWh / igc.YearCapacityTWhr (see XValue in IntelCore class)
            
            // Adjust utilization down for renew take that and split it among the other three


            // Search, calculate LR for each, take some capacity from the lowest and add that to each of the other three, repeat until cap's are equal.  
            // Keep track of how much was moved around to each one, this is the estimated utilization

            Console.WriteLine();
        }
    }
}
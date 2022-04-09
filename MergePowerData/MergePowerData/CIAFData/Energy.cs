using System;
// ReSharper disable All

/* 
 The majority of this code was written by an online JSON to C# class generator.  
 Customization added for Installed generating capacity (which is why an interface was added)
*/

namespace MergePowerData.CIAFdata
{
    public class PopulationWithoutElectricity
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class TotalElectrification
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class UrbanElectrification
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class RuralElectrification
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class Access
    {
        public PopulationWithoutElectricity population_without_electricity { get; set; }
        public TotalElectrification total_electrification { get; set; }
        public UrbanElectrification urban_electrification { get; set; }
        public RuralElectrification rural_electrification { get; set; }
        public string date { get; set; }
    }

    public class Production
    {
        public double TWh => kWh / 1.0e9;
        public long kWh { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
    }

    public class Consumption
    {
        public double TWh => kWh / 1.0e9;
        public double kWh { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
    }

    public class Exports
    {
        public double TWh => kWh / 1.0e9;
        public double kWh { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
    }

    public class Imports
    {
        public double TWh => kWh / 1.0e9;
        public long kWh { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
    }

    public class InstalledGeneratingCapacity
    {
        public static double HoursPerYear => 365.242198781 * 24;
        public double kW { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
        public double TW => kW / 1.0e9;
        public double YearCapacityTWhr => HoursPerYear * TW;
        /// <summary>
        /// Calculate correct percentage, handeling 0 to 1 or values 0 to 100, and setting NaN input to zero.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        public double YearCapTWhrByPercent(double percent)
        {
            if (Math.Abs(percent) < 0.0000001 || double.IsNaN(percent))
                return 0.0;

            var p = percent >= 1 ? percent / 100 : percent;

            return YearCapacityTWhr * p;
        }
    }

    public interface IPercent
    {
        double KWh(InstalledGeneratingCapacity igc);
    }

    public class FossilFuels : IPercent
    {
        public double percent { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }

        public double KWh(InstalledGeneratingCapacity igc)
        {
            if (Math.Abs(percent) < 0.0000001 || double.IsNaN(percent))
                return 0.0;

            return percent / 100 * igc.kW;
        }
    }

    public class NuclearFuels : IPercent
    {
        public double percent { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }

        public double KWh(InstalledGeneratingCapacity igc)
        {
            if (Math.Abs(percent) < 0.0000001 || double.IsNaN(percent))
                return 0.0;

            return percent / 100 * igc.kW;
        }
    }

    public class HydroelectricPlants : IPercent
    {
        public double percent { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }

        public double KWh(InstalledGeneratingCapacity igc)
        {
            if (Math.Abs(percent) < 0.0000001 || double.IsNaN(percent))
                return 0.0;

            return percent / 100 * igc.kW;
        }
    }

    public class OtherRenewableSources : IPercent
    {
        public double percent { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }

        public double KWh(InstalledGeneratingCapacity igc)
        {
            if (Math.Abs(percent) < 0.0000001 || double.IsNaN(percent))
                return 0.0;

            return percent / 100 * igc.kW;
        }
    }

    public class BySource
    {
        public FossilFuels fossil_fuels { get; set; }
        public NuclearFuels nuclear_fuels { get; set; }
        public HydroelectricPlants hydroelectric_plants { get; set; }
        public OtherRenewableSources other_renewable_sources { get; set; }
    }

    public class Electricity
    {
        public Access access { get; set; }
        public Production production { get; set; }
        public Consumption consumption { get; set; }
        public Exports exports { get; set; }
        public Imports imports { get; set; }
        public InstalledGeneratingCapacity installed_generating_capacity { get; set; }
        public BySource by_source { get; set; }
    }
}
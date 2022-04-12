using MergePowerData.CIAFdata;
using System;

namespace MergePowerData.IntelMath
{
    public class PercentCapacityUtilization
    {
        private readonly InstalledGeneratingCapacity _igc;
        private readonly double _countryUtilization; // energy produced / energy installed
        private readonly EnergySource _source;

        public PercentCapacityUtilization(
            InstalledGeneratingCapacity igc, 
            double countryUtilization, // energy produced / energy installed
            EnergySource source)
        {
            _igc = igc;
            _source = source;
            _countryUtilization = countryUtilization;
        }

        public double Percent => ConvertCorrectScale(_source.percent);

        private static double ConvertCorrectScale(double percent)
        {
            double result;

            if (Math.Abs(percent) < 0.0000001 || double.IsNaN(percent))
                result = 0.0;
            else
                result = percent / 100;

            return result;
        }

        public EEnergy Capacity => new EEnergy
        {
            TeraWattHours = _igc.YearCapTWhrByPercent(Percent)
        };

        public EEnergy Utilization => new EEnergy
        {
            KiloGram = 
                Capacity.KiloGram *  _countryUtilization
        };
    }

    public class EnergySource : IPercent
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

}

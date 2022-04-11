using MergePowerData.CIAFdata;
using System;

namespace MergePowerData.IntelMath
{
    public class ElectricUtilization
    {
        private readonly Country _country;
        public readonly PercentCapacityUtilization Fossil;
        public readonly PercentCapacityUtilization HydroElectric;
        public readonly PercentCapacityUtilization Nuclear;
        public readonly PercentCapacityUtilization Renewable;

        public ElectricUtilization(Country country)
        {
            _country = country;

            Fossil = new PercentCapacityUtilization(
                InstalledCapacityYearTWhr,
                CountryUtilization,
                _country,
                _country.Electric.Electricity.by_source.fossil_fuels);

            HydroElectric = new PercentCapacityUtilization(
                InstalledCapacityYearTWhr,
                CountryUtilization,
                _country,
                _country.Electric.Electricity.by_source.hydroelectric_plants);

            Nuclear = new PercentCapacityUtilization(
                InstalledCapacityYearTWhr,
                CountryUtilization,
                _country,
                _country.Electric.Electricity.by_source.nuclear_fuels);

            Renewable = new PercentCapacityUtilization(
                InstalledCapacityYearTWhr,
                CountryUtilization,
                _country,
                _country.Electric.Electricity.by_source.other_renewable_sources);
        }

        public InstalledGeneratingCapacity InstalledCapacityYearTWhr
        {
            get
            {
                var result =
                    _country.Electric.Electricity.installed_generating_capacity
                    ?? new InstalledGeneratingCapacity
                    {
                        date = DateTime.UtcNow.ToShortDateString(),
                        global_rank = 99,
                        kW = 0
                    };

                return result;
            }
        }

        public EEnergy InstalledCapacity => new EEnergy
        {
            TeraWattHours =
            InstalledCapacityYearTWhr.YearCapacityTWhr
        };

        public EEnergy ElectricProduction => new EEnergy { TeraWattHours = _country.Electric.ProdTWh };

        public double CountryUtilization => ElectricProduction.TeraWattHours / InstalledCapacity.TeraWattHours;

        public EEnergy SumSourceCapacity =>
            new EEnergy
            {
                KiloGram = Fossil.Capacity.KiloGram
                                   + HydroElectric.Capacity.KiloGram
                                   + Nuclear.Capacity.KiloGram
                                   + Renewable.Capacity.KiloGram
            };

        public double SumSourceFraction =>
            Fossil.Percent + HydroElectric.Percent + Nuclear.Percent + Renewable.Percent;

        public EEnergy SumSourceUtilization =>   // This should equal electric production, the differences will be informative.  Equals CIA data error check
        new EEnergy
        {
            KiloGram = Fossil.Utilization.KiloGram
                       + HydroElectric.Utilization.KiloGram
                       + Nuclear.Utilization.KiloGram
                       + Renewable.Utilization.KiloGram
        };

    }
}

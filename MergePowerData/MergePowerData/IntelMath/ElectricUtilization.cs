using MergePowerData.CIAFdata;
using System;
using InstalledGeneratingCapacity = MergePowerData.CIAFdata.InstalledGeneratingCapacity;

namespace MergePowerData.IntelMath
{
    public class ElectricUtilization
    {
        public readonly PercentCapacityUtilization Fossil;
        public readonly PercentCapacityUtilization HydroElectric;
        public readonly PercentCapacityUtilization Nuclear;
        public readonly PercentCapacityUtilization Renewable;
        private readonly InstalledGeneratingCapacity _installedGeneratingCapacity;
        private readonly double _countryElectricProdTWh;

        public ElectricUtilization(Country country)
        {
            _installedGeneratingCapacity = country.Electric.Electricity.installed_generating_capacity;
            _countryElectricProdTWh = country.Electric.ProdTWh;

            Fossil = new PercentCapacityUtilization(
             InstalledCapacityYearTWhr,
             CountryUtilization,
             country.Electric.Electricity.by_source.fossil_fuels);

            HydroElectric = new PercentCapacityUtilization(
                InstalledCapacityYearTWhr,
                CountryUtilization,
                country.Electric.Electricity.by_source.hydroelectric_plants);

            Nuclear = new PercentCapacityUtilization(
                InstalledCapacityYearTWhr,
                CountryUtilization,
                country.Electric.Electricity.by_source.nuclear_fuels);

            Renewable = new PercentCapacityUtilization(
                InstalledCapacityYearTWhr,
                CountryUtilization,
                country.Electric.Electricity.by_source.other_renewable_sources);
        }

        public ElectricUtilization(
            InstalledGeneratingCapacity igc,
            double countryElectricProdTWh,
            EnergySource fossil,
            EnergySource hydro,
            EnergySource nuclear,
            EnergySource renewable)
        {
            _installedGeneratingCapacity = igc;
            _countryElectricProdTWh = countryElectricProdTWh;

            Fossil = new PercentCapacityUtilization(
                InstalledCapacityYearTWhr,
                CountryUtilization,
                fossil);

            HydroElectric = new PercentCapacityUtilization(
                InstalledCapacityYearTWhr,
                CountryUtilization,
                hydro);

            Nuclear = new PercentCapacityUtilization(
                InstalledCapacityYearTWhr,
                CountryUtilization,
                nuclear);

            Renewable = new PercentCapacityUtilization(
                InstalledCapacityYearTWhr,
                CountryUtilization,
                renewable);
        }

        public InstalledGeneratingCapacity InstalledCapacityYearTWhr
        {
            get
            {
                var result =
                   // _country.Electric.Electricity.installed_generating_capacity
                   _installedGeneratingCapacity
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

        public EEnergy ElectricProduction => new EEnergy { TeraWattHours = _countryElectricProdTWh };

        public double CountryUtilization
        {
            get => ElectricProduction.KiloGram / InstalledCapacity.KiloGram;
            set => throw new NotImplementedException();
        }

        public double SumSourceFraction =>
            Fossil.Percent
            + HydroElectric.Percent
            + Nuclear.Percent
            + Renewable.Percent;

        public EEnergy SumSourceCapacity =>
            new EEnergy
            {
                KiloGram =
                    Fossil.Capacity.KiloGram
                    + HydroElectric.Capacity.KiloGram
                    + Nuclear.Capacity.KiloGram
                    + Renewable.Capacity.KiloGram
            };


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

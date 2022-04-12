using System.Collections.Generic;
// ReSharper disable All
#pragma warning disable CS8618

namespace MergePowerData.Test
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Country>(myJsonResponse);
    public class TotalElectrification
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class Access
    {
        public object population_without_electricity { get; set; }
        public TotalElectrification total_electrification { get; set; }
        public object urban_electrification { get; set; }
        public object rural_electrification { get; set; }
        public string date { get; set; }
    }

    public class Production
    {
        public double TWh { get; set; }
        public long kWh { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
    }

    public class Consumption
    {
        public double TWh { get; set; }
        public double kWh { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
    }

    public class Exports
    {
        public double TWh { get; set; }
        public double kWh { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
    }

    public class Imports
    {
        public double TWh { get; set; }
        public long kWh { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
    }

    public class InstalledGeneratingCapacity
    {
        public double kW { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
        public double TW { get; set; }
        public double YearCapacityTWhr { get; set; }
    }

    public class FossilFuels
    {
        public double percent { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
    }

    public class NuclearFuels
    {
        public double percent { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
    }

    public class HydroelectricPlants
    {
        public double percent { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
    }

    public class OtherRenewableSources
    {
        public double percent { get; set; }
        public int global_rank { get; set; }
        public string date { get; set; }
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

    public class Electric
    {
        public double MtonCo2 { get; set; }
        public double ConsKWh { get; set; }
        public double ConsTWh { get; set; }
        public double KWfossil { get; set; }
        public double KWhydro { get; set; }
        public double KWnuke { get; set; }
        public double KWrenew { get; set; }
        public double PrcntTtl { get; set; }
        public double ProdKWh { get; set; }
        public double ProdTWh { get; set; }
        public double TtonCo2 { get; set; }
        public Electricity Electricity { get; set; }
    }

    public class Exports2
    {
        public double Value { get; set; }
        public string Date { get; set; }
    }

    public class Imports2
    {
        public double Value { get; set; }
        public string Date { get; set; }
    }

    public class Production2
    {
        public double Value { get; set; }
        public string Date { get; set; }
    }

    public class ProvedReserves
    {
        public double Value { get; set; }
        public string Date { get; set; }
    }

    public class CrudeOil
    {
        public Exports Exports { get; set; }
        public Imports Imports { get; set; }
        public Production Production { get; set; }
        public ProvedReserves ProvedReserves { get; set; }
    }

    public class Consumption2
    {
        public double Value { get; set; }
        public string Date { get; set; }
    }

    public class NaturalGas
    {
        public Consumption Consumption { get; set; }
        public Exports Exports { get; set; }
        public Imports Imports { get; set; }
        public Production Production { get; set; }
        public ProvedReserves ProvedReserves { get; set; }
    }

    public class RefinedPetroleum
    {
        public Production Production { get; set; }
        public Consumption Consumption { get; set; }
        public Exports Exports { get; set; }
        public Imports Imports { get; set; }
    }

    public class FossilFuelDetail
    {
        public CrudeOil CrudeOil { get; set; }
        public NaturalGas NaturalGas { get; set; }
        public RefinedPetroleum RefinedPetroleum { get; set; }
    }

    public class AnnualValue
    {
        public double value { get; set; }
        public string units { get; set; }
        public string date { get; set; }
    }

    public class PurchasingPowerParity
    {
        public List<AnnualValue> annual_values { get; set; }
        public int global_rank { get; set; }
        public string note { get; set; }
    }

    public class OfficialExchangeRate
    {
        public long USD { get; set; }
        public string date { get; set; }
    }

    public class RealGrowthRate
    {
        public List<AnnualValue> annual_values { get; set; }
        public int global_rank { get; set; }
    }

    public class PerCapitaPurchasingPowerParity
    {
        public List<AnnualValue> annual_values { get; set; }
        public int global_rank { get; set; }
        public string note { get; set; }
    }

    public class HouseholdConsumption
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class GovernmentConsumption
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class InvestmentInFixedCapital
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class InvestmentInInventories
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class ExportsOfGoodsAndServices
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class ImportsOfGoodsAndServices
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class EndUses
    {
        public HouseholdConsumption household_consumption { get; set; }
        public GovernmentConsumption government_consumption { get; set; }
        public InvestmentInFixedCapital investment_in_fixed_capital { get; set; }
        public InvestmentInInventories investment_in_inventories { get; set; }
        public ExportsOfGoodsAndServices exports_of_goods_and_services { get; set; }
        public ImportsOfGoodsAndServices imports_of_goods_and_services { get; set; }
    }

    public class ByEndUse
    {
        public EndUses end_uses { get; set; }
        public string date { get; set; }
    }

    public class Agriculture
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class Industry
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class Services
    {
        public double value { get; set; }
        public string units { get; set; }
    }

    public class Sectors
    {
        public Agriculture agriculture { get; set; }
        public Industry industry { get; set; }
        public Services services { get; set; }
    }

    public class BySectorOfOrigin
    {
        public Sectors sectors { get; set; }
        public string date { get; set; }
    }

    public class Composition
    {
        public ByEndUse by_end_use { get; set; }
        public BySectorOfOrigin by_sector_of_origin { get; set; }
    }

    public class Gdp
    {
        public PurchasingPowerParity purchasing_power_parity { get; set; }
        public OfficialExchangeRate official_exchange_rate { get; set; }
        public RealGrowthRate real_growth_rate { get; set; }
        public PerCapitaPurchasingPowerParity per_capita_purchasing_power_parity { get; set; }
        public Composition composition { get; set; }
    }

    public class GrowthRate
    {
        public double value { get; set; }
        public string units { get; set; }
        public string date { get; set; }
    }

    public class PurchasePower
    {
        public double value { get; set; }
        public string units { get; set; }
        public string date { get; set; }
    }
    /// <summary>
    /// this is a mirror of the important parts of the country model in the MergePowerData project.
    /// Created by turning MergePowerData into JSON and then running that json through
    /// an online json -> c# converter
    /// </summary>
    public class CountryTest
    {
        public Electric Electric { get; set; }
        public FossilFuelDetail FossilFuelDetail { get; set; }
        public Gdp Gdp { get; set; }
        public string Name { get; set; }
        public int Pop { get; set; }
        public GrowthRate GrowthRate { get; set; }
        public PurchasePower PurchasePower { get; set; }
    }


}

using System.Collections.Generic;

namespace MergePowerData.CIAFdata
{
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

    public class AnnualValue2
    {
        public double value { get; set; }
        public string units { get; set; }
        public string date { get; set; }
    }

    public class RealGrowthRate
    {
        public List<AnnualValue2> annual_values { get; set; }
        public int global_rank { get; set; }
    }

    public class AnnualValue3
    {
        public double value { get; set; }
        public string units { get; set; }
        public string date { get; set; }
    }

    public class PerCapitaPurchasingPowerParity
    {
        public List<AnnualValue3> annual_values { get; set; }
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
}
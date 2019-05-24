using System.Collections.Generic;
using System.Linq;

namespace MergePowerData.CIAFData
{
    /// <summary>
    /// Country is not intended to absorb all the data from CIAF.
    /// It is used in memory for sorting and should be kept to a minimum of data
    /// </summary>
    public class Country
    {
        public readonly Electric Electric;
        public readonly Gdp Gdp;
        public readonly string Name;
        public readonly long Pop;
        public readonly AnnualValue2 GrowthRate;
        public readonly AnnualValue3 PurchasePower;

        public Country(string name, Electric electric, Gdp gdp, long pop)
        {
            Name = name;
            Electric = electric;
            Gdp = gdp;
            Pop = pop;

            PurchasePower = EconBase(gdp, out GrowthRate);
        }

        /// <summary>
        /// </summary>
        /// <param name="gdp"></param>
        /// <param name="growthRate"></param>
        /// <returns></returns>
        private static AnnualValue3 EconBase(Gdp gdp, out AnnualValue2 growthRate)
        {
            var gr = gdp.real_growth_rate ?? new RealGrowthRate();

            if (gr.annual_values == null)
                gr.annual_values = new List<AnnualValue2>
                    {new AnnualValue2 {date = "2000", units = "USD", value = double.NaN}};

            growthRate = gr.annual_values.OrderBy(av => av.date).LastOrDefault();

            AnnualValue3 purchasingPower;
            if (gdp.per_capita_purchasing_power_parity == null)
            {
                purchasingPower = new AnnualValue3 { date = "2000", units = "USD", value = double.NaN };
            }
            else
            {
                var pp = gdp.per_capita_purchasing_power_parity.annual_values;

                purchasingPower = pp != null
                    ? pp.OrderBy(p => p.date).LastOrDefault()
                    : new AnnualValue3 { date = "1990", value = 0, units = "USD" };
            }

            return purchasingPower;
        }
    }
}

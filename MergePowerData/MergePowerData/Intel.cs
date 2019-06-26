using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MergePowerData.CIAFdata;
using MergePowerData.IntelMath;

namespace MergePowerData
{
    /*
     * CIA Factbook; Acronym "CIAF".
     */
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Intel
    {
        // Report constant and conversion factors
        public const double Tera = 1.0e12;
        public const double Giga = 1.0e9;
        public const double Mega = 1.0e6;
        public const double Kilo = 1.0e3;

        public const double kgU245perkWh = 24.0e6;
        public const double TWh2kg = 0.040055402; // TWh = 0.040055402 kg
        public const double TwentyMtonTnt = 0.93106557; //

        // 20 Mton_e = 0.93106557 kg, ~1 kg
        protected const double TWh2MTonTnt = 0.86042065; // TWh = 0.86042 M ton Tnt
        protected const double TWh2PJ = 3.6; // TWh = 3.6 PJ (Peta Joule's)

        public static double
            windMillTWh = 2.0e-6 * InstalledGeneratingCapacity.HoursPerYear * 0.4; // 2MW wind mill 40% eff

        // CIAF is a collection of data by country.  To analyze a specific set of data into a report we must extract a relevant subset 
        protected readonly List<Country> _countries = new List<Country>();
        protected Statistic _energyDeviationRelativeToGrowth;
        protected Statistic _ffDevRelativeToGrowth;
        protected StatCollector _stats;
        protected Country _world;

        /// <summary>
        /// Drive / Control CIAF data processing and filter based on minimum Gdp
        /// </summary>
        /// <param name="minimumGdp">Limit, in Billion $ (GIGA $) to filter out countries below a minimum GDP from report.</param>
        /// <param name="filter"> </param>
        public Intel(double minimumGdp, string filter)
        {
            MinimumGdp = minimumGdp;
            Filter = filter;

            _stats = new StatCollector(minimumGdp);
        }

        public double MinimumGdp { get; set; }
        public string Filter { get; set; }

        // These linear regressions showed that there is no correlation between gdp growth & countries diff from average energy / gdp 
        /// <summary>
        /// Temporary obsolete.  Interesting bit of code because it can perform linear regressions on the data from the completed LR's in the report
        ///  Kind of like taking a second integral representation of the data.
        /// </summary>
        /// <returns></returns>
        public Statistic DevRelativeToGrowth()
        {
            _energyDeviationRelativeToGrowth = new Statistic();
            _ffDevRelativeToGrowth = new Statistic();

            foreach (var country in _countries)
            {
                // do not add aggregation country entries to stats
                if (Regex.IsMatch(country.Name, @"world|european", RegexOptions.IgnoreCase)) continue;

                var x = _stats.Stand("eprod_gdp", country.Electric.ProdTWh * TWh2kg, country.PurchasePower.value / Giga);
                _energyDeviationRelativeToGrowth.Add(x, country.GrowthRate.value);

                x = _stats.Stand("pcff_gdp",
                    country.Electric.Electricity.by_source.fossil_fuels.percent / 100 * country.Electric.ProdTWh *
                    TWh2kg,
                    country.PurchasePower.value / Giga);

                _ffDevRelativeToGrowth.Add(x, country.GrowthRate.value);
            }

            Console.WriteLine(_energyDeviationRelativeToGrowth);
            Console.WriteLine(_ffDevRelativeToGrowth);

            return _energyDeviationRelativeToGrowth;
        }

        /// <summary>
        ///     Countries added here will be part of report.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="electric"></param>
        /// <param name="ff"></param>
        /// <param name="gdp"></param>
        /// <param name="pop"></param>
        public void Add(string name, Electric electric, FossilFuelDetail ff, Gdp gdp, long pop)
        {
            if (name.Equals("World"))
                _world = new Country(name, electric, ff, gdp, pop);

            var country = new Country(name, electric, ff, gdp, pop);

            if (country.PurchasePower.value / Giga < _stats.PLimit) return;

            _countries.Add(country);
            _stats.Add(country);
        }

        /// <summary>
        ///  Research report.  Many report lines are commented out and can be added back if that data becomes significant to the current analysis OODA loop 
        /// </summary>
        public void CsvReport()
        {
            Console.WriteLine($"Gross Domestic product greater than: Giga ${MinimumGdp} (billion)\n");
            var dv = "\t"; // For example; if you are documenting an .md format file for example the col separator can be changed to '|'

            List<string> ReportColumns = new List<string>(new[]
           {
                "eprod",
                "pcff",
                "emission",
                "ffcrudeprod",
                "ffnatgasprod",
                "pchydro",
                "pcoilgdp",
                "gdp",
                });

            var headersb = new StringBuilder();

            foreach (var col in ReportColumns)
            {
                headersb.Append(StatCollector.ColumnConfigs[col].Short);
                headersb.Append(dv);

                if (col.Equals("eprod"))
                    headersb.Append($"Qx{dv}");
            }

            headersb.Append($"Country{dv}");

            Console.WriteLine(headersb);

            //// header
            //Console.WriteLine(
            //        //"ProdTWh{dv}"-
            //        $"ekg{dv}"
            //        + $"GTR>avgE{dv}"
            //        + $"LSS<avgE{dv}"
            //        + $"ffGenCap/Y kg{dv}"
            //        + $"dev_FF{dv}"
            //        + $"FuelMbl{dv}"
            //        + $"NatGasGcm{dv}"
            //        + $"Co2Tton{dv}"
            //        ////+ "maxFF_kWh"
            //        ////+ "kw/pop{dv}"
            //        ////+ "pop_M{dv}"
            //        ////+ "kWh/pop{dv}"
            //        ////+ "Prod_FF_TWh{dv}"
            //        ////+ "Capacity_TWh/y{dv}"
            //        ////+ "$Growth{dv}"
            //        + $"$G PP{dv}"
            //        // $"WindMillRepl{dv}"
            //        // + $"$PP/TWh{dv}"
            //        + "Country");

            foreach (var c in _countries.OrderByDescending(d => d.Electric.ProdTWh))
            {
                var kgEfossil = c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh *
                                TWh2kg;
                var wrldKgEfossil = _world.Electric.Electricity.by_source.fossil_fuels.percent / 100 *
                                    _world.Electric.ProdTWh * TWh2kg;

                var igc = c.Electric.Electricity.installed_generating_capacity;
                if (igc == null) continue;

                var currentCap = igc.YearCapacityTWhr;
                var futureCap50Y = igc.YearCapacityTWhr * 2;

                var standElectricProd =
                    _stats.Stand("eprod_gdp", StatCollector.XValue("eprod", c), StatCollector.XValue("gdp", c));
                //_stats.Stand("eprod_gdp", c.Electric.ProdTWh * TWh2kg, c.PurchasePower.value / Giga);

                var rowSb = new StringBuilder();

                foreach (var col in ReportColumns)
                {
                    switch (StatCollector.ColumnConfigs[col].Format)
                    {
                        case "1": rowSb.Append($"{StatCollector.XValue(col, c):F1}"); break;
                        case "2": rowSb.Append($"{StatCollector.XValue(col, c):F2}"); break;
                        case "3": rowSb.Append($"{StatCollector.XValue(col, c):F3}"); break;
                        case "4": rowSb.Append($"{StatCollector.XValue(col, c):F4}"); break;
                        case "5": rowSb.Append($"{StatCollector.XValue(col, c):F5}"); break;

                        default: rowSb.Append($"{StatCollector.XValue(col, c)}"); break;
                    }

                    rowSb.Append(dv);

                    if (col.Equals("eprod"))
                        rowSb.Append($"{Math.Abs(standElectricProd):F3}{dv}");
                }

                rowSb.Append($"{c.Name}");
                Console.WriteLine(rowSb);

                #region reference
                /*
                // Understanding Country wealth relative to use of FF and electricity.
            //    Console.WriteLine(
            //        $"{c.Electric.ProdTWh * TWh2kg:F1}{dv}"
            //        + $"{(standElectricProd >= 0 ? Math.Abs(standElectricProd) : 0):F3}{dv}"
            //        + $"{(standElectricProd <= 0 ? Math.Abs(standElectricProd) : 0):F3}{dv}"
            //        + $"{igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.fossil_fuels.percent) * TWh2kg:F1}{dv}"
            //        + $"{_stats.Stand("pcff_gdp", igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.fossil_fuels.percent) * TWh2kg, c.PurchasePower.value / Giga):F3}{dv}"
            //        // + $"{c.Electric.Electricity.by_source.nuclear_fuels.percent / 100 * c.Electric.ProdKWh / kgU245perkWh * 1.6:F1}{dv}" // 1.6 is power xfer loss estimate
            //        + $"{c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / Mega:F2}{dv}"
            //        + $"{c.FossilFuelDetail.NaturalGas.Consumption.Value / Giga:F1}{dv}"

            //        //+ $"{c.Pop / 1e6:F0}{dv}"
            //        //+ $"{c.Electric.ProdKWh / c.Pop:F0}{dv}"
            //        //+ $"{c.Electric.ProdTWh / _world.Electric.ProdTWh:F3}{dv}"
            //        //+ $"{kgEfossil / wrldKgEfossil:F3}{dv}"
            //        //+ $"{c.Electric.Electricity.by_source.nuclear_fuels.percent}{dv}"
            //        //+ $"{c.Electric.Electricity.by_source.hydroelectric_plants.percent}{dv}"
            //        //+ $"{c.Electric.Electricity.by_source.fossil_fuels.percent}{dv}"
            //        //+ $"{c.Electric.Electricity.by_source.other_renewable_sources.percent}{dv}"
            //        //+ $"{igc.YearCapacityTWhr:F0}{dv}"
            //        //+ $"{igc.YearCapacityTWhr / c.Electric.ProdTWh:F2}{dv}"
            //        //+ $"{c.GrowthRate.value:F1}{dv}"
            //        //+ $"{c.GrowthRate.date}{dv}"
            //        //+ $"{c.PurchasePower.value / _world.PurchasePower.value:F3}{dv}"
            //        //+ $"{c.PurchasePower.value}{dv}"
            //        //+ $"{c.PurchasePower.value / c.Electric.ProdTWh:F2}{dv}"
            //        //+ $"{c.FossilFuelDetail.RefinedPetroleum.Consumption.Value:F0}{dv}"
            //        //+ $"{c.FossilFuelDetail.RefinedPetroleum.Production.Value:F0}{dv}"
            //        //+ $"{c.FossilFuelDetail.CrudeOil.Exports.Value / c.FossilFuelDetail.CrudeOil.Production.Value:F2}{dv}"
            //        //+
    */
                #endregion
            }

            Console.WriteLine(); // add blank line separation

            Console.WriteLine($"Statistic Count: {_stats.Count}\n");
            Console.WriteLine(_stats.ToReport(dv, .890, -.290) + "\n");

            Console.WriteLine(_stats.ToReport(dv, "gdp") + "\n");
            Console.WriteLine(_stats.ToReport(dv, Filter));

        }
    }
}
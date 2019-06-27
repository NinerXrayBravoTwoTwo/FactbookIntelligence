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

        public const double kgU235perkWh = 24.0e6;
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

                x = _stats.Stand("capff_gdp",
                    country.Electric.Electricity.by_source.fossil_fuels.percent / 100 * country.Electric.ProdTWh * TWh2kg,
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

            var ReportColumns = new List<string>(new[]
           {
               "eprod",
               "capff",
               "emission",
               "gdp",
               });

            var headersb = new StringBuilder();

            foreach (var col in ReportColumns)
            {
                headersb.Append($"{IntelCore.ColumnConfigs[col].Short} {IntelCore.ColumnConfigs[col].Unit}{dv}");

                if (Regex.IsMatch(col, @"(eprod|capff|caphydro)"))
                    headersb.Append($"Qx{dv}");
            }

            headersb.Append($"Country{dv}");

            Console.WriteLine(headersb);

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
                    _stats.Stand("eprod_gdp", IntelCore.XValue("eprod", c), IntelCore.XValue("gdp", c));

                var standCapFF =
                    _stats.Stand("capff_gdp", IntelCore.XValue("capff", c), IntelCore.XValue("gdp", c));

                var standCapHydro =
                    _stats.Stand("caphydro_gdp", IntelCore.XValue("caphydro", c), IntelCore.XValue("gdp", c));

                //_stats.Stand("eprod_gdp", c.Electric.ProdTWh * TWh2kg, c.PurchasePower.value / Giga);

                var rowSb = new StringBuilder();

                foreach (var col in ReportColumns)
                {
                    switch (IntelCore.ColumnConfigs[col].Format)
                    {
                        case "1": rowSb.Append($"{IntelCore.XValue(col, c):F1}"); break;
                        case "2": rowSb.Append($"{IntelCore.XValue(col, c):F2}"); break;
                        case "3": rowSb.Append($"{IntelCore.XValue(col, c):F3}"); break;
                        case "4": rowSb.Append($"{IntelCore.XValue(col, c):F4}"); break;
                        case "5": rowSb.Append($"{IntelCore.XValue(col, c):F5}"); break;

                        default: rowSb.Append($"{IntelCore.XValue(col, c)}"); break;
                    }

                    rowSb.Append(dv);

                    var match = Regex.Match(col, @"(eprod|capff|caphydro)");

                    if (match.Success)
                        switch (match.Groups[1].Value)
                        {
                            case "eprod": rowSb.Append($"{standElectricProd:F3}{dv}"); break;
                            case "caphydro": rowSb.Append($"{standCapHydro:F3}{dv}"); break;
                            case "capff": rowSb.Append($"{standCapFF:F3}{dv}"); break;
                        }
                }

                rowSb.Append($"{c.Name}");
                Console.WriteLine(rowSb);
            }

            Console.WriteLine(); // add blank line separation

            Console.WriteLine($"Statistic Count: {_stats.Count}\n");
            Console.WriteLine(_stats.ToReport(dv, .95, -.5) + "\n");

            if (!string.IsNullOrEmpty(Filter))
                Console.WriteLine(_stats.ToReport(dv, Filter));
        }
    }
}
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
        // CIAF is a collection of data by country.  To analyze a specific set of data into a report we must extract a relevant subset 
        protected readonly List<Country> _countries = new List<Country>();
        protected Statistic _energyDeviationRelativeToGrowth;
        protected Statistic _ffDevRelativeToGrowth;
        protected StatCollector _stats;
        protected Country _world;

        /// <summary>
        ///     Drive / Control CIAF data processing and filter based on minimum Gdp
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
        ///     Temporary obsolete.  Interesting bit of code because it can perform linear regressions on the data from the
        ///     completed LR's in the report
        ///     Kind of like taking a second integral representation of the data.
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

                var x = _stats.Stand("eprod_gdp", country.Electric.ProdTWh * IntelCore.TWh2Kg,
                    country.PurchasePower.value / IntelCore.Giga);
                _energyDeviationRelativeToGrowth.Add(x, country.GrowthRate.value);

                x = _stats.Stand("capff_gdp",
                    country.Electric.Electricity.by_source.fossil_fuels.percent / 100 * country.Electric.ProdTWh *
                    IntelCore.TWh2Kg,
                    country.PurchasePower.value / IntelCore.Giga);

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

            if (country.PurchasePower.value / IntelCore.Giga < _stats.PLimit) return;

            _countries.Add(country);
            _stats.Add(country);
        }

        /// <summary>
        ///     Research report.  Many report lines are commented out and can be added back if that data becomes significant to the
        ///     current analysis OODA loop
        /// </summary>
        public void CsvReport()
        {
            Console.WriteLine($"Gross Domestic product greater than: Giga ${MinimumGdp} (billion)\n");
            const string
                dv = "\t"; // For example; if you are documenting an .md format file for example the col separator can be changed to '|'

            var ReportColumns = new List<string>(new[]
            {
                "eprod",
                "capff",
                "emission",
                "gdp"
            });

            var reportStats = @"(eprod|capff|caphydro|emission)";

            var reportSb = BuildReport(ReportColumns, reportStats, dv);

            Console.WriteLine(reportSb);
            Console.WriteLine(); // add blank line separation

            Console.WriteLine($"Statistic Count: {_stats.Count}\n");
            Console.WriteLine(_stats.ToReport(dv, .88, -.5) + "\n");

            if (!string.IsNullOrEmpty(Filter))
                Console.WriteLine(_stats.ToReport(dv, Filter));
        }

        private StringBuilder BuildReport(List<string> ReportColumns, string reportStats, string dv)
        {
            var reportSb = new StringBuilder();

            #region Report header

            foreach (var col in ReportColumns)
            {
                reportSb.Append($"{IntelCore.ColumnConfigs[col].Short} {IntelCore.ColumnConfigs[col].Unit}{dv}");

                var match = Regex.Match(col, reportStats);
                if (match.Success)
                    switch (match.Groups[1].Value)
                    {
                        case "eprod":
                            reportSb.Append($"Qx {IntelCore.GetXyUnits("eprod_gdp")}{dv}");
                            reportSb.Append($"Qx {IntelCore.GetXyUnits("eprod_emission")}{dv}");
                            break;
                        case "caphydro":
                            reportSb.Append($"Qx {IntelCore.GetXyUnits("capff_gdp")}{dv}");
                            break;
                        case "capff":
                            reportSb.Append($"Qx {IntelCore.GetXyUnits("caphydro_gdp")}{dv}");
                            reportSb.Append($"Qx {IntelCore.GetXyUnits("caphydro_emission")}{dv}");
                            break;
                        case "emission":
                            reportSb.Append($"Qx {IntelCore.GetXyUnits("gdp_emission")}{dv}");
                            break;
                    }
            }

            reportSb.Append("Country\n");

            if (dv.Equals("|"))
                reportSb.Append(IntelCore.WriteDivider(Regex.Matches(reportSb.ToString(), @"\|").Count) + "\n");

            #endregion

            #region report body

            foreach (var c in _countries.OrderByDescending(d => d.Electric.ProdTWh))
            {
                var kgEfossil = c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh *
                                IntelCore.TWh2Kg;
                var wrldKgEfossil = _world.Electric.Electricity.by_source.fossil_fuels.percent / 100 *
                                    _world.Electric.ProdTWh * IntelCore.TWh2Kg;

                var igc = c.Electric.Electricity.installed_generating_capacity;
                if (igc == null) continue;

                var currentCap = igc.YearCapacityTWhr;
                var futureCap50Y = igc.YearCapacityTWhr * 2;

                foreach (var col in ReportColumns)
                {
                    switch (IntelCore.ColumnConfigs[col].Format)
                    {
                        case "1":
                            reportSb.Append($"{IntelCore.XValue(col, c):F1}");
                            break;
                        case "2":
                            reportSb.Append($"{IntelCore.XValue(col, c):F2}");
                            break;
                        case "3":
                            reportSb.Append($"{IntelCore.XValue(col, c):F3}");
                            break;
                        case "4":
                            reportSb.Append($"{IntelCore.XValue(col, c):F4}");
                            break;
                        case "5":
                            reportSb.Append($"{IntelCore.XValue(col, c):F5}");
                            break;

                        default:
                            reportSb.Append($"{IntelCore.XValue(col, c)}");
                            break;
                    }

                    reportSb.Append($"{dv}");

                    var match = Regex.Match(col, reportStats);

                    if (match.Success)
                        switch (match.Groups[1].Value)
                        {
                            case "eprod":
                                reportSb.Append($"{_stats.Stand("eprod_gdp", c):F3}{dv}");
                                reportSb.Append($"{_stats.Stand("eprod_emission", c):F3}{dv}");
                                break;
                            case "caphydro":
                                reportSb.Append($"{_stats.Stand("caphydro_gdp", c):F3}{dv}");
                                break;
                            case "capff":
                                reportSb.Append($"{_stats.Stand("capff_gdp", c):F3}{dv}");
                                reportSb.Append($"{_stats.Stand("capff_emission", c):F3}{dv}");
                                break;
                            case "emission":
                                reportSb.Append($"{_stats.Stand("gdp_emission", c):F3}{dv}");
                                break;
                        }
                }

                reportSb.Append($"{c.Name}\n");
            }

            #endregion

            return reportSb;
        }
    }
}
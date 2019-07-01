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

            _stats = new StatCollector();
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

            if (country.PurchasePower.value / IntelCore.Giga < MinimumGdp) return;

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

            // For example; if you are documenting an .md format file for example the col separator can be changed to '|'
            const string dv = "\t";

            #region Table Report

            var ReportColumns = new List<string>(new[]
            {
                "eprodtwh",
                "capfftwh",
                "emission",
                "gdp",
                "pctcaprenew",
                "eutilization"
            });

            var reportStats = @"(eprod|capff|eprodtwh|capfftwh|emission)";

            var reportSb = BuildReport(ReportColumns, reportStats, dv);
            Console.WriteLine(reportSb + "\n");

            #endregion

            #region Statistics report

            Console.WriteLine($"Statistic Count: {_stats.Count}\n");
            //Console.WriteLine(_stats.ToReport(dv, .88, -.5) + "\n"); // Just the relevent stat's ma'm, just the relevenet ones

            if (!string.IsNullOrEmpty(Filter))
                Console.WriteLine(_stats.ToReport(dv, Filter));

            #endregion

            #region Report Wind Mill cost in TW

            // TODO: Verify that one axis is a GDP value and then determine if the gdp is X or Y (invert division)
            // TODO: Verify that cost of money is $ / kWh; or G$ / TWh * 1.0e-9
            var costOfMoney = _stats.Stats["eprodtwh_gdp"].Slope();
            double replacementTme;

            Console.WriteLine("Time it takes One windmill to replace the energy required to create it;");

            Console.WriteLine(WindMillCost(out replacementTme, costOfMoney, 3.5 * IntelCore.Mega, .33, 0.30));
            Console.WriteLine(WindMillCost(out replacementTme, costOfMoney, 3.5 * IntelCore.Mega, .33, 0.35));
            Console.WriteLine(WindMillCost(out replacementTme, costOfMoney, 3.5 * IntelCore.Mega, .33, .38));

            Console.WriteLine($"Years For One windmill to replace it's self: {replacementTme}");
            #endregion

            // ./ MergePowerData $=0 filter = eutilization | egrep - i 'capacity' | grep - v Generating

        }


        private static string WindMillCost(out double replacementTme, double costOfMoney,
            double priceOfwindmill, double efficiency, double utilizationPercent)
        {
            // Covert stat to $ per kW hours, Since $ are in Billion dollars and energy is in TW divide both by a billion

            // priceOfwindmill KWh
            var kWcostPerWindmill = priceOfwindmill / (costOfMoney);
            var kWGenPerHr = 2.0 * IntelCore.Mega * efficiency * utilizationPercent / IntelCore.YearHours;

            var winmillPerhour = kWGenPerHr / kWcostPerWindmill;
            var windmillPerYear = winmillPerhour * IntelCore.YearHours;

            var result =
                 $"Eff: {efficiency} Util: {utilizationPercent:F3} Gen: {kWGenPerHr:F3} kW/hr, money: {costOfMoney:F3} $/kWh, installedCost$: {kWcostPerWindmill:F3} kWh/windmill, " +
                 $" windmillPerYear: { windmillPerYear:F3}, yearsPerWindmill: {1 / windmillPerYear:F3}";

            replacementTme = 1 / windmillPerYear;

            return result;
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
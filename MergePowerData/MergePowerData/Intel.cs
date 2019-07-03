using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        protected CountryData _countryData;

        /// <summary>
        ///     Drive / Control CIAF data processing and filter based on minimum Gdp
        /// </summary>
        /// <param name="minimumGdp">Limit, in Billion $ (GIGA $) to filter out countries below a minimum GDP from report.</param>
        /// <param name="filter"> </param>
        public Intel(CountryData countryData, string filter)
        {
            Filter = filter;
            _countryData = countryData;
        }

        public string Filter { get; set; }

        /// <summary>
        ///     Research report.  Many report lines are commented out and can be added back if that data becomes significant to the
        ///     current analysis OODA loop
        /// </summary>
        public void CsvReport()
        {
            if (!double.IsNaN(_countryData.GdpMaximum))
                Console.WriteLine($"GDP less or equal: Giga ${_countryData.GdpMaximum} (billion)");

            Console.WriteLine($"GDP greater than: Giga ${_countryData.GdpMinimum} (billion)");

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

            Console.WriteLine();
            var reportSb = BuildReport(ReportColumns, reportStats, dv);
            Console.WriteLine(reportSb + "\n");

            #endregion

            #region Statistics report

            Console.WriteLine($"Statistic Count: {_countryData.Stats.Count}\n");
            //Console.WriteLine(_stats.ToReport(dv, .88, -.5) + "\n"); // Just the relevent stat's ma'm, just the relevenet ones

            if (string.IsNullOrEmpty(Filter)) Filter = "eutilization_pctcap";

            Console.WriteLine(_countryData.Stats.ToReport(dv, Filter));

            #endregion

            #region Report Wind Mill cost in TW

            // TODO: Verify that one axis is a GDP value and then determine if the gdp is X or Y (invert division)
            // TODO: Verify that cost of money is $ / kWh; or G$ / TWh * 1.0e-9
            var costOfMoney = _countryData.Stats.Stats["eprodtwh_gdp"];

            Console.WriteLine($"N: {costOfMoney.N} $Corr: {costOfMoney.Correlation():F3}  $slope: {costOfMoney.Slope():F3}");
            double replacementTme;

            Console.WriteLine("Time it takes One windmill to replace the energy required to create it;");

            Console.WriteLine(WindMillCost(out replacementTme, costOfMoney.Slope(), 3.5 * IntelCore.Mega, .33, 0.30));
            Console.WriteLine(WindMillCost(out replacementTme, costOfMoney.Slope(), 3.5 * IntelCore.Mega, .33, 0.35));
            Console.WriteLine(WindMillCost(out replacementTme, costOfMoney.Slope(), 3.5 * IntelCore.Mega, .33, .38));

            Console.WriteLine($"Years For One windmill to replace it's self: {replacementTme}");
            #endregion

            #region utilization by source report
            Console.WriteLine("\nFind Source Utilization;");

            var equalCap = new Dictionary<string, Statistic>
            {
                { "utilFossil",  _countryData.Stats.Stats["eutilization_pctcapfossil"] },
                { "utilHydro",   _countryData.Stats.Stats["eutilization_pctcaphydro"] },
                { "utilNuclear", _countryData.Stats.Stats["eutilization_pctcapnuclear"] },
                { "utilRenew",   _countryData.Stats.Stats["eutilization_pctcaprenew"] }
            };

            var xxx = new ElectricSourceUtilizationAdjustment(equalCap, _countryData.Countries);
            #endregion
        }

        private static string WindMillCost(out double replacementTme, double costOfMoney, double priceOfwindmill, double efficiency, double utilizationPercent)
        {
            // Covert stat to $ per kW hours, Since $ are in Billion dollars and energy is in TW divide both by a billion

            // priceOfwindmill KWh
            var kWcostPerWindmill = priceOfwindmill / (costOfMoney);
            var kWGenPerHr = 2.0 * IntelCore.Mega * efficiency * utilizationPercent / IntelCore.YearHours;

            var winmillPerhour = kWGenPerHr / kWcostPerWindmill;
            var windmillPerYear = winmillPerhour * IntelCore.YearHours;

            var result =
                 $"Eff: {efficiency} Util: {utilizationPercent:F3} Gen: {kWGenPerHr:F3} kW/hr, money: {costOfMoney:F3}"
                 + $" $/kWh, installedCost$: {kWcostPerWindmill:F3} kWh/windmill,"
                 + $" windmillPerYear: { windmillPerYear:F3}, yearsPerWindmill: {1 / windmillPerYear:F3}";

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

            foreach (var c in _countryData.Countries.OrderByDescending(d => d.Electric.ProdTWh))
            {
                var kgEfossil = c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh *
                                IntelCore.TWh2Kg;

                var wrldKgEfossil = _countryData.World.Electric.Electricity.by_source.fossil_fuels.percent / 100 *
                                    _countryData.World.Electric.ProdTWh * IntelCore.TWh2Kg;

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
                                reportSb.Append($"{_countryData.Stats.Stand("eprod_gdp", c):F3}{dv}");
                                reportSb.Append($"{_countryData.Stats.Stand("eprod_emission", c):F3}{dv}");
                                break;
                            case "caphydro":
                                reportSb.Append($"{_countryData.Stats.Stand("caphydro_gdp", c):F3}{dv}");
                                break;
                            case "capff":
                                reportSb.Append($"{_countryData.Stats.Stand("capff_gdp", c):F3}{dv}");
                                reportSb.Append($"{_countryData.Stats.Stand("capff_emission", c):F3}{dv}");
                                break;
                            case "emission":
                                reportSb.Append($"{_countryData.Stats.Stand("gdp_emission", c):F3}{dv}");
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
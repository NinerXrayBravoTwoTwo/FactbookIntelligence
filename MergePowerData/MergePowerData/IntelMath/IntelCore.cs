using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MergePowerData.CIAFdata;
using Newtonsoft.Json;

namespace MergePowerData.IntelMath
{
    public static class IntelCore
    {
        #region Constants

        // Report constant and conversion factors
        public const double Tera = 1.0e12;
        public const double Giga = 1.0e9;
        public const double Mega = 1.0e6;
        public const double Kilo = 1.0e3;

        public const double KgU235PerkWh = 24.0e6;

        // ReSharper disable once InconsistentNaming
        public const double TWh2Kg = 0.040055402; // TWh = 0.040055402 kg
        public const double TwentyMtonTnt = 0.93106557; //

        // 20 Mton_e = 0.93106557 kg, ~1 kg
        // ReSharper disable once InconsistentNaming
        public const double TWh2MTonTnt = 0.86042065; // TWh = 0.86042 M ton Tnt

        // ReSharper disable once InconsistentNaming
        public const double TWh2PJ = 3.6; // TWh = 3.6 PJ (Peta Joule's)

        public static double
            WindMillTWh = 2.0e-6 * InstalledGeneratingCapacity.HoursPerYear * 0.4; // 2MW wind mill 40% eff

        public static double YearHours = 8765.8128;

        #endregion

        internal static readonly string ConfigFile = Environment.CurrentDirectory + "/Columns.json";

        public static Dictionary<string, ColumnConfig> ColumnConfigs =>
            JsonConvert.DeserializeObject<Dictionary<string, ColumnConfig>>(File.ReadAllText(ConfigFile));


        /// <summary>
        ///     Preforms primary column computation for a data element from CIAF
        /// </summary>
        /// <param name="key">Identifying key for column configuration</param>
        /// <param name="c">Country object with all related CIAF data</param>
        /// <returns></returns>
        public static double XValue(string key, Country c)
        {
            double result;

            var igc = c.Electric.Electricity.installed_generating_capacity;
            var elcity = c.Electric.Electricity;

            switch (key)
            {
                case "emission":
                    result = c.Electric.TtonCo2;
                    break;
                case "gdp":
                    result = c.PurchasePower.value / Giga;
                    break;
                case "growth":
                    result = c.GrowthRate.value;
                    break;
                case "pop":
                    result = c.Pop / Mega;
                    break;
                case "kwpop":
                    result = c.Electric.ProdKWh / c.Pop;
                    break;
                case "eprod":
                    result = c.Electric.ProdTWh * TWh2Kg;
                    break;
                case "econs":
                    result = c.Electric.ConsTWh * TWh2Kg;
                    break;
                case "eprodtwh":
                    result = c.Electric.ProdTWh;
                    break;
                case "econstwh":
                    result = c.Electric.ConsTWh;
                    break;
                case "eimport":
                    result = elcity.imports?.TWh * TWh2Kg ?? double.NaN;
                    break;
                case "eexport":
                    result = elcity.exports?.TWh * TWh2Kg ?? double.NaN;
                    break;
                case "capff":
                    result = igc?.YearCapTWhrByPercent(c.Electric.Electricity.by_source.fossil_fuels.percent) * TWh2Kg
                             ?? double.NaN;
                    break;
                case "capfftwh":
                    result = igc?.YearCapTWhrByPercent(c.Electric.Electricity.by_source.fossil_fuels.percent)
                             ?? double.NaN;
                    break;
                case "capnuke":
                    result = igc?.YearCapTWhrByPercent(c.Electric.Electricity.by_source.nuclear_fuels.percent) *
                             TWh2Kg
                             ?? double.NaN;
                    break;
                case "caphydro":
                    result = igc?.YearCapTWhrByPercent(c.Electric.Electricity.by_source.hydroelectric_plants.percent) *
                             TWh2Kg
                             ?? double.NaN;
                    break;
                case "caprenew":
                    result =
                        igc?.YearCapTWhrByPercent(c.Electric.Electricity.by_source.other_renewable_sources.percent) *
                        TWh2Kg
                        ?? double.NaN;
                    break;
                case "caprenewtwh":
                    result =
                        igc?.YearCapTWhrByPercent(c.Electric.Electricity.by_source.other_renewable_sources.percent)
                        ?? double.NaN;
                    break;
                case "ffrefineprod":
                    result = c.FossilFuelDetail.RefinedPetroleum.Production.Value / Mega;
                    break;
                case "ffrefinecons":
                    result = c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / Mega;
                    break;
                case "ffrefineimport":
                    result = c.FossilFuelDetail.RefinedPetroleum.Imports.Value / Mega;
                    break;
                case "ffrefineexport":
                    result = c.FossilFuelDetail.RefinedPetroleum.Exports.Value / Mega;
                    break;
                case "ffnatgasprod":
                    result = c.FossilFuelDetail.NaturalGas.Production.Value / Giga;
                    break;
                case "ffnatgascons":
                    result = c.FossilFuelDetail.NaturalGas.Consumption.Value / Giga;
                    break;
                case "ffnatgasimport":
                    result = c.FossilFuelDetail.NaturalGas.Imports.Value / Giga;
                    break;
                case "ffnatgasexport":
                    result = c.FossilFuelDetail.NaturalGas.Exports.Value / Giga;
                    break;
                case "ffnatgasreserv":
                    result = c.FossilFuelDetail.NaturalGas.ProvedReserves.Value / Giga;
                    break;
                case "ffcrudereserv":
                    result = c.FossilFuelDetail.CrudeOil.ProvedReserves.Value / Mega;
                    break;
                case "ffcrudeprod":
                    result = c.FossilFuelDetail.CrudeOil.Production.Value / Mega;
                    break;
                case "ffcrudeimport":
                    result = c.FossilFuelDetail.CrudeOil.Imports.Value / Mega;
                    break;
                case "ffcrudeexport":
                    result = c.FossilFuelDetail.CrudeOil.Exports.Value / Mega;
                    break;
                case "pcoilgdp":
                    result = c.FossilFuelDetail.CrudeOil.Production.Value * 50 * 365.242198781 / c.PurchasePower.value * 100;
                    break;
                case "pctcaprenew":
                    result = c.Electric.Electricity.by_source.other_renewable_sources.percent / 100;
                    break;
                case "pctcapfossil":
                    result = c.Electric.Electricity.by_source.fossil_fuels.percent / 100;
                    break;
                case "pctcapnuclear":
                    result = c.Electric.Electricity.by_source.nuclear_fuels.percent / 100;
                    break;
                case "pctcaphydro":
                    result = c.Electric.Electricity.by_source.hydroelectric_plants.percent / 100;
                    break;
                case "eutilization": // should be fraction between 0 -.9999999 
                    result = igc != null ? (c.Electric.ProdTWh / igc.YearCapacityTWhr) : double.NaN;
                    break;
                case "kwu235":
                    result = c.Electric.Electricity.by_source.nuclear_fuels.percent / 100 * c.Electric.ProdKWh /
                             KgU235PerkWh;
                    break;

                default:
                    throw new ArgumentException($"Undefined {key}");
            }

            return result;
        }

        public static Dictionary<string, ColumnConfig> SelectConfigs(string[] configKeys)
        {
            var match = string.Join("|", configKeys);

            var result = new Dictionary<string, ColumnConfig>();
            foreach (var item in ColumnConfigs)
            {
                if (Regex.IsMatch(item.Key, $"^({match})$", RegexOptions.IgnoreCase))
                    result.Add(item.Key, item.Value);

                if (result.Count >= configKeys.Length)
                    break;
            }

            return result;
        }

        /// <summary>
        ///     Returns the Long Name field from a column parameter.
        /// </summary>
        /// <param name="configKeys">List of column identifying keys</param>
        /// <returns>List of LongNames </returns>
        public static string[] GetNames(string[] configKeys)
        {
            var names = new List<string>();

            foreach (var item in SelectConfigs(configKeys))
                names.Add(item.Value.Name);
            return names.ToArray();
        }

        public static string[] GetUnits(string[] configKeys)
        {
            var names = new List<string>();

            foreach (var item in SelectConfigs(configKeys))
                names.Add(item.Value.Unit);

            return names.ToArray();
        }

        public static string GetXyUnits(string itemKey)
        {
            var units = GetUnits(itemKey.Split('_'));
            return $"{units[1]}/{units[0]}"; // rise over run.  run is x - horizontal, rise is y - vertical
        }

        public static string WriteDivider(int count)
        {
            var divider = new StringBuilder();

            for (var i = 0; i <= count; i++)
                if (i == 0)
                    divider.Append("----:|");
                else if (i != count)
                    divider.Append($"---:|");
                else
                    divider.Append("----:");

            return divider.ToString();
        }
    }

    /// <summary>
    /// </summary>
    public class ColumnConfig
    {
        public string Short { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public string Unit { get; set; }
    }
}
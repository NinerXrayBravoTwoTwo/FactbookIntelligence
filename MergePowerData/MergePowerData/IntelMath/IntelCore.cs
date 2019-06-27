using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MergePowerData.CIAFdata;
using Newtonsoft.Json;

namespace MergePowerData.IntelMath
{
    public static class IntelCore
    {
        internal static readonly string ConfigFile = Environment.CurrentDirectory + "/Columns.json";

        public static Dictionary<string, ColumnConfig> ColumnConfigs =>
            JsonConvert.DeserializeObject<Dictionary<string, ColumnConfig>>(File.ReadAllText(ConfigFile));

        /// <summary>
        /// Preforms primary column computation for a data element from CIAF
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
                    result = c.PurchasePower.value / Intel.Giga;
                    break;
                case "growth":
                    result = c.GrowthRate.value;
                    break;
                case "pop":
                    result = c.Pop / Intel.Mega;
                    break;
                case "kwpop":
                    result = c.Electric.ProdKWh / c.Pop;
                    break;
                case "eprod":
                    result = c.Electric.ProdTWh * Intel.TWh2kg;
                    break;
                case "econs":
                    result = c.Electric.ConsTWh * Intel.TWh2kg;
                    break;
                case "eprodtw":
                    result = c.Electric.ProdTWh;
                    break;
                case "econstw":
                    result = c.Electric.ConsTWh;
                    break;
                case "eimport":
                    result = elcity.imports != null ? elcity.imports.TWh * Intel.TWh2kg : double.NaN;
                    break;
                case "eexport":
                    result = elcity.exports != null ? elcity.exports.TWh * Intel.TWh2kg : double.NaN;
                    break;
                case "capff":
                    result = igc != null
                        ? igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.fossil_fuels.percent) * Intel.TWh2kg
                        : double.NaN;
                    break;
                case "capnuke":
                    result = igc != null
                        ? igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.nuclear_fuels.percent) *
                          Intel.TWh2kg
                        : double.NaN;
                    break;
                case "caphydro":
                    result = igc != null
                        ? igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.hydroelectric_plants.percent) *
                          Intel.TWh2kg
                        : double.NaN;
                    break;
                case "caprenew":
                    result = igc != null
                        ? igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.other_renewable_sources.percent) *
                          Intel.TWh2kg
                        : double.NaN;
                    break;
                case "ffrefineprod":
                    result = c.FossilFuelDetail.RefinedPetroleum.Production.Value / Intel.Mega;
                    break;
                case "ffrefinecons":
                    result = c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / Intel.Mega;
                    break;
                case "ffrefineimport":
                    result = c.FossilFuelDetail.RefinedPetroleum.Imports.Value / Intel.Mega;
                    break;
                case "ffrefineexport":
                    result = c.FossilFuelDetail.RefinedPetroleum.Exports.Value / Intel.Mega;
                    break;

                case "ffnatgasprod":
                    result = c.FossilFuelDetail.NaturalGas.Production.Value / Intel.Giga;
                    break;
                case "ffnatgascons":
                    result = c.FossilFuelDetail.NaturalGas.Consumption.Value / Intel.Giga;
                    break;
                case "ffnatgasimport":
                    result = c.FossilFuelDetail.NaturalGas.Imports.Value / Intel.Giga;
                    break;
                case "ffnatgasexport":
                    result = c.FossilFuelDetail.NaturalGas.Exports.Value / Intel.Giga;
                    break;
                case "ffnatgasreserv":
                    result = c.FossilFuelDetail.NaturalGas.ProvedReserves.Value / Intel.Giga;
                    break;
                case "ffcrudereserv":
                    result = c.FossilFuelDetail.CrudeOil.ProvedReserves.Value / Intel.Mega;
                    break;
                case "ffcrudeprod":
                    result = c.FossilFuelDetail.CrudeOil.Production.Value / Intel.Mega;
                    break;
                case "ffcrudeimport":
                    result = c.FossilFuelDetail.CrudeOil.Imports.Value / Intel.Mega;
                    break;
                case "ffcrudeexport":
                    result = c.FossilFuelDetail.CrudeOil.Exports.Value / Intel.Mega;
                    break;
                case "pcoilgdp":
                    result = c.FossilFuelDetail.CrudeOil.Production.Value * 50 * 365.242198781 / c.PurchasePower.value * 100;
                    break;
                // ratio capacity / ElecProd
                case "ratiocap2eprod":
                    result = igc.YearCapacityTWhr / c.Electric.ProdTWh;
                    break;
                case "kwu235":
                    result = c.Electric.Electricity.by_source.nuclear_fuels.percent / 100 * c.Electric.ProdKWh /
                             Intel.kgU235perkWh;
                    // result = igc?.YearCapTWhrByPercent(c.Electric.Electricity.by_source.nuclear_fuels.percent) 
                               // / 1.0e+09 * Intel.kgU235perkWh 
                             //?? double.NaN;
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
                if (Regex.IsMatch(item.Key, $"({match})", RegexOptions.IgnoreCase))
                    result.Add(item.Key, item.Value);

            return result;
        }
        /// <summary>
        /// Returns the Long Name field from a column parameter.
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
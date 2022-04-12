using MergePowerData.CIAFdata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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
        public const double KgU235PerTWh = 0.024;

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
            var electricByType = new ElectricUtilization(c);
            var elcity = c.Electric.Electricity;

            try
            {
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
                    case "eprodekg":
                        result = electricByType.ElectricProduction.KiloGram;
                        break;
                    case "econs":
                        result = c.Electric.ConsTWh * TWh2Kg;
                        break;
                    case "eprodtwh":
                        result = electricByType.ElectricProduction.TeraWattHours;
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

                    #region Electric by Type
                    // Electric By Type

                    case "eutilization": // should be fraction between 0 -.9999999 
                        result = electricByType.CountryUtilization;
                        break;

                    case "electricinstalledcapacityekg":
                        result = electricByType.InstalledCapacity.KiloGram;
                        break;

                    case "sumsrcpct":
                        result = electricByType.SumSourceFraction;
                        break;

                    case "sumsourcecapacity":
                        result = electricByType.SumSourceCapacity.TeraWattHours;
                        break;

                    case "sumsourceutilization":
                        result = electricByType.SumSourceUtilization.TeraWattHours;
                        break;

                    // fossil

                    case "capffkg":
                        result = electricByType.Fossil.Capacity.KiloGram;
                        break;

                    case "capfftwh":
                        result = electricByType.Fossil.Capacity.TeraWattHours;
                        break;

                    case "utilfftwh":
                        result = electricByType.Fossil.Utilization.TeraWattHours;
                        break;

                    case "utilffekg":
                        result = electricByType.Fossil.Utilization.KiloGram;
                        break;

                    case "pctcapfossil":
                        result = electricByType.Fossil.Percent;
                        break;

                    // nuclear
                    case "capnucleartwh":
                        result = electricByType.Nuclear.Capacity.TeraWattHours;
                        break;

                    case "utilnucleartwh":
                        result = electricByType.Nuclear.Utilization.TeraWattHours;
                        break;

                    case "capnuclearekg":
                        result = electricByType.Nuclear.Capacity.KiloGram;
                        break;

                    case "utilnuclearekg":
                        result = electricByType.Nuclear.Utilization.KiloGram;
                        break;

                    case "pctcapnuclear":
                        result = electricByType.Nuclear.Percent;
                        break;

                    // hydro
                    case "caphydrotwh":
                        result = electricByType.HydroElectric.Capacity.TeraWattHours;
                        break;
                    
                    case "pctcaphydro":
                        result = electricByType.HydroElectric.Percent;
                        break;

                    case "caphydrokg":
                        result = electricByType.HydroElectric.Capacity.KiloGram;
                        break;

                    case "utilhydrotwh":
                        result = electricByType.HydroElectric.Utilization.TeraWattHours;
                        break;

                    case "utilhydroekg":
                        result = electricByType.HydroElectric.Utilization.KiloGram;
                        break;
                    // renew
                    case "caprenewkg":
                        result = electricByType.Renewable.Capacity.KiloGram;
                        break;

                    case "pctcaprenew":
                        result = electricByType.Renewable.Percent;
                        break;

                    case "utilrenewtwh":
                        result = electricByType.Renewable.Utilization.TeraWattHours;
                        break;

                    case "utilrenewekg":
                        result = electricByType.Renewable.Utilization.KiloGram;
                        break;


                    case "caprenewtwh":
                        result = electricByType.Renewable.Capacity.TeraWattHours;
                        break;
                    
                    case "utilizationrenewtwh":
                        result = electricByType.Renewable.Utilization.TeraWattHours;
                        break;
                    #endregion Electric by type

                    // fossil OIL production
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
                    case "pcoilgdp":  // Oil production vs GDP - Annual
                        result = c.FossilFuelDetail.CrudeOil.Production.Value * 50 * 365.242198781 /
                            c.PurchasePower.value * 100;
                        break;


                    case "prodkwhgdp":
                        result = c.Electric.ProdKWh / c.PurchasePower.value;
                        break;

                    //case "pctcaprenewxxx":
                    //    result = (c.Electric.Electricity.by_source.other_renewable_sources.percent / 100) * .08;
                    //    break;




                    // Uranium
                    case "kwu235":
                        result = c.Electric.Electricity.by_source.nuclear_fuels.percent / 100 * c.Electric.ProdKWh /
                                 KgU235PerkWh;
                        break;

                    case "ukgburned":
                        if (c.Electric.Electricity.by_source.nuclear_fuels.percent == 0 || igc == null)
                        {
                            result = double.NaN;
                        }
                        else
                        {
                            var twhNuclear =
                                igc.YearCapTWhrByPercent(c.Electric.Electricity.by_source.nuclear_fuels.percent);

                            result = twhNuclear;
                        }

                        break;
                    default:
                        throw new ArgumentException($"Undefined {key}");
                }
            }
            catch (Exception error)
            {
                Console.WriteLine($"Country; '{c.Name}', Data: {key}, Error: {error.GetType()}");
                throw;
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
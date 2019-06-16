﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using MergePowerData.CIAFdata;
using MergePowerData.Report;

namespace MergePowerData
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Intel
    {
        private const double TWh2kg = 0.040055402; // TWh = 0.040055402 kg

        private const double Tera = 1.0e12;
        private const double Giga = 1.0e9;
        private const double Mega = 1.0e6;
        private const double Kilo = 1.0e3;

        private const double kgU245perkWh = 24.0e6;
        //private const double TwentyMtonTnt = 0.93106557; //
        //20 Mton_e = 0.93106557 kg, ~1 kg
        //private const double TWh2MTonTnt = 0.86042065; // TWh = 0.86042 M ton Tnt
        //private const double TWh2PJ = 3.6; // TWh = 3.6 PJ (Peta Joule's)
        private Country _world;

        // CIAF is a collection of data by country.  To analyze a specific set of data into a report we must extract a relevant subset 
        private readonly List<Country> _countries = new List<Country>();

        public void Add(string name, Electric electric, FossilFuelDetail ff, Gdp gdp, long pop)
        {
            if (name.Equals("World"))
                _world = new Country(name, electric, ff, gdp, pop);

            _countries.Add(new Country(name, electric, ff, gdp, pop));
        }

        public void CsvReport()
        {
            var dv = "\t";
            // header
            Console.WriteLine(
                 //"ProdTWh{dv}"-
                 $"ekg{dv}"
                 + $"eFFkg{dv}"
                 + $"U235kg{dv}"
                 + $"FuelMbl{dv}"
                 + $"NatGasGcm{dv}"
                 + $"Co2Tton{dv}"
                //+ "maxFF_kWh"
                //+ "kw/pop{dv}"
                //+ "pop_M{dv}"
                //+ "kWh/pop{dv}"
                //+ "Prod_FF_TWh{dv}"
                //+ "Capacity_TWh/y{dv}"
                //+ "$Growth{dv}"
                + $"$PP{dv}"
                //+ "$PP/TWh{dv}"
                + $"Country");

            var statEnergy = new Statistic();
            var statEmissions = new Statistic();
            var statFuel = new Statistic();
            var statGrowth = new Statistic();
                

            foreach (var c in _countries.OrderByDescending(d => d.Electric.ProdTWh))
            {
                var kgEfossil = c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh * TWh2kg;
                var wrldKgEfossil = _world.Electric.Electricity.by_source.fossil_fuels.percent / 100 *
                                    _world.Electric.ProdTWh * TWh2kg;

                var igc = c.Electric.Electricity.installed_generating_capacity;
                if (igc == null) continue;
                var currentCap = igc.YearCapacityTWhr();
                var futureCap50Y = igc.YearCapacityTWhr() * 2;
                if (c.PurchasePower.value / Giga < 2000)
                    continue;

                statEmissions.Add(c.Electric.TtonCo2, c.PurchasePower.value / Giga);
                statEnergy.Add(c.Electric.ConsTWh * TWh2kg, c.PurchasePower.value / Giga);
                statFuel.Add(x: c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / Mega, y: c.PurchasePower.value / Giga);
                statGrowth.Add(x: c.GrowthRate.value, y: c.PurchasePower.value / Giga);


                // Note; Working on making sense of Economics relative to use of FF and electricity.
                // Note; Need to bring in oil usage numbers
                Console.WriteLine(
                    // $"{c.Electric.ProdTWh}{dv}"
                    $"{c.Electric.ProdTWh * TWh2kg:F1}{dv}"
                    + $"{c.Electric.Electricity.by_source.fossil_fuels.percent / 100 * c.Electric.ProdTWh * TWh2kg:F1}{dv}"
                    + $"{c.Electric.Electricity.by_source.nuclear_fuels.percent / 100 * c.Electric.ProdKWh / kgU245perkWh * 1.6:F1}{dv}" // 1.6 is power xfer loss estimate
                    + $"{c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / Mega:F2}{dv}"
                    + $"{c.FossilFuelDetail.NaturalGas.Consumption.Value / Giga:F1}{dv}"
                    //+ $"{c.Pop / 1e6:F0}{dv}"
                    //+ $"{c.Electric.ProdKWh / c.Pop:F0}{dv}"
                    //+ $"{c.Electric.ProdTWh / _world.Electric.ProdTWh:F3}{dv}"
                    //+ $"{kgEfossil / wrldKgEfossil:F3}{dv}"
                    //$"{c.Electric.MtonCo2 / c.Pop:F2}{dv}"
                    //$"{c.Electric.TtonCo2 / _world.Electric.TtonCo2:F3}{dv}" // want amtCo2 per TW consumed Fossil
                    //  Emissions Co2
                    + $"{c.Electric.TtonCo2:F0}{dv}"
                    //+ $"{c.Electric.Electricity.by_source.nuclear_fuels.percent}{dv}"
                    //+ $"{c.Electric.Electricity.by_source.hydroelectric_plants.percent}{dv}"
                    //+ $"{c.Electric.Electricity.by_source.fossil_fuels.percent}{dv}"
                    //+ $"{c.Electric.Electricity.by_source.other_renewable_sources.percent}{dv}"
                    //+ $"{igc.YearCapacityTWhr():F0}{dv}"
                    //+ $"{igc.YearCapacityTWhr() / c.Electric.ProdTWh:F2}{dv}"
                    + $"{c.PurchasePower.value / Giga:F0}{dv}"
                    //+ $"{c.GrowthRate.value:F1}{dv}"
                    //+ $"{c.GrowthRate.date}{dv}"
                    //+ $"{c.PurchasePower.value / _world.PurchasePower.value:F3}{dv}"
                    //+ $"{c.PurchasePower.value}{dv}"
                    //+ $"{c.PurchasePower.value / c.Electric.ProdTWh:F2}{dv}"
                    //+ $"{c.FossilFuelDetail.RefinedPetroleum.Consumption.Value:F0}{dv}"
                    // + $"{c.FossilFuelDetail.RefinedPetroleum.Production.Value:F0}{dv}"
                    //+ $"{c.FossilFuelDetail.CrudeOil.Exports.Value / c.FossilFuelDetail.CrudeOil.Production.Value:F2}{dv}"
                    //+ $"{c.FossilFuelDetail.RefinedPetroleum.Consumption.Value / _world.FossilFuelDetail.RefinedPetroleum.Consumption.Value:F5}{dv}"
                    //+$"{c.FossilFuelDetail.RefinedPetroleum.Consumption.Date}{dv}"
                    + $"{c.Name}");
            }

            Console.WriteLine("Elec: " + statEnergy.ToString());
            Console.WriteLine("Fuel: " + statFuel.ToString());
            Console.WriteLine("CO2L: " + statEmissions.ToString());
            Console.WriteLine("Growth: " + statGrowth.ToString()); // test, I expect no correlation
        }

        public void PdfReport()
        {
            var pdf = new PowerPdf(_world, _countries);

            var path = Environment.CurrentDirectory;

            var stream = new FileStream(path + "/EnergyUseReport.pdf", FileMode.Create);

            pdf.Create(stream);

            Process.Start(path + "/EnergyUseReport.pdf");

        }
    }


}

using MergePowerData.IntelMath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using InstalledGeneratingCapacity = MergePowerData.CIAFdata.InstalledGeneratingCapacity;

namespace CountryTest;

[TestClass]
public class EnergyUtilizationTest1
{
    [TestMethod]
    public void EEnergy()
    {
        var energy = new EEnergy { TeraWattHours = 5 };
        Assert.IsNotNull(energy);
        Assert.AreEqual(5, energy.TeraWattHours);
        Assert.AreEqual(0.20027700999999998, energy.KiloGram);
    }

    [TestMethod]
    public void ElectricUtilization()
    {
        // get set up data
        var today = DateTime.Now.ToShortDateString();

        InstalledGeneratingCapacity igc =
            new InstalledGeneratingCapacity
            {
                date = today,
                global_rank = 99,
                kW = 2.8480441e+09 * 2 // 118668.51 MW installed capacity =1000 kg / year
                /*
                 * You have: 1Mg / year  i.e. 1000 kg per year energy
                 *  1Mg / year = 2.8480441e+09 kW
                 *  1Mg / year = (1 / 3.5111815e-10) kW
                 *  1Mg / year = 2.848 TW installed capacity
                 */
            };

        EEnergy countryElectricProd = new EEnergy { KiloGram = 1000 }; // Energy / year in kg

        // by definition the sum of percents should be 100
        EnergySource fossil = new() { date = today, global_rank = 99, percent = 40 };
        EnergySource hydro = new() { date = today, global_rank = 99, percent = 30 };
        EnergySource nuclear = new() { date = today, global_rank = 99, percent = 20 };
        EnergySource renewable = new() { date = today, global_rank = 99, percent = 10 };

        var testme = new ElectricUtilization(
            igc,
            countryElectricProd.TeraWattHours,
            fossil,
            hydro,
            nuclear,
            renewable);

        Assert.IsNotNull(testme);

        // Expect the sum of all source percentages to equal 1, by definition
        Assert.AreEqual(1, testme.SumSourceFraction, 0.01);

        // Expect the sum of all source capacity to be equal to the Installed capacity
        Assert.AreEqual(2000, testme.SumSourceCapacity.KiloGram, .1);

        // Expect sum of all source utilization to be equal to the total production value
        Assert.AreEqual(1000, testme.SumSourceUtilization.KiloGram, 0.01);
    }
}
using MergePowerData.IntelMath;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UtilizationTests
{
    [TestClass]
    public class UtilizationTests
    {
        [TestMethod]
        public void InstantiateEEnergy()
        {
            var result = new EEnergy();

            Assert.IsNotNull(result);
        }
    

    [TestMethod]
    public void ElectricUtilization()
    {
        var result = new ElectricUtilization(null);


    }}
}
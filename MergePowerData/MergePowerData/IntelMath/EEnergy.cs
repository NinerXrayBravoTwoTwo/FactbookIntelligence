namespace MergePowerData.IntelMath
{
    /// <summary>
    ///     I am energy
    ///     I am mass
    ///     I can represent myself in either units of energy or mass they are equal to each other
    /// </summary>
    public class EEnergy
    {
        public double KiloGram { get; set; }

        public double TeraWattHours
        {
            get => KiloGram / IntelCore.TWh2Kg;
            set => KiloGram = value * IntelCore.TWh2Kg;
        }
    }
}
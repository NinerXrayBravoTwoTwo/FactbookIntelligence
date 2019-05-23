namespace MergePowerData
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var report = new AnalysisCapacity("../../../../../cia_world_factbook_api/data/" + "factbook.json");
            report.ElectricReport();
        }
    }
}
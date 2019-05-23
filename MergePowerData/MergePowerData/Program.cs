namespace MergePowerData
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var report = new AnalysisCapacity("/Users/Jillian England/source/repos/cia_world_factbook_api/data/" + "factbook.json");
            report.ElectricReport();
        }
    }
}
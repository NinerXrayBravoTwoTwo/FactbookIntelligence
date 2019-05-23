namespace MergePowerData
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Currently getting factbook data from Ian's Factbook project
            // https://github.com/iancoleman/cia_world_factbook_api

            var report = new AnalysisCapacity("../../../../../cia_world_factbook_api/data/" + "factbook.json");
            report.ElectricReport();
        }
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MergePowerData.CIAFdata
{
    public class Electric
    {
        public double TtonCo2;
        public double ConsKWh;
        public double ConsTWh;
        public double KWfossil;
        public double KWhydro;
        public double KWnuke;
        public double KWrenew;
        public double PrcntTtl;
        public double ProdKWh;
        public double ProdTWh;

        public Electric(JToken energy)
        {
            Electricity = JsonConvert.DeserializeObject<Electricity>(energy["electricity"].ToString());

            if (Electricity.by_source == null)
                Electricity.by_source = new BySource();

            {
                if (Electricity.by_source.nuclear_fuels == null)
                    Electricity.by_source.nuclear_fuels = new NuclearFuels { percent = 0, global_rank = int.MaxValue };
                if (Electricity.by_source.hydroelectric_plants == null)
                    Electricity.by_source.hydroelectric_plants = new HydroelectricPlants { percent = 0, global_rank = int.MaxValue };
                if (Electricity.by_source.fossil_fuels == null)
                    Electricity.by_source.fossil_fuels = new FossilFuels { percent = 0, global_rank = int.MaxValue };
                if (Electricity.by_source.other_renewable_sources == null)
                    Electricity.by_source.other_renewable_sources = new OtherRenewableSources { percent = 0, global_rank = int.MaxValue };
            }

            // electricity = JsonConvert.DeserializeObject<Electricity>(el);
            var co2Json = energy["carbon_dioxide_emissions_from_consumption_of_energy"];
            TtonCo2 = co2Json?["megatonnes"].Value<double>() ?? 0;
            TtonCo2 /= 1.0e6; // convert M ton to T ton

            PrcntTtl += Electricity.by_source.nuclear_fuels.percent;
            KWnuke = Electricity.by_source.nuclear_fuels.KWh(Electricity.installed_generating_capacity);
            PrcntTtl += Electricity.by_source.hydroelectric_plants.percent;
            KWhydro = Electricity.by_source.hydroelectric_plants.KWh(Electricity.installed_generating_capacity);
            PrcntTtl += Electricity.by_source.fossil_fuels.percent;
            KWfossil = Electricity.by_source.fossil_fuels.KWh(Electricity.installed_generating_capacity);
            PrcntTtl += Electricity.by_source.other_renewable_sources.percent;
            KWrenew = Electricity.by_source.other_renewable_sources.KWh(Electricity.installed_generating_capacity);

            ProdTWh = Electricity?.production?.TWh ?? 0;
            ConsTWh = Electricity?.consumption?.TWh ?? 0;
            ProdKWh = Electricity?.production?.kWh ?? 0;
            ConsKWh = Electricity?.consumption?.kWh ?? 0;
        }

        public Electricity Electricity { get; set; }
    }
}
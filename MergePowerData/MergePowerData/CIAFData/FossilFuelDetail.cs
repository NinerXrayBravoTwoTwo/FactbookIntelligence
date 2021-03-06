﻿using Newtonsoft.Json.Linq;

namespace MergePowerData.CIAFdata
{
    public class FossilFuelDetail
    {
        public readonly CrudeOil CrudeOil;
        public readonly NaturalGas NaturalGas;
        public readonly RefinedPetroleum RefinedPetroleum;

        public FossilFuelDetail(JToken energy)
        {
            CrudeOil = new CrudeOil(energy["crude_oil"]);
            RefinedPetroleum = new RefinedPetroleum(energy["refined_petroleum_products"]);
            NaturalGas = new NaturalGas(energy["natural_gas"]);
        }
    }

    /*  There is no coal section in CIAF did find the following note regarding this omission;

        Electricity - from fossil fuels Electricity - from fossil fuels field listing
        This entry measures the capacity of plants that generate electricity by burning fossil fuels 
        (such as coal, petroleum products, and natural gas), expressed as a share of the country's total generating capacity.
     */

    public class CrudeOil
    {
        public readonly ValueDate Exports;
        public readonly ValueDate Imports;

        public readonly ValueDate Production;
        public readonly ValueDate ProvedReserves;

        public CrudeOil(JToken token)
        {
            if (token == null)
            {
                Production = ValueDate.Empty;
                Exports = ValueDate.Empty;
                Imports = ValueDate.Empty;
                ProvedReserves = ValueDate.Empty;

                return;
            }

            Production = new ValueDate(token["production"], "bbl_per_day");
            Exports = new ValueDate(token["exports"], "bbl_per_day");
            Imports = new ValueDate(token["imports"], "bbl_per_day");
            ProvedReserves = new ValueDate(token["proved_reserves"], "bbl");

            //System.Console.WriteLine($"{token.ToString(Newtonsoft.Json.Formatting.Indented)}");
        }
    }

    public class RefinedPetroleum
    {
        public readonly ValueDate Production;
        public readonly ValueDate Consumption;
        public readonly ValueDate Exports;
        public readonly ValueDate Imports;

        public RefinedPetroleum(JToken token)
        {
            // Console.WriteLine($"{token.ToString(Newtonsoft.Json.Formatting.Indented)}");

            if (token == null)
            {
                Production = ValueDate.Empty;
                Exports = ValueDate.Empty;
                Imports = ValueDate.Empty;
                Consumption = ValueDate.Empty;

                return;
            }

            Production = new ValueDate(token["production"], "bbl_per_day");
            Consumption = new ValueDate(token["consumption"], "bbl_per_day"); // Trap null exception
            Imports = new ValueDate(token["imports"], "bbl_per_day");
            Exports = new ValueDate(token["exports"], "bbl_per_day");
        }
    }

    public class NaturalGas
    {
        public readonly ValueDate Consumption;
        public readonly ValueDate Exports;
        public readonly ValueDate Imports;

        public readonly ValueDate Production;
        public readonly ValueDate ProvedReserves;

        public NaturalGas(JToken token)
        {
            // Console.WriteLine($"{token.ToString(Newtonsoft.Json.Formatting.Indented)}");

            if (token == null)
            {
                Production = ValueDate.Empty;
                Consumption = ValueDate.Empty;
                Imports = ValueDate.Empty;
                Exports = ValueDate.Empty;
                ProvedReserves = ValueDate.Empty;
                return;
            }

            Production = new ValueDate(token["production"], "cubic_metres");
            Consumption = new ValueDate(token["consumption"], "cubic_metres");
            Imports = new ValueDate(token["imports"], "cubic_metres");
            Exports = new ValueDate(token["exports"], "cubic_metres");
            ProvedReserves = new ValueDate(token["proved_reserves"], "cubic_metres");
        }
    }

    public class ValueDate
    {
        public ValueDate()
        {
            Value = 0;
            Date = string.Empty;
        }

        public ValueDate(JToken token, string name)
        {
            if (token == null)
            {
                Value = 0;
                Date = string.Empty;
                return;
            }

            Value = token[name].Value<double>();
            Date = token["date"] != null ? token["date"].Value<string>() : string.Empty;
        }

        public static ValueDate Empty => new ValueDate();
        public double Value { get; set; }
        public string Date { get; set; }
    }
}
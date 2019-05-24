using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MergePowerData.CIAFData;

namespace MergePowerData
{

    // <summary>
    //  I am a report in PDF format
    //  I accept data from CIAF about countries
    //  I produce a report about countries use of energy and their related economies.
    //  I produce a report regarding CO2 production and Fossil fuel use by country.
    // o Reports are ordered by Electricity usage by country.
    // </summary>
    public class PowerPdf
    {
        private readonly List<Country> _data;
        private readonly Country _world;

        public PowerPdf(Country world, List<Country> data)
        {
            _world = world;
            _data = data;
        }
    }
}

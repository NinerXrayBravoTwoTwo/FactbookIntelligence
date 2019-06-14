using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MergePowerData.CIAFdata;

namespace MergePowerData.Report
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

        public void Create(Stream stream)
        {
            const float margin = 10f;
            const float marginTop = 60f;

            //var headerEvents = new PdfReportEvents(PdfReportData reportData);

            var doc = new Document(PageSize.A4.Rotate(), margin, margin, marginTop, margin);
            doc.AddAuthor("Boo:");
            doc.AddCreationDate();
            doc.AddTitle("boohoo");

            var writer = PdfWriter.GetInstance(doc, stream);
            //writer.PageEvent = headerEvents;

            doc.Open();

           // Document(doc, stream, writer);

            doc.Close();
        }

    }
}

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
        private readonly PdfReportData _reportData;

        public PowerPdf(Country world, List<Country> countries)
        {

            _reportData = new PdfReportData(world, countries);
        }

        public void Create(Stream stream)
        {
            const float margin = 10f;
            const float marginTop = 60f;

            var headerEvents = new PdfReportEvents(_reportData);

            var doc = new Document(PageSize.A4.Rotate(), margin, margin, marginTop, margin);
            doc.AddAuthor("Boo:");
            doc.AddTitle("boohoo");
            doc.AddCreationDate();

            var writer = PdfWriter.GetInstance(doc, stream);
            // writer.PageEvent = headerEvents;

            doc.Open();

            Document(doc, stream, writer);

            doc.Close();
        }

        protected void Document<T>(Document doc, T stream, PdfWriter writer) where T : Stream
        {

            doc.Add(BodyRow(doc));

            doc.Add(new Phrase(Chunk.NEWLINE)
            { Font = FontFactory.GetFont(FontFactory.HELVETICA, 6) });
        }

        private Phrase NumberCell(double value)
        {
            var phrase = new Phrase
            {
                Font = FontFactory.GetFont(FontFactory.HELVETICA, 8, Font.NORMAL)
            };

            phrase.Add(Chunk.NEWLINE);
            phrase.Add(new Chunk(content: $"{value:F3}"));
            phrase.Add(Chunk.NEWLINE);

            return phrase;
        }

        private PdfPTable BodyRow(Document doc)
        {
            var table = new PdfPTable(new[] { 1f, 1f, 1f, 1f, 1f })
            {
                TotalWidth = doc.PageSize.Width - 2 * doc.LeftMargin,
                LockedWidth = true,
                SpacingAfter = 2,
                SpacingBefore = 2,
                HorizontalAlignment = Element.ALIGN_LEFT
            };

            table.AddCell(new PdfPCell(NumberCell(111)) { BorderWidth = 0 });
            table.AddCell(new PdfPCell(NumberCell(222)) { BorderWidth = 0 });
            table.AddCell(new PdfPCell(NumberCell(333)) { BorderWidth = 0 });
            table.AddCell(new PdfPCell(NumberCell(444)) { BorderWidth = 0 });
            table.AddCell(new PdfPCell(NumberCell(555)) { BorderWidth = 0 });


            return table;
        }
    }
}
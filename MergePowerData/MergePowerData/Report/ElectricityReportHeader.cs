using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.xml.xmp;

namespace MergePowerData.Report
{
    public class ElectricityReportHeader
    {

        public ElectricityReportHeader(PdfReportData reportData)
        {
            ReportData = reportData;
        }

        public PdfReportData ReportData { get; set; }

        private PdfPCell ReportTitle()
        {
            var phrase = new Phrase { Font = FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.BOLD) };

            phrase.Add(new Chunk("Energy Intelligence Report"));

            var result = new PdfPCell(phrase)
            {
                BorderWidth = 0,
                HorizontalAlignment = Element.ALIGN_LEFT
            };

            return result;
        }

        public PdfPTable TopRow(Document doc, int pagecount)
        {
            var result = new PdfPTable(new[] {1.0f})
            {
                TotalWidth = doc.PageSize.Width - 2 * doc.LeftMargin,
                LockedWidth = true,
                SpacingAfter = 2,
                SpacingBefore = 2,
                HorizontalAlignment = Element.ALIGN_LEFT
            };

            result.AddCell(ReportTitle());
            //result.AddCell() // Description

            return result;

        }
    }
}

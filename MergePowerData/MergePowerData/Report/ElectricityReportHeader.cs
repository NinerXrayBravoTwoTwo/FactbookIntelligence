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
            var phrase = new Phrase { Font = FontFactory.GetFont(FontFactory.HELVETICA, 10, Font.BOLD, BaseColor.BLUE) };

            phrase.Add(new Chunk("Energy Intelligence Report"));
            phrase.Add(new Chunk(" "));
            phrase.Add(new Chunk(content: "PDF Testing"));
            var result = new PdfPCell(phrase)
            {
                BorderWidth = 0,
                HorizontalAlignment = Element.ALIGN_CENTER
            };

            return result;
        }

        public PdfPTable TopRow(Document doc, int pageCount)
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

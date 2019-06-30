using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MergePowerData.Report
{
    public class PdfReportEvents : PdfPageEventHelper
    {
        private readonly PdfReportData _reportData;

        private int _pageCount;

        public PdfReportEvents(PdfReportData reportData)
        {
            _reportData = reportData;
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            _pageCount++;
            var header = new ElectricityReportHeader(_reportData);
            var table = header.TopRow(document, _pageCount);
            table.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - 10, writer.DirectContent);
        }
    }
}
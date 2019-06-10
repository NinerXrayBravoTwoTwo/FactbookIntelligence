using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MergePowerData.Report
{
    public class ReportEvents : PdfPageEventHelper
    {
        private readonly PdfReportData _reportData;

        private int _pageCount;

        public ReportEvents(PdfReportData reportData)
        {
            _reportData = reportData;
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            _pageCount++;

            var header = new ElectricityReportHeader(_reportData);

           // var table = header.TopRow(document, _pageCount, _reportData.PageEstimate());

           // table.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - 10, writer.DirectContent);
        }
    }
}
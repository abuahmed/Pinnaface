using System;
using System.Drawing.Printing;
using CrystalDecisions.CrystalReports.Engine;
using System.Windows.Forms;
using PinnaFace.WPF.Properties;

namespace PinnaFace.WPF.Reports
{
    public class ReportUtility
    {
        ReportDocument _crReportDocument;
        public bool DirectPrinter(ReportDocument cReport)
        {
            try
            {
                var printDialog = new PrintDialog();
                var printDocument = new PrintDocument();

                printDialog.Document = printDocument;
                printDialog.AllowSomePages = true;
                printDialog.AllowCurrentPage = true;

                var dialogue = printDialog.ShowDialog();
                if (dialogue == DialogResult.OK)
                {
                    int nCopy = printDocument.PrinterSettings.Copies;
                    var sPage = printDocument.PrinterSettings.FromPage;
                    var ePage = printDocument.PrinterSettings.ToPage;
                    var printerName = printDocument.PrinterSettings.PrinterName;
                    _crReportDocument = new ReportDocument();
                    _crReportDocument = cReport;
                    try
                    {
                        _crReportDocument.PrintOptions.PrinterName = printerName;
                        _crReportDocument.Refresh();
                        _crReportDocument.PrintToPrinter(nCopy, false, sPage, ePage);
                        return true;
                    }
                    catch
                    {
                        MessageBox.Show(Resources.Error_Printing_Document);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException,"Printing Error");
            }
            return false;
        }
    }
}

using CrystalDecisions.CrystalReports.Engine;
using MessagingToolkit.QRCode.Codec;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mapster;
using Uni.Scan.Reports.Helper;
using Uni.Scan.Reports.Models;

namespace Uni.Scan.Reports.Controllers
{
    public class LogisticController : Controller
    {
        [HttpPost]
        public ActionResult PrintLabel(LogisticPrintRequest request)
        {
            var reportPath = Path.Combine(Server.MapPath($@"~/Reports/Labels/{request.ModelId}.rpt"));

            ReportDocument rd = new ReportDocument();
            rd.Load(reportPath);

            var printlist = new List<LogisticPrintRequestLine>();

            foreach (var item in request.Labels)
            {
                item.UniqueIdBarcode = BarCodeConverter.GetBarcodeImage(item.UniqueId, item.UniqueIdBarcodeType);
                item.IdentifiedStockBarcode = BarCodeConverter.GetBarcodeImage(item.IdentifiedStock, item.IdentifiedStockBarcodeType);
                item.ProductIdBarcode = BarCodeConverter.GetBarcodeImage(item.ProductId, item.ProductIdBarcodeType);

                for (int i = 0; i < item.NbrEtiquettes; i++)
                {
                    printlist.Add(item.Adapt<LogisticPrintRequestLine>());
                }
            }

            rd.SetDataSource(printlist);
            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", request.ModelId.ToUpper() + DateTime.Now.ToString(" yy-MM-dd hhmmss") + ".pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [HttpPost]
        public ActionResult PrintTask(LogisticPrintRequest request)
        {
            var reportPath = Path.Combine(Server.MapPath($@"~/Reports/Tasks/{request.ModelId}.rpt"));

            ReportDocument rd = new ReportDocument();
            rd.Load(reportPath);


            request.TaskIdBarcode = BarCodeConverter.GetBarcodeImage(request.TaskId, request.TaskIdBarcodeType);
            foreach (var item in request.Labels)
            {
                item.UniqueIdBarcode = BarCodeConverter.GetBarcodeImage(item.UniqueId, item.UniqueIdBarcodeType);
                item.IdentifiedStockBarcode = BarCodeConverter.GetBarcodeImage(item.IdentifiedStock, item.IdentifiedStockBarcodeType);
                item.ProductIdBarcode = BarCodeConverter.GetBarcodeImage(item.ProductId, item.ProductIdBarcodeType);
            }


            try
            {
                rd.SetDataSource(new List<LogisticPrintRequest>() { request });
                //rd.Database.Tables["Uni_Scan_Reports_Models_LogisticPrintRequest"].SetDataSource(request);
                //rd.Database.Tables["Uni_Scan_Reports_Models_LogisticPrintRequestLine"].SetDataSource(request.Labels);
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", request.ModelId.ToUpper() + DateTime.Now.ToString(" yy-MM-dd hhmmss") + ".pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
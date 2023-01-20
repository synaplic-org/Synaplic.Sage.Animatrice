using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using BarcodeLib;
using CrystalDecisions.CrystalReports.Engine;
using Mapster;
using MessagingToolkit.QRCode.Codec;
using Uni.Scan.Reports.Models;

namespace Uni.Scan.Reports.Controllers
{
    public class PdfController : Controller
    {
        [HttpPost]
        public ActionResult PrintLabel(LabelPrintRequest request)
        {
            var reportPath = Path.Combine(Server.MapPath($@"~/Reports/{request.ModelName}.rpt"));


            ReportDocument rd = new ReportDocument();
            rd.Load(reportPath);


            var printlist = new List<Label>();
            foreach (var item in request.Lables)
            {
                // to do generate codebarimages 
                var copies = (item.Duplicata * item.NbrEtiquettes) + item.Supplement;
                if (request.ModelName.Equals("SITI-SEMI-FINI"))
                {
                    string yourcode = item.ProductId + "#" + item.IdentifiedStock + "#" + item.QuatityOnLabel + item.QuatityUnite;
                    QRCodeEncoder enc = new QRCodeEncoder();
                    enc.QRCodeScale = 1;
                    Bitmap grcode = enc.Encode(yourcode);
                    var img = grcode as Image;
                    MemoryStream fs = new MemoryStream();
                    ((Bitmap)img).Save(fs, ImageFormat.Jpeg);
                    byte[] retval = fs.ToArray();
                    fs.Dispose();
                    item.barcode = retval;
                }

                //generate code128
                if (request.ModelName.Equals("SITI-STANDARD"))
                {
                    BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                    if (item.IdentifiedStock != null)
                    {
                        var img2 = b.Encode(BarcodeLib.TYPE.CODE128, item.IdentifiedStock, 281, 34);
                        item.IdentifiedStockBarcode = ImageToByteArray(img2);
                    }

                    if (item.IdentifiedStock != null)
                    {
                        var img2 = b.Encode(BarcodeLib.TYPE.CODE128, item.IdentifiedStock, 281, 34);
                        item.IdentifiedStockBarcode = ImageToByteArray(img2);
                    }

                    if (item.ProductId != null)
                    {
                        var img1 = b.Encode(BarcodeLib.TYPE.CODE128, item.ProductId, Color.Black, Color.White, 376, 34);
                        item.ProductIdBarcode = ImageToByteArray(img1);
                    }
                }


                for (int i = 0; i < copies; i++)
                {
                    var lbl = item.Adapt(new Label());
                    //set codebarimages 
                    printlist.Add(lbl);
                }
            }

            rd.SetDataSource(printlist);

            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);

                return File(stream, "application/pdf", request.ModelName.ToUpper() + ".pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        [HttpPost]
        public ActionResult PrintTask(TaskPrintRequest request)
        {
            var reportPath = Path.Combine(Server.MapPath($@"~/Reports/{request.ModelName}.rpt"));
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            string TaskId = request.Id;
            var img1 = b.Encode(BarcodeLib.TYPE.CODE128, TaskId, Color.Black, Color.White, 376, 34);

            request.TaskIdBarcode = ImageToByteArray(img1);

            ReportDocument rd = new ReportDocument();
            rd.Load(reportPath);

            rd.SetDataSource(new[] { request });

            if (!request.ModelName.Equals("SITI-TASK"))
            {
                List<TaskLine> _listTaskLineDetails = new List<TaskLine>();
                foreach (var list in request.TaskLineDetails)
                {
                    string ProductId = list.ProductID;
                    var img = b.Encode(BarcodeLib.TYPE.CODE128, ProductId, Color.Black, Color.White, 376, 34);


                    list.ProductIDBarcode = ImageToByteArray(img);

                    _listTaskLineDetails.Add(list);
                }

                rd.Subreports[0].SetDataSource(_listTaskLineDetails);
            }


            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);

                return File(stream, "application/pdf", request.ModelName.ToUpper() + ".pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [HttpPost]
        public ActionResult PrintAreaLabel(AreaLabel request)
        {
            var reportPath = Path.Combine(Server.MapPath($@"~/Reports/{request.ModelName}.rpt"));

            ReportDocument rd = new ReportDocument();
            rd.Load(reportPath);

            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            string logisticId = request.LogisticAreaID;
            var img1 = b.Encode(BarcodeLib.TYPE.CODE128, logisticId, Color.Black, Color.White, 376, 34);

            request.LogisticAreaIDBarcode = ImageToByteArray(img1);
            rd.SetDataSource(new[] { request });

            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);

                return File(stream, "application/pdf", request.ModelName.ToUpper() + ".pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [HttpPost]
        public ActionResult PrintZoneLogistic(ZoneLogisticObject request)
        {
            var reportPath = Path.Combine(Server.MapPath($@"~/Reports/{request.ModelName}.rpt"));
            List<ZoneLogistic> listzone = new List<ZoneLogistic>();
            ZoneLogistic zoneLogistic = new ZoneLogistic();

            ReportDocument rd = new ReportDocument();
            rd.Load(reportPath);
            if (request.ModelName.Equals("SITI-ZONE-LOGISTIQUE"))
            {
                for (int i = 0; i < request.zoneLogistic.Count; i++)
                {
                    zoneLogistic.zoneLogistic1 = request.zoneLogistic[i];
                    if ((i + 1) < request.zoneLogistic.Count)
                    {
                        zoneLogistic.zoneLogistic2 = request.zoneLogistic[i + 1];
                    }

                    if ((i + 2) < request.zoneLogistic.Count)
                    {
                        zoneLogistic.zoneLogistic3 = request.zoneLogistic[i + 2];
                    }

                    i += 2;
                    listzone.Add(zoneLogistic);
                    zoneLogistic = new ZoneLogistic();
                }
            }

            if (request.ModelName.Equals("SITI-ZONE-IMAGE"))
            {
                BarcodeLib.Barcode b = new BarcodeLib.Barcode();


                for (int i = 0; i < request.zoneLogistic.Count; i++)
                {
                    var img1 = b.Encode(BarcodeLib.TYPE.CODE128, request.zoneLogistic[i], Color.Black, Color.White, 376, 34);

                    zoneLogistic.zoneBarcode1 = ImageToByteArray(img1);
                    zoneLogistic.zoneLogistic1 = request.zoneLogistic[i];
                    if ((i + 1) < request.zoneLogistic.Count)
                    {
                        var img2 = b.Encode(BarcodeLib.TYPE.CODE128, request.zoneLogistic[i + 1], Color.Black, Color.White, 376, 34);

                        zoneLogistic.zoneBarcode2 = ImageToByteArray(img2);
                        zoneLogistic.zoneLogistic2 = request.zoneLogistic[i + 1];
                    }

                    if ((i + 2) < request.zoneLogistic.Count)
                    {
                        var img3 = b.Encode(BarcodeLib.TYPE.CODE128, request.zoneLogistic[i + 2], Color.Black, Color.White, 376, 34);

                        zoneLogistic.zoneBarcode3 = ImageToByteArray(img3);
                        zoneLogistic.zoneLogistic3 = request.zoneLogistic[i + 2];
                    }

                    i += 2;
                    listzone.Add(zoneLogistic);
                    zoneLogistic = new ZoneLogistic();
                }
            }


            rd.SetDataSource(listzone);

            try
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);

                return File(stream, "application/pdf", request.ModelName.ToUpper() + ".pdf");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
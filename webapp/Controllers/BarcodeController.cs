using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class BarcodeController : Controller
    {
        // GET: Barcode
        public ActionResult Home()
        {
           
            return View();
        }
        [HttpPost]
        public ActionResult Home(string barcode)
        {

            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                var qrCodedata = qrGenerator.CreateQrCode("efregf", QRCodeGenerator.ECCLevel.H);
                var qrcode = new QRCoder.QRCode(qrCodedata);
                var BarcodeImage = qrcode.GetGraphic(150);
                ViewBag.BarcodeImage = BarcodeImage;
                using (Bitmap bitMap = qrcode.GetGraphic(15))
                {
                    bitMap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
            return View();
        }
    }
}
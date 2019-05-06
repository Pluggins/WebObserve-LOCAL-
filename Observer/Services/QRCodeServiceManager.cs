using Observer.Models;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Observer.Services
{
    public static class QRCodeServiceManager
    {
        public static string GenerateLoginQRCode(string KeyLink)
        {
            string id = Guid.NewGuid().ToString();
            string Url = HttpRuntime.AppDomainAppPath;
            string fileName = "Content/QRCode/" + id + ".png";
            string fullPath = Path.Combine(Url, fileName);
            //string Url = "ftp://sg1-wss1%255Citzmoon@amazecraft.net/QRCode/" + id + ".png";
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(KeyLink, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Url);
            //request.Credentials = new NetworkCredential("sg1-wss1\\itzmoon", "pD74W87h^#");
            //request.Method = WebRequestMethods.Ftp.UploadFile;
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    qrCodeImage.Save(memory, ImageFormat.Png);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            //}
            ObserverDbContext _db = new ObserverDbContext();
            string url = _db.WebConfigs
                .Where(e => e.Key == "SITE_URL")
                .FirstOrDefault()
                .Value + "/Content/QRCode/" + id + ".png";

            return url;
        }
    }
}
using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Observer.Services
{
    public static class DocumentRenderServiceManager
    {
        public static string A4Html2Pdf(string Html)
        {
            string Url = HttpRuntime.AppDomainAppPath;
            string fileName = "Content/PDF/" + Guid.NewGuid().ToString() + ".pdf";
            string fullPath = Path.Combine(Url, fileName);
            HtmlToPdf convert = new HtmlToPdf();
            convert.Options.MarginBottom = 30;
            convert.Options.MarginTop = 30;
            convert.Options.MarginLeft = 30;
            convert.Options.MarginRight = 30;
            convert.Options.PdfPageSize = PdfPageSize.A4;
            SelectPdf.PdfDocument doc = convert.ConvertHtmlString(Html);

            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Url);
            //request.Credentials = new NetworkCredential("sg1-wss1\\itzmoon", "pD74W87h^#");
            //request.Method = WebRequestMethods.Ftp.UploadFile;
            byte[] pdf = doc.Save();
            Stream stream = new MemoryStream(pdf);
            
            using (FileStream outputStream = new FileStream(@fullPath, FileMode.OpenOrCreate))
            {
                stream.CopyTo(outputStream);
            }

            return fullPath;
        }
    }
}
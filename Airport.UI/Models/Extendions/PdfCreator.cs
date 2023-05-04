using iText.Html2pdf;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Airport.UI.Models.Extendions
{
    public class PdfCreator
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PdfCreator(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public void CreatePdfFromHtml(string htmlContent, string fileName)
        {
            var outputPath = Path.Combine(_hostingEnvironment.WebRootPath, "pdf", fileName);
            var pdfWriter = new PdfWriter(outputPath);
            var pdfDoc = new PdfDocument(pdfWriter);
            var converterProperties = new ConverterProperties();

            HtmlConverter.ConvertToPdf(htmlContent, pdfDoc, converterProperties);

            pdfDoc.Close();
        }
    }
}

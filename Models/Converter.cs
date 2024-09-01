using Microsoft.VisualBasic;
using Aspose.Words;
using System.IO;

namespace SchoolProject1640.Models
{
    public class Converter
    {
        public void ConvertToPdf(string docxFilePath, string pdfFilePath)
        {
            // Load the document
            Document doc = new Document(docxFilePath);

            // Save the document in PDF format
            doc.Save(pdfFilePath, SaveFormat.Pdf);
        }
    }
}

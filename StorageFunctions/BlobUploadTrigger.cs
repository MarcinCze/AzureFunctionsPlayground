using System;
using System.IO;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace StorageFunctions
{
    public static class BlobUploadTrigger
    {
        [FunctionName("BlobUploadTrigger")]
        public static void Run(
            [BlobTrigger("images-container/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, 
            string name, 
            ILogger log,
            [Blob("summary-container", Connection = "AzureWebJobsStorage")] CloudBlobContainer outputContainer)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var filename = $"{Guid.NewGuid()}.pdf";
            var writer = new PdfWriter(filename, new WriterProperties().SetPdfVersion(PdfVersion.PDF_2_0));
            var pdfDocument = new PdfDocument(writer);
            pdfDocument.SetTagged();
            
            var document = new Document(pdfDocument);
            document.Add(new Paragraph($"File: {name}"));

            ImageData imageData = ImageDataFactory.Create(ReadFully(myBlob));
            var image = new Image(imageData).ScaleToFit(500, 300);
            document.Add(image);

            document.Close();

            CloudBlockBlob cloudBlockBlob = outputContainer.GetBlockBlobReference(filename);
            cloudBlockBlob.UploadFromFile(filename);

            File.Delete(filename);

            log.LogInformation("Blob trigger finished");
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }
            return ms.ToArray();
        }
    }
}

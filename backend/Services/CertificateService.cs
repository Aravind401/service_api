using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ServiceApi.Models;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp;

namespace ServiceApi.Services;

public class CertificateService
{
    public byte[] GenerateExcel(EmployeeRecord record)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Certificate");

        ws.Cell("A1").Value = "My Company";
        ws.Cell("A2").Value = "Employee Certificate";
        ws.Cell("A4").Value = "Certificate Code";
        ws.Cell("B4").Value = record.CertificateCode;
        ws.Cell("A5").Value = "Employee Name";
        ws.Cell("B5").Value = record.EmployeeName;
        ws.Cell("A6").Value = "User Type";
        ws.Cell("B6").Value = record.UserType;
        ws.Cell("A7").Value = "Age";
        ws.Cell("B7").Value = record.Age;
        ws.Cell("A8").Value = "DOB";
        ws.Cell("B8").Value = record.Dob.ToString("yyyy-MM-dd");
        ws.Cell("A9").Value = "Email";
        ws.Cell("B9").Value = record.Email;
        ws.Cell("A10").Value = "Salary";
        ws.Cell("B10").Value = record.Salary?.ToString("0.00") ?? "N/A";

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GeneratePdf(EmployeeRecord record)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var barcodePng = GenerateBarcode(record.CertificateCode);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Header().Row(row =>
                {
                    row.ConstantItem(60).Height(40).Background(Colors.Blue.Lighten3).AlignCenter().AlignMiddle().Text("LOGO").Bold();
                    row.RelativeItem().AlignMiddle().PaddingLeft(10).Text("My Company Certificate").FontSize(18).Bold();
                });

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Spacing(10);
                    column.Item().Text($"Certificate Code: {record.CertificateCode}").Bold();
                    column.Item().Text($"Employee Name: {record.EmployeeName}");
                    column.Item().Text($"User Type: {record.UserType}");
                    column.Item().Text($"Age: {record.Age}");
                    column.Item().Text($"DOB: {record.Dob:yyyy-MM-dd}");
                    column.Item().Text($"Email: {record.Email}");
                    column.Item().Text($"Salary: {(record.Salary.HasValue ? record.Salary.Value.ToString("0.00") : "N/A")}");
                    column.Item().PaddingTop(20).Text("Scan Barcode to verify this certificate.").Italic();
                    column.Item().Height(80).Image(barcodePng);
                });

                page.Footer().AlignCenter().Text($"Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC");
            });
        }).GeneratePdf();
    }

    private static byte[] GenerateBarcode(string text)
    {
        var writer = new BarcodeWriter<SkiaSharp.SKBitmap>
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Height = 100,
                Width = 400,
                Margin = 2
            },
            Renderer = new SKBitmapRenderer()
        };

        using var bitmap = writer.Write(text);
        using var image = SkiaSharp.SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}

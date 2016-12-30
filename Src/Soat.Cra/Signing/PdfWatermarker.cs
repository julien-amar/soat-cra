using iTextSharp.text;
using iTextSharp.text.pdf;
using Soat.Cra.Interfaces;
using System;
using System.IO;

namespace Soat.Cra.Signing
{
	public class PdfWatermarker : IPdfWatermarker
    {
        private void ValidateWatermarkSize(string watermark)
        {
            Image jpeg = Image.GetInstance(watermark, true);

            if (jpeg.Width != 140 || jpeg.Height != 60)
            {
                throw new Exception("Watermark size must be 140x60.");
            }
        }

        public void AddWatermark(string input, string destination, string watermark)
		{
            ValidateWatermarkSize(watermark);

			using (PdfReader pdfReader = new PdfReader(input))
			{
				using (Stream output = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					using (PdfStamper pdfStamper = new PdfStamper(pdfReader, output))
					{
						for (int pageIndex = 1; pageIndex <= pdfReader.NumberOfPages; pageIndex++)
						{
							pdfStamper.FormFlattening = false;

							pdfReader.GetPageSizeWithRotation(pageIndex);

							PdfContentByte pdfData = pdfStamper.GetOverContent(pageIndex);

							pdfData.SetFontAndSize(BaseFont.CreateFont("Helvetica-Bold", "Cp1252", false), 10f);

							pdfData.SetGState(new PdfGState()
							{
								FillOpacity = 1f
							});

							pdfData.BeginText();

							Image jpeg = Image.GetInstance(watermark, true);

							jpeg.ScaleToFit(jpeg.Width, jpeg.Height);
							jpeg.SetAbsolutePosition(50f, 250f);
							jpeg.Rotation = 0f;

							pdfData.AddImage(jpeg);

							pdfData.EndText();
						}

						pdfStamper.Close();
					}

					output.Close();
					output.Dispose();
				}

				pdfReader.Close();
				pdfReader.Dispose();
			}
		}
	}
}
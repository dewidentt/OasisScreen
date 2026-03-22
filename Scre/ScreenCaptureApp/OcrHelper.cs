using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;

namespace ScreenCaptureApp
{
	internal static class OcrHelper
	{
		public static async Task<string> RecognizeTextAsync(Bitmap bitmap)
		{
			var engine = OcrEngine.TryCreateFromUserProfileLanguages();
			if (engine == null)
				return null;

			string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".png");
			try
			{
				bitmap.Save(tempPath, ImageFormat.Png);

				var file = await StorageFile.GetFileFromPathAsync(tempPath);
				using (var stream = await file.OpenReadAsync())
				{
					var decoder = await BitmapDecoder.CreateAsync(stream);
					var softwareBitmap = await decoder.GetSoftwareBitmapAsync(
						BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

					var ocrResult = await engine.RecognizeAsync(softwareBitmap);
					return ocrResult?.Text;
				}
			}
			finally
			{
				try { File.Delete(tempPath); } catch { }
			}
		}
	}
}

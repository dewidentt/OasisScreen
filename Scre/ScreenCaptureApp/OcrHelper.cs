using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Tesseract;

namespace ScreenCaptureApp
{
internal static class OcrHelper
{
public static Task<string> RecognizeTextAsync(Bitmap bitmap)
{
return Task.Run(() => RecognizeText(bitmap));
}

private static string RecognizeText(Bitmap bitmap)
{
string tessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
if (!Directory.Exists(tessDataPath))
throw new DirectoryNotFoundException("Папка tessdata не найдена рядом с exe");

using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
{
using (var pix = PixConverter.ToPix(bitmap))
{
using (var page = engine.Process(pix))
{
string text = page.GetText();
return string.IsNullOrWhiteSpace(text) ? null : text.Trim();
}
}
}
}
}
}

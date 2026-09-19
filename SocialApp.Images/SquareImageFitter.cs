using System.Drawing;
using System.Drawing.Drawing2D;

namespace SocialApp.Images;

// image shalgadag, resize hiideg class (lab2)
public static class SquareImageFitter
{
    public const int TargetSize = 512;


    public static Bitmap? Fit(Image source)
    {
        if (source.Width < TargetSize || source.Height < TargetSize)
            return null;

        // Iluu baival hoyor talaas ni ijleer avna. TEgvel gollonos
        int cropSize = Math.Min(source.Width, source.Height);
        var cropArea = new Rectangle(
            (source.Width - cropSize) / 2,
            (source.Height - cropSize) / 2,
            cropSize,
            cropSize);


        var result = new Bitmap(TargetSize, TargetSize);
        using var g = Graphics.FromImage(result);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.DrawImage(source, new Rectangle(0, 0, TargetSize, TargetSize), cropArea, GraphicsUnit.Pixel);
        return result;
    }
}

using System.Drawing;
using SocialApp.Images;

namespace SocialApp.Images.Tests;

[TestClass]
public class SquareImageFitterTests
{
    [TestMethod]
    public void Fit_RejectsImageNarrowerThanTargetSize()
    {
        // Spec example: 128x512 must not be allowed.
        using var tooNarrow = new Bitmap(128, 512);
        Assert.IsNull(SquareImageFitter.Fit(tooNarrow));
    }

    [TestMethod]
    public void Fit_RejectsImageShorterThanTargetSize()
    {
        using var tooShort = new Bitmap(512, 128);
        Assert.IsNull(SquareImageFitter.Fit(tooShort));
    }

    [TestMethod]
    public void Fit_KeepsAlreadySquareImageAtTargetSize()
    {
        using var square = new Bitmap(SquareImageFitter.TargetSize, SquareImageFitter.TargetSize);
        using var result = SquareImageFitter.Fit(square);

        Assert.IsNotNull(result);
        Assert.AreEqual(SquareImageFitter.TargetSize, result!.Width);
        Assert.AreEqual(SquareImageFitter.TargetSize, result.Height);
    }

    [TestMethod]
    public void Fit_CropsTallImageEvenlyTopAndBottom()
    {
        // Spec example: 512x768 -> trim 128 off the top and 128 off the bottom -> 512x512.
        using var tall = new Bitmap(512, 768);
        using (var g = Graphics.FromImage(tall))
        {
            g.Clear(Color.Black);
            // Only rows 128..639 (the middle 512) are white - that's the band that should survive.
            g.FillRectangle(Brushes.White, 0, 128, 512, 512);
        }

        using var result = SquareImageFitter.Fit(tall);

        Assert.IsNotNull(result);
        Assert.AreEqual(512, result!.Width);
        Assert.AreEqual(512, result.Height);
        // Sample well inside the edges to avoid resize blending artifacts.
        Assert.AreEqual(Color.White.ToArgb(), result.GetPixel(10, 10).ToArgb());
        Assert.AreEqual(Color.White.ToArgb(), result.GetPixel(500, 500).ToArgb());
    }

    [TestMethod]
    public void Fit_CropsWideImageEvenlyLeftAndRight()
    {
        using var wide = new Bitmap(768, 512);
        using (var g = Graphics.FromImage(wide))
        {
            g.Clear(Color.Black);
            g.FillRectangle(Brushes.White, 128, 0, 512, 512);
        }

        using var result = SquareImageFitter.Fit(wide);

        Assert.IsNotNull(result);
        Assert.AreEqual(512, result!.Width);
        Assert.AreEqual(512, result.Height);
        Assert.AreEqual(Color.White.ToArgb(), result.GetPixel(10, 10).ToArgb());
        Assert.AreEqual(Color.White.ToArgb(), result.GetPixel(500, 500).ToArgb());
    }

    [TestMethod]
    public void Fit_ShrinksLargeImageDownToTargetSize()
    {
        // Spec example: 4096x6000 -> shrink down to 512x512.
        using var huge = new Bitmap(4096, 6000);
        using var result = SquareImageFitter.Fit(huge);

        Assert.IsNotNull(result);
        Assert.AreEqual(512, result!.Width);
        Assert.AreEqual(512, result.Height);
    }
}

namespace SKInvalidateMeasure.Text;

using SkiaSharp;
using System.Runtime.CompilerServices;
using System.Threading;

/// <summary>
/// Provides font extensions
/// </summary>
public static class Fonts
{
    /// <summary>
    /// Converts a <see cref="FontAttributes"/> to an <see cref="SKFontStyle"/>.
    /// </summary>
    /// <param name="attributes">The <see cref="FontAttributes"/> to convert.</param>
    /// <returns>The <see cref="SKFontStyle"/> for the <paramref name="attributes"/>.</returns>
    public static SKFontStyle ToFontStyle(this FontAttributes attributes)
    {
        return attributes switch
        {
            FontAttributes.Bold => SKFontStyle.Bold,
            FontAttributes.Italic => SKFontStyle.Italic,
            (FontAttributes.Bold | FontAttributes.Italic) => SKFontStyle.BoldItalic,
            _ => SKFontStyle.Normal
        };
    }

    /// <summary>
    /// Converts a <see cref="TextAlignment"/> to an <see cref="SKTextAlign"/>.
    /// </summary>
    /// <param name="alignment">The <see cref="FontAttributes"/> to convert.</param>
    /// <returns>The <see cref="SKFontStyle"/> for the <paramref name="alignment"/>.</returns>
    public static SKTextAlign ToTextAlign(this TextAlignment alignment)
    {
        return alignment switch
        {
            TextAlignment.Start => SKTextAlign.Left,
            TextAlignment.Center => SKTextAlign.Center,
            TextAlignment.End => SKTextAlign.Right,
            _ => SKTextAlign.Left
        };
    }

    /// <summary>
    /// Converts a text point size to a pixels.
    /// </summary>
    /// <param name="emSize">The font size in points.</param>
    /// <returns>The font size in pixels.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToPixels(this float emSize)
    {
        return emSize * 96 / 72;
    }

    /// <summary>
    /// Experimental: Scales font points to pixels at the current display density.
    /// </summary>
    /// <param name="emSize">The font size in points.</param>
    /// <remarks>
    /// The goal is to scale the font size to the current display density such that 
    /// a character displayed via SkiaSharp is the same size as the same character displayed
    /// in Word using the same font and font size.
    /// </remarks>
    public static float ScalePoints(this float emSize)
    {
        float scale = (float)DeviceDisplay.Current.MainDisplayInfo.Density;
        return emSize.ToPixels() * scale;
    }

    /// <summary>
    /// Experimental: Measures the text using the current display density.
    /// </summary>
    /// <param name="font">The <see cref="SKFont"/> to use to measure the text.</param>
    /// <param name="text">The text to measure.</param>
    /// <param name="bounds">The <see cref="SKRect"/> containing the bounds for the measured text.</param>
    /// <param name="paint">The optional <see cref="SKPaint"/> to use to measure.</param>
    /// <returns>The text width of the measured text.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="font"/> or <paramref name="text"/> is a null reference.
    /// </exception>
    public static float Measure(this SKFont font, string text, out SKRect bounds, SKPaint paint = null)
    {
        ArgumentNullException.ThrowIfNull(font);
        ArgumentNullException.ThrowIfNull(text);
        float size = font.Size;
        font.Size = font.Size.ScalePoints();
        float width = font.MeasureText(text, out bounds, paint);
        font.Size = size;
        return width;
    }

    /// <summary>
    /// Experimental: Draw text on a canvas.
    /// </summary>
    /// <param name="canvas">The <see cref="SKCanvas"/> to draw on.</param>
    /// <param name="paint">The <see cref="SKPaint"/> to use to draw.</param>
    /// <param name="font">The <see cref="SKFont"/> to use to draw the text.</param>
    /// <param name="text">The string text to draw.</param>
    /// <param name="left">The left position to draw the text.</param>
    /// <param name="baseline">The text baseline.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="font"/> or <paramref name="text"/> or <paramref name="paint"/> is a null reference.
    /// </exception>
    public static void DrawText
    (
        this SKCanvas canvas,
        SKFont font,
        SKPaint paint, 
        string text, 
        float left, float baseline, 
        SKTextAlign align = SKTextAlign.Left
    )
    {
        float size = font.Size;
        font.Size = font.Size.ScalePoints();
        canvas.DrawText(text, left, baseline, align, font, paint);
        font.Size = size;
    }

    /// <summary>
    /// Gets the <see cref="Size"/> needed to render the specified <paramref name="text"/>.
    /// </summary>
    /// <param name="text">The text to render.</param>
    /// <param name="fontFamily">The string font family name of the font to use to render the text.</param>
    /// <param name="fontAttributes">The <see cref="FontAttributes"/>.</param>
    /// <param name="fontSize">The font size.</param>
    /// <returns>The <see cref="Size"/> of the rectangle needed to render the text.</returns>
    public static Size GetStringSize(this string text, string fontFamily, FontAttributes fontAttributes, double fontSize)
    {
        IFont font = GetFont(fontFamily, fontAttributes);
        // StringSizeService does not return the correct width.  See https://github.com/dotnet/maui/issues/27142,
        // so we'll add a fudge factor.
        return StringSizeService.GetStringSize(text + "A", font, (float)fontSize);
    }

    /// <summary>
    /// Gets an <see cref="IFont"/> to use with <see cref="IStringSizeService.GetStringSize(string, IFont, float)"/>
    /// </summary>
    /// <param name="fontFamily">The font family name.</param>
    /// <param name="fontAttributes">The <see cref="FontAttributes"/>.</param>
    /// <returns>
    /// An <see cref="IFont"/> for the specified <paramref name="fontFamily"/> and <paramref name="fontAttributes"/>.
    /// </returns>
    static IFont GetFont(string fontFamily, FontAttributes fontAttributes)
    {
        FontStyleType fontStyle = fontAttributes == FontAttributes.Italic
                      ? FontStyleType.Italic
                      : FontStyleType.Normal;

        int fontWeight = fontAttributes == FontAttributes.Bold
            ? FontWeights.Bold
            : FontWeights.Normal;

        return new Microsoft.Maui.Graphics.Font(fontFamily, fontWeight, fontStyle);
    }

    static readonly Lock _lock = new();
    static IStringSizeService _stringSizeService;

    static IStringSizeService StringSizeService
    {
        get
        {
            lock (_lock)
            {
                if (_stringSizeService is null)
                {
                    _stringSizeService = new Microsoft.Maui.Graphics.Platform.PlatformStringSizeService();
                }
                return _stringSizeService;
            }
        }
    }
}

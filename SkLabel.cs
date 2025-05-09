﻿namespace SKInvalidateMeasure;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using SKInvalidateMeasure.Text;
using System.Diagnostics;

internal class SkLabel : SKCanvasView
{
    #region Fields

    static readonly Color DefaultTextColor = Colors.Black;
    const double DefaultFontSize = 12;
    const double MinimumFontSize = 6;
    const string DefaultFontFamily = "OpenSansRegular";
    const FontAttributes DefaultFontAttributes = FontAttributes.None;

    SKTextMetrics _metrics;

    #endregion Fields

    #region Text

    /// <summary>
    /// Gets or sets the color to use to draw the glyph.
    /// </summary>
    public string Text
    {
        get => GetValue(TextProperty) as string;
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="Text"/> property.
    /// </summary>
    public static readonly BindableProperty TextProperty = BindableProperty.Create
    (
        nameof(Text),
        typeof(string),
        typeof(SkLabel),
        string.Empty,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is string)
            {
                return value;
            }
            return string.Empty;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateTextMetrics();
        }
    );

    #endregion ItemColor

    #region TextColor

    /// <summary>
    /// Gets or sets the color to use to draw the glyph.
    /// </summary>
    public Color TextColor
    {
        get => GetValue(TextColorProperty) as Color;
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="TextColor"/> property.
    /// </summary>
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create
    (
        nameof(TextColor),
        typeof(Color),
        typeof(SkLabel),
        DefaultTextColor,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is Color color)
            {
                return color;
            }
            return DefaultTextColor;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateSurface();
        }
    );

    #endregion ItemColor

    #region FontFamily

    /// <summary>
    /// Gets or sets the <see cref="Text"/> font size.
    /// </summary>
    public string FontFamily
    {
        get => GetValue(FontFamilyProperty) as string;
        set => SetValue(FontFamilyProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="FontFamily"/> property.
    /// </summary>
    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create
    (
        nameof(FontFamily),
        typeof(string),
        typeof(SkLabel),
        DefaultFontFamily,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is string family)
            {
                if (family.Trim().Length > 0)
                {
                    return family;
                }
            }
            return DefaultFontFamily;
        },
        propertyChanged: (bindable, oldValue, newValue) => 
        { 
            ((SkLabel)bindable).InvalidateTextMetrics(); 
        }
    );

    #endregion FontFamily

    #region FontAttributes

    /// <summary>
    /// Gets or sets the <see cref="Text"/> font size.
    /// </summary>
    public FontAttributes FontAttributes
    {
        get => (FontAttributes)GetValue(FontAttributesProperty);
        set => SetValue(FontAttributesProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="FontAttributes"/> property.
    /// </summary>
    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create
    (
        nameof(FontAttributes),
        typeof(FontAttributes),
        typeof(SkLabel),
        DefaultFontAttributes,
        BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) => 
        { 
            ((SkLabel)bindable).InvalidateTextMetrics(); 
        }
    );

    #endregion FontAttributes

    #region FontSize

    /// <summary>
    /// Gets or sets the <see cref="Text"/> font size.
    /// </summary>
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="FontSize"/> property.
    /// </summary>
    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create
    (
        nameof(FontSize),
        typeof(double),
        typeof(SkLabel),
        DefaultFontSize,
        BindingMode.OneWay,
        coerceValue: (bindable, value) =>
        {
            if (value is double size)
            {
                if (size >= MinimumFontSize)
                {
                    return size;
                }
            }
            return MinimumFontSize;
        },
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateTextMetrics();
        }
    );

    #endregion FontSize

    #region HorizontalTextAlignment

    /// <summary>
    /// Gets or sets the horizontal <see cref="Text"/> alignment.
    /// </summary>
    public TextAlignment HorizontalTextAlignment
    {
        get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
        set => SetValue(HorizontalTextAlignmentProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="HorizontalTextAlignment"/> property.
    /// </summary>
    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create
    (
        nameof(HorizontalTextAlignment),
        typeof(TextAlignment),
        typeof(SkLabel),
        TextAlignment.Start,
        BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateTextMetrics();
        }
    );

    #endregion HorizontalTextAlignment

    #region VerticalTextAlignment

    /// <summary>
    /// Gets or sets the vertical <see cref="Text"/> alignment.
    /// </summary>
    public TextAlignment VerticalTextAlignment
    {
        get => (TextAlignment)GetValue(VerticalTextAlignmentProperty);
        set => SetValue(VerticalTextAlignmentProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="VerticalTextAlignment"/> property.
    /// </summary>
    public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create
    (
        nameof(VerticalTextAlignment),
        typeof(TextAlignment),
        typeof(SkLabel),
        TextAlignment.Start,
        BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateTextMetrics();
        }
    );

    #endregion VerticalTextAlignment

    #region EnableWorkaround

    /// <summary>
    /// Gets or sets the color to use to draw the glyph.
    /// </summary>
    public bool EnableWorkaround
    {
        get => (bool) GetValue(EnableWorkaroundProperty);
        set => SetValue(EnableWorkaroundProperty, value);
    }

    /// <summary>
    /// Provides a <see cref="BindableProperty"/> for the <see cref="EnableWorkaround"/> property.
    /// </summary>
    public static readonly BindableProperty EnableWorkaroundProperty = BindableProperty.Create
    (
        nameof(EnableWorkaround),
        typeof(bool),
        typeof(SkLabel),
        (bool)false,
        BindingMode.OneWay,
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            ((SkLabel)bindable).InvalidateTextMetrics();
        }
    );

    #endregion ItemColor

    SKFont GetFont()
    {
        using (SKTypeface typeface = SKTypeface.FromFamilyName(FontFamily, FontAttributes.ToFontStyle()))
        {
            return new SKFont(typeface, (float)FontSize)
            {
                Subpixel = true,
                Edging = SKFontEdging.SubpixelAntialias,
                Size = (float)FontSize
            };
        }
    }

    void InvalidateTextMetrics()
    {
        Trace.WriteLine("********************");
        using (SKFont font = GetFont())
        {
            using (SKPaint paint = new SKPaint() { IsAntialias = true })
            {
                _metrics = new SKTextMetrics(Text, font, paint);
            }
        }

        if (EnableWorkaround && !string.IsNullOrEmpty(Text))
        {
            WidthRequest = _metrics.TextWidth;
            HeightRequest = _metrics.Size.Height;
            Trace.WriteLine($"SKLabel.InvalidateTextMetrics FontSize={FontSize}pts WidthRequest={WidthRequest} HeightRequest={HeightRequest}");
        }
        InvalidateMeasure();
        InvalidateSurface();
    }

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        Trace.WriteLine($"SKLabel.MeasureOverride FontSize={FontSize}pts EnableWorkaround={EnableWorkaround}");
        Size size = new Size(_metrics.TextWidth, _metrics.Size.Height);
        Trace.WriteLine($"SKLabel.MeasureOverride Width={size.Width} Height={size.Height}");
        return size;
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;

        Trace.WriteLine($"SKLabel.OnPaintSurface FontSize={FontSize}pts EnableWorkaround={EnableWorkaround}");
        Trace.WriteLine($"SKLabel.OnPaintSurface Width={Width} Height={Height}  WidthRequest={WidthRequest} HeightRequest={HeightRequest}");
        Trace.WriteLine($"SKLabel.OnPaintSurface CanvasWidth={CanvasSize.Width} CanvasHeight={CanvasSize.Height}");

        if (BackgroundColor is not null && BackgroundColor != Colors.Transparent)
        {
           canvas.Clear(BackgroundColor.ToSKColor());
        }
        else
        {
            canvas.Clear();
        }

        if (!string.IsNullOrEmpty(Text))
        {
            float y = 0;
            float x = 0;
            float width = CanvasSize.Width;
            float height = CanvasSize.Height;

            using (SKPaint paint = new() {IsAntialias = true, Color = TextColor.ToSKColor()})
            {
                using (SKFont font = GetFont())
                {
                    SKTextAlign verticalAlignment = VerticalTextAlignment.ToTextAlign();
                    SKTextAlign horizontalAlignment = HorizontalTextAlignment.ToTextAlign();
                    switch (verticalAlignment)
                    {
                        // TextAlignment.Center
                        case SKTextAlign.Center:
                            y += _metrics.Ascent + (height - _metrics.Size.Height) / 2;
                            break;
                        // TextAlignment.Bottom
                        case SKTextAlign.Right:
                            y += height - _metrics.Descent;
                            break;
                        // TextAlignment.Top and Justify
                        default:
                            y -= _metrics.Ascent;
                            break;
                    }
                    switch (horizontalAlignment)
                    {
                        // TextAlignment.Center
                        case SKTextAlign.Center:
                            x += (width - _metrics.TextWidth) / 2;
                            break;
                        // TextAlignment.Right
                        case SKTextAlign.Right:
                            x += width - _metrics.TextWidth;
                            break;
                        // TextAlignment.Left and Justify
                        default:
                            break;
                    }
                    canvas.DrawText(font, paint, Text, x, y, SKTextAlign.Left);
                }
            }
        }
    }
}

using System.Collections.ObjectModel;

namespace ProjectHX.Mobile.Components;

public partial class ImageCarousel : ContentView
{
    public static readonly BindableProperty ImagesProperty =
        BindableProperty.Create(
            nameof(Images),
            typeof(List<string>),
            typeof(ImageCarousel),
            defaultValue: new List<string>(),
            propertyChanged: OnImagesChanged);

    public static readonly BindableProperty CurrentImageIndexProperty =
        BindableProperty.Create(
            nameof(CurrentImageIndex),
            typeof(int),
            typeof(ImageCarousel),
            defaultValue: 0,
            propertyChanged: OnCurrentImageIndexChanged);

    public static readonly BindableProperty IndicatorTextProperty =
        BindableProperty.Create(
            nameof(IndicatorText),
            typeof(string),
            typeof(ImageCarousel),
            defaultValue: "1 / 1");

    public List<string> Images
    {
        get => (List<string>)GetValue(ImagesProperty);
        set => SetValue(ImagesProperty, value);
    }

    public int CurrentImageIndex
    {
        get => (int)GetValue(CurrentImageIndexProperty);
        set => SetValue(CurrentImageIndexProperty, value);
    }

    public string IndicatorText
    {
        get => (string)GetValue(IndicatorTextProperty);
        set => SetValue(IndicatorTextProperty, value);
    }

    public ImageCarousel()
    {
        InitializeComponent();
    }

    private static void OnImagesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ImageCarousel carousel && newValue is List<string> images)
        {
            carousel.CurrentImageIndex = 0;
            carousel.UpdateIndicatorText();
        }
    }

    private static void OnCurrentImageIndexChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ImageCarousel carousel)
        {
            carousel.UpdateIndicatorText();
        }
    }

    private void UpdateIndicatorText()
    {
        if (Images?.Count > 0)
        {
            IndicatorText = $"{CurrentImageIndex + 1} / {Images.Count}";
        }
        else
        {
            IndicatorText = "0 / 0";
        }
    }
}

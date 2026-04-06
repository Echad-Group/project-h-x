using CommunityToolkit.Mvvm.ComponentModel;

namespace ProjectHX.Mobile.ViewModels;

public partial class SelectableOptionItem : ObservableObject
{
    public SelectableOptionItem(string value, string? icon = null)
    {
        Value = value;
        Label = value;
        Icon = icon ?? string.Empty;
    }

    public string Label { get; }

    public string Value { get; }

    public string Icon { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayLabel))]
    private bool isSelected;

    public string DisplayLabel => string.IsNullOrWhiteSpace(Icon) ? Label : $"{Icon}  {Label}";
}

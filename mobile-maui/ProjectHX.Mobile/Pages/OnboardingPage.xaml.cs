using ProjectHX.Mobile.Contexts;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ProjectHX.Mobile.Pages;

public class OnboardingItem
{
    public string Image { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Color? CardColor { get; set; }
}

public partial class OnboardingPage : ContentPage
{
    private ObservableCollection<OnboardingItem> _onboardingItems = new();
    public ObservableCollection<OnboardingItem> OnboardingItems
    {
        get => _onboardingItems;
        set
        {
            _onboardingItems = value;
            OnPropertyChanged(nameof(OnboardingItems));
            OnPropertyChanged(nameof(IsLastItem));
            OnPropertyChanged(nameof(NextButtonText));
            OnPropertyChanged(nameof(ShowSkip));
        }
    }

    private int _currentIndex;
    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            if (_currentIndex == value)
            {
                return;
            }

            _currentIndex = value;
            OnPropertyChanged(nameof(CurrentIndex));
            OnPropertyChanged(nameof(IsLastItem));
            OnPropertyChanged(nameof(NextButtonText));
            OnPropertyChanged(nameof(ShowSkip));
        }
    }

    public bool IsLastItem => OnboardingItems.Count > 0 && CurrentIndex >= OnboardingItems.Count - 1;

    public string NextButtonText => IsLastItem ? "GET STARTED" : "NEXT";

    public bool ShowSkip => !IsLastItem;

    private readonly AppStorageContext _appStorageContext;

    public ICommand NextCommand { get; }

    public ICommand SkipCommand { get; }

    public OnboardingPage(AppStorageContext appStorageContext)
    {
        _appStorageContext = appStorageContext;

        InitializeComponent();
        BindingContext = this;

        NextCommand = new Command(async () =>
        {
            if (IsLastItem)
            {
                Preferences.Set(_appStorageContext.UserIsBoarded, true);
                await Shell.Current.GoToAsync("//welcome");
                return;
            }

            OnboardingCarousel.Position = CurrentIndex + 1;
        });

        SkipCommand = new Command(async () =>
        {
            Preferences.Set(_appStorageContext.UserIsBoarded, true);
            await Shell.Current.GoToAsync("//welcome");
        });

        OnboardingItems = new ObservableCollection<OnboardingItem>
        {
            new OnboardingItem {
                Image = "new_kenya_2_0.png",
                Title = "Organize Campaign Tasks",
                Description = "Receive assignments, track your priorities, and keep your daily campaign work focused.",
                CardColor = Color.FromArgb("#F2FAF6")
            },
            new OnboardingItem {
                Image = "new_kenya_2_0.png",
                Title = "Submit Results in Real Time",
                Description = "Report field updates quickly so team leaders can see progress and coordinate effectively.",
                CardColor = Color.FromArgb("#ECF6F0")
            },
            new OnboardingItem {
                Image = "new_kenya_2_0.png",
                Title = "Stay Connected to New Kenya",
                Description = "Access updates, campaign news, and team communication from one secure volunteer app.",
                CardColor = Color.FromArgb("#E5F2EA")
            }
        };
        CurrentIndex = 0;
        OnboardingCarousel.PositionChanged += OnCarouselPositionChanged;
    }

    private void OnCarouselPositionChanged(object? sender, PositionChangedEventArgs e)
    {
        CurrentIndex = e.CurrentPosition;
    }

}
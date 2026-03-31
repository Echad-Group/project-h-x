using ProjectHX.Mobile.Contexts;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ProjectHX.Mobile.Pages;

public class OnboardingItem
{
    public string Image { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ButtonText { get; set; } = "NEXT"; // Default button text
    public ICommand ButtonCommand { get; set; } = null!; // Command for button click, can be set in XAML or code-behind
    public string Description { get; set; } = string.Empty;
    public Color? CardColor { get; set; } // For card background color
}

public partial class OnboardingPage : ContentPage, INotifyPropertyChanged
{
    private ObservableCollection<OnboardingItem> _onboardingItems = new();
    public ObservableCollection<OnboardingItem> OnboardingItems
    {
        get => _onboardingItems;
        set { _onboardingItems = value; OnPropertyChanged(); }
    }

    private int _currentIndex;
    public int CurrentIndex
    {
        get => _currentIndex;
        set { _currentIndex = value; OnPropertyChanged(); }
    }

    private readonly AppStorageContext _appStorageContext;

    private ICommand _nextCommand;
    private ICommand _startCommand;
    private ICommand _skipCommand;

    public OnboardingPage(AppStorageContext appStorageContext)
    {
        InitializeComponent();
        BindingContext = this;

        _nextCommand = new Command(() =>
        {
            // Navigate to the next onboarding item
            if (CurrentIndex < OnboardingItems.Count - 1)
            {
                OnboardingCarousel.Position = CurrentIndex + 1;
            }
        });

        _startCommand = new Command(async () =>
        {
            // After onboarding..
            Preferences.Set(_appStorageContext.UserIsBoarded, true);
            if (CurrentIndex == OnboardingItems.Count - 1)
            {
                await Shell.Current.GoToAsync($"//welcome");
            }
        });

        _skipCommand = new Command(async () =>
        {
            // Skip onboarding...
            Preferences.Set(_appStorageContext.UserIsBoarded, true);
            await Shell.Current.GoToAsync($"//welcome");
        });

        OnboardingItems = new ObservableCollection<OnboardingItem>
        {
            new OnboardingItem {
                Image = "taxi_finder_logo_big_black.png", // Replace with your actual image asset
                Title = "Find a Taxi Fast",
                ButtonCommand = _nextCommand,
                Description = "Quickly locate nearby taxis and get a ride in minutes, wherever you are.",
                CardColor = Color.FromArgb("#E7E3DD")
            },
            new OnboardingItem {
                Image = "taxi_finder_logo_big_black.png", // Replace with your actual image asset
                Title = "Book & Track Easily",
                ButtonCommand = _nextCommand,
                Description = "Book your taxi with one tap and track your driver in real time on the map.",
                CardColor = Color.FromArgb("#F8EDE8")
            },
            new OnboardingItem {
                Image = "taxi_finder_logo_big_black.png", // Replace with your actual image asset
                Title = "Safe & Reliable Rides",
                ButtonText = "START",
                ButtonCommand = _startCommand,
                Description = "Enjoy safe, reliable, and affordable rides with trusted drivers, anytime.",
                CardColor = Color.FromArgb("#E6F3ED")
            }
        };
        CurrentIndex = 0;
        OnboardingCarousel.PositionChanged += OnCarouselPositionChanged;

        _appStorageContext = appStorageContext;
    }

    private void OnCarouselPositionChanged(object? sender, PositionChangedEventArgs e)
    {
        CurrentIndex = e.CurrentPosition;
    }

    private void OnSkipClicked(object sender, EventArgs e)
    {
        // Skip onboarding and navigate to ChooseCityPage
        _skipCommand.Execute(null);
    }

    // Change the method signature to use the 'new' keyword to explicitly hide the inherited member.
    public new event PropertyChangedEventHandler? PropertyChanged;
    protected new void OnPropertyChanged([CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
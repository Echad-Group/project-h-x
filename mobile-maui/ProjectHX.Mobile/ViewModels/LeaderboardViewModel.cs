using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Leaderboard;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class LeaderboardViewModel : BaseViewModel
{
    private readonly ILeaderboardApiService _leaderboardApiService;

    [ObservableProperty]
    private MyRankModel? myRank;

    [ObservableProperty]
    private string selectedScope = "National";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNoRank))]
    private bool hasRank;

    public string[] ScopeOptions { get; } = ["National", "Region", "County"];
    public bool HasNoRank => !HasRank;


    public LeaderboardViewModel(ILeaderboardApiService leaderboardApiService)
    {
        _leaderboardApiService = leaderboardApiService;
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;
        InfoMessage = null;
        HasRank = false;

        try
        {
            MyRank = await _leaderboardApiService.GetMyRankAsync(SelectedScope);
            HasRank = true;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task RefreshAsync()
    {
        await LoadAsync();
    }
}

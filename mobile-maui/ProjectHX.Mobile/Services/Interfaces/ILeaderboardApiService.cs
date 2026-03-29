using ProjectHX.Mobile.Models.Leaderboard;

namespace ProjectHX.Mobile.Services.Interfaces;

public interface ILeaderboardApiService
{
    Task<MyRankModel> GetMyRankAsync(string scope = "National", CancellationToken cancellationToken = default);
}

using NewKenyaAPI.Models;
using System.Collections.Concurrent;

namespace NewKenyaAPI.Services
{
    public class VerificationReviewService
    {
        private readonly ConcurrentDictionary<string, List<VerificationTimelineEvent>> _timelineByUser = new();

        public List<VerificationTimelineEvent> GetTimeline(string userId)
        {
            if (_timelineByUser.TryGetValue(userId, out var timeline))
            {
                return timeline
                    .OrderByDescending(item => item.Timestamp)
                    .ToList();
            }

            return new List<VerificationTimelineEvent>();
        }

        public void AddEvent(string userId, VerificationTimelineEvent timelineEvent)
        {
            var timeline = _timelineByUser.GetOrAdd(userId, _ => new List<VerificationTimelineEvent>());
            lock (timeline)
            {
                timeline.Add(timelineEvent);
                if (timeline.Count > 100)
                {
                    timeline.RemoveRange(0, timeline.Count - 100);
                }
            }
        }
    }
}

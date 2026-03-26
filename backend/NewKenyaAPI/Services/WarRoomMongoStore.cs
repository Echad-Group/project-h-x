using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Services
{
    public class WarRoomPersistenceSnapshot
    {
        public List<WarRoomCommandLane> Lanes { get; set; } = new();
        public List<WarRoomCommandPodItem> Pods { get; set; } = new();
        public List<WarRoomIncidentItem> Incidents { get; set; } = new();
        public List<WarRoomBattleRhythmItem> BattleRhythm { get; set; } = new();
        public List<WarRoomLegalCaseItem> LegalCases { get; set; } = new();
        public List<WarRoomCommandGridStatusItem> CommandGrid { get; set; } = new();
        public List<CoalitionCommandItem> CoalitionCommands { get; set; } = new();
        public List<MobilizationRoleAssignmentItem> MobilizationRoles { get; set; } = new();
        public WarRoomRedZoneState RedZoneState { get; set; } = new();
        public CampaignPhaseState CampaignPhaseState { get; set; } = new();
        public int NextIncidentId { get; set; }
        public int NextLegalCaseId { get; set; }
        public int NextRedZoneDecisionId { get; set; }
        public int NextPodId { get; set; }
        public int NextCoalitionId { get; set; }
    }

    public class WarRoomMongoStore
    {
        private readonly IMongoCollection<WarRoomCommandLane> _lanes;
        private readonly IMongoCollection<WarRoomCommandPodItem> _pods;
        private readonly IMongoCollection<WarRoomIncidentItem> _incidents;
        private readonly IMongoCollection<WarRoomBattleRhythmDocument> _battleRhythm;
        private readonly IMongoCollection<WarRoomLegalCaseItem> _legalCases;
        private readonly IMongoCollection<WarRoomCommandGridStatusItem> _commandGrid;
        private readonly IMongoCollection<CoalitionCommandItem> _coalitions;
        private readonly IMongoCollection<MobilizationRoleAssignmentItem> _mobilizationRoles;
        private readonly IMongoCollection<WarRoomRedZoneStateDocument> _redZone;
        private readonly IMongoCollection<CampaignPhaseStateDocument> _campaignPhase;
        private readonly IMongoCollection<WarRoomCountersDocument> _counters;

        public WarRoomMongoStore(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDb:ConnectionString"] ?? "mongodb://localhost:27017";
            var databaseName = configuration["MongoDb:Database"] ?? "newkenya";

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            _lanes = database.GetCollection<WarRoomCommandLane>("warroom_lanes");
            _pods = database.GetCollection<WarRoomCommandPodItem>("warroom_pods");
            _incidents = database.GetCollection<WarRoomIncidentItem>("warroom_incidents");
            _battleRhythm = database.GetCollection<WarRoomBattleRhythmDocument>("warroom_battle_rhythm");
            _legalCases = database.GetCollection<WarRoomLegalCaseItem>("warroom_legal_cases");
            _commandGrid = database.GetCollection<WarRoomCommandGridStatusItem>("warroom_command_grid");
            _coalitions = database.GetCollection<CoalitionCommandItem>("warroom_coalitions");
            _mobilizationRoles = database.GetCollection<MobilizationRoleAssignmentItem>("warroom_mobilization_roles");
            _redZone = database.GetCollection<WarRoomRedZoneStateDocument>("warroom_red_zone");
            _campaignPhase = database.GetCollection<CampaignPhaseStateDocument>("warroom_campaign_phase");
            _counters = database.GetCollection<WarRoomCountersDocument>("warroom_counters");
        }

        public WarRoomPersistenceSnapshot? LoadSnapshot()
        {
            var lanes = _lanes.Find(_ => true).ToList();
            var pods = _pods.Find(_ => true).ToList();
            var incidents = _incidents.Find(_ => true).ToList();
            var battleRhythmDocs = _battleRhythm.Find(_ => true).ToList();
            var legalCases = _legalCases.Find(_ => true).ToList();
            var commandGrid = _commandGrid.Find(_ => true).ToList();
            var coalitions = _coalitions.Find(_ => true).ToList();
            var mobilizationRoles = _mobilizationRoles.Find(_ => true).ToList();
            var redZone = _redZone.Find(item => item.Id == "red-zone").FirstOrDefault();
            var phase = _campaignPhase.Find(item => item.Id == "campaign-phase").FirstOrDefault();
            var counters = _counters.Find(item => item.Id == "sequence").FirstOrDefault();

            var hasAnyData = lanes.Count > 0 || pods.Count > 0 || incidents.Count > 0 ||
                             battleRhythmDocs.Count > 0 || legalCases.Count > 0 || commandGrid.Count > 0 ||
                             coalitions.Count > 0 || mobilizationRoles.Count > 0 || redZone != null ||
                             phase != null || counters != null;

            if (!hasAnyData)
            {
                return null;
            }

            return new WarRoomPersistenceSnapshot
            {
                Lanes = lanes,
                Pods = pods,
                Incidents = incidents,
                BattleRhythm = battleRhythmDocs.Select(MapBattleRhythm).ToList(),
                LegalCases = legalCases,
                CommandGrid = commandGrid,
                CoalitionCommands = coalitions,
                MobilizationRoles = mobilizationRoles,
                RedZoneState = redZone == null
                    ? new WarRoomRedZoneState()
                    : new WarRoomRedZoneState
                    {
                        IsElectionWeekModeEnabled = redZone.IsElectionWeekModeEnabled,
                        ActivatedAt = redZone.ActivatedAt,
                        ActivatedByUserId = redZone.ActivatedByUserId,
                        DecisionIntervalMinutes = redZone.DecisionIntervalMinutes,
                        LastDecisionAt = redZone.LastDecisionAt,
                        NextCheckpointAt = redZone.NextCheckpointAt,
                        Decisions = redZone.Decisions
                    },
                CampaignPhaseState = phase == null
                    ? new CampaignPhaseState()
                    : new CampaignPhaseState
                    {
                        ActivePhase = phase.ActivePhase,
                        ActiveSince = phase.ActiveSince,
                        UpdatedByUserId = phase.UpdatedByUserId,
                        Phases = phase.Phases
                    },
                NextIncidentId = counters?.NextIncidentId ?? 0,
                NextLegalCaseId = counters?.NextLegalCaseId ?? 0,
                NextRedZoneDecisionId = counters?.NextRedZoneDecisionId ?? 0,
                NextPodId = counters?.NextPodId ?? 0,
                NextCoalitionId = counters?.NextCoalitionId ?? 0
            };
        }

        public void SaveSnapshot(WarRoomPersistenceSnapshot snapshot)
        {
            ReplaceCollection(_lanes, snapshot.Lanes);
            ReplaceCollection(_pods, snapshot.Pods);
            ReplaceCollection(_incidents, snapshot.Incidents);
            ReplaceCollection(_battleRhythm, snapshot.BattleRhythm.Select(MapBattleRhythm).ToList());
            ReplaceCollection(_legalCases, snapshot.LegalCases);
            ReplaceCollection(_commandGrid, snapshot.CommandGrid);
            ReplaceCollection(_coalitions, snapshot.CoalitionCommands);
            ReplaceCollection(_mobilizationRoles, snapshot.MobilizationRoles);

            _redZone.ReplaceOne(
                item => item.Id == "red-zone",
                new WarRoomRedZoneStateDocument
                {
                    Id = "red-zone",
                    IsElectionWeekModeEnabled = snapshot.RedZoneState.IsElectionWeekModeEnabled,
                    ActivatedAt = snapshot.RedZoneState.ActivatedAt,
                    ActivatedByUserId = snapshot.RedZoneState.ActivatedByUserId,
                    DecisionIntervalMinutes = snapshot.RedZoneState.DecisionIntervalMinutes,
                    LastDecisionAt = snapshot.RedZoneState.LastDecisionAt,
                    NextCheckpointAt = snapshot.RedZoneState.NextCheckpointAt,
                    Decisions = snapshot.RedZoneState.Decisions
                },
                new ReplaceOptions { IsUpsert = true });

            _campaignPhase.ReplaceOne(
                item => item.Id == "campaign-phase",
                new CampaignPhaseStateDocument
                {
                    Id = "campaign-phase",
                    ActivePhase = snapshot.CampaignPhaseState.ActivePhase,
                    ActiveSince = snapshot.CampaignPhaseState.ActiveSince,
                    UpdatedByUserId = snapshot.CampaignPhaseState.UpdatedByUserId,
                    Phases = snapshot.CampaignPhaseState.Phases
                },
                new ReplaceOptions { IsUpsert = true });

            _counters.ReplaceOne(
                item => item.Id == "sequence",
                new WarRoomCountersDocument
                {
                    Id = "sequence",
                    NextIncidentId = snapshot.NextIncidentId,
                    NextLegalCaseId = snapshot.NextLegalCaseId,
                    NextRedZoneDecisionId = snapshot.NextRedZoneDecisionId,
                    NextPodId = snapshot.NextPodId,
                    NextCoalitionId = snapshot.NextCoalitionId
                },
                new ReplaceOptions { IsUpsert = true });
        }

        private static void ReplaceCollection<TDocument>(IMongoCollection<TDocument> collection, List<TDocument> documents)
        {
            collection.DeleteMany(_ => true);
            if (documents.Count > 0)
            {
                collection.InsertMany(documents);
            }
        }

        private static WarRoomBattleRhythmDocument MapBattleRhythm(WarRoomBattleRhythmItem item)
        {
            return new WarRoomBattleRhythmDocument
            {
                Id = item.Id,
                Date = item.Date.ToString("yyyy-MM-dd"),
                TimeSlot = item.TimeSlot,
                Name = item.Name,
                Purpose = item.Purpose,
                Completed = item.Completed,
                CompletedAt = item.CompletedAt,
                CompletedByUserId = item.CompletedByUserId,
                AttendanceCount = item.AttendanceCount,
                CompletionNotes = item.CompletionNotes
            };
        }

        private static WarRoomBattleRhythmItem MapBattleRhythm(WarRoomBattleRhythmDocument item)
        {
            return new WarRoomBattleRhythmItem
            {
                Id = item.Id,
                Date = DateOnly.TryParse(item.Date, out var parsedDate)
                    ? parsedDate
                    : DateOnly.FromDateTime(DateTime.UtcNow),
                TimeSlot = item.TimeSlot,
                Name = item.Name,
                Purpose = item.Purpose,
                Completed = item.Completed,
                CompletedAt = item.CompletedAt,
                CompletedByUserId = item.CompletedByUserId,
                AttendanceCount = item.AttendanceCount,
                CompletionNotes = item.CompletionNotes
            };
        }

        private sealed class WarRoomBattleRhythmDocument
        {
            [BsonId]
            public string Id { get; set; } = string.Empty;
            public string Date { get; set; } = string.Empty;
            public string TimeSlot { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string? Purpose { get; set; }
            public bool Completed { get; set; }
            public DateTime? CompletedAt { get; set; }
            public string? CompletedByUserId { get; set; }
            public int AttendanceCount { get; set; }
            public string? CompletionNotes { get; set; }
        }

        private sealed class WarRoomRedZoneStateDocument
        {
            [BsonId]
            public string Id { get; set; } = "red-zone";
            public bool IsElectionWeekModeEnabled { get; set; }
            public DateTime? ActivatedAt { get; set; }
            public string? ActivatedByUserId { get; set; }
            public int DecisionIntervalMinutes { get; set; }
            public DateTime? LastDecisionAt { get; set; }
            public DateTime? NextCheckpointAt { get; set; }
            public List<WarRoomRedZoneDecisionLogItem> Decisions { get; set; } = new();
        }

        private sealed class CampaignPhaseStateDocument
        {
            [BsonId]
            public string Id { get; set; } = "campaign-phase";
            public string ActivePhase { get; set; } = "Exploratory";
            public DateTime ActiveSince { get; set; }
            public string? UpdatedByUserId { get; set; }
            public List<CampaignPhasePlanItem> Phases { get; set; } = new();
        }

        private sealed class WarRoomCountersDocument
        {
            [BsonId]
            public string Id { get; set; } = "sequence";
            public int NextIncidentId { get; set; }
            public int NextLegalCaseId { get; set; }
            public int NextRedZoneDecisionId { get; set; }
            public int NextPodId { get; set; }
            public int NextCoalitionId { get; set; }
        }
    }
}

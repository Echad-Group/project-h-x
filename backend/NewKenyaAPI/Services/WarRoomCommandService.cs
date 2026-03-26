using NewKenyaAPI.Models;
using System.Collections.Concurrent;

namespace NewKenyaAPI.Services
{
    public class WarRoomCommandService
    {
        private readonly WarRoomMongoStore _mongoStore;
        private readonly ConcurrentDictionary<string, WarRoomCommandLane> _lanes = new();
        private readonly ConcurrentDictionary<string, WarRoomCommandPodItem> _pods = new();
        private readonly ConcurrentDictionary<int, WarRoomIncidentItem> _incidents = new();
        private readonly ConcurrentDictionary<string, WarRoomBattleRhythmItem> _battleRhythm = new();
        private readonly ConcurrentDictionary<int, WarRoomLegalCaseItem> _legalCases = new();
        private readonly ConcurrentDictionary<string, WarRoomCommandGridStatusItem> _commandGrid = new();
        private readonly ConcurrentDictionary<string, CoalitionCommandItem> _coalitionCommands = new();
        private readonly ConcurrentDictionary<string, MobilizationRoleAssignmentItem> _mobilizationRoles = new();
        private readonly WarRoomRedZoneState _redZoneState = new();
        private readonly CampaignPhaseState _campaignPhaseState = new();
        private int _nextIncidentId = 0;
        private int _nextLegalCaseId = 0;
        private int _nextRedZoneDecisionId = 0;
        private int _nextPodId = 0;
        private int _nextCoalitionId = 0;

        public WarRoomCommandService(WarRoomMongoStore mongoStore)
        {
            _mongoStore = mongoStore;
            SeedDefaultLanes();
            SeedDefaultPods();
            SeedDefaultCommandGrid();
            SeedDefaultCoalitionCommands();
            SeedDefaultMobilizationRoles();
            SeedDefaultCampaignPhases();
            RestoreStateFromPersistence();
            PersistSnapshot();
        }

        public WarRoomStateResponse GetState()
        {
            EnsureBattleRhythmForDate(DateOnly.FromDateTime(DateTime.UtcNow));

            return new WarRoomStateResponse
            {
                WarRoomName = "National Campaign War Room",
                SnapshotAt = DateTime.UtcNow,
                Lanes = _lanes.Values
                    .OrderByDescending(l => l.Priority)
                    .ThenBy(l => l.Name)
                    .ToList(),
                CommandPods = _pods.Values
                    .OrderBy(p => p.Name)
                    .ToList(),
                ActiveIncidents = _incidents.Values
                    .OrderByDescending(i => i.UpdatedAt)
                    .ToList()
            };
        }

        public List<WarRoomCommandPodItem> GetCommandPods()
        {
            return _pods.Values
                .OrderBy(p => p.Name)
                .ToList();
        }

        public WarRoomCommandPodItem CreateCommandPod(WarRoomCommandPodCreateRequest request)
        {
            var id = $"pod-{Interlocked.Increment(ref _nextPodId):D3}";
            var now = DateTime.UtcNow;

            var pod = new WarRoomCommandPodItem
            {
                Id = id,
                Name = request.Name.Trim(),
                Status = "Active",
                BattlegroundRegion = request.BattlegroundRegion.Trim(),
                DemographicBloc = request.DemographicBloc.Trim(),
                OpponentLane = request.OpponentLane.Trim(),
                FocusObjective = request.FocusObjective?.Trim(),
                Members = BuildDefaultPodMembers(),
                UpdatedAt = now
            };

            _pods[id] = pod;
            PersistSnapshot();
            return pod;
        }

        public WarRoomCommandPodItem? UpdateCommandPod(string podId, WarRoomCommandPodUpdateRequest request)
        {
            if (!_pods.TryGetValue(podId, out var pod))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                pod.Status = NormalizePodStatus(request.Status);
            }

            if (!string.IsNullOrWhiteSpace(request.BattlegroundRegion))
            {
                pod.BattlegroundRegion = request.BattlegroundRegion.Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.DemographicBloc))
            {
                pod.DemographicBloc = request.DemographicBloc.Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.OpponentLane))
            {
                pod.OpponentLane = request.OpponentLane.Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.FocusObjective))
            {
                pod.FocusObjective = request.FocusObjective.Trim();
            }

            pod.UpdatedAt = DateTime.UtcNow;
            PersistSnapshot();
            return pod;
        }

        public WarRoomRedZoneState GetRedZoneState(bool includeDecisions = true)
        {
            if (_redZoneState.IsElectionWeekModeEnabled && _redZoneState.NextCheckpointAt == null)
            {
                _redZoneState.NextCheckpointAt = DateTime.UtcNow.AddMinutes(_redZoneState.DecisionIntervalMinutes);
            }

            if (includeDecisions)
            {
                return _redZoneState;
            }

            // Return metadata-only state for lightweight dashboard polling.
            return new WarRoomRedZoneState
            {
                IsElectionWeekModeEnabled = _redZoneState.IsElectionWeekModeEnabled,
                ActivatedAt = _redZoneState.ActivatedAt,
                ActivatedByUserId = _redZoneState.ActivatedByUserId,
                DecisionIntervalMinutes = _redZoneState.DecisionIntervalMinutes,
                LastDecisionAt = _redZoneState.LastDecisionAt,
                NextCheckpointAt = _redZoneState.NextCheckpointAt,
                Decisions = new List<WarRoomRedZoneDecisionLogItem>()
            };
        }

        public PagedResponse<WarRoomIncidentItem> GetIncidents(string? status, string? search, int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _incidents.Values.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(status) && !string.Equals(status, "all", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(incident =>
                    string.Equals(incident.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(incident =>
                    incident.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    incident.Description.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    incident.Lane.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            var filtered = query
                .OrderByDescending(incident => incident.UpdatedAt)
                .ToList();

            return new PagedResponse<WarRoomIncidentItem>
            {
                Items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = filtered.Count,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResponse<WarRoomLegalCaseItem> GetLegalCases(string? status, string? search, int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _legalCases.Values.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(status) && !string.Equals(status, "all", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(item =>
                    string.Equals(item.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(item =>
                    item.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    item.Jurisdiction.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    item.FilingType.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            var filtered = query
                .OrderByDescending(item => item.UpdatedAt)
                .ToList();

            return new PagedResponse<WarRoomLegalCaseItem>
            {
                Items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = filtered.Count,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResponse<WarRoomRedZoneDecisionLogItem> GetRedZoneDecisions(string? severity, string? search, int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _redZoneState.Decisions.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(severity) && !string.Equals(severity, "all", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(item =>
                    string.Equals(item.Severity, severity, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(item =>
                    item.DecisionTitle.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    item.DecisionSummary.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    (item.OwnerRole != null && item.OwnerRole.Contains(term, StringComparison.OrdinalIgnoreCase)));
            }

            var filtered = query
                .OrderByDescending(item => item.LoggedAt)
                .ToList();

            return new PagedResponse<WarRoomRedZoneDecisionLogItem>
            {
                Items = filtered.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = filtered.Count,
                Page = page,
                PageSize = pageSize
            };
        }

        public WarRoomRedZoneState ToggleRedZoneMode(string currentUserId, WarRoomRedZoneToggleRequest request)
        {
            _redZoneState.IsElectionWeekModeEnabled = request.Enable;
            _redZoneState.DecisionIntervalMinutes = request.DecisionIntervalMinutes;

            if (request.Enable)
            {
                _redZoneState.ActivatedAt = DateTime.UtcNow;
                _redZoneState.ActivatedByUserId = currentUserId;
                _redZoneState.NextCheckpointAt = DateTime.UtcNow.AddMinutes(request.DecisionIntervalMinutes);
            }
            else
            {
                _redZoneState.NextCheckpointAt = null;
            }

            PersistSnapshot();
            return _redZoneState;
        }

        public WarRoomRedZoneDecisionLogItem AddRedZoneDecision(WarRoomRedZoneDecisionCreateRequest request)
        {
            var now = DateTime.UtcNow;

            var item = new WarRoomRedZoneDecisionLogItem
            {
                Id = Interlocked.Increment(ref _nextRedZoneDecisionId),
                LoggedAt = now,
                DecisionTitle = request.DecisionTitle.Trim(),
                DecisionSummary = request.DecisionSummary.Trim(),
                CheckpointHourLabel = request.CheckpointHourLabel.Trim(),
                OwnerRole = request.OwnerRole?.Trim(),
                Severity = NormalizeSeverity(request.Severity),
                Notes = request.Notes?.Trim()
            };

            _redZoneState.Decisions.Insert(0, item);
            _redZoneState.LastDecisionAt = now;
            if (_redZoneState.IsElectionWeekModeEnabled)
            {
                _redZoneState.NextCheckpointAt = now.AddMinutes(_redZoneState.DecisionIntervalMinutes);
            }

            if (_redZoneState.Decisions.Count > 300)
            {
                _redZoneState.Decisions = _redZoneState.Decisions.Take(300).ToList();
            }

            PersistSnapshot();
            return item;
        }

        public List<WarRoomCommandGridStatusItem> GetCommandGridStatus()
        {
            return _commandGrid.Values
                .OrderBy(item => ResolveTierRank(item.Tier))
                .ThenBy(item => item.Name)
                .ToList();
        }

        public WarRoomCommandGridStatusItem? UpdateCommandGridNode(string nodeId, WarRoomCommandGridUpdateRequest request)
        {
            if (!_commandGrid.TryGetValue(nodeId, out var node))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(request.ReadinessStatus))
            {
                node.ReadinessStatus = NormalizeReadinessStatus(request.ReadinessStatus);
            }

            if (request.StaffingPercent.HasValue)
            {
                node.StaffingPercent = request.StaffingPercent.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.ConnectivityStatus))
            {
                node.ConnectivityStatus = request.ConnectivityStatus.Trim();
            }

            if (request.ActiveIncidents.HasValue)
            {
                node.ActiveIncidents = request.ActiveIncidents.Value;
            }

            node.LastHeartbeatAt = DateTime.UtcNow;
            node.UpdatedAt = DateTime.UtcNow;
            PersistSnapshot();
            return node;
        }

        public List<CoalitionCommandItem> GetCoalitionCommands()
        {
            return _coalitionCommands.Values
                .OrderBy(item => item.Name)
                .ToList();
        }

        public CoalitionCommandItem CreateCoalitionCommand(CoalitionCommandCreateRequest request)
        {
            var id = $"coalition-{Interlocked.Increment(ref _nextCoalitionId):D3}";
            var item = new CoalitionCommandItem
            {
                Id = id,
                Name = request.Name.Trim(),
                DirectorName = request.DirectorName.Trim(),
                Status = "Active",
                UpdatedAt = DateTime.UtcNow,
                Modules = BuildDefaultCoalitionModules()
            };

            _coalitionCommands[id] = item;
            PersistSnapshot();
            return item;
        }

        public CoalitionCommandItem? UpdateCoalitionModule(string coalitionId, CoalitionModuleUpdateRequest request)
        {
            if (!_coalitionCommands.TryGetValue(coalitionId, out var coalition))
            {
                return null;
            }

            var targetModule = coalition.Modules.FirstOrDefault(module =>
                string.Equals(module.GroupType, request.GroupType, StringComparison.OrdinalIgnoreCase));

            if (targetModule == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                targetModule.Status = NormalizeCoalitionModuleStatus(request.Status);
            }

            if (!string.IsNullOrWhiteSpace(request.ExecutionPlan))
            {
                targetModule.ExecutionPlan = request.ExecutionPlan.Trim();
            }

            targetModule.LastEngagementAt = DateTime.UtcNow;
            coalition.UpdatedAt = DateTime.UtcNow;
            PersistSnapshot();
            return coalition;
        }

        public List<MobilizationRoleAssignmentItem> GetMobilizationRoles()
        {
            return _mobilizationRoles.Values
                .OrderBy(item => item.RoleTitle)
                .ToList();
        }

        public MobilizationRoleAssignmentItem? UpdateMobilizationRole(string roleCode, MobilizationRoleAssignmentUpdateRequest request)
        {
            if (!_mobilizationRoles.TryGetValue(roleCode, out var role))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(request.AssignedLeadName))
            {
                role.AssignedLeadName = request.AssignedLeadName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.RegionFocus))
            {
                role.RegionFocus = request.RegionFocus.Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                role.Status = NormalizeMobilizationStatus(request.Status);
            }

            if (!string.IsNullOrWhiteSpace(request.OperationalNotes))
            {
                role.OperationalNotes = request.OperationalNotes.Trim();
            }

            PersistSnapshot();
            return role;
        }

        public CampaignPhaseState GetCampaignPhaseState()
        {
            return _campaignPhaseState;
        }

        public CampaignPhaseState SwitchCampaignPhase(string currentUserId, CampaignPhaseSwitchRequest request)
        {
            var normalizedPhase = NormalizeCampaignPhase(request.Phase);
            foreach (var phase in _campaignPhaseState.Phases)
            {
                phase.Status = phase.Phase == normalizedPhase ? "Active" : "Inactive";
            }

            _campaignPhaseState.ActivePhase = normalizedPhase;
            _campaignPhaseState.ActiveSince = DateTime.UtcNow;
            _campaignPhaseState.UpdatedByUserId = currentUserId;
            PersistSnapshot();
            return _campaignPhaseState;
        }

        public List<WarRoomBattleRhythmItem> GetBattleRhythm(DateOnly date)
        {
            EnsureBattleRhythmForDate(date);

            var datePrefix = $"{date:yyyy-MM-dd}:";
            return _battleRhythm.Values
                .Where(item => item.Id.StartsWith(datePrefix, StringComparison.Ordinal))
                .OrderBy(item => item.TimeSlot)
                .ToList();
        }

        public WarRoomBattleRhythmItem? CompleteBattleRhythmItem(
            string itemId,
            string completedByUserId,
            WarRoomBattleRhythmCompleteRequest request)
        {
            if (!_battleRhythm.TryGetValue(itemId, out var item))
            {
                return null;
            }

            item.Completed = true;
            item.CompletedAt = DateTime.UtcNow;
            item.CompletedByUserId = completedByUserId;
            item.AttendanceCount = request.AttendanceCount;
            item.CompletionNotes = request.CompletionNotes?.Trim();

            PersistSnapshot();
            return item;
        }

        public WarRoomIncidentItem CreateIncident(string createdByUserId, WarRoomIncidentCreateRequest request)
        {
            var incidentId = Interlocked.Increment(ref _nextIncidentId);
            var now = DateTime.UtcNow;

            var normalizedSeverity = NormalizeSeverity(request.Severity);
            var normalizedLane = string.IsNullOrWhiteSpace(request.Lane)
                ? "strategy"
                : request.Lane.Trim().ToLowerInvariant();

            var incident = new WarRoomIncidentItem
            {
                Id = incidentId,
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Severity = normalizedSeverity,
                Status = "Open",
                Lane = normalizedLane,
                OwnerUserId = createdByUserId,
                EscalationLevel = ResolveDefaultEscalationLevel(normalizedSeverity),
                EscalationOwnerRole = ResolveEscalationOwnerRole(ResolveDefaultEscalationLevel(normalizedSeverity)),
                EscalationDueAt = ResolveEscalationDueAt(ResolveDefaultEscalationLevel(normalizedSeverity)),
                CreatedAt = now,
                UpdatedAt = now
            };

            _incidents[incidentId] = incident;

            if (_lanes.TryGetValue(normalizedLane, out var lane))
            {
                lane.Status = normalizedSeverity is "Critical" or "High" ? "Alert" : "Monitoring";
                lane.OpenIncidents = _incidents.Values.Count(i => i.Lane == normalizedLane && i.Status != "Resolved");
                lane.UpdatedAt = now;
            }

            PersistSnapshot();
            return incident;
        }

        public WarRoomIncidentItem? UpdateIncident(int incidentId, WarRoomIncidentUpdateRequest request)
        {
            if (!_incidents.TryGetValue(incidentId, out var incident))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                incident.Status = NormalizeIncidentStatus(request.Status);
            }

            if (!string.IsNullOrWhiteSpace(request.OwnerUserId))
            {
                incident.OwnerUserId = request.OwnerUserId.Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.ResolutionNotes))
            {
                incident.ResolutionNotes = request.ResolutionNotes.Trim();
            }

            incident.UpdatedAt = DateTime.UtcNow;

            if (_lanes.TryGetValue(incident.Lane, out var lane))
            {
                lane.OpenIncidents = _incidents.Values.Count(i => i.Lane == incident.Lane && i.Status != "Resolved");
                lane.Status = lane.OpenIncidents > 0 ? "Monitoring" : "Operational";
                lane.UpdatedAt = DateTime.UtcNow;
            }

            PersistSnapshot();
            return incident;
        }

        public WarRoomIncidentItem? EscalateIncident(int incidentId, WarRoomIncidentEscalateRequest request)
        {
            if (!_incidents.TryGetValue(incidentId, out var incident))
            {
                return null;
            }

            incident.EscalationLevel = request.EscalationLevel;
            incident.EscalationOwnerRole = ResolveEscalationOwnerRole(request.EscalationLevel);
            incident.EscalationDueAt = ResolveEscalationDueAt(request.EscalationLevel);
            incident.Status = request.EscalationLevel >= 2 ? "Escalated" : incident.Status;

            if (!string.IsNullOrWhiteSpace(request.Rationale))
            {
                incident.ResolutionNotes = request.Rationale.Trim();
            }

            incident.UpdatedAt = DateTime.UtcNow;
            PersistSnapshot();
            return incident;
        }

        public List<WarRoomLegalCaseItem> GetLegalCases()
        {
            return _legalCases.Values
                .OrderByDescending(c => c.UpdatedAt)
                .ToList();
        }

        public WarRoomLegalCaseItem CreateLegalCase(string createdByUserId, WarRoomLegalCaseCreateRequest request)
        {
            var caseId = Interlocked.Increment(ref _nextLegalCaseId);
            var now = DateTime.UtcNow;

            var legalCase = new WarRoomLegalCaseItem
            {
                Id = caseId,
                IncidentId = request.IncidentId,
                Title = request.Title.Trim(),
                Jurisdiction = request.Jurisdiction.Trim(),
                FilingType = request.FilingType.Trim(),
                Status = "Open",
                AssignedLawyerUserId = createdByUserId,
                Summary = request.Summary?.Trim(),
                CreatedAt = now,
                UpdatedAt = now
            };

            _legalCases[caseId] = legalCase;
            PersistSnapshot();
            return legalCase;
        }

        public WarRoomLegalCaseItem? UpdateLegalCase(int caseId, WarRoomLegalCaseUpdateRequest request)
        {
            if (!_legalCases.TryGetValue(caseId, out var legalCase))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                legalCase.Status = NormalizeLegalStatus(request.Status);
            }

            if (!string.IsNullOrWhiteSpace(request.AssignedLawyerUserId))
            {
                legalCase.AssignedLawyerUserId = request.AssignedLawyerUserId.Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.Summary))
            {
                legalCase.Summary = request.Summary.Trim();
            }

            legalCase.UpdatedAt = DateTime.UtcNow;
            PersistSnapshot();
            return legalCase;
        }

        private void RestoreStateFromPersistence()
        {
            try
            {
                var snapshot = _mongoStore.LoadSnapshot();
                if (snapshot == null)
                {
                    return;
                }

                ApplySnapshot(snapshot);
            }
            catch
            {
                // Fall back to seeded in-memory defaults if persisted state is unavailable.
            }
        }

        private void PersistSnapshot()
        {
            try
            {
                _mongoStore.SaveSnapshot(BuildSnapshot());
            }
            catch
            {
                // Persistence issues should not block war-room command operations.
            }
        }

        private WarRoomPersistenceSnapshot BuildSnapshot()
        {
            return new WarRoomPersistenceSnapshot
            {
                Lanes = _lanes.Values.ToList(),
                Pods = _pods.Values.ToList(),
                Incidents = _incidents.Values.ToList(),
                BattleRhythm = _battleRhythm.Values.ToList(),
                LegalCases = _legalCases.Values.ToList(),
                CommandGrid = _commandGrid.Values.ToList(),
                CoalitionCommands = _coalitionCommands.Values.ToList(),
                MobilizationRoles = _mobilizationRoles.Values.ToList(),
                RedZoneState = _redZoneState,
                CampaignPhaseState = _campaignPhaseState,
                NextIncidentId = _nextIncidentId,
                NextLegalCaseId = _nextLegalCaseId,
                NextRedZoneDecisionId = _nextRedZoneDecisionId,
                NextPodId = _nextPodId,
                NextCoalitionId = _nextCoalitionId
            };
        }

        private void ApplySnapshot(WarRoomPersistenceSnapshot snapshot)
        {
            _lanes.Clear();
            foreach (var item in snapshot.Lanes)
            {
                _lanes[item.Id] = item;
            }

            _pods.Clear();
            foreach (var item in snapshot.Pods)
            {
                _pods[item.Id] = item;
            }

            _incidents.Clear();
            foreach (var item in snapshot.Incidents)
            {
                _incidents[item.Id] = item;
            }

            _battleRhythm.Clear();
            foreach (var item in snapshot.BattleRhythm)
            {
                _battleRhythm[item.Id] = item;
            }

            _legalCases.Clear();
            foreach (var item in snapshot.LegalCases)
            {
                _legalCases[item.Id] = item;
            }

            _commandGrid.Clear();
            foreach (var item in snapshot.CommandGrid)
            {
                _commandGrid[item.Id] = item;
            }

            _coalitionCommands.Clear();
            foreach (var item in snapshot.CoalitionCommands)
            {
                _coalitionCommands[item.Id] = item;
            }

            _mobilizationRoles.Clear();
            foreach (var item in snapshot.MobilizationRoles)
            {
                _mobilizationRoles[item.RoleCode] = item;
            }

            _redZoneState.IsElectionWeekModeEnabled = snapshot.RedZoneState.IsElectionWeekModeEnabled;
            _redZoneState.ActivatedAt = snapshot.RedZoneState.ActivatedAt;
            _redZoneState.ActivatedByUserId = snapshot.RedZoneState.ActivatedByUserId;
            _redZoneState.DecisionIntervalMinutes = snapshot.RedZoneState.DecisionIntervalMinutes;
            _redZoneState.LastDecisionAt = snapshot.RedZoneState.LastDecisionAt;
            _redZoneState.NextCheckpointAt = snapshot.RedZoneState.NextCheckpointAt;
            _redZoneState.Decisions = snapshot.RedZoneState.Decisions;

            _campaignPhaseState.ActivePhase = snapshot.CampaignPhaseState.ActivePhase;
            _campaignPhaseState.ActiveSince = snapshot.CampaignPhaseState.ActiveSince;
            _campaignPhaseState.UpdatedByUserId = snapshot.CampaignPhaseState.UpdatedByUserId;
            _campaignPhaseState.Phases = snapshot.CampaignPhaseState.Phases;

            _nextIncidentId = Math.Max(snapshot.NextIncidentId, _incidents.Keys.DefaultIfEmpty(0).Max());
            _nextLegalCaseId = Math.Max(snapshot.NextLegalCaseId, _legalCases.Keys.DefaultIfEmpty(0).Max());
            _nextRedZoneDecisionId = Math.Max(snapshot.NextRedZoneDecisionId, _redZoneState.Decisions.Select(item => item.Id).DefaultIfEmpty(0).Max());
            _nextPodId = Math.Max(snapshot.NextPodId, ResolveHighestSequence(_pods.Keys, "pod-"));
            _nextCoalitionId = Math.Max(snapshot.NextCoalitionId, ResolveHighestSequence(_coalitionCommands.Keys, "coalition-"));
        }

        private static int ResolveHighestSequence(IEnumerable<string> ids, string prefix)
        {
            var max = 0;
            foreach (var id in ids)
            {
                if (!id.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (int.TryParse(id[prefix.Length..], out var value) && value > max)
                {
                    max = value;
                }
            }

            return max;
        }

        private void SeedDefaultLanes()
        {
            AddLane("strategy", "Strategy & Intelligence", "Operational", "Chief Strategist", 6);
            AddLane("communications", "Communications Command", "Operational", "Communications Director", 5);
            AddLane("data", "Data & Analytics", "Operational", "Data Director", 4);
            AddLane("field", "Field Operations", "Operational", "Field Director", 5);
            AddLane("legal", "Legal & Compliance", "Operational", "General Counsel", 6);
            AddLane("digital", "Digital & Cyber", "Operational", "Digital Director", 4);
        }

        private void SeedDefaultPods()
        {
            AddPod(
                "Nairobi Rapid Persuasion Pod",
                "Nairobi",
                "Urban Youth + First-time Voters",
                "Opponent Digital Narrative",
                "Counter misinformation clusters and convert undecided digital audiences.");

            AddPod(
                "Rift Valley Turnout Pod",
                "Rift Valley",
                "Rural Women + Smallholder Families",
                "Opponent Ground Mobilization",
                "Lift turnout by 12% in low-propensity wards through door-to-door activation.");

            AddPod(
                "Coastal Coalition Pod",
                "Coast",
                "Faith + Business Constituency",
                "Opponent Coalition Messaging",
                "Protect coalition vote share and neutralize wedge narrative attacks.");
        }

        private void AddPod(
            string name,
            string battlegroundRegion,
            string demographicBloc,
            string opponentLane,
            string focusObjective)
        {
            var id = $"pod-{Interlocked.Increment(ref _nextPodId):D3}";
            _pods[id] = new WarRoomCommandPodItem
            {
                Id = id,
                Name = name,
                Status = "Active",
                BattlegroundRegion = battlegroundRegion,
                DemographicBloc = demographicBloc,
                OpponentLane = opponentLane,
                FocusObjective = focusObjective,
                Members = BuildDefaultPodMembers(),
                UpdatedAt = DateTime.UtcNow
            };
        }

        private static List<WarRoomCommandPodMember> BuildDefaultPodMembers()
        {
            return new List<WarRoomCommandPodMember>
            {
                new() { Role = "Data Lead", LeadName = "Data Director", Lane = "data" },
                new() { Role = "Field Lead", LeadName = "Field Director", Lane = "field" },
                new() { Role = "Digital Lead", LeadName = "Digital Director", Lane = "digital" },
                new() { Role = "Comms Lead", LeadName = "Communications Director", Lane = "communications" }
            };
        }

        private void AddLane(string id, string name, string status, string lead, int priority)
        {
            _lanes[id] = new WarRoomCommandLane
            {
                Id = id,
                Name = name,
                Status = status,
                Lead = lead,
                Priority = priority,
                OpenIncidents = 0,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private static string NormalizeSeverity(string? severity)
        {
            var value = (severity ?? string.Empty).Trim().ToLowerInvariant();
            return value switch
            {
                "critical" => "Critical",
                "high" => "High",
                "medium" => "Medium",
                "low" => "Low",
                _ => "Medium"
            };
        }

        private static string NormalizeIncidentStatus(string? status)
        {
            var value = (status ?? string.Empty).Trim().ToLowerInvariant();
            return value switch
            {
                "open" => "Open",
                "triaged" => "Triaged",
                "escalated" => "Escalated",
                "resolved" => "Resolved",
                _ => "Open"
            };
        }

        private static string NormalizePodStatus(string? status)
        {
            var value = (status ?? string.Empty).Trim().ToLowerInvariant();
            return value switch
            {
                "active" => "Active",
                "standby" => "Standby",
                "escalated" => "Escalated",
                "completed" => "Completed",
                _ => "Active"
            };
        }

        private static int ResolveDefaultEscalationLevel(string severity)
        {
            return severity switch
            {
                "Critical" => 4,
                "High" => 3,
                "Medium" => 2,
                _ => 1
            };
        }

        private static string ResolveEscalationOwnerRole(int escalationLevel)
        {
            return escalationLevel switch
            {
                1 => "Rapid Response Team",
                2 => "Strategy + Communications Joint Command",
                3 => "War Room Commander",
                _ => "Legal Command"
            };
        }

        private static DateTime ResolveEscalationDueAt(int escalationLevel)
        {
            var now = DateTime.UtcNow;
            return escalationLevel switch
            {
                1 => now.AddHours(4),
                2 => now.AddHours(2),
                3 => now.AddHours(1),
                _ => now.AddMinutes(30)
            };
        }

        private static string NormalizeLegalStatus(string? status)
        {
            var value = (status ?? string.Empty).Trim().ToLowerInvariant();
            return value switch
            {
                "open" => "Open",
                "drafting" => "Drafting",
                "filed" => "Filed",
                "hearing" => "Hearing",
                "closed" => "Closed",
                _ => "Open"
            };
        }

        private static string NormalizeReadinessStatus(string? status)
        {
            var value = (status ?? string.Empty).Trim().ToLowerInvariant();
            return value switch
            {
                "ready" => "Ready",
                "degraded" => "Degraded",
                "offline" => "Offline",
                "critical" => "Critical",
                _ => "Ready"
            };
        }

        private static string NormalizeCoalitionModuleStatus(string? status)
        {
            var value = (status ?? string.Empty).Trim().ToLowerInvariant();
            return value switch
            {
                "planned" => "Planned",
                "active" => "Active",
                "blocked" => "Blocked",
                "completed" => "Completed",
                _ => "Planned"
            };
        }

        private static string NormalizeMobilizationStatus(string? status)
        {
            var value = (status ?? string.Empty).Trim().ToLowerInvariant();
            return value switch
            {
                "open" => "Open",
                "assigned" => "Assigned",
                "active" => "Active",
                "standby" => "Standby",
                "completed" => "Completed",
                _ => "Open"
            };
        }

        private static string NormalizeCampaignPhase(string? phase)
        {
            var value = (phase ?? string.Empty).Trim().ToLowerInvariant();
            return value switch
            {
                "exploratory" => "Exploratory",
                "launch" => "Launch",
                "persuasion" => "Persuasion",
                "gotv" => "GOTV",
                _ => "Exploratory"
            };
        }

        private void SeedDefaultCommandGrid()
        {
            AddGridNode("national-hq", "National", "National HQ", null, "Ready", 96, "Online", 1);
            AddGridNode("regional-nairobi", "Regional", "Nairobi Regional Command", "national-hq", "Ready", 91, "Online", 2);
            AddGridNode("regional-rift", "Regional", "Rift Valley Regional Command", "national-hq", "Degraded", 84, "Online", 3);
            AddGridNode("county-nairobi", "County", "Nairobi County Command", "regional-nairobi", "Ready", 93, "Online", 2);
            AddGridNode("county-nakuru", "County", "Nakuru County Command", "regional-rift", "Degraded", 80, "Intermittent", 4);
            AddGridNode("subcounty-westlands", "SubCounty", "Westlands Sub-County Post", "county-nairobi", "Ready", 88, "Online", 1);
            AddGridNode("subcounty-naivasha", "SubCounty", "Naivasha Sub-County Post", "county-nakuru", "Degraded", 75, "Intermittent", 2);
            AddGridNode("ward-parklands", "Ward", "Parklands/H'lands Ward Desk", "subcounty-westlands", "Ready", 90, "Online", 0);
            AddGridNode("ward-bahati", "Ward", "Bahati Ward Desk", "subcounty-naivasha", "Critical", 61, "Intermittent", 3);
            AddGridNode("polling-bahati-01", "PollingUnit", "Bahati Polling Unit 01", "ward-bahati", "Degraded", 68, "Intermittent", 1);
        }

        private void AddGridNode(
            string id,
            string tier,
            string name,
            string? parentId,
            string readinessStatus,
            int staffingPercent,
            string connectivityStatus,
            int activeIncidents)
        {
            _commandGrid[id] = new WarRoomCommandGridStatusItem
            {
                Id = id,
                Tier = tier,
                Name = name,
                ParentId = parentId,
                ReadinessStatus = readinessStatus,
                StaffingPercent = staffingPercent,
                ConnectivityStatus = connectivityStatus,
                ActiveIncidents = activeIncidents,
                LastHeartbeatAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private static int ResolveTierRank(string tier)
        {
            var value = tier.Trim().ToLowerInvariant();
            return value switch
            {
                "national" => 1,
                "regional" => 2,
                "county" => 3,
                "subcounty" => 4,
                "ward" => 5,
                "pollingunit" => 6,
                _ => 99
            };
        }

        private void SeedDefaultCoalitionCommands()
        {
            AddCoalitionCommand("National Coalition Desk", "Coalition Director");
            AddCoalitionCommand("Faith and Values Desk", "Faith Outreach Director");
        }

        private void AddCoalitionCommand(string name, string directorName)
        {
            var id = $"coalition-{Interlocked.Increment(ref _nextCoalitionId):D3}";
            _coalitionCommands[id] = new CoalitionCommandItem
            {
                Id = id,
                Name = name,
                DirectorName = directorName,
                Status = "Active",
                Modules = BuildDefaultCoalitionModules(),
                UpdatedAt = DateTime.UtcNow
            };
        }

        private static List<CoalitionOutreachModuleItem> BuildDefaultCoalitionModules()
        {
            return new List<CoalitionOutreachModuleItem>
            {
                new() { GroupType = "Youth", Objective = "Campus and first-jobber mobilization", Status = "Planned" },
                new() { GroupType = "Women", Objective = "Women-led voter turnout cells", Status = "Planned" },
                new() { GroupType = "Faith", Objective = "Faith community engagement", Status = "Planned" },
                new() { GroupType = "Diaspora", Objective = "Diaspora digital fundraising and advocacy", Status = "Planned" },
                new() { GroupType = "Business", Objective = "SME and enterprise coalition dialogues", Status = "Planned" }
            };
        }

        private void SeedDefaultMobilizationRoles()
        {
            AddMobilizationRole("party-liaison", "Party Liaison");
            AddMobilizationRole("tribal-regional-mobilizer", "Tribal and Regional Mobilization Lead");
            AddMobilizationRole("religious-advisory", "Religious Advisory Lead");
            AddMobilizationRole("village-elder-coordinator", "Village Elder Coordinator");
        }

        private void AddMobilizationRole(string roleCode, string title)
        {
            _mobilizationRoles[roleCode] = new MobilizationRoleAssignmentItem
            {
                RoleCode = roleCode,
                RoleTitle = title,
                Status = "Open"
            };
        }

        private void SeedDefaultCampaignPhases()
        {
            _campaignPhaseState.ActivePhase = "Exploratory";
            _campaignPhaseState.ActiveSince = DateTime.UtcNow;
            _campaignPhaseState.Phases = new List<CampaignPhasePlanItem>
            {
                new()
                {
                    Phase = "Exploratory",
                    Status = "Active",
                    PlaybookSummary = "Stakeholder mapping, coalition scouting, and early message testing.",
                    Kpis = new List<CampaignPhaseKpiTarget>
                    {
                        new() { Name = "County listening forums", TargetValue = "47 counties" },
                        new() { Name = "Core volunteer captains", TargetValue = "2,000" }
                    }
                },
                new()
                {
                    Phase = "Launch",
                    Status = "Inactive",
                    PlaybookSummary = "National launch rallies, manifesto rollout, and coalition endorsements.",
                    Kpis = new List<CampaignPhaseKpiTarget>
                    {
                        new() { Name = "Launch events", TargetValue = "24 major events" },
                        new() { Name = "Press reach", TargetValue = "85% weekly coverage" }
                    }
                },
                new()
                {
                    Phase = "Persuasion",
                    Status = "Inactive",
                    PlaybookSummary = "Targeted persuasion in battleground wards and issue-based contrasts.",
                    Kpis = new List<CampaignPhaseKpiTarget>
                    {
                        new() { Name = "Door knocks", TargetValue = "3.5 million" },
                        new() { Name = "Digital persuasion CTR", TargetValue = ">= 4.2%" }
                    }
                },
                new()
                {
                    Phase = "GOTV",
                    Status = "Inactive",
                    PlaybookSummary = "Turnout command operations, transport, and polling-day rapid response.",
                    Kpis = new List<CampaignPhaseKpiTarget>
                    {
                        new() { Name = "Voter contact confirmation", TargetValue = "95%" },
                        new() { Name = "Polling unit staffing", TargetValue = "100%" }
                    }
                }
            };
        }

        private void EnsureBattleRhythmForDate(DateOnly date)
        {
            var dailyTemplate = new[]
            {
                (Time: "06:00", Name: "Intelligence Brief", Purpose: "Overnight media scan, sentiment report, opponent activity and polling update."),
                (Time: "09:00", Name: "Messaging Alignment", Purpose: "Approve daily narrative, talking points, and digital creatives."),
                (Time: "14:00", Name: "Field and Data Sync", Purpose: "Review turnout metrics, re-prioritize battlegrounds, and adjust ad spend."),
                (Time: "19:00", Name: "Performance Review", Purpose: "Assess media coverage, engagement analytics, risk monitoring, and crisis planning.")
            };

            var addedAny = false;
            foreach (var slot in dailyTemplate)
            {
                var id = $"{date:yyyy-MM-dd}:{slot.Time}";
                addedAny = _battleRhythm.TryAdd(id, new WarRoomBattleRhythmItem
                {
                    Id = id,
                    Date = date,
                    TimeSlot = slot.Time,
                    Name = slot.Name,
                    Purpose = slot.Purpose,
                    Completed = false,
                    AttendanceCount = 0
                }) || addedAny;
            }

            if (addedAny)
            {
                PersistSnapshot();
            }
        }

    }
}

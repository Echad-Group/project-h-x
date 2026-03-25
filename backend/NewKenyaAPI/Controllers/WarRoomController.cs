using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewKenyaAPI.Models;
using NewKenyaAPI.Services;
using System.Security.Claims;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.LeadershipAccess)]
    public class WarRoomController : ControllerBase
    {
        private readonly WarRoomCommandService _warRoomService;

        public WarRoomController(WarRoomCommandService warRoomService)
        {
            _warRoomService = warRoomService;
        }

        [HttpGet("state")]
        public ActionResult<WarRoomStateResponse> GetState()
        {
            var snapshot = _warRoomService.GetState();
            return Ok(snapshot);
        }

        [HttpGet("pods")]
        public ActionResult<List<WarRoomCommandPodItem>> GetCommandPods()
        {
            var pods = _warRoomService.GetCommandPods();
            return Ok(pods);
        }

        [HttpPost("pods")]
        public ActionResult<WarRoomCommandPodItem> CreateCommandPod([FromBody] WarRoomCommandPodCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var pod = _warRoomService.CreateCommandPod(request);
            return CreatedAtAction(nameof(GetCommandPods), new { }, pod);
        }

        [HttpPut("pods/{podId}")]
        public ActionResult<WarRoomCommandPodItem> UpdateCommandPod(string podId, [FromBody] WarRoomCommandPodUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updated = _warRoomService.UpdateCommandPod(podId, request);
            if (updated == null)
            {
                return NotFound(new { message = "Command pod not found." });
            }

            return Ok(updated);
        }

        [HttpPost("incidents")]
        public ActionResult<WarRoomIncidentItem> CreateIncident([FromBody] WarRoomIncidentCreateRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var incident = _warRoomService.CreateIncident(currentUserId, request);
            return CreatedAtAction(nameof(GetState), new { }, incident);
        }

        [HttpGet("battle-rhythm")]
        public ActionResult<List<WarRoomBattleRhythmItem>> GetBattleRhythm([FromQuery] string? date = null)
        {
            var targetDate = DateOnly.FromDateTime(DateTime.UtcNow);
            if (!string.IsNullOrWhiteSpace(date) && !DateOnly.TryParse(date, out targetDate))
            {
                return BadRequest(new { message = "Date must be in ISO format: YYYY-MM-DD." });
            }

            var items = _warRoomService.GetBattleRhythm(targetDate);
            return Ok(items);
        }

        [HttpPut("incidents/{incidentId:int}")]
        public ActionResult<WarRoomIncidentItem> UpdateIncident(int incidentId, [FromBody] WarRoomIncidentUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updated = _warRoomService.UpdateIncident(incidentId, request);
            if (updated == null)
            {
                return NotFound(new { message = "Incident not found." });
            }

            return Ok(updated);
        }

        [HttpPost("incidents/{incidentId:int}/escalate")]
        public ActionResult<WarRoomIncidentItem> EscalateIncident(int incidentId, [FromBody] WarRoomIncidentEscalateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updated = _warRoomService.EscalateIncident(incidentId, request);
            if (updated == null)
            {
                return NotFound(new { message = "Incident not found." });
            }

            return Ok(updated);
        }

        [HttpPost("battle-rhythm/{itemId}/complete")]
        public ActionResult<WarRoomBattleRhythmItem> CompleteBattleRhythmItem(
            string itemId,
            [FromBody] WarRoomBattleRhythmCompleteRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updated = _warRoomService.CompleteBattleRhythmItem(itemId, currentUserId, request);
            if (updated == null)
            {
                return NotFound(new { message = "Battle rhythm item not found." });
            }

            return Ok(updated);
        }

        [HttpGet("red-zone")]
        public ActionResult<WarRoomRedZoneState> GetRedZoneState([FromQuery] bool includeDecisions = true)
        {
            var state = _warRoomService.GetRedZoneState(includeDecisions);
            return Ok(state);
        }

        [HttpGet("incidents")]
        public ActionResult<PagedResponse<WarRoomIncidentItem>> GetIncidents(
            [FromQuery] string? status = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = _warRoomService.GetIncidents(status, search, page, pageSize);
            return Ok(result);
        }

        [HttpGet("red-zone/decisions")]
        public ActionResult<PagedResponse<WarRoomRedZoneDecisionLogItem>> GetRedZoneDecisions(
            [FromQuery] string? severity = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = _warRoomService.GetRedZoneDecisions(severity, search, page, pageSize);
            return Ok(result);
        }

        [HttpPost("red-zone/toggle")]
        public ActionResult<WarRoomRedZoneState> ToggleRedZoneMode([FromBody] WarRoomRedZoneToggleRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var state = _warRoomService.ToggleRedZoneMode(currentUserId, request);
            return Ok(state);
        }

        [HttpPost("red-zone/decisions")]
        public ActionResult<WarRoomRedZoneDecisionLogItem> AddRedZoneDecision([FromBody] WarRoomRedZoneDecisionCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var decision = _warRoomService.AddRedZoneDecision(request);
            return CreatedAtAction(nameof(GetRedZoneState), new { }, decision);
        }

        [HttpGet("command-grid")]
        public ActionResult<List<WarRoomCommandGridStatusItem>> GetCommandGrid()
        {
            var grid = _warRoomService.GetCommandGridStatus();
            return Ok(grid);
        }

        [HttpPut("command-grid/{nodeId}")]
        public ActionResult<WarRoomCommandGridStatusItem> UpdateCommandGridNode(string nodeId, [FromBody] WarRoomCommandGridUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updated = _warRoomService.UpdateCommandGridNode(nodeId, request);
            if (updated == null)
            {
                return NotFound(new { message = "Command grid node not found." });
            }

            return Ok(updated);
        }

        [HttpGet("coalitions")]
        public ActionResult<List<CoalitionCommandItem>> GetCoalitionCommands()
        {
            var commands = _warRoomService.GetCoalitionCommands();
            return Ok(commands);
        }

        [HttpPost("coalitions")]
        public ActionResult<CoalitionCommandItem> CreateCoalitionCommand([FromBody] CoalitionCommandCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var item = _warRoomService.CreateCoalitionCommand(request);
            return CreatedAtAction(nameof(GetCoalitionCommands), new { }, item);
        }

        [HttpPut("coalitions/{coalitionId}/modules")]
        public ActionResult<CoalitionCommandItem> UpdateCoalitionModule(string coalitionId, [FromBody] CoalitionModuleUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updated = _warRoomService.UpdateCoalitionModule(coalitionId, request);
            if (updated == null)
            {
                return NotFound(new { message = "Coalition command or module not found." });
            }

            return Ok(updated);
        }

        [HttpGet("mobilization-roles")]
        public ActionResult<List<MobilizationRoleAssignmentItem>> GetMobilizationRoles()
        {
            var items = _warRoomService.GetMobilizationRoles();
            return Ok(items);
        }

        [HttpPut("mobilization-roles/{roleCode}")]
        public ActionResult<MobilizationRoleAssignmentItem> UpdateMobilizationRole(string roleCode, [FromBody] MobilizationRoleAssignmentUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updated = _warRoomService.UpdateMobilizationRole(roleCode, request);
            if (updated == null)
            {
                return NotFound(new { message = "Mobilization role not found." });
            }

            return Ok(updated);
        }

        [HttpGet("campaign-phases")]
        public ActionResult<CampaignPhaseState> GetCampaignPhases()
        {
            var state = _warRoomService.GetCampaignPhaseState();
            return Ok(state);
        }

        [HttpPost("campaign-phases/switch")]
        public ActionResult<CampaignPhaseState> SwitchCampaignPhase([FromBody] CampaignPhaseSwitchRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var state = _warRoomService.SwitchCampaignPhase(currentUserId, request);
            return Ok(state);
        }

        [HttpGet("legal-cases")]
        public ActionResult<List<WarRoomLegalCaseItem>> GetLegalCases()
        {
            var items = _warRoomService.GetLegalCases();
            return Ok(items);
        }

        [HttpGet("legal-cases/query")]
        public ActionResult<PagedResponse<WarRoomLegalCaseItem>> GetLegalCasesQuery(
            [FromQuery] string? status = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var items = _warRoomService.GetLegalCases(status, search, page, pageSize);
            return Ok(items);
        }

        [HttpPost("legal-cases")]
        public ActionResult<WarRoomLegalCaseItem> CreateLegalCase([FromBody] WarRoomLegalCaseCreateRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var legalCase = _warRoomService.CreateLegalCase(currentUserId, request);
            return CreatedAtAction(nameof(GetLegalCases), new { }, legalCase);
        }

        [HttpPut("legal-cases/{caseId:int}")]
        public ActionResult<WarRoomLegalCaseItem> UpdateLegalCase(int caseId, [FromBody] WarRoomLegalCaseUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var updated = _warRoomService.UpdateLegalCase(caseId, request);
            if (updated == null)
            {
                return NotFound(new { message = "Legal case not found." });
            }

            return Ok(updated);
        }
    }
}

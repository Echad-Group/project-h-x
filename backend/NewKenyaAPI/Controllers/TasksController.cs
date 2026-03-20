using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using NewKenyaAPI.Services;
using System.Security.Claims;
using CampaignTask = NewKenyaAPI.Models.Task;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CampaignHierarchyService _hierarchyService;

        public TasksController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            CampaignHierarchyService hierarchyService)
        {
            _context = context;
            _userManager = userManager;
            _hierarchyService = hierarchyService;
        }

        [HttpPost("create")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> Create([FromBody] CampaignTaskCreateRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            if (request.Deadline <= DateTime.UtcNow)
            {
                return BadRequest(new { message = "Deadline must be in the future." });
            }

            if (!string.IsNullOrWhiteSpace(request.AssignedToUserId))
            {
                var canAssign = await CanAssignTaskAsync(currentUserId, request.AssignedToUserId);
                if (!canAssign)
                {
                    return BadRequest(new { message = "Assigned user must be within your downline hierarchy." });
                }
            }

            var task = new CampaignTask
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.Deadline,
                Priority = request.Priority,
                Status = CampaignTaskStatuses.Pending,
                AssignedToUserId = request.AssignedToUserId,
                AssignedByUserId = request.AssignedToUserId == null ? null : currentUserId,
                CreatedByUserId = currentUserId,
                Location = request.Location,
                Region = request.Region,
                County = request.County,
                SubCounty = request.SubCounty,
                Constituency = request.Constituency,
                Ward = request.Ward,
                PollingStation = request.PollingStation,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMyTasks), new { }, new
            {
                task.Id,
                task.Title,
                task.Status,
                deadline = task.DueDate,
                task.AssignedToUserId
            });
        }

        [HttpPost("assign")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> Assign([FromBody] CampaignTaskAssignRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            var task = await _context.Tasks.FirstOrDefaultAsync(item => item.Id == request.TaskId);
            if (task == null)
            {
                return NotFound();
            }

            var canAssign = await CanAssignTaskAsync(currentUserId, request.AssignedToUserId);
            if (!canAssign)
            {
                return BadRequest(new { message = "Assigned user must be within your downline hierarchy." });
            }

            task.AssignedToUserId = request.AssignedToUserId;
            task.AssignedByUserId = currentUserId;
            task.Status = CampaignTaskStatuses.Pending;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Task assigned successfully.",
                task.Id,
                task.AssignedToUserId,
                task.Status
            });
        }

        [HttpGet("my-tasks")]
        public async Task<ActionResult<object>> GetMyTasks()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            var tasks = await _context.Tasks
                .Where(task => task.AssignedToUserId == currentUserId)
                .OrderBy(task => task.DueDate)
                .Select(task => new
                {
                    task.Id,
                    task.Title,
                    task.Description,
                    task.Status,
                    task.Priority,
                    deadline = task.DueDate,
                    task.Region,
                    task.County,
                    task.Constituency,
                    task.Ward,
                    task.PollingStation,
                    task.CreatedAt,
                    task.UpdatedAt,
                    task.CompletedAt
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpPost("status")]
        public async Task<ActionResult<object>> UpdateStatus([FromBody] CampaignTaskStatusUpdateRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            var task = await _context.Tasks.FirstOrDefaultAsync(item => item.Id == request.TaskId);
            if (task == null)
            {
                return NotFound();
            }

            if (task.AssignedToUserId != currentUserId)
            {
                return Forbid();
            }

            if (request.Status != CampaignTaskStatuses.InProgress)
            {
                return BadRequest(new { message = "Only transition to 'In Progress' is supported from this endpoint." });
            }

            if (task.Status != CampaignTaskStatuses.Pending)
            {
                return BadRequest(new { message = "Task must be Pending before it can move to In Progress." });
            }

            task.Status = CampaignTaskStatuses.InProgress;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Task moved to In Progress.", task.Id, task.Status });
        }

        [HttpPost("complete")]
        public async Task<ActionResult<object>> Complete([FromBody] CampaignTaskCompleteRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            var task = await _context.Tasks.FirstOrDefaultAsync(item => item.Id == request.TaskId);
            if (task == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            var canOverride = currentRoles.Contains(UserRoles.Admin) || currentRoles.Contains(UserRoles.SuperAdmin);

            if (!canOverride && task.AssignedToUserId != currentUserId)
            {
                return Forbid();
            }

            if (task.Status != CampaignTaskStatuses.InProgress)
            {
                return BadRequest(new { message = "Task must be In Progress before it can be completed." });
            }

            task.Status = CampaignTaskStatuses.Completed;
            task.CompletionNotes = request.CompletionNotes;
            task.CompletedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Task completed successfully.", task.Id, task.Status, task.CompletedAt });
        }

        private async Task<bool> CanAssignTaskAsync(string currentUserId, string assignedToUserId)
        {
            if (currentUserId == assignedToUserId)
            {
                return true;
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null)
            {
                return false;
            }

            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            if (currentRoles.Contains(UserRoles.Admin) || currentRoles.Contains(UserRoles.SuperAdmin))
            {
                return true;
            }

            return await _hierarchyService.IsDownlineAsync(currentUserId, assignedToUserId);
        }
    }
}
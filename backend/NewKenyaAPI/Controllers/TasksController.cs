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

        [HttpGet("manage")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> GetManageTasks(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] string? priority = null,
            [FromQuery] string? region = null,
            [FromQuery] int? page = null,
            [FromQuery] int pageSize = 25,
            [FromQuery] int limit = 300)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            var isAdmin = currentRoles.Contains(UserRoles.Admin) || currentRoles.Contains(UserRoles.SuperAdmin);
            var descendantIds = isAdmin
                ? new HashSet<string>()
                : await _hierarchyService.GetDescendantIdsAsync(currentUserId);

            var query = _context.Tasks.AsQueryable();

            if (!isAdmin)
            {
                query = query.Where(task =>
                    task.CreatedByUserId == currentUserId ||
                    task.AssignedByUserId == currentUserId ||
                    task.AssignedToUserId == currentUserId ||
                    (task.AssignedToUserId != null && descendantIds.Contains(task.AssignedToUserId)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(task => task.Status.ToLower() == status.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(priority))
            {
                query = query.Where(task => task.Priority.ToLower() == priority.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(region))
            {
                query = query.Where(task => task.Region != null && task.Region.ToLower() == region.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim().ToLower();
                query = query.Where(task =>
                    task.Title.ToLower().Contains(q) ||
                    task.Description.ToLower().Contains(q) ||
                    (task.Region != null && task.Region.ToLower().Contains(q)) ||
                    (task.County != null && task.County.ToLower().Contains(q)) ||
                    (task.Constituency != null && task.Constituency.ToLower().Contains(q)) ||
                    (task.Ward != null && task.Ward.ToLower().Contains(q)));
            }

            limit = Math.Clamp(limit, 1, 1000);

            if (page.HasValue)
            {
                // Paged mode — returns { tasks, totalCount, page, pageSize }
                var totalCount = await query.CountAsync();
                var pageVal = Math.Max(1, page.Value);
                var pageSizeVal = Math.Clamp(pageSize, 1, 100);

                var tasks = await query
                    .OrderByDescending(task => task.CreatedAt)
                    .Skip((pageVal - 1) * pageSizeVal)
                    .Take(pageSizeVal)
                    .Select(task => new
                    {
                        task.Id,
                        task.Title,
                        task.Description,
                        task.Status,
                        task.Priority,
                        task.DueDate,
                        task.Location,
                        task.Region,
                        task.County,
                        task.SubCounty,
                        task.Constituency,
                        task.Ward,
                        task.PollingStation,
                        task.AssignedToUserId,
                        task.AssignedByUserId,
                        task.CreatedByUserId,
                        task.CreatedAt,
                        task.UpdatedAt,
                        task.CompletedAt,
                        task.CompletionNotes
                    })
                    .ToListAsync();

                return Ok(new { tasks, totalCount, page = pageVal, pageSize = pageSizeVal });
            }
            else
            {
                // Legacy flat-array mode — uses limit param
                var tasks = await query
                    .OrderByDescending(task => task.CreatedAt)
                    .Take(limit)
                    .Select(task => new
                    {
                        task.Id,
                        task.Title,
                        task.Description,
                        task.Status,
                        task.Priority,
                        task.DueDate,
                        task.Location,
                        task.Region,
                        task.County,
                        task.SubCounty,
                        task.Constituency,
                        task.Ward,
                        task.PollingStation,
                        task.AssignedToUserId,
                        task.AssignedByUserId,
                        task.CreatedByUserId,
                        task.CreatedAt,
                        task.UpdatedAt,
                        task.CompletedAt,
                        task.CompletionNotes
                    })
                    .ToListAsync();

                return Ok(tasks);
            }
        }

        [HttpPut("{taskId:int}")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> UpdateTask(int taskId, [FromBody] CampaignTaskUpdateRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            var task = await _context.Tasks.FirstOrDefaultAsync(item => item.Id == taskId);
            if (task == null)
            {
                return NotFound();
            }

            var canManage = await CanManageTaskAsync(currentUserId, task);
            if (!canManage)
            {
                return Forbid();
            }

            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                task.Title = request.Title;
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                task.Description = request.Description;
            }

            if (!string.IsNullOrWhiteSpace(request.Priority))
            {
                task.Priority = request.Priority;
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                task.Status = request.Status;
            }

            if (request.Deadline.HasValue)
            {
                task.DueDate = request.Deadline;
            }

            if (request.AssignedToUserId != null)
            {
                task.AssignedToUserId = request.AssignedToUserId;
                task.AssignedByUserId = currentUserId;
            }

            task.Location = request.Location ?? task.Location;
            task.Region = request.Region ?? task.Region;
            task.County = request.County ?? task.County;
            task.SubCounty = request.SubCounty ?? task.SubCounty;
            task.Constituency = request.Constituency ?? task.Constituency;
            task.Ward = request.Ward ?? task.Ward;
            task.PollingStation = request.PollingStation ?? task.PollingStation;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Task updated successfully.", task.Id });
        }

        [HttpDelete("{taskId:int}")]
        [Authorize(Roles = UserRoles.LeadershipAccess)]
        public async Task<ActionResult<object>> DeleteTask(int taskId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Unauthorized();
            }

            var task = await _context.Tasks.FirstOrDefaultAsync(item => item.Id == taskId);
            if (task == null)
            {
                return NotFound();
            }

            var canManage = await CanManageTaskAsync(currentUserId, task);
            if (!canManage)
            {
                return Forbid();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Task deleted successfully.", taskId });
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

        private async Task<bool> CanManageTaskAsync(string currentUserId, CampaignTask task)
        {
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

            if (task.CreatedByUserId == currentUserId || task.AssignedByUserId == currentUserId || task.AssignedToUserId == currentUserId)
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(task.AssignedToUserId))
            {
                return await _hierarchyService.IsDownlineAsync(currentUserId, task.AssignedToUserId);
            }

            return false;
        }
    }
}
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
    public class AdminEmailArchiveController : ControllerBase
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AdminEmailArchiveController> _logger;

        public AdminEmailArchiveController(IWebHostEnvironment environment, ILogger<AdminEmailArchiveController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArchivedEmailDto>>> GetArchive([FromQuery] int limit = 200)
        {
            if (limit <= 0)
            {
                limit = 200;
            }

            if (limit > 1000)
            {
                limit = 1000;
            }

            var archivePath = Path.Combine(_environment.ContentRootPath, "App_Data", "email-archive.json");

            if (!System.IO.File.Exists(archivePath))
            {
                return Ok(Array.Empty<ArchivedEmailDto>());
            }

            try
            {
                await using var readStream = System.IO.File.OpenRead(archivePath);
                var records = await JsonSerializer.DeserializeAsync<List<ArchivedEmailDto>>(readStream, JsonOptions)
                              ?? new List<ArchivedEmailDto>();

                var result = records
                    .OrderByDescending(item => item.SentAtUtc)
                    .Take(limit)
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read email archive from JSON file.");
                return StatusCode(500, new { message = "Could not read email archive." });
            }
        }

        public sealed class ArchivedEmailDto
        {
            public DateTime SentAtUtc { get; set; }
            public string To { get; set; } = string.Empty;
            public string Subject { get; set; } = string.Empty;
            public string HtmlBody { get; set; } = string.Empty;
            public string FromEmail { get; set; } = string.Empty;
            public string FromName { get; set; } = string.Empty;
            public bool SmtpConfigured { get; set; }
            public string DeliveryStatus { get; set; } = string.Empty;
            public string? Error { get; set; }
        }
    }
}

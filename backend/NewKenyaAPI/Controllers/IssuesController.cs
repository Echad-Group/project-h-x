using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;

namespace NewKenyaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IssuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Issues
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetIssues([FromQuery] bool includeUnpublished = false)
        {
            var query = _context.Issues
                .Include(i => i.Initiatives.OrderBy(init => init.DisplayOrder))
                .Include(i => i.Questions.OrderBy(q => q.DisplayOrder))
                .AsQueryable();

            if (!includeUnpublished)
            {
                query = query.Where(i => i.IsPublished);
            }

            var issues = await query
                .OrderBy(i => i.DisplayOrder)
                .Select(i => new
                {
                    i.Id,
                    i.Title,
                    i.Slug,
                    i.Summary,
                    i.Icon,
                    i.Color,
                    i.DisplayOrder,
                    i.IsPublished,
                    Initiatives = i.Initiatives.Select(init => new
                    {
                        init.Id,
                        init.Title,
                        init.Description,
                        init.DisplayOrder
                    }).ToList(),
                    Questions = i.Questions.Select(q => new
                    {
                        q.Id,
                        q.Question,
                        q.Answer,
                        q.DisplayOrder
                    }).ToList()
                })
                .ToListAsync();

            return Ok(issues);
        }

        // GET: api/Issues/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetIssue(int id)
        {
            var issue = await _context.Issues
                .Include(i => i.Initiatives.OrderBy(init => init.DisplayOrder))
                .Include(i => i.Questions.OrderBy(q => q.DisplayOrder))
                .Where(i => i.Id == id)
                .Select(i => new
                {
                    i.Id,
                    i.Title,
                    i.Slug,
                    i.Summary,
                    i.Icon,
                    i.Color,
                    i.DisplayOrder,
                    i.IsPublished,
                    Initiatives = i.Initiatives.Select(init => new
                    {
                        init.Id,
                        init.Title,
                        init.Description,
                        init.DisplayOrder
                    }).ToList(),
                    Questions = i.Questions.Select(q => new
                    {
                        q.Id,
                        q.Question,
                        q.Answer,
                        q.DisplayOrder
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (issue == null)
            {
                return NotFound();
            }

            return Ok(issue);
        }

        // GET: api/Issues/slug/economic-growth-jobs
        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<object>> GetIssueBySlug(string slug)
        {
            var issue = await _context.Issues
                .Include(i => i.Initiatives.OrderBy(init => init.DisplayOrder))
                .Include(i => i.Questions.OrderBy(q => q.DisplayOrder))
                .Where(i => i.Slug == slug && i.IsPublished)
                .Select(i => new
                {
                    i.Id,
                    i.Title,
                    i.Slug,
                    i.Summary,
                    i.Icon,
                    i.Color,
                    i.DisplayOrder,
                    Initiatives = i.Initiatives.Select(init => new
                    {
                        init.Id,
                        init.Title,
                        init.Description,
                        init.DisplayOrder
                    }).ToList(),
                    Questions = i.Questions.Select(q => new
                    {
                        q.Id,
                        q.Question,
                        q.Answer,
                        q.DisplayOrder
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (issue == null)
            {
                return NotFound();
            }

            return Ok(issue);
        }

        // POST: api/Issues
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<Issue>> CreateIssue(Issue issue)
        {
            issue.CreatedAt = DateTime.UtcNow;
            issue.Slug = GenerateSlug(issue.Title);
            
            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIssue), new { id = issue.Id }, issue);
        }

        // PUT: api/Issues/5
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UpdateIssue(int id, Issue issue)
        {
            if (id != issue.Id)
            {
                return BadRequest();
            }

            var existingIssue = await _context.Issues.FindAsync(id);
            if (existingIssue == null)
            {
                return NotFound();
            }

            existingIssue.Title = issue.Title;
            existingIssue.Slug = GenerateSlug(issue.Title);
            existingIssue.Summary = issue.Summary;
            existingIssue.Icon = issue.Icon;
            existingIssue.Color = issue.Color;
            existingIssue.DisplayOrder = issue.DisplayOrder;
            existingIssue.IsPublished = issue.IsPublished;
            existingIssue.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Issues/5
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteIssue(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }

            _context.Issues.Remove(issue);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Issues/5/initiatives
        [HttpPost("{issueId}/initiatives")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<IssueInitiative>> AddInitiative(int issueId, IssueInitiative initiative)
        {
            var issue = await _context.Issues.FindAsync(issueId);
            if (issue == null)
            {
                return NotFound();
            }

            initiative.IssueId = issueId;
            _context.IssueInitiatives.Add(initiative);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIssue), new { id = issueId }, initiative);
        }

        // PUT: api/Issues/initiatives/5
        [HttpPut("initiatives/{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UpdateInitiative(int id, IssueInitiative initiative)
        {
            if (id != initiative.Id)
            {
                return BadRequest();
            }

            _context.Entry(initiative).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.IssueInitiatives.Any(i => i.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Issues/initiatives/5
        [HttpDelete("initiatives/{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteInitiative(int id)
        {
            var initiative = await _context.IssueInitiatives.FindAsync(id);
            if (initiative == null)
            {
                return NotFound();
            }

            _context.IssueInitiatives.Remove(initiative);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Issues/5/questions
        [HttpPost("{issueId}/questions")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<IssueQuestion>> AddQuestion(int issueId, IssueQuestion question)
        {
            var issue = await _context.Issues.FindAsync(issueId);
            if (issue == null)
            {
                return NotFound();
            }

            question.IssueId = issueId;
            _context.IssueQuestions.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIssue), new { id = issueId }, question);
        }

        // PUT: api/Issues/questions/5
        [HttpPut("questions/{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> UpdateQuestion(int id, IssueQuestion question)
        {
            if (id != question.Id)
            {
                return BadRequest();
            }

            _context.Entry(question).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.IssueQuestions.Any(q => q.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Issues/questions/5
        [HttpDelete("questions/{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.IssueQuestions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _context.IssueQuestions.Remove(question);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Issues/seed
        [HttpPost("seed")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> SeedIssues()
        {
            // Check if issues already exist
            if (await _context.Issues.AnyAsync())
            {
                return BadRequest(new { message = "Issues already exist. Clear the database first if you want to reseed." });
            }

            var issues = new List<Issue>
            {
                new Issue
                {
                    Title = "Economic Growth & Jobs",
                    Slug = "economic-growth-jobs",
                    Summary = "Create pathways for youth employment and support SMEs.",
                    Icon = "💼",
                    Color = "#2563eb",
                    DisplayOrder = 1,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow,
                    Initiatives = new List<IssueInitiative>
                    {
                        new IssueInitiative { Title = "Digital economy development and innovation hubs", DisplayOrder = 1 },
                        new IssueInitiative { Title = "Youth entrepreneurship funding and mentorship", DisplayOrder = 2 },
                        new IssueInitiative { Title = "SME tax incentives and simplified registration", DisplayOrder = 3 },
                        new IssueInitiative { Title = "Technical skills training and apprenticeships", DisplayOrder = 4 }
                    },
                    Questions = new List<IssueQuestion>
                    {
                        new IssueQuestion
                        {
                            Question = "What is the plan for youth employment?",
                            Answer = "Our comprehensive plan includes apprenticeships, startup funding, and skills training programs focused on digital economy and future industries.",
                            DisplayOrder = 1
                        },
                        new IssueQuestion
                        {
                            Question = "How will you support small businesses?",
                            Answer = "Through tax incentives, simplified registration, access to low-interest loans, and digital transformation support.",
                            DisplayOrder = 2
                        }
                    }
                },
                new Issue
                {
                    Title = "Healthcare Access",
                    Slug = "healthcare-access",
                    Summary = "Universal primary healthcare and stronger clinics.",
                    Icon = "🏥",
                    Color = "#dc2626",
                    DisplayOrder = 2,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow,
                    Initiatives = new List<IssueInitiative>
                    {
                        new IssueInitiative { Title = "Universal primary healthcare coverage", DisplayOrder = 1 },
                        new IssueInitiative { Title = "Modern county health facilities", DisplayOrder = 2 },
                        new IssueInitiative { Title = "Preventive care programs", DisplayOrder = 3 },
                        new IssueInitiative { Title = "Digital health records system", DisplayOrder = 4 }
                    },
                    Questions = new List<IssueQuestion>
                    {
                        new IssueQuestion
                        {
                            Question = "What does universal primary healthcare mean?",
                            Answer = "It means ensuring every Kenyan has access to essential health services without financial hardship, supported by modern facilities and digital health systems.",
                            DisplayOrder = 1
                        }
                    }
                },
                new Issue
                {
                    Title = "Education & Skills",
                    Slug = "education-skills",
                    Summary = "Invest in technical training, scholarships and digital learning.",
                    Icon = "📚",
                    Color = "#059669",
                    DisplayOrder = 3,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow,
                    Initiatives = new List<IssueInitiative>
                    {
                        new IssueInitiative { Title = "Digital learning infrastructure", DisplayOrder = 1 },
                        new IssueInitiative { Title = "STEM education focus", DisplayOrder = 2 },
                        new IssueInitiative { Title = "Technical training partnerships", DisplayOrder = 3 },
                        new IssueInitiative { Title = "Higher education scholarships", DisplayOrder = 4 }
                    },
                    Questions = new List<IssueQuestion>
                    {
                        new IssueQuestion
                        {
                            Question = "How will you improve digital learning?",
                            Answer = "By providing tablets, internet connectivity, digital curriculum resources, and teacher training in modern educational technology.",
                            DisplayOrder = 1
                        }
                    }
                },
                new Issue
                {
                    Title = "Security & Justice",
                    Slug = "security-justice",
                    Summary = "Community safety and accountable institutions.",
                    Icon = "🛡️",
                    Color = "#7c3aed",
                    DisplayOrder = 4,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow,
                    Initiatives = new List<IssueInitiative>
                    {
                        new IssueInitiative { Title = "Community policing programs", DisplayOrder = 1 },
                        new IssueInitiative { Title = "Modern police equipment and training", DisplayOrder = 2 },
                        new IssueInitiative { Title = "Judicial system reforms", DisplayOrder = 3 },
                        new IssueInitiative { Title = "Anti-corruption measures", DisplayOrder = 4 }
                    },
                    Questions = new List<IssueQuestion>
                    {
                        new IssueQuestion
                        {
                            Question = "How will you enhance community safety?",
                            Answer = "Through community policing programs, better street lighting, rapid response systems, and modern police training.",
                            DisplayOrder = 1
                        }
                    }
                },
                new Issue
                {
                    Title = "Infrastructure & Technology",
                    Slug = "infrastructure-technology",
                    Summary = "Modern infrastructure and digital connectivity.",
                    Icon = "🌐",
                    Color = "#0891b2",
                    DisplayOrder = 5,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow,
                    Initiatives = new List<IssueInitiative>
                    {
                        new IssueInitiative { Title = "Rural internet connectivity", DisplayOrder = 1 },
                        new IssueInitiative { Title = "Smart city initiatives", DisplayOrder = 2 },
                        new IssueInitiative { Title = "Green energy projects", DisplayOrder = 3 },
                        new IssueInitiative { Title = "Transport modernization", DisplayOrder = 4 }
                    },
                    Questions = new List<IssueQuestion>
                    {
                        new IssueQuestion
                        {
                            Question = "What is the plan for digital connectivity?",
                            Answer = "We will implement a comprehensive rural internet program, smart city initiatives, and support for tech innovation hubs.",
                            DisplayOrder = 1
                        }
                    }
                },
                new Issue
                {
                    Title = "Agriculture & Environment",
                    Slug = "agriculture-environment",
                    Summary = "Sustainable farming and environmental protection.",
                    Icon = "🌾",
                    Color = "#65a30d",
                    DisplayOrder = 6,
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow,
                    Initiatives = new List<IssueInitiative>
                    {
                        new IssueInitiative { Title = "Modern farming techniques", DisplayOrder = 1 },
                        new IssueInitiative { Title = "Climate change adaptation", DisplayOrder = 2 },
                        new IssueInitiative { Title = "Water conservation", DisplayOrder = 3 },
                        new IssueInitiative { Title = "Forest protection", DisplayOrder = 4 }
                    },
                    Questions = new List<IssueQuestion>
                    {
                        new IssueQuestion
                        {
                            Question = "How will you support farmers?",
                            Answer = "Through modern farming technology, irrigation systems, market access, and climate-smart agriculture training.",
                            DisplayOrder = 1
                        }
                    }
                }
            };

            _context.Issues.AddRange(issues);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Successfully seeded {issues.Count} issues with initiatives and questions." });
        }

        private bool IssueExists(int id)
        {
            return _context.Issues.Any(e => e.Id == id);
        }

        private string GenerateSlug(string title)
        {
            return title.ToLowerInvariant()
                .Replace(" & ", "-")
                .Replace(" ", "-")
                .Replace("&", "and")
                .Replace(",", "")
                .Replace(".", "")
                .Replace("'", "")
                .Replace("\"", "");
        }
    }
}

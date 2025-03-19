using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DevHouse1.Models;
using Microsoft.AspNetCore.Authorization;
using DevHouse1.DTOs;

namespace DevHouse1.Controllers
{
    /// <summary>
    /// Controller for managing developers.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DeveloperController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeveloperController"/> class.
        /// </summary>
        /// <param name="context">The database context for accessing developer data.</param>
        public DeveloperController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all developers.
        /// </summary>
        /// <response code="200">Developers retrieved successfully</response>
        /// <response code="404">No developers found</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<DeveloperDTO>>> GetDevelopers()
        {
            if (_context.Developers == null)
            {
                return NotFound();
            }

            var developers = await _context.Developers
                .Include(d => d.Role)
                .Include(d => d.Team)
                .ToListAsync();

            var developerDTOs = developers.Select(d => new DeveloperDTO
            {
                Id = d.Id,
                Firstname = d.Firstname,
                Lastname = d.Lastname,
                RoleId = d.RoleId,
                RoleName = d.Role?.Name,
                TeamId = d.TeamId,
                TeamName = d.Team?.Name
            }).ToList();

            return Ok(developerDTOs);
        }

        /// <summary>
        /// Retrieves a developer by ID.
        /// </summary>
        /// <response code="200">Developer found</response>
        /// <response code="404">Developer not found</response>
        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeveloperDTO>> GetDeveloper(int Id)
        {
            if (_context.Developers == null)
            {
                return NotFound();
            }

            var developer = await _context.Developers
                .Include(d => d.Role)
                .Include(d => d.Team)
                .FirstOrDefaultAsync(d => d.Id == Id);

            if (developer == null)
            {
                return NotFound(new { Message = "Developer not found" });
            }

            var developerDTO = new DeveloperDTO
            {
                Id = developer.Id,
                Firstname = developer.Firstname,
                Lastname = developer.Lastname,
                RoleId = developer.RoleId,
                RoleName = developer.Role?.Name,
                TeamId = developer.TeamId,
                TeamName = developer.Team?.Name
            };

            return Ok(developerDTO);
        }

        /// <summary>
        /// Creates a new developer.
        /// </summary>
        /// <response code="201">Developer created successfully</response>
        /// <response code="400">Invalid developer data</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DeveloperDTO>> AddDeveloper([FromBody] DeveloperDTO developerDTO)
        {
            if (developerDTO == null)
            {
                return BadRequest(new { Message = "Invalid developer data" });
            }

            var team = await _context.Teams.FindAsync(developerDTO.TeamId);
            var role = await _context.Roles.FindAsync(developerDTO.RoleId);

            if (team == null || role == null)
            {
                return BadRequest(new { Message = "Invalid TeamId or RoleId." });
            }

            var developer = new Developer
            {
                Firstname = developerDTO.Firstname,
                Lastname = developerDTO.Lastname,
                RoleId = developerDTO.RoleId,
                TeamId = developerDTO.TeamId,
                Role = role,
                Team = team
            };

            await _context.Developers.AddAsync(developer);
            await _context.SaveChangesAsync();

            developerDTO.Id = developer.Id;
            developerDTO.RoleName = role.Name;
            developerDTO.TeamName = team.Name;

            return CreatedAtAction(nameof(GetDeveloper), new { Id = developer.Id }, developerDTO);
        }

        /// <summary>
        /// Updates an existing developer.
        /// </summary>
        /// <response code="200">Developer updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Developer not found</response>
        [HttpPut("{Id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeveloperDTO>> UpdateDeveloper(int Id, [FromBody] DeveloperDTO developerDTO)
        {
            if (developerDTO == null)
            {
                return BadRequest(new { Message = "Invalid request. Provide developer details." });
            }

            var existingDeveloper = await _context.Developers.FindAsync(Id);
            if (existingDeveloper == null)
            {
                return NotFound(new { Message = "Developer not found" });
            }

            existingDeveloper.Firstname = developerDTO.Firstname;
            existingDeveloper.Lastname = developerDTO.Lastname;
            existingDeveloper.RoleId = developerDTO.RoleId;
            existingDeveloper.TeamId = developerDTO.TeamId;

            await _context.SaveChangesAsync();
            return Ok(developerDTO);
        }

        /// <summary>
        /// Deletes a developer by ID.
        /// </summary>
        /// <response code="204">Developer successfully deleted</response>
        /// <response code="404">Developer not found</response>
        [HttpDelete("{Id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteDeveloper(int Id)
        {
            if (_context.Developers == null)
            {
                return NotFound();
            }

            var developer = await _context.Developers.FindAsync(Id);
            if (developer == null)
            {
                return NotFound(new { Message = "Developer not found" });
            }

            _context.Developers.Remove(developer);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

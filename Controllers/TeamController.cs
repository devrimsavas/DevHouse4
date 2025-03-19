using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DevHouse1.Models;
using Microsoft.AspNetCore.Authorization;
using DevHouse1.DTOs;

namespace DevHouse1.Controllers
{

    /// <summary>
    /// Controller for managing teams.
    /// </summary>
    [Route("api/[Controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamController"/> class.
        /// </summary>
        /// <param name="context">The database context for accessing team data.</param>

        public TeamController(AppDbContext context)
        {
            _context = context;
        }

        //  Get all Teams
        /// <summary>
        /// Retrieves all teams.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     GET /api/Team
        /// 
        /// </remarks>
        /// <response code="200">Teams retrieved successfully</response>
        /// <response code="404">No teams found</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TeamDTO>>> GetTeams()
        {
            if (_context.Teams == null)
            {
                return NotFound();
            }
            var teams = await _context.Teams.ToListAsync();

            var teamDTOs = teams.Select(t => new TeamDTO
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();

            return Ok(teamDTOs);
        }

        //  Get a Single Team by ID
        /// <summary>
        /// Retrieves a team by ID.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     GET /api/Team/1
        /// 
        /// **Example Response:**
        /// 
        /// ```json
        /// {
        ///   "id": 1,
        ///   "name": "Development Team"
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Team found</response>
        /// <response code="404">Team not found</response>
        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeamDTO>> GetTeam(int Id)
        {
            if (_context.Teams == null)
            {
                return NotFound();
            }
            var team = await _context.Teams.FindAsync(Id);

            if (team == null)
            {
                return NotFound(new { Message = "Team not found" });
            }

            var teamDTO = new TeamDTO
            {
                Id = team.Id,
                Name = team.Name
            };

            return Ok(teamDTO);
        }

        //  Create a New Team
        /// <summary>
        /// Creates a new team.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     POST /api/Team
        /// 
        /// **Example Body:**
        /// 
        /// ```json
        /// {
        ///   "name": "Development Team"
        /// }
        /// ```
        /// </remarks>
        /// <response code="201">Team created successfully</response>
        /// <response code="400">Invalid team data or team already exists</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TeamDTO>> AddTeam([FromBody] TeamDTO teamDTO)
        {
            if (teamDTO == null || string.IsNullOrEmpty(teamDTO.Name))
            {
                return BadRequest(new { Message = "Invalid team data" });
            }

            var existingTeam = await _context.Teams.FirstOrDefaultAsync(t => t.Name == teamDTO.Name);
            if (existingTeam != null)
            {
                return BadRequest(new { Message = $"A team with the name '{teamDTO.Name}' already exists." });
            }

            var team = new Team
            {
                Name = teamDTO.Name
            };

            await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();

            teamDTO.Id = team.Id;
            return CreatedAtAction(nameof(GetTeam), new { Id = team.Id }, teamDTO);
        }

        //  Update an Existing Team
        /// <summary>
        /// Updates an existing team.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     PUT /api/Team/1
        /// 
        /// **Example Body:**
        /// 
        /// ```json
        /// {
        ///   "name": "Updated Development Team"
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Team updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Team not found</response>
        [HttpPut("{Id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeamDTO>> UpdateTeam(int Id, [FromBody] TeamDTO teamDTO)
        {
            if (teamDTO == null || string.IsNullOrEmpty(teamDTO.Name))
            {
                return BadRequest(new { Message = "Invalid request. Name cannot be empty." });
            }

            var existingTeam = await _context.Teams.FindAsync(Id);
            if (existingTeam == null)
            {
                return NotFound(new { Message = "Team not found" });
            }

            existingTeam.Name = teamDTO.Name;

            await _context.SaveChangesAsync();

            teamDTO.Id = existingTeam.Id;
            return Ok(teamDTO);
        }

        //  Delete a Team
        /// <summary>
        /// Deletes a team by ID.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     DELETE /api/Team/1
        /// 
        /// </remarks>
        /// <response code="204">Team successfully deleted</response>
        /// <response code="404">Team not found</response>
        [HttpDelete("{Id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteTeam(int Id)
        {
            if (_context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(Id);
            if (team == null)
            {
                return NotFound(new { Message = "Team not found" });
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

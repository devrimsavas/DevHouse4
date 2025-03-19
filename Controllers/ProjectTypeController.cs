using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DevHouse1.Models;
using DevHouse1.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DevHouse1.Controllers
{
    /// <summary>
    /// Controller for managing project types.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectTypeController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTypeController"/> class.
        /// </summary>
        /// <param name="context">The database context for accessing project type data.</param>
        public ProjectTypeController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all project types.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     GET /api/ProjectType
        /// 
        /// </remarks>
        /// <response code="200">Project types retrieved successfully</response>
        /// <response code="404">No project types found</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<ProjectTypeDTO>>> GetProjectTypes()
        {
            if (_context.ProjectTypes == null)
            {
                return NotFound();
            }

            var projectTypes = await _context.ProjectTypes.ToListAsync();
            var projectTypeDTOs = projectTypes.Select(pt => new ProjectTypeDTO
            {
                Id = pt.Id,
                Name = pt.Name
            }).ToList();

            return Ok(projectTypeDTOs);
        }

        /// <summary>
        /// Retrieves a project type by ID.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     GET /api/ProjectType/1
        /// 
        /// **Example Response:**
        /// 
        /// ```json
        /// {
        ///   "id": 1,
        ///   "name": "Web Development"
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Project type found</response>
        /// <response code="404">Project type not found</response>
        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectTypeDTO>> GetProjectType(int Id)
        {
            if (_context.ProjectTypes == null)
            {
                return NotFound();
            }

            var projectType = await _context.ProjectTypes.FindAsync(Id);
            if (projectType == null)
            {
                return NotFound(new { Message = "Project Type not found" });
            }

            var projectTypeDTO = new ProjectTypeDTO
            {
                Id = projectType.Id,
                Name = projectType.Name
            };

            return Ok(projectTypeDTO);
        }

        /// <summary>
        /// Creates a new project type.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     POST /api/ProjectType
        /// 
        /// **Example Body:**
        /// 
        /// ```json
        /// {
        ///   "name": "Web Development"
        /// }
        /// ```
        /// </remarks>
        /// <response code="201">Project type created successfully</response>
        /// <response code="400">Project type already exists</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProjectTypeDTO>> AddProjectType([FromBody] ProjectTypeDTO projectTypeDTO)
        {
            if (string.IsNullOrWhiteSpace(projectTypeDTO.Name))
            {
                return BadRequest(new { Message = "Project type name is required." });
            }

            var existingProjectType = await _context.ProjectTypes
                .FirstOrDefaultAsync(p => p.Name == projectTypeDTO.Name);

            if (existingProjectType != null)
            {
                return BadRequest(new { Message = $"Project type '{projectTypeDTO.Name}' already exists." });
            }

            var projectType = new ProjectType
            {
                Name = projectTypeDTO.Name
            };

            await _context.ProjectTypes.AddAsync(projectType);
            await _context.SaveChangesAsync();

            projectTypeDTO.Id = projectType.Id;
            return CreatedAtAction(nameof(GetProjectType), new { Id = projectType.Id }, projectTypeDTO);
        }

        /// <summary>
        /// Updates an existing project type.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     PUT /api/ProjectType/updateprojecttype/1
        /// 
        /// **Example Body:**
        /// 
        /// ```json
        /// {
        ///   "id": 1,
        ///   "name": "Updated Project Type"
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Project type updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Project type not found</response>
        [HttpPut("updateprojecttype/{Id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectTypeDTO>> UpdateProjectType(int Id, [FromBody] ProjectTypeDTO projectTypeDTO)
        {
            if (string.IsNullOrWhiteSpace(projectTypeDTO.Name))
            {
                return BadRequest(new { Message = "Project type name cannot be empty." });
            }

            var existingProjectType = await _context.ProjectTypes.FindAsync(Id);
            if (existingProjectType == null)
            {
                return NotFound(new { Message = "Project Type not found" });
            }

            existingProjectType.Name = projectTypeDTO.Name;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Project type updated successfully", projectTypeDTO });
        }

        /// <summary>
        /// Deletes a project type by ID.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     DELETE /api/ProjectType/1
        /// 
        /// </remarks>
        /// <response code="204">Project type successfully deleted</response>
        /// <response code="404">Project type not found</response>
        [HttpDelete("{Id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProjectType(int Id)
        {
            if (_context.ProjectTypes == null)
            {
                return NotFound();
            }

            var projectType = await _context.ProjectTypes.FindAsync(Id);
            if (projectType == null)
            {
                return NotFound(new { Message = "Project Type not found" });
            }

            _context.ProjectTypes.Remove(projectType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

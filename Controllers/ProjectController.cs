using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DevHouse1.Models;
using Microsoft.AspNetCore.Authorization;
using DevHouse1.DTOs;

namespace DevHouse1.Controllers
{

    /// <summary>
    /// Controller for managing projects.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectController"/> class.
        /// </summary>
        /// <param name="context">The database context for accessing project data.</param>

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        //  Get all projects
        /// <summary>
        /// Retrieves all projects.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     GET /api/Project
        /// 
        /// </remarks>
        /// <response code="200">Projects retrieved successfully</response>
        /// <response code="404">No projects found</response>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<ProjectDTO>>> GetAllProjects()
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }

            var projects = await _context.Projects
                .Include(p => p.ProjectType)
                .Include(p => p.Team)
                .ToListAsync();

            var projectDTOs = projects.Select(p => new ProjectDTO
            {
                Id = p.Id,
                Name = p.Name,
                ProjectTypeId = p.ProjectTypeId,
                ProjectTypeName = p.ProjectType?.Name, // Get project type name
                TeamId = p.TeamId,
                TeamName = p.Team?.Name // Get team name
            }).ToList();

            return Ok(projectDTOs);
        }



        //  Get a single project by ID
        /// <summary>
        /// Retrieves a project by ID.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     GET /api/Project/1
        /// 
        /// **Example Response:**
        /// 
        /// ```json
        /// {
        ///   "id": 1,
        ///   "name": "Website Redesign",
        ///   "projectTypeId": 2,
        ///   "teamId": 1
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Project found</response>
        /// <response code="404">Project not found</response>

        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ProjectDTO>> GetProject(int Id)
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.ProjectType)
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Id == Id);

            if (project == null)
            {
                return NotFound(new { Message = "Project not found" });
            }

            var projectDTO = new ProjectDTO
            {
                Id = project.Id,
                Name = project.Name,
                ProjectTypeId = project.ProjectTypeId,
                ProjectTypeName = project.ProjectType?.Name,
                TeamId = project.TeamId,
                TeamName = project.Team?.Name
            };

            return Ok(projectDTO);
        }



        //  Create a new project
        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     POST /api/Project
        /// 
        /// **Example Body:**
        /// 
        /// ```json
        /// {
        ///   "name": "Website Redesign",
        ///   "projectTypeId": 2,
        ///   "teamId": 1
        /// }
        /// ```
        /// </remarks>
        /// <response code="201">Project created successfully</response>
        /// <response code="400">Invalid project data</response>

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<ProjectDTO>> AddProject([FromBody] ProjectDTO projectDTO)
        {
            if (projectDTO == null)
            {
                return BadRequest(new { Message = "Invalid project data" });
            }

            // 🔹 ADD VALIDATION HERE
            if (projectDTO.TeamId <= 0 || projectDTO.ProjectTypeId <= 0)
            {
                return BadRequest(new { Message = "TeamId and ProjectTypeId must be valid positive numbers." });
            }

            var team = await _context.Teams.FindAsync(projectDTO.TeamId);
            var projectType = await _context.ProjectTypes.FindAsync(projectDTO.ProjectTypeId);

            if (team == null)
            {
                return BadRequest(new { Message = "Invalid TeamId. Team does not exist." });
            }
            if (projectType == null)
            {
                return BadRequest(new { Message = "Invalid ProjectTypeId. Project Type does not exist." });
            }

            var project = new Project
            {
                Name = projectDTO.Name,
                ProjectTypeId = projectDTO.ProjectTypeId,
                TeamId = projectDTO.TeamId,
                Team = team,
                ProjectType = projectType
            };

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            projectDTO.Id = project.Id;
            projectDTO.TeamName = team.Name;
            projectDTO.ProjectTypeName = projectType.Name;

            return CreatedAtAction(nameof(GetProject), new { Id = project.Id }, projectDTO);
        }


        //  Update an existing project
        /// <summary>
        /// Updates an existing project.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     PUT /api/Project/1
        /// 
        /// **Example Body:**
        /// 
        /// ```json
        /// {
        ///   "name": "Website Redesign Updated",
        ///   "projectTypeId": 2,
        ///   "teamId": 1
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Project updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Project not found</response>
        [HttpPut("{Id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ProjectDTO>> UpdateProject(int Id, [FromBody] ProjectDTO projectDTO)
        {
            if (projectDTO == null)
            {
                return BadRequest(new { Message = "Invalid request. Provide project details." });
            }

            var existingProject = await _context.Projects.FindAsync(Id);
            if (existingProject == null)
            {
                return NotFound(new { Message = "Project not found" });
            }

            // 🔹 ADD VALIDATION HERE
            if (projectDTO.TeamId <= 0 || projectDTO.ProjectTypeId <= 0)
            {
                return BadRequest(new { Message = "TeamId and ProjectTypeId must be valid positive numbers." });
            }

            var team = await _context.Teams.FindAsync(projectDTO.TeamId);
            var projectType = await _context.ProjectTypes.FindAsync(projectDTO.ProjectTypeId);

            if (team == null)
            {
                return BadRequest(new { Message = "Invalid TeamId. Team does not exist." });
            }
            if (projectType == null)
            {
                return BadRequest(new { Message = "Invalid ProjectTypeId. Project Type does not exist." });
            }

            existingProject.Name = projectDTO.Name;
            existingProject.TeamId = projectDTO.TeamId;
            existingProject.ProjectTypeId = projectDTO.ProjectTypeId;
            existingProject.Team = team;
            existingProject.ProjectType = projectType;

            await _context.SaveChangesAsync();

            projectDTO.TeamName = team.Name;
            projectDTO.ProjectTypeName = projectType.Name;

            return Ok(projectDTO);
        }


        //  Delete a project
        /// <summary>
        /// Deletes a project by ID.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     DELETE /api/Project/1
        /// 
        /// </remarks>
        /// <response code="204">Project successfully deleted</response>
        /// <response code="404">Project not found</response>
        [HttpDelete("{Id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult> DeleteProject(int Id)
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(Id);
            if (project == null)
            {
                return NotFound(new { Message = "Project not found" });
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

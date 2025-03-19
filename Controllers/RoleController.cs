using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DevHouse1.Models;
using Microsoft.AspNetCore.Authorization;
using DevHouse1.DTOs;

namespace DevHouse1.Controllers
{

    /// <summary>
    /// Controller for managing roles.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleController"/> class.
        /// </summary>
        /// <param name="context">The database context for accessing role data.</param>

        public RoleController(AppDbContext context)
        {
            _context = context;
        }

        //  Get all roles
        /// <summary>
        /// Retrieves all roles.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     GET /api/Role
        /// 
        /// </remarks>
        /// <response code="200">Roles retrieved successfully</response>
        /// <response code="404">No roles found</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<RoleDTO>>> GetRoles()
        {
            if (_context.Roles == null)
            {
                return NotFound();
            }

            var roles = await _context.Roles
                .Include(r => r.Developers)
                .ToListAsync();

            var roleDTOs = roles.Select(r => new RoleDTO
            {
                Id = r.Id,
                Name = r.Name,
                
            }).ToList();

            return Ok(roleDTOs);
        }

        //  Get a single role by ID
        /// <summary>
        /// Retrieves a role by ID.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     GET /api/Role/1
        /// 
        /// **Example Response:**
        /// 
        /// ```json
        /// {
        ///   "id": 1,
        ///   "name": "Software Engineer",
        ///   "developerCount": 5
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Role found</response>
        /// <response code="404">Role not found</response>
        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDTO>> GetRole(int Id)
        {
            if (_context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .Include(r => r.Developers)
                .FirstOrDefaultAsync(r => r.Id == Id);

            if (role == null)
            {
                return NotFound(new { Message = "Role not found" });
            }

            var roleDTO = new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                
            };

            return Ok(roleDTO);
        }

        //  Create a new role
        /// <summary>
        /// Creates a new role.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     POST /api/Role
        /// 
        /// **Example Body:**
        /// 
        /// ```json
        /// {
        ///   "name": "Software Engineer"
        /// }
        /// ```
        /// </remarks>
        /// <response code="201">Role created successfully</response>
        /// <response code="400">Invalid role data or role already exists</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleDTO>> AddRole([FromBody] RoleDTO roleDTO)
        {
            if (roleDTO == null || string.IsNullOrEmpty(roleDTO.Name))
            {
                return BadRequest(new { Message = "Invalid role data" });
            }

            var existingRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleDTO.Name);
            if (existingRole != null)
            {
                return BadRequest(new { Message = $"Role '{roleDTO.Name}' already exists." });
            }

            var role = new Role
            {
                Name = roleDTO.Name
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            roleDTO.Id = role.Id;
           

            return CreatedAtAction(nameof(GetRole), new { Id = role.Id }, roleDTO);
        }

        //  Update an existing role
        /// <summary>
        /// Updates an existing role.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     PUT /api/Role/1
        /// 
        /// **Example Body:**
        /// 
        /// ```json
        /// {
        ///   "name": "Senior Software Engineer"
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Role updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Role not found</response>
        [HttpPut("{Id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDTO>> UpdateRole(int Id, [FromBody] RoleDTO roleDTO)
        {
            if (roleDTO == null || string.IsNullOrEmpty(roleDTO.Name))
            {
                return BadRequest(new { Message = "Invalid request. Provide role details." });
            }

            var existingRole = await _context.Roles.FindAsync(Id);
            if (existingRole == null)
            {
                return NotFound(new { Message = "Role not found" });
            }

            existingRole.Name = roleDTO.Name;

            await _context.SaveChangesAsync();

            roleDTO.Id = existingRole.Id;
            

            return Ok(roleDTO);
        }

        //  Delete a role
        /// <summary>
        /// Deletes a role by ID.
        /// </summary>
        /// <remarks>
        /// **Sample Request:**
        /// 
        ///     DELETE /api/Role/1
        /// 
        /// </remarks>
        /// <response code="204">Role successfully deleted</response>
        /// <response code="404">Role not found</response>
        [HttpDelete("{Id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteRole(int Id)
        {
            if (_context.Roles == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(Id);
            if (role == null)
            {
                return NotFound(new { Message = "Role not found" });
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicPortal.BLL.DTO;
using MusicPortal.BLL.Interfaces;
using MusicPortal.Models.Api;
using System.Security.Claims;

namespace MusicPortal.Controllers.Api;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class UsersApiController(IUserService users) : ControllerBase
{
    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
        => Ok(await users.GetAllUsers());

    // GET: api/users/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDTO>> GetById(int id)
    {
        var user = await users.GetUser(id);
        return user == null ? NotFound() : Ok(user);
    }

    // POST: api/users
    // New accounts are always created unapproved and without administrator rights.
    [HttpPost]
    public async Task<ActionResult<UserDTO>> Create(RegisterDTO request)
    {
        try
        {
            await users.CreateUser(request);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }

        var created = (await users.GetAllUsers())
            .First(x => x.Username.Equals(request.Username.Trim(), StringComparison.OrdinalIgnoreCase));

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT: api/users/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UserDTO request)
    {
        if (id != request.Id)
            return BadRequest(new { message = "ID у маршруті не збігається з ID у тілі запиту." });

        var existing = await users.GetUser(id);
        if (existing == null)
            return NotFound();

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == id.ToString() && !request.IsAdmin)
            return BadRequest(new { message = "Адміністратор не може зняти адміністративні права із власного облікового запису." });

        try
        {
            await users.UpdateUser(request);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }

        return NoContent();
    }

    // PATCH: api/users/5/approval
    // true = approve; false = reject / revoke approval.
    [HttpPatch("{id:int}/approval")]
    public async Task<IActionResult> SetApproval(int id, UserApprovalRequest request)
    {
        var user = await users.GetUser(id);
        if (user == null)
            return NotFound();

        user.IsRegistrationConfirmed = request.IsRegistrationConfirmed;
        await users.UpdateUser(user);
        return NoContent();
    }

    // DELETE: api/users/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await users.GetUser(id);
        if (user == null)
            return NotFound();

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId == id.ToString())
            return BadRequest(new { message = "Не можна видалити власний обліковий запис під час поточного сеансу." });

        await users.DeleteUser(id);
        return NoContent();
    }
}

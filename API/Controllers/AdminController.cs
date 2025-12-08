using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> userManager) : BaseApiController
{
    [Authorize(Policy="RequireAdminRole")]
    [HttpGet("user-roles")]
    public async Task<ActionResult> GetUserWithRoles()
    {
        var users = await userManager.Users.ToListAsync();
        var userList = new List<object>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userList.Add(new
            {
                user.Id,
                user.Email,
                Roles = roles.ToList()
            });
        }

        return Ok(userList);
    }

    [Authorize(Policy="RequireAdminRole")]
    [HttpPost("edit-roles/{userId}")]
    public async Task<ActionResult<IList<string>>> EditRoles(string userId, [FromQuery] string roles)
    { 
        if (string.IsNullOrEmpty(roles)) return BadRequest("Choose Atleast 1 Role.");

        var selectedRoles = roles.Split(",").ToArray();
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return BadRequest("Cannot Fetch User.");
        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded) return BadRequest("Failed To Add Roles.");
        
        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded) return BadRequest("Failed To Remove Roles.");
        return Ok(await userManager.GetRolesAsync(user));
    }

    [Authorize(Policy="ModeratePhotoRole")]
    [HttpGet("photo-moderation")]
     public ActionResult GetPhotosForModeration()
    {
        return Ok("Admin(s) And Moderator(s) Can See Photos.");
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IDatingRepository _repo;
        private readonly IPhotoRepository _repoPhoto;

        public AdminController(DataContext context, UserManager<User> userManager, IDatingRepository repo, IPhotoRepository repoPhoto)
        {
            _repoPhoto = repoPhoto;
            _repo = repo;
            _userManager = userManager;
            _context = context;

        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var userList = await _context.Users
            .OrderBy(x => x.UserName)
            .Select(user => new
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = (from userRole in user.UserRoles
                         join role in _context.Roles
                         on userRole.RoleId
                         equals role.Id
                         select role.Name).ToList()
            }).ToListAsync();

            return Ok(userList);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDTO roleEditDTO)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return BadRequest();
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var selectedRoles = roleEditDTO.RoleNames;

            /*
            ?? > if a value within "selectedRoles" then selectedRoles is assigned with that value otherwise assign as a new string object.
            -OI equivalent-
            if selectedRoles then
                selectedRoles = selectedRoles
            end else
                * new string array/list
                selectedRoles = ''
            end
            */
            selectedRoles = selectedRoles ?? new string[] { };

            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to add to roles.");
            }

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to remove the roles.");
            }

            return Ok(await _userManager.GetRolesAsync(user));

        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            //return Ok(_context.Photos.Where(p => p.IsApproved == false));
            //var photosToReturn = await _context.Photos.Where(p => p.IsApproved == false).ToListAsync();

            //return Ok(photosToReturn);
            
            return Ok(await _repoPhoto.GetPhotosForModeration());
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("photosForModeration/approve/{id}")]
        public async Task<IActionResult> ApprovePhoto(int id)
        {

            var photoToApprove = await _repo.GetPhoto(id);

            if (photoToApprove == null)
            {
                return BadRequest("Photo does not exist.");
            }

            if (photoToApprove.IsApproved == true)
            {
                return BadRequest("Photo has already been approved.");
            }

            photoToApprove.IsApproved = true;

            if (await _repo.SaveAll())
                return NoContent();


            throw new Exception("Error approving photo.");
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("photosForModeration/reject/{id}")]
        public async Task<IActionResult> RejectPhoto(int id)
        {
            if (await _repoPhoto.DeletePhoto(id))
            {
                return Ok();
            }
                        
            return BadRequest("Error rejecting photo.");
        }


    }
}
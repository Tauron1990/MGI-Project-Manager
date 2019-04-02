using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MGIProjectManagerServer.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using Tauron.Application.MgiProjectManager.Data.Api;
using Tauron.Application.MgiProjectManager.Resources.Web;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MGIProjectManagerServer.Api.User
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserGridController : Controller
    {
        [DataContract]
        public class AppUserKey
        {
            [DataMember]
            public string Key { get; set; }
        }

        [DataContract]
        public class AppUserValue
        {
            [DataMember]
            public AppUser Value { get; set; }
        }

        private readonly UserManager<IdentityUser> _userManager;
        private AppUserValidator _validator;

        public UserGridController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        private async Task<IActionResult> Validate(AppUser user)
        {
            if (_validator == null)
                _validator = new AppUserValidator();

            var result = await _validator.ValidateAsync(user);

            return result.IsValid ? null : BadRequest(result.ToString());
        }

        private IEnumerable<AppUser> GetUserList()
        {
            foreach (var identityUser in _userManager.Users)
                yield return new AppUser
                {
                    Id = identityUser.Id,
                    Name = identityUser.UserName,
                    Email = identityUser.Email,
                    Role = string.Join(',', _userManager.GetRolesAsync(identityUser).Result)
                };
        }

        // GET: api/<controller>
        [HttpPost]
        public JsonResult GridGetData([FromBody] DataManagerRequest dm)
        {
            var dataSource = GetUserList().ToArray().AsEnumerable();
            var operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
                dataSource = operation.PerformSearching(dataSource, dm.Search); //Search
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                dataSource = operation.PerformSorting(dataSource, dm.Sorted);
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
                dataSource = operation.PerformFiltering(dataSource, dm.Where, dm.Where[0].Operator);
            var count = dataSource.Count();
            if (dm.Skip != 0) dataSource = operation.PerformSkip(dataSource, dm.Skip); //Paging
            if (dm.Take != 0) dataSource = operation.PerformTake(dataSource, dm.Take);
            return dm.RequiresCounts
                ? Json(new {result = dataSource, count})
                : Json(dataSource);
        }

        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> GridUpdateData(AppUserValue userValue)
        {
            try
            {
                //AppUser user = null;
                //var validate = await Validate(user);
                //if (validate != null) return validate;

                var user = userValue.Value;
                var roles = user.Role.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToArray();

                var internalUser = await _userManager.FindByIdAsync(user.Id);
                var userRoles = await _userManager.GetRolesAsync(internalUser);
                bool canAdmin = userRoles.Contains(RoleNames.Admin);

                if (roles.Length == 0 || roles.Any(s => !(RoleNames.IsValidRole(s) || canAdmin || s == RoleNames.Admin)))
                    return Json(GetUserList().First(u => u.Id == user.Id));

                var errors = new List<IdentityError>();

                var isEdited = false;

                if (internalUser.Email != user.Email)
                {
                    internalUser.Email = user.Email;
                    isEdited = true;
                }

                if (internalUser.UserName != user.Name)
                {
                    internalUser.UserName = user.Name;
                    isEdited = true;
                }

                if (isEdited)
                {
                    var updateResult = await _userManager.UpdateAsync(internalUser);
                    if (!updateResult.Succeeded)
                        errors.AddRange(updateResult.Errors);
                }

                foreach (var role in roles)
                    if (userRoles.Contains(role))
                        userRoles.Remove(role);
                    else
                    {
                        var result = await _userManager.AddToRoleAsync(internalUser, role);
                        if (result.Succeeded) continue;

                        errors.AddRange(result.Errors);
                    }

                if (userRoles.Count != 0)
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(internalUser, userRoles);
                    if (!removeResult.Succeeded)
                        errors.AddRange(removeResult.Errors);
                }

                if (errors.Count == 0) return Json(user);

                foreach (var identityError in errors)
                    ModelState.TryAddModelError(identityError.Code, identityError.Description);

                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("Insert")]
        [HttpPost]
        public async Task<IActionResult> GridAddData(AppUserValue userValue)
        {
            try
            {
                var user = userValue.Value;

                var roles = user.Role.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToArray();

                if (roles.Length == 0 || roles.Any(s => !RoleNames.IsValidRole(s)))
                    return BadRequest(roles.Where(s => !RoleNames.IsValidRole(s)));

                var internalUser = await _userManager.FindByIdAsync(user.Id);

                if (internalUser != null)
                    return BadRequest(WebResources.Api_GridUserController_UserExis);

                internalUser = new IdentityUser(user.Name) {Email = user.Email};

                var result = await _userManager.CreateAsync(internalUser, "Temp123");
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                result = await _userManager.AddToRolesAsync(internalUser, roles);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Json(new AppUser {Id = internalUser.Id, Name = internalUser.UserName, Role = user.Role, Email = internalUser.Email});
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("Remove")]
        [HttpPost]
        public async Task<IActionResult> GridRemoveData(AppUserKey user)
        {
            try
            {
                var internalUser = await _userManager.FindByIdAsync(user.Key);
                if (internalUser == null || await _userManager.IsInRoleAsync(internalUser, RoleNames.Admin))
                    return BadRequest(WebResources.Api_GridUserController_NoUser);

                var result = await _userManager.DeleteAsync(internalUser);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Json(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
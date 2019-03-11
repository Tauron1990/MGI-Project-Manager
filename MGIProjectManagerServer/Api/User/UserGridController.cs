using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MGIProjectManagerServer.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
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
        private readonly UserManager<IdentityUser> _userManager;

        public UserGridController(UserManager<IdentityUser> userManager) => _userManager = userManager;

        private IEnumerable<Tauron.Application.MgiProjectManager.Data.Api.User> GetUserList()
        {
            foreach (var identityUser in _userManager.Users)
            {
                yield return new Tauron.Application.MgiProjectManager.Data.Api.User
                {
                    Id = identityUser.Id,
                    Name = identityUser.UserName,
                    Role = string.Join(',', _userManager.GetRolesAsync(identityUser).Result)
                };
            }
        }

        // GET: api/<controller>
        [HttpPost]
        public JsonResult GridGetData([FromBody]DataManagerRequest dm)
        {
            var dataSource = GetUserList().ToArray().AsEnumerable();
            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
                dataSource = operation.PerformSearching(dataSource, dm.Search); //Search
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
                dataSource = operation.PerformSorting(dataSource, dm.Sorted);
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
                dataSource = operation.PerformFiltering(dataSource, dm.Where, dm.Where[0].Operator);
            int count = dataSource.Count();
            if (dm.Skip != 0) dataSource = operation.PerformSkip(dataSource, dm.Skip); //Paging
            if (dm.Take != 0) dataSource = operation.PerformTake(dataSource, dm.Take);
            return dm.RequiresCounts 
                ? Json(new { result = dataSource, count = count }) 
                : Json(dataSource);
        }

        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> GridUpdateData(Tauron.Application.MgiProjectManager.Data.Api.User user)
        {
            string[] roles = user.Role.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();

            if (roles.Length == 0 || roles.Any(s => !RoleNames.IsValidRole(s)))
                return Json(GetUserList().First(u => u.Id == user.Id));

            var internalUser = await _userManager.FindByIdAsync(user.Id);
            var userRoles = await _userManager.GetRolesAsync(internalUser);
            List<IdentityError> errors = new List<IdentityError>();

            foreach (var role in roles)
            {
                if (userRoles.Contains(role))
                    userRoles.Remove(role);
                else
                {
                    var result = await _userManager.AddToRoleAsync(internalUser, role);
                    if(result.Succeeded) continue;

                    errors.AddRange(result.Errors);
                }
            }

            var removeResult = await _userManager.RemoveFromRolesAsync(internalUser, userRoles);
            if (!removeResult.Succeeded) 
                errors.AddRange(removeResult.Errors);

            if (errors.Count == 0) return Json(user);

            foreach (var identityError in errors)
                ModelState.TryAddModelError(identityError.Code, identityError.Description);

            return BadRequest(ModelState);

        }

        [Route("Insert")]
        [HttpPost]
        public async Task<IActionResult> GridAddData(Tauron.Application.MgiProjectManager.Data.Api.User user)
        {
            string[] roles = user.Role.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();

            if (roles.Length == 0 || roles.Any(s => !RoleNames.IsValidRole(s)))
                return BadRequest(roles.Where(s => !RoleNames.IsValidRole(s)));

            var internalUser = await _userManager.FindByIdAsync(user.Id);

            if (internalUser != null)
                return BadRequest(WebResources.Api_GridUserController_UserExis);

            internalUser = new IdentityUser(user.Name) { Email = user.Name };

            var result = await _userManager.CreateAsync(internalUser, "temp");
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            result = await _userManager.AddToRolesAsync(internalUser, roles);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Json(new Tauron.Application.MgiProjectManager.Data.Api.User { Id = internalUser.Id, Name = internalUser.UserName, Role = user.Role});
        }

        [Route("Remove")]
        [HttpPost]
        public async Task<IActionResult> GridRemoveData(string user)
        {
            var internalUser = await _userManager.FindByIdAsync(user);
            if (internalUser == null  || await _userManager.IsInRoleAsync(internalUser, RoleNames.Admin))
                return BadRequest(WebResources.Api_GridUserController_NoUser);

            var result = await _userManager.DeleteAsync(internalUser);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Json(user);
        }
    }
}

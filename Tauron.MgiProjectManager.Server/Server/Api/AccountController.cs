using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Tauron.MgiProjectManager.Data.Core;
using Tauron.MgiProjectManager.Model;
using Tauron.MgiProjectManager.Model.Api;
using Tauron.MgiProjectManager.Resources;
using Tauron.MgiProjectManager.Server.Authorization;
using Tauron.MgiProjectManager.Server.Core;

namespace Tauron.MgiProjectManager.Server.Api
{
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountManager _accountManager;
        private readonly IAuthorizationService _authorizationService;
        private const string GetUserByIdActionName = "GetUserById";
        private const string GetRoleByIdActionName = "GetRoleById";

        public AccountController(IAccountManager accountManager, IAuthorizationService authorizationService)
        {
            _accountManager = accountManager;
            _authorizationService = authorizationService;
        }


        [HttpGet("users/me")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        public async Task<IActionResult> GetCurrentUser()
        {
            return await GetUserById(User.GetUserId());
        }


        [HttpGet("users/{id}", Name = GetUserByIdActionName)]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(string id)
        {
            if (!(await _authorizationService.AuthorizeAsync(User, id, AccountManagementOperations.Read)).Succeeded)
                return new ChallengeResult();


            UserViewModel userVm = await GetUserViewModelHelper(id);

            return userVm != null ? (IActionResult) Ok(userVm) : NotFound(id);
        }


        [HttpGet("users/username/{userName}")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByUserName(string userName)
        {
            ApplicationUser appUser = await _accountManager.GetUserByUserNameAsync(userName);

            if (!(await _authorizationService.AuthorizeAsync(User, appUser?.Id ?? "", AccountManagementOperations.Read)).Succeeded)
                return new ChallengeResult();

            if (appUser == null)
                return NotFound(userName);

            return await GetUserById(appUser.Id);
        }


        [HttpGet("users")]
        [Authorize(Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetUsers()
        {
            return await GetUsers(-1, -1);
        }


        [HttpGet("users/{pageNumber:int}/{pageSize:int}")]
        [Authorize(Policies.ViewAllUsersPolicy)]
        [ProducesResponseType(200, Type = typeof(List<UserViewModel>))]
        public async Task<IActionResult> GetUsers(int pageNumber, int pageSize)
        {
            var usersAndRoles = await _accountManager.GetUsersAndRolesAsync(pageNumber, pageSize);

            List<UserViewModel> usersVm = new List<UserViewModel>();

            foreach (var item in usersAndRoles)
            {
                var userVm = Mapper.Map<UserViewModel>(item.User);
                userVm.Roles = item.Roles;

                usersVm.Add(userVm);
            }

            return Ok(usersVm);
        }


        [HttpPut("users/me")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserEditViewModel user)
        {
            return await UpdateUser(User.GetUserId(), user);
        }


        [HttpPut("users/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserEditViewModel user)
        {
            ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);
            string[] currentRoles = appUser != null ? (await _accountManager.GetUserRolesAsync(appUser)).ToArray() : null;

            var manageUsersPolicy = _authorizationService.AuthorizeAsync(User, id, AccountManagementOperations.Update);
            var assignRolePolicy = _authorizationService.AuthorizeAsync(User, (user.Roles, currentRoles), Policies.AssignAllowedRolesPolicy);


            if ((await Task.WhenAll(manageUsersPolicy, assignRolePolicy)).Any(r => !r.Succeeded))
                return new ChallengeResult();


            if (!ModelState.IsValid) return BadRequest(ModelState);

            //if (user == null)
            //    return BadRequest($"{nameof(user)} cannot be null");

            if (!string.IsNullOrWhiteSpace(user.Id) && id != user.Id)
                return BadRequest(AppRes.AccountController_UpdateUser_IdMismatch);

            if (appUser == null)
                return NotFound(id);

            bool isPasswordChanged = !string.IsNullOrWhiteSpace(user.NewPassword);
            bool isUserNameChanged = !appUser.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase);

            if (User.GetUserId() == id)
            {
                if (string.IsNullOrWhiteSpace(user.CurrentPassword))
                {
                    if (isPasswordChanged)
                        AddError(AppRes.AccountController_UpdateUser_PasswordChanged, "Password");

                    if (isUserNameChanged)
                        AddError(AppRes.AccountController_UpdateUser_UserChanged, "Username");
                }
                else if (isPasswordChanged || isUserNameChanged)
                {
                    if (!await _accountManager.CheckPasswordAsync(appUser, user.CurrentPassword))
                        AddError(AppRes.AccountController_UpdateUser_PasswordCheck);
                }
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);

            Mapper.Map(user, appUser);

            var result = await _accountManager.UpdateUserAsync(appUser, user.Roles);
            if (result.Succeeded)
            {
                if (isPasswordChanged)
                {
                    if (!string.IsNullOrWhiteSpace(user.CurrentPassword))
                        result = await _accountManager.UpdatePasswordAsync(appUser, user.CurrentPassword, user.NewPassword);
                    else
                        result = await _accountManager.ResetPasswordAsync(appUser, user.NewPassword);
                }

                if (result.Succeeded)
                    return NoContent();
            }

            AddError(result.Errors);

            return BadRequest(ModelState);
        }


        [HttpPatch("users/me")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] JsonPatchDocument<UserPatchViewModel> patch) 
            => await UpdateUser(User.GetUserId(), patch);


        [HttpPatch("users/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] JsonPatchDocument<UserPatchViewModel> patch)
        {
            if (!(await _authorizationService.AuthorizeAsync(User, id, AccountManagementOperations.Update)).Succeeded)
                return new ChallengeResult();


            if (ModelState.IsValid)
            {
                if (patch == null)
                    return BadRequest($"{nameof(patch)} cannot be null");


                ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);

                if (appUser == null)
                    return NotFound(id);


                UserPatchViewModel userPvm = Mapper.Map<UserPatchViewModel>(appUser);
                patch.ApplyTo(userPvm, ModelState);


                if (!ModelState.IsValid) return BadRequest(ModelState);

                Mapper.Map(userPvm, appUser);

                var result = await _accountManager.UpdateUserAsync(appUser);
                if (result.Succeeded)
                    return NoContent();


                AddError(result.Errors);
            }

            return BadRequest(ModelState);
        }


        [HttpPost("users")]
        [Authorize(Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(201, Type = typeof(UserViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Register([FromBody] UserEditViewModel user)
        {
            if (!(await _authorizationService.AuthorizeAsync(User, (user.Roles, new string[] { }), Policies.AssignAllowedRolesPolicy)).Succeeded)
                return new ChallengeResult();


            if (!ModelState.IsValid) return BadRequest(ModelState);

            //if (user == null)
            //    return BadRequest($"{nameof(user)} cannot be null");


            ApplicationUser appUser = Mapper.Map<ApplicationUser>(user);

            var result = await _accountManager.CreateUserAsync(appUser, user.Roles, user.NewPassword);
            if (result.Succeeded)
            {
                UserViewModel userVm = await GetUserViewModelHelper(appUser.Id);
                return CreatedAtAction(GetUserByIdActionName, new { id = userVm.Id }, userVm);
            }

            AddError(result.Errors);

            return BadRequest(ModelState);
        }


        [HttpDelete("users/{id}")]
        [ProducesResponseType(200, Type = typeof(UserViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (!(await _authorizationService.AuthorizeAsync(User, id, AccountManagementOperations.Delete)).Succeeded)
                return new ChallengeResult();


            ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);

            if (appUser == null)
                return NotFound(id);

            if (!await _accountManager.TestCanDeleteUserAsync(id))
                return BadRequest(AppRes.AccountController_DeleteUser_TestFailed);


            UserViewModel userVm = await GetUserViewModelHelper(appUser.Id);

            var result = await _accountManager.DeleteUserAsync(appUser);
            if (!result.Succeeded)
                throw new Exception(AppRes.AccountController_DeleteUser_Error + string.Join(", ", result.Errors));


            return Ok(userVm);
        }


        [HttpPut("users/unblock/{id}")]
        [Authorize(Policies.ManageAllUsersPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UnblockUser(string id)
        {
            ApplicationUser appUser = await _accountManager.GetUserByIdAsync(id);

            if (appUser == null)
                return NotFound(id);

            appUser.LockoutEnd = null;
            var result = await _accountManager.UpdateUserAsync(appUser);
            if (!result.Succeeded)
                throw new Exception(AppRes.AccountController_UnblockUser_Error + string.Join(", ", result.Errors));


            return NoContent();
        }


        [HttpGet("users/me/preferences")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> UserPreferences()
        {
            var userId = User.GetUserId();
            ApplicationUser appUser = await _accountManager.GetUserByIdAsync(userId);

            return Ok(appUser.Configuration);
        }


        [HttpPut("users/me/preferences")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UserPreferences([FromBody] string data)
        {
            var userId = User.GetUserId();
            ApplicationUser appUser = await _accountManager.GetUserByIdAsync(userId);

            appUser.Configuration = data;

            var result = await _accountManager.UpdateUserAsync(appUser);
            if (!result.Succeeded)
                throw new Exception(AppRes.AccountController_UserPreferences_Error + string.Join(", ", result.Errors));

            return NoContent();
        }





        [HttpGet("roles/{id}", Name = GetRoleByIdActionName)]
        [ProducesResponseType(200, Type = typeof(RoleViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var appRole = await _accountManager.GetRoleByIdAsync(id);

            if (!(await _authorizationService.AuthorizeAsync(User, appRole?.Name ?? "", Policies.ViewRoleByRoleNamePolicy)).Succeeded)
                return new ChallengeResult();

            if (appRole == null)
                return NotFound(id);

            return await GetRoleByName(appRole.Name);
        }


        [HttpGet("roles/name/{name}")]
        [ProducesResponseType(200, Type = typeof(RoleViewModel))]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRoleByName(string name)
        {
            if (!(await _authorizationService.AuthorizeAsync(User, name, Policies.ViewRoleByRoleNamePolicy)).Succeeded)
                return new ChallengeResult();


            RoleViewModel roleVm = await GetRoleViewModelHelper(name);

            if (roleVm == null)
                return NotFound(name);

            return Ok(roleVm);
        }


        [HttpGet("roles")]
        [Authorize(Policies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]
        public async Task<IActionResult> GetRoles()
        {
            return await GetRoles(-1, -1);
        }


        [HttpGet("roles/{pageNumber:int}/{pageSize:int}")]
        [Authorize(Policies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<RoleViewModel>))]
        public async Task<IActionResult> GetRoles(int pageNumber, int pageSize)
        {
            var roles = await _accountManager.GetRolesLoadRelatedAsync(pageNumber, pageSize);
            return Ok(Mapper.Map<List<RoleViewModel>>(roles));
        }


        [HttpPut("roles/{id}")]
        [Authorize(Policies.ManageAllRolesPolicy)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleViewModel role)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (role == null)
                return BadRequest($"{nameof(role)} null");

            if (!string.IsNullOrWhiteSpace(role.Id) && id != role.Id)
                return BadRequest(AppRes.AccountController_UpdateRole_ConflictigId);



            ApplicationRole appRole = await _accountManager.GetRoleByIdAsync(id);

            if (appRole == null)
                return NotFound(id);


            Mapper.Map(role, appRole);

            var result = await _accountManager.UpdateRoleAsync(appRole, role.Permissions?.Select(p => p.Value).ToArray());
            if (result.Succeeded)
                return NoContent();

            AddError(result.Errors);

            return BadRequest(ModelState);
        }


        [HttpPost("roles")]
        [Authorize(Policies.ManageAllRolesPolicy)]
        [ProducesResponseType(201, Type = typeof(RoleViewModel))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateRole([FromBody] RoleViewModel role)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (role == null)
                return BadRequest($"{nameof(role)} null");


            ApplicationRole appRole = Mapper.Map<ApplicationRole>(role);

            var result = await _accountManager.CreateRoleAsync(appRole, role.Permissions?.Select(p => p.Value).ToArray());
            if (result.Succeeded)
            {
                RoleViewModel roleVm = await GetRoleViewModelHelper(appRole.Name);
                return CreatedAtAction(GetRoleByIdActionName, new { id = roleVm.Id }, roleVm);
            }

            AddError(result.Errors);

            return BadRequest(ModelState);
        }


        [HttpDelete("roles/{id}")]
        [Authorize(Policies.ManageAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(RoleViewModel))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            ApplicationRole appRole = await _accountManager.GetRoleByIdAsync(id);

            if (appRole == null)
                return NotFound(id);

            if (!await _accountManager.TestCanDeleteRoleAsync(id))
                return BadRequest(AppRes.AccountController_DeleteRole_Test);


            RoleViewModel roleVm = await GetRoleViewModelHelper(appRole.Name);

            var result = await _accountManager.DeleteRoleAsync(appRole);
            if (!result.Succeeded)
                throw new Exception(AppRes.AccountController_DeleteRole_Error + string.Join(", ", result.Errors));


            return Ok(roleVm);
        }


        [HttpGet("permissions")]
        [Authorize(Policies.ViewAllRolesPolicy)]
        [ProducesResponseType(200, Type = typeof(List<PermissionViewModel>))]
        public IActionResult GetAllPermissions() 
            => Ok(Mapper.Map<List<PermissionViewModel>>(ApplicationPermissions.AllPermissions));


        private async Task<UserViewModel> GetUserViewModelHelper(string userId)
        {
            var userAndRoles = await _accountManager.GetUserAndRolesAsync(userId);
            if (userAndRoles == null)
                return null;

            var userVm = Mapper.Map<UserViewModel>(userAndRoles.Value.User);
            userVm.Roles = userAndRoles.Value.Roles;

            return userVm;
        }


        private async Task<RoleViewModel> GetRoleViewModelHelper(string roleName)
        {
            var role = await _accountManager.GetRoleLoadRelatedAsync(roleName);
            if (role != null)
                return Mapper.Map<RoleViewModel>(role);


            return null;
        }


        private void AddError(IEnumerable<string> errors, string key = "")
        {
            foreach (var error in errors)
            {
                AddError(error, key);
            }
        }

        private void AddError(string error, string key = "")
        {
            ModelState.AddModelError(key, error);
        }

    }
}

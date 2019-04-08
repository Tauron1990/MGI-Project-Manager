using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tauron.Application.MgiProjectManager.Server.Data.Api;

namespace Tauron.Application.MgiProjectManager.Server.api.User
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(UserManager<IdentityUser> userManager) 
            => _userManager = userManager;

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
        [HttpGet]
        public ActionResult<UserList> Get()
        {
            var list = new UserList();

            list.Items.AddRange(GetUserList());

            list.Count = list.Items.Count;

            return list;
        }

        //// GET api/<controller>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
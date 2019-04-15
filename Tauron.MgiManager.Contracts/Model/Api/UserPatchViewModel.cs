using System.ComponentModel.DataAnnotations;

namespace Tauron.MgiProjectManager.Model.Api
{
    public class UserPatchViewModel
    {
        public string FullName { get; set; }

        public string JobTitle { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }

        public string Configuration { get; set; }
    }
}
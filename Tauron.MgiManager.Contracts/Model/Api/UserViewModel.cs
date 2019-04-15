namespace Tauron.MgiProjectManager.Model.Api
{
    public class UserViewModel : UserBaseViewModel
    {
        public bool IsLockedOut { get; set; }

        public string[] Roles { get; set; }
    }
}
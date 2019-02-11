namespace Tauron.Application.ProjectManager.Generic.Windows
{
    public sealed class LogInWindowSettings
    {
        public LogInWindowSettings(string password, string userName)
        {
            Password = password;
            UserName = userName;
        }

        public string Password { get; set; }

        public string UserName { get; set; }
    }
}
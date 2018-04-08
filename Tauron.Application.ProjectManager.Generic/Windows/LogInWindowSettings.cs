namespace Tauron.Application.ProjectManager.Generic.Windows
{
    public sealed class LogInWindowSettings
    {
        public string Password { get; set; }

        public string UserName { get; set; }

        public LogInWindowSettings(string password, string userName)
        {
            Password = password;
            UserName = userName;
        }
    }
}
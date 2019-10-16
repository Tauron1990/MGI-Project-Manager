using CQRSlite.Commands;

namespace Tauron.MgiManager.User.Shared.Command
{
    public sealed class CreateUserCommand : ICommand
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public CreateUserCommand()
        {
            
        }

        public CreateUserCommand(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }
}
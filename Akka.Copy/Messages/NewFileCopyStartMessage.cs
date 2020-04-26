namespace Akka.Copy.Messages
{
    public class NewFileCopyStartMessage
    {
        public string Id { get; }

        public string Name { get; }

        public NewFileCopyStartMessage(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
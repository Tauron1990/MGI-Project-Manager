namespace ServiceManager.Core
{
    public class LogEntries : ObservableConcurrentDictionary<string, ObservableQueue<string>>
    {
        public void AddLog(string name, string content)
        {
            var queue = GetOrAdd(name, s => new ObservableQueue<string>());

            lock (queue)
            {
                queue.Enqueue(content);
                if (queue.Count > 100)
                    queue.Dequeue();
            }
        }
    }
}
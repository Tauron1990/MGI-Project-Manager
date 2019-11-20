using System;
using System.Collections.Immutable;
using System.Threading;
using Functional;
using JetBrains.Annotations;

namespace Tauron.Application.Deployment.AutoUpload
{
    [PublicAPI]
    public sealed class ConsoleUi
    {
        private abstract class ConsoleEntry
        {
            public abstract void Execute();
        }

        private class WriteLineEntry : ConsoleEntry
        {
            private readonly string _line;

            public WriteLineEntry(string line) => _line = line;
            public override void Execute() 
                => Console.WriteLine(_line);
        }

        private class WriteErrorEntry : ConsoleEntry
        {
            private readonly string _errorMessage;

            public WriteErrorEntry(string errorMessage) => _errorMessage = errorMessage;
            
            public override void Execute()
            {
                var bColor = Console.BackgroundColor;
                var fColor = Console.ForegroundColor;

                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write("Error: ");

                Console.BackgroundColor = bColor;
                Console.ForegroundColor = fColor;

                Console.WriteLine(_errorMessage);
            }
        }

        private readonly object _lock = new object();

        public static string Title
        {
            get => Console.Title;
            set => Console.Title = value;
        }

        private ImmutableArray<ConsoleEntry> _persistent = ImmutableArray<ConsoleEntry>.Empty;
        private ImmutableArray<ConsoleEntry> _lines = ImmutableArray<ConsoleEntry>.Empty;

        private void UpdateUI()
        {
            if(!Monitor.TryEnter(_lock)) return;

            try
            {
                Console.Clear();

                var title = Title;
                var singleLenght = (Console.BufferWidth - title.Length / 2) / 2;

                Console.WriteLine(title.PadLeft(singleLenght).PadRight(Console.BufferWidth));

                _persistent.Do(e => e.Execute());
                _lines.Do(e => e.Execute());
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        private void AddEntry(ConsoleEntry entry, bool persistent)
        {
            if(persistent)
                ImmutableInterlocked.InterlockedExchange(ref _persistent, _persistent.Add(entry));
            else
                ImmutableInterlocked.InterlockedExchange(ref _lines, _lines.Add(entry));
        }

        public void WriteError(Exception e, bool persistent = false)
        {
            AddEntry(new WriteErrorEntry(e.Message), persistent);
            UpdateUI();
        }

        public void WriteLine(string line, bool persistent = false)
        {
            AddEntry(new WriteLineEntry(line), persistent);
            UpdateUI();
        }

        public void Clear()
        {
            ImmutableInterlocked.InterlockedExchange(ref _lines, ImmutableArray<ConsoleEntry>.Empty);
            UpdateUI();
        }
    }
}
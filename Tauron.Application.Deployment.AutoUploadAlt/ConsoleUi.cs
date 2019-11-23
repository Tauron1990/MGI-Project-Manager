using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Tauron.Application.Deployment.AutoUpload
{
    [PublicAPI]
    public sealed class ConsoleUi
    {
        private class ActionDispose : IDisposable
        {
            private readonly Action _executor;
            [DebuggerHidden]
            public ActionDispose(Action executor) 
                => _executor = executor;

            [DebuggerHidden]
            public void Dispose() => _executor();
        }

        private abstract class ConsoleEntry
        {
            public abstract void Execute();
        }

        private class WriteLineEntry : ConsoleEntry
        {
            private readonly string _line;
            [DebuggerHidden]
            public WriteLineEntry(string line) => _line = line;
            [DebuggerHidden]
            public override void Execute() 
                => Console.WriteLine(_line);
        }

        private class WriteErrorEntry : ConsoleEntry
        {
            private readonly string _errorMessage;
            [DebuggerHidden]
            public WriteErrorEntry(string errorMessage) => _errorMessage = errorMessage;
            [DebuggerHidden]
            public override void Execute()
            {
                Console.WriteLine();

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

        private class WriteEntry : ConsoleEntry
        {
            private readonly string _value;
            [DebuggerHidden]
            public WriteEntry(string value) 
                => _value = value;
            [DebuggerHidden]
            public override void Execute() 
                => Console.Write(_value);
        }

        private class EmptyLine : ConsoleEntry
        {
            private readonly int _count;
            [DebuggerHidden]
            public EmptyLine(int count) 
                => _count = count;
            [DebuggerHidden]
            public override void Execute()
            {
                for (var i = 0; i < _count; i++) Console.WriteLine();
            }
        }

        private readonly object _lock = new object();

        public string Title
        {
            get => Console.Title;
            set
            {
                Console.Title = value;
                UpdateUI();
            }
        }

        private ImmutableArray<ConsoleEntry> _persistent = ImmutableArray<ConsoleEntry>.Empty;
        private ImmutableArray<ConsoleEntry> _lines = ImmutableArray<ConsoleEntry>.Empty;
        [DebuggerHidden]
        private void UpdateUI()
        {
            if(!Monitor.TryEnter(_lock)) return;

            try
            {
                Console.Clear();

                var title = Title;
                var singleLenght = (Console.BufferWidth + Title.Length) / 2;

                Console.WriteLine(title.PadLeft(singleLenght, '-').PadRight(Console.BufferWidth, '-'));

                foreach (var consoleEntry in _persistent) consoleEntry.Execute();
                foreach (var consoleEntry in _lines) consoleEntry.Execute();
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }
        [DebuggerHidden]
        private void AddEntry(ConsoleEntry entry, bool persistent)
        {
            if(persistent)
                ImmutableInterlocked.InterlockedExchange(ref _persistent, _persistent.Add(entry));
            else
                ImmutableInterlocked.InterlockedExchange(ref _lines, _lines.Add(entry));
        }
        [DebuggerHidden]
        public IDisposable SupressUpdate()
        {
            Monitor.Enter(_lock);
            return new ActionDispose(StartUpdate);
        }
        [DebuggerHidden]
        public void StartUpdate()
        {
            Monitor.Exit(_lock);
        }
        [DebuggerHidden]
        public ConsoleUi WriteError(Exception e)
        {
            AddEntry(new WriteErrorEntry(e.Message), false);
            UpdateUI();
            return this;
        }
        [DebuggerHidden]
        public ConsoleUi WriteError(Exception e, bool persistent)
        {
            AddEntry(new WriteErrorEntry(e.Message), persistent);
            UpdateUI();
            return this;
        }
        [DebuggerHidden]
        public ConsoleUi PrintList<TType>(IEnumerable<TType> enumerable, Action<TType, ConsoleUi> elementAction)
        {
            foreach (var ele in enumerable) 
                elementAction(ele, this);

            return this;
        }
        [DebuggerHidden]
        public ConsoleUi ReplaceLast(string line)
        {
            ImmutableInterlocked.InterlockedExchange(ref _lines, _lines.RemoveAt(_lines.Length - 1));
            AddEntry(new WriteLineEntry(line), false);
            UpdateUI();

            return this;
        }
        [DebuggerHidden]
        public ConsoleUi WriteLine(string line, bool persistent = false)
        {
            AddEntry(new WriteLineEntry(line), persistent);
            UpdateUI();
            return this;
        }
        [DebuggerHidden]
        public ConsoleUi WriteLine(int count = 1)
        {
            AddEntry(new EmptyLine(count), false);
            UpdateUI();
            return this;
        }
        [DebuggerHidden]
        public ConsoleUi Write(string value, bool persistent = false)
        {
            AddEntry(new WriteEntry(value), persistent);
            UpdateUI();
            return this;
        }
        [DebuggerHidden]
        public ConsoleUi Clear()
        {
            ImmutableInterlocked.InterlockedExchange(ref _lines, ImmutableArray<ConsoleEntry>.Empty);
            UpdateUI();
            return this;
        }
        [DebuggerHidden]
        public Task<string> ReadLine()
        {
            var line = Console.ReadLine();

            AddEntry(new WriteLineEntry(line), false);

            return Task.FromResult(line);
        }

        [DebuggerHidden]
        public  async Task<string> ReadLine(string description)
        {
            Write(description);
            return await ReadLine();
        }
        [DebuggerHidden]
        public Task<bool> Allow(string question)
        {
            WriteLine(question + "y/n");
            return Task.FromResult( Console.ReadKey().Key == ConsoleKey.Y);
        }
    }
}
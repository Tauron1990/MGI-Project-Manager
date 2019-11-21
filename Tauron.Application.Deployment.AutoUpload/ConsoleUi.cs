using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

            public WriteEntry(string value) 
                => _value = value;

            public override void Execute() 
                => Console.Write(_value);
        }

        private class EmptyLine : ConsoleEntry
        {
            private readonly int _count;

            public EmptyLine(int count) 
                => _count = count;

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

        private void AddEntry(ConsoleEntry entry, bool persistent)
        {
            if(persistent)
                ImmutableInterlocked.InterlockedExchange(ref _persistent, _persistent.Add(entry));
            else
                ImmutableInterlocked.InterlockedExchange(ref _lines, _lines.Add(entry));
        }

        public ConsoleUi WriteError(Exception e)
        {
            AddEntry(new WriteErrorEntry(e.Message), false);
            UpdateUI();
            return this;
        }

        public ConsoleUi WriteError(Exception e, bool persistent)
        {
            AddEntry(new WriteErrorEntry(e.Message), persistent);
            UpdateUI();
            return this;
        }

        public ConsoleUi PrintList<TType>(IEnumerable<TType> enumerable, Action<TType, ConsoleUi> elementAction)
        {
            foreach (var ele in enumerable) 
                elementAction(ele, this);

            return this;
        }

        public ConsoleUi ReplaceLast(string line)
        {
            ImmutableInterlocked.InterlockedExchange(ref _lines, _lines.Remove(_lines.Last()));
            AddEntry(new WriteLineEntry(line), false);
            UpdateUI();

            return this;
        }

        public ConsoleUi WriteLine(string line, bool persistent = false)
        {
            AddEntry(new WriteLineEntry(line), persistent);
            UpdateUI();
            return this;
        }

        public ConsoleUi WriteLine(int count = 1)
        {
            AddEntry(new EmptyLine(count), false);
            UpdateUI();
            return this;
        }

        public ConsoleUi Write(string value, bool persistent = false)
        {
            AddEntry(new WriteEntry(value), persistent);
            UpdateUI();
            return this;
        }

        public ConsoleUi Clear()
        {
            ImmutableInterlocked.InterlockedExchange(ref _lines, ImmutableArray<ConsoleEntry>.Empty);
            UpdateUI();
            return this;
        }

        public string ReadLine() => Console.ReadLine();

        public string ReadLine(string description)
        {
            Write(description);
            return Console.ReadLine();
        }

        public bool Allow(string question)
        {
            WriteLine(question + "y/n");
            return Console.ReadKey().Key == ConsoleKey.Y;
        }
    }
}
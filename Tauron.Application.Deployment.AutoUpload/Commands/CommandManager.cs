using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Tauron.Application.Deployment.AutoUpload.Build;

namespace Tauron.Application.Deployment.AutoUpload.Commands
{
    public sealed class CommandManager
    {
        private class CommandEntry
        {
            public string Description { get; }

            public CommandBase Command { get; }

            public CommandEntry(string description, CommandBase command)
            {
                Description = description;
                Command = command;
            }
        }

        private class InputManagerSingle : InputManager
        {
            private readonly ConsoleUi _ui;

            public InputManagerSingle(ConsoleUi ui) => _ui = ui;

            [DebuggerHidden]
            public override async Task<string> ReadLine(string description) 
                =>  await _ui.ReadLine(description);
        }

        private class InputManagerArray  : InputManager
        { 
            private readonly string[] _args;
            private readonly ConsoleUi _ui;
            private int _pos;

            public InputManagerArray(string[] args, ConsoleUi ui)
            {
                _args = args;
                _ui = ui;
            }

            public override async Task<string> ReadLine(string description)
            {
                if (_args.Length <= _pos) return await _ui.ReadLine(description);

                var result = _args[_pos];
                _pos++;
                _ui.Write(description).WriteLine(" " + result);
                return result;

            }
        }

        private readonly ConsoleUi _ui;
        private readonly ApplicationContext _context;
        private readonly Dictionary<string, CommandEntry> _commandEntries = new Dictionary<string, CommandEntry>();

        public CommandManager(ConsoleUi ui, ApplicationContext context)
        {
            _ui = ui;
            _context = context;

            foreach (var type in Assembly.GetEntryAssembly()?.GetTypes() ?? Type.EmptyTypes)
            {
                var attr = type.GetCustomAttribute<CommandAttribute>();
                if(attr == null || attr.Name.Contains(" ")) continue;

                try
                {
                    var commad = Activator.CreateInstance(type);
                    if(commad is CommandBase commandBase)
                        _commandEntries[attr.Name] = new CommandEntry(attr.Description, commandBase);
                }
                catch (Exception e)
                {
                    ui.WriteError(e);
                }
            }
        }

        public void PrintHelp()
        {
            using (_ui.SupressUpdate())
            {

                _ui.Clear().WriteLine(3);
                foreach (var (name, commandEntry) in _commandEntries)
                {
                    _ui.Write(name).WriteLine(":")
                        .WriteLine(commandEntry.Description).WriteLine();
                }
            }
        }

        public async Task<bool> ExecuteNext(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return true;

            var arguments = input.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            var inputManager = arguments.Length == 1
                ? (InputManager) new InputManagerSingle(_ui)
                : new InputManagerArray(arguments[Range.StartAt(1)], _ui);

            if (!_commandEntries.TryGetValue(arguments[0], out var command)) return true;

            using (_ui.SupressUpdate())
            {
                _ui.Clear().WriteLine(3);
                command.Command.PrintHeader(_ui);
            }

            return await command.Command.Execute(_context, _ui, inputManager);

        }
    }
}
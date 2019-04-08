using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tauron.Application.MgiProjectManager.BL.Contracts;
using Tauron.Application.MgiProjectManager.BL.Contracts.Hubs;
using Tauron.Application.MgiProjectManager.BL.Impl.Hubs;
using Tauron.Application.MgiProjectManager.Server.Data.Entitys;
using Tauron.Application.MgiProjectManager.Server.Data.Repository;

namespace Tauron.Application.MgiProjectManager.BL.Impl.Operations
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public sealed class MultifileOperation : IOperationAction
    {
        private class RangeHelper
        {
            private class Index
            {
                private readonly int _start;
                private readonly int _ent;
                private readonly Func<char, char> _edit;

                public Index(int start, int ent, Func<char, char> edit)
                {
                    _start = start;
                    _ent = ent;
                    _edit = edit;
                }

                public char Edit(char c, int index)
                {
                    if (index >= _start && index <= _ent)
                        return _edit(c);
                    return c;
                }
            }

            private static readonly char[] Spliter = {','};

            private readonly StringBuilder _stringBuilder = new StringBuilder();
            private readonly List<Index> _editors = new List<Index>();

            public RangeHelper(string configInput)
            {
                string[] array = configInput.Split(Spliter, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

                foreach (var s in array)
                {
                    Func<char, char> editor;
                    switch (s[0])
                    {
                        case 'u':
                        case 'U':
                            editor = char.ToUpper;
                            break;

                        case 'd':
                        case 'D':
                            editor = char.ToLower;
                            break;

                        default:
                            continue;
                    }

                    var range = s.Substring(1);
                    if (range.Contains('-'))
                    {
                        string[] posions = range.Split('-');
                        int start = int.Parse(posions[0]);
                        int end = int.Parse(posions[1]);
                        _editors.Add(new Index(start, end, editor));
                    }
                    else
                    {
                        int common = int.Parse(range);
                        _editors.Add(new Index(common, common, editor));
                    }
                }
            }

            public string Edit(string match)
            {
                _stringBuilder.Clear();

                for (int i = 0; i < match.Length; i++)
                {
                    char c = match[i];

                    foreach (var editor in _editors)
                        c = editor.Edit(c, i);

                    _stringBuilder.Append(c);
                }

                return _stringBuilder.ToString();
            }
        }

        private readonly IHubContext<FilesHub, IFilesHub> _filesHub;
        private readonly IFileRepository _repository;
        private readonly ILogger<MultifileOperation> _logger;
        private readonly Regex _nameMatch;
        private readonly RangeHelper _rangeHelper;

        public MultifileOperation(IHubContext<FilesHub, IFilesHub> filesHub, IFileRepository repository, IConfiguration configuration, ILogger<MultifileOperation> logger)
        {
            _nameMatch = new Regex(configuration.GetSection("FilsConfig")["NameExpression"], RegexOptions.Compiled);
            _rangeHelper = new RangeHelper(configuration.GetSection("FilsConfig")["CaseRange"]);
            _filesHub = filesHub;
            _repository = repository;
            _logger = logger;
        }

        public string Name => OperationNames.MultiFileOperation;

        public async Task<Operation[]> Execute(Operation op)
        {
            List<Operation> newOperations = new List<Operation>();
            string user = op.OperationContext["UserName"];

            foreach (var file in op.OperationContext)
            {
                if(file.Key == "UserName") continue;

                var result = _nameMatch.Match(file.Key);
                if (result.Success)
                {
                    string name = result.Value;
                    name = _rangeHelper.Edit(name);

                    await _repository.AddFile(new FileEntity
                    {
                        Age = DateTime.Now,
                        Name = name,
                        Path = file.Value,
                        User = user
                    });
                }
                else
                {
                    newOperations.Add(new Operation(
                        Guid.NewGuid().ToString("D"), 
                        OperationNames.LinkingFileOperation, 
                        OperationNames.FileOperationType,
                        new Dictionary<string, string>
                        {
                            { file.Key, file.Value },
                            { "UserName", user },
                            { "StartId", op.OperationId }
                        }, 
                        DateTime.Now + TimeSpan.FromDays(3)));
                }
            }

            return await newOperations.ToAsyncEnumerable().ToArray();
        }

        public async Task PostExecute(Operation op) 
            => await _filesHub.Clients.All.SendMultifileProcessingCompled(op.OperationId, false);

        public Task<bool> Remove(Operation op)
        {
            try
            {
                foreach (var file in op.OperationContext.Select(p => p.Value))
                {
                    var info = new FileInfo(file);
                    if(info.Exists)
                        info.Delete();
                }

                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error on Delete files: {op.OperationId}");
                return Task.FromResult(false);
            }
        }

        public async Task Error(Operation op, Exception e)
        {
            await _filesHub.Clients.All.SendMultifileProcessingCompled(e.Message, true);
            await Remove(op);
        }
    }
}
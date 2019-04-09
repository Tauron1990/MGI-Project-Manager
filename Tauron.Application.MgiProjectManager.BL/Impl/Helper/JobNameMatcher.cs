using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Tauron.Application.MgiProjectManager.BL.Contracts.Helper;

namespace Tauron.Application.MgiProjectManager.BL.Impl.Helper
{
    public class JobNameMatcher : IJobNameMatcher
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

            private static readonly char[] Spliter = { ',' };

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

        private readonly Regex _nameMatch;
        private RangeHelper _rangeHelper;

        public JobNameMatcher(IConfiguration configuration)
        {
            _nameMatch = new Regex(configuration.GetSection("FilsConfig")["NameExpression"], RegexOptions.Compiled);
            _rangeHelper = new RangeHelper(configuration.GetSection("FilsConfig")["CaseRange"]);
        }

        public Match GetMatch(string name) 
            => _nameMatch.Match(name);

        public string EditJobName(string name) => _rangeHelper.Edit(name);
    }
}
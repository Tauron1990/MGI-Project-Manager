using System;
using System.Text.RegularExpressions;

namespace Tauron.Application.MgiProjectManager.BL.Contracts.Helper
{
    public interface IJobNameMatcher
    {
        Match GetMatch(string name);

        string EditJobName(string name);
    }
}
using System.Text.RegularExpressions;

namespace Tauron.MgiProjectManager.BL
{
    public interface IJobNameMatcher
    {
        Match GetMatch(string name);

        string EditJobName(string name);
    }
}
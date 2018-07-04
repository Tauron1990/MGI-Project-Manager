using System;
using JetBrains.Annotations;

namespace Tauron.Application.ProjectManager.ApplicationServer.BussinesLayer
{
    [PublicAPI]
    public static class ValidationRuleHelper
    {
        public static bool Assert(bool predicate, string message, out string outputMessage)
        {
            if (predicate)
            {
                outputMessage = null;
                return true;
            }

            outputMessage = message;
            return false;
        }

        public static bool AssertList([NotNull] AssertHelp[] asserts, out string outputMessage)
        {
            if (asserts == null) throw new ArgumentNullException(nameof(asserts));

            outputMessage = null;
            foreach (var assert in asserts)
            {
                if (!Assert(assert.Ok, assert.Message, out outputMessage)) return false;
            }

            return true;
        }
    }
}
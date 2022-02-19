using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace KPBSD.PowerShell.WindowsUpdate
{
    public class ServerSelectionArgumentCompleterAttribute : ArgumentCompleterAttribute, IArgumentCompleter {
        public ServerSelectionArgumentCompleterAttribute() : base(typeof(ServerSelectionArgumentCompleterAttribute)) {

        }

        public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            return new CompletionResult[]
            {
                new CompletionResult("Default"),
                new CompletionResult("ManagedServer"),
                new CompletionResult("WindowsUpdate"),
            };
        }
    }
}

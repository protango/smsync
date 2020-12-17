using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Management.Automation;
using System.Linq;

namespace smsync
{
    public class PowershellTerminal : IDisposable
    {
        private PowerShell ps = PowerShell.Create();

        public void Dispose()
        {
            ps.Dispose();
        }

        public string[] Execute(string command)
        {
            var results = ps.AddScript(command).Invoke();
            return results.Select(x => x.ToString()).ToArray();
        }
    }
}

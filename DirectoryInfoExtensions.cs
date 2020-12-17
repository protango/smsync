using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace smsync
{
    static class DirectoryInfoExtensions
    {
        public static bool EquivalentTo(this DirectoryInfo @this, DirectoryInfo other) {
            string p1 = @this.FullName, p2 = other.FullName;
            if (Path.EndsInDirectorySeparator(p1)) p1 = p1.Substring(0, p1.Length - 1);
            if (Path.EndsInDirectorySeparator(p2)) p2 = p2.Substring(0, p2.Length - 1);
            return string.Equals(p1, p2, StringComparison.OrdinalIgnoreCase);
        }
    }
}

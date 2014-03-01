using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeaverSoft.Common.Shell
{
    public static class TempFileCleanup
    {
        public static void RegisterTempFile(string tempfile)
        {
            RegisteredFiles.Add(tempfile);
        }

        public static void Unregister(string tempfile)
        {
            if (RegisteredFiles.Contains(tempfile))
            {
                RegisteredFiles.Remove(tempfile);
            }
        }

        public static void PerformCleanup()
        {
            foreach (var path in RegisteredFiles)
            {
                DestroyTempFile(path);
            }

            RegisteredFiles.Clear();
        }

        public static void DestroyTempFile(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                try
                {
                    System.IO.File.Delete(filename);
                }
                catch
                {
                }
            }
        }

        private static HashSet<string> RegisteredFiles = new HashSet<string>();

    }
}

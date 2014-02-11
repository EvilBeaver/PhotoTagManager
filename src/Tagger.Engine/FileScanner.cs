using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tagger.Engine
{
    public class FileScanner
    {
        public IList<FileLink> ScanFolder(string path, string pattern, bool recursive)
        {
            var resultQry = QueryFolder(path, pattern, recursive);
            return resultQry.ToList();
        }

        public IList<FileLink> ScanFolder(string path, string pattern)
        {
            return ScanFolder(path, pattern, false);
        }

        public IQueryable<FileLink> QueryFolder(string path, string pattern, bool recursive)
        {
            var option = recursive ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly;
            var entries = System.IO.Directory.EnumerateFiles(path, pattern, option);

            return entries.AsParallel().Select(x => FileLink.Create(x)).AsQueryable();
        }

    }
}

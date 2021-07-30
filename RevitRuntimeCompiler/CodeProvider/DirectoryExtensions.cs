using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RevitRuntimeCompiler.Editor
{
    internal static class DirectoryExtensions
    {
        public static void CopyFilesRecursively(this DirectoryInfo source, DirectoryInfo target, Predicate<FileSystemInfo> shouldRemove = null)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                if (shouldRemove(dir))
                    CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name),shouldRemove);

            foreach (FileInfo file in source.GetFiles())
                if (shouldRemove(file))
                    file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
    }
}

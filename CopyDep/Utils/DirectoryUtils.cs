using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CopyDep.Utils
{
    public class DirectoryUtils
    {

        public DirectoryUtils()
        {
            
        }

        public Boolean ExistsDir(String dirPath)
        {
            return System.IO.Directory.Exists(dirPath);
        }
        public IEnumerable<String> GetSubDirs(String dirPath)
        {
            if (!ExistsDir(dirPath)) return null;
            return System.IO.Directory.EnumerateDirectories(dirPath);
        }
        public Boolean ExistsFile(String filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        private String FixDirPath(String dir)
        {
            if (String.IsNullOrWhiteSpace(dir)) return dir;
            dir = dir.Replace("\r\n", String.Empty);
            return dir;
        }


        /// <summary>
        /// Сравнение контента (и только контента) файлов.
        /// </summary>
        /// <returns>True, если контент идентичен.</returns>
        public Boolean FileContentCompare(string file1, string file2, Boolean onlyRead)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;
            if (file1 == file2)  return true;
            fs1 = onlyRead ? new FileStream(file1, FileMode.Open) : new FileStream(file1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fs2 = onlyRead ? new FileStream(file2, FileMode.Open) : new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            do
            {
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));
            fs1.Close();
            fs2.Close();
            return ((file1byte - file2byte) == 0);
        }




        private void SearchInDirDir(String dir, List<String> outDirs, Boolean withInner)
        {
            if (String.IsNullOrWhiteSpace(dir)) return;
            if (dir.StartsWith(Conventions.DirCommented)) return;
            if (!this.ExistsDir(dir)) return;
            if (dir.EndsWith("\\")) dir = dir.Remove(dir.Length - 1);
            outDirs.Add(dir);
            if (withInner)
            {
                var subDirs = this.GetSubDirs(dir);
                if (subDirs != null && subDirs.Any())
                {
                    foreach(var subDir in subDirs)
                    {
                        SearchInDirDir(subDir, outDirs, withInner);
                    }
                }
            }
        }


        public List<String> SearchDirs(List<String> dirsInput) //, String dirSeparator)
        {
            var dirs = new List<String>();
            if (dirsInput == null || !dirsInput.Any()) return dirs;
            foreach (var dir in dirsInput)
            {
                if (String.IsNullOrWhiteSpace(dir)) continue;
                var dirSearch = dir;
                var withInner = dirSearch.EndsWith(Conventions.DirSubdirSymbol);
                if (withInner) dirSearch = dirSearch.Remove(dirSearch.Length - Conventions.DirSubdirSymbol.Length);
                SearchInDirDir(dirSearch, dirs, withInner);
            }
            return dirs;
        }



        public List<String> SearchFiles(List<String> dirsInput, bool topDirectoryOnly = true, string searchPattern = "*")
        {
            var files = new List<String>();
            if (dirsInput == null || !dirsInput.Any()) return files;
            foreach (var dir in dirsInput)
            {
                if (String.IsNullOrWhiteSpace(dir)) continue;
                foreach (string file in Directory.EnumerateFiles(dir, searchPattern, topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories))
                {
                    files.Add(file);
                }
            }
            return files;
        }



    }
}

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




        private void SearchInDirDir(String dir, List<String> outDirs, List<String> dirsIgnore, Boolean withInner, Action<String> onCheckFn)
        {
            if (String.IsNullOrWhiteSpace(dir)) return;
            if (dir.StartsWith(Conventions.DirCommented)) return;
            if (onCheckFn != null) onCheckFn(dir);
            if (!this.ExistsDir(dir)) return;
            if (dir.EndsWith("\\")) dir = dir.Remove(dir.Length - 1);
            if (dirsIgnore == null || !dirsIgnore.Contains(dir)) outDirs.Add(dir);
            if (withInner)
            {
                var subDirs = this.GetSubDirs(dir);
                if (subDirs != null && subDirs.Any())
                {
                    foreach(var subDir in subDirs)
                    {
                        SearchInDirDir(subDir, outDirs, dirsIgnore, withInner, onCheckFn);
                    }
                }
            }
        }


        public List<String> SearchDirs(List<String> dirsInput, List<String> dirsIgnore, Action<String> onCheckFn) //, String dirSeparator)
        {
            var dirs = new List<String>();
            if (dirsInput == null || !dirsInput.Any()) return dirs;
            foreach (var dir in dirsInput)
            {
                if (String.IsNullOrWhiteSpace(dir)) continue;
                var dirSearch = dir;
                var withInner = dirSearch.EndsWith(Conventions.DirSubdirSymbol);
                if (withInner) dirSearch = dirSearch.Remove(dirSearch.Length - Conventions.DirSubdirSymbol.Length);
                SearchInDirDir(dirSearch, dirs, dirsIgnore, withInner, onCheckFn);
            }
            return dirs;
        }



        public List<String> SearchFiles(List<String> dirsInput, Action<String> onCheckFn, bool topDirectoryOnly = true, string searchPattern = "*")
        {
            var files = new List<String>();
            if (dirsInput == null || !dirsInput.Any()) return files;
            foreach (var dir in dirsInput)
            {
                if (String.IsNullOrWhiteSpace(dir)) continue;
                if (onCheckFn != null) onCheckFn(dir);
                var filesInDir = Directory.EnumerateFiles(dir, searchPattern, topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
                foreach (string file in filesInDir)
                {
                    if (onCheckFn != null) onCheckFn(file);
                    files.Add(file);
                }
            }
            return files;
        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CopyDep.Utils
{
    public static class StringUtils
    {

        public static String AssetImage(String fileName)
        {
            return String.Format("{0}{1}", Conventions.AssetImagePath, fileName);
        }


        public static String StringRemoveNewLines(String str)
        {
            if (String.IsNullOrWhiteSpace(str)) return str;
            return str.Replace("\r\n", String.Empty);
        }


        public static String StringAddNewLines(String str, String afterSymbol = ";")
        {
            var placeStr = String.Format("{0}\r\n", afterSymbol);
            return str.Replace(afterSymbol, placeStr);
        }


        public static List<String> StringSplitNewLines(String str)
        {
            var list = new List<String>();
            if (String.IsNullOrWhiteSpace(str)) return list;
            var dirs = str.Split("\r\n");
            foreach(var dir in dirs)
            {
                if (String.IsNullOrWhiteSpace(dir)) continue;
                list.Add(dir);
            }
            return list;
        }


        public static String StringJoinNewLines(List<String> dirs)
        {
            if (dirs == null || !dirs.Any()) return String.Empty;
            return String.Join("\r\n", dirs);
        }




    }
}

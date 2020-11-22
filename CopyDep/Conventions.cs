using System;
using System.Collections.Generic;
using System.Text;

namespace CopyDep
{
    public class Conventions
    {
        public static String ConfigFilePath { get { return "config.json"; } }

        //public static String DirSeparatorStr { get { return ";"; } }

        public static String DirSubdirSymbol { get { return "*"; } }
        public static String DirCommented { get { return "--"; } }

        public static String AssetImagePath { get { return "/CopyDep;component/Assets/Images/"; } }


    }
}

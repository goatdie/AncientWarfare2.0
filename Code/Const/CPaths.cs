using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Const
{
    internal class CPaths
    {
        public static readonly string ChineseNamePackagePath = Combine(Main.mainPath, "ChineseNamePackage");
        public static readonly string WordLibrariesPath = Combine(ChineseNamePackagePath, "WordLibraries");
        private static string Combine(params string[] paths) => new FileInfo(paths.Aggregate("", Path.Combine)).FullName;
    }
}

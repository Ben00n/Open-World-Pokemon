using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.TextureGraph
{
    /// <summary>
    /// Utility class contains product info
    /// </summary>
    public static class TVersionInfo
    {
        public static double Number
        {
            get
            {
                return 0.3;
            }
        }

        public static bool IsBeta
        {
            get
            {
                return true;
            }
        }

        public static string Code
        {
            get
            {
                return Number.ToString() + (IsBeta ? "b" : "");
            }
        }

        public static string ProductName
        {
            get
            {
                return "Texture Graph";
            }
        }

        public static string ProductNameAndVersion
        {
            get
            {
                return string.Format("{0} v{1}", ProductName, Code);
            }
        }

        public static string ProductNameShort
        {
            get
            {
                return "Texture Graph";
            }
        }

        public static string ProductNameAndVersionShort
        {
            get
            {
                return string.Format("{0} v{1}", ProductNameShort, Code);
            }
        }
    }
}

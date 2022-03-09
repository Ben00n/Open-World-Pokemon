using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.Poseidon
{
    /// <summary>
    /// Utility class contains product info
    /// </summary>
    public static class PVersionInfo
    {
        public static float Number
        {
            get
            {
                return 175;
            }
        }

        public static string Code
        {
            get
            {
                return "1.7.5";
            }
        }

        public static string ProductName
        {
            get
            {
                return "Poseidon - Low Poly Water System";
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
                return "Poseidon";
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

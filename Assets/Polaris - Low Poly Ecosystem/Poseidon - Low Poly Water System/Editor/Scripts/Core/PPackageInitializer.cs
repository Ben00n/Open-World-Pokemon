using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace Pinwheel.Poseidon
{
    public static class PPackageInitializer
    {
        public static string POSEIDON_KW = "POSEIDON";
        public static string POSEIDON_URP_KW = "POSEIDON_URP";

        private static ListRequest listPackageRequest = null;
#pragma warning disable 0414
        private static bool isUrpInstalled = false;
        private static bool isShaderGraphInstalled = false;
#pragma warning restore 0414

        [DidReloadScripts]
        public static void Init()
        {
            isUrpInstalled = false;
            isShaderGraphInstalled = false;

            CheckUnityPackagesAndInit();
        }

        private static void CheckUnityPackagesAndInit()
        {
            listPackageRequest = Client.List(true);
            EditorApplication.update += OnRequestingPackageList;
        }

        private static void OnRequestingPackageList()
        {
            if (listPackageRequest == null)
                return;
            if (!listPackageRequest.IsCompleted)
                return;
            if (listPackageRequest.Error != null)
            {
                isUrpInstalled = false;
                isShaderGraphInstalled = false;
            }
            else
            {
                isUrpInstalled = false;
                isShaderGraphInstalled = false;
                foreach (UnityEditor.PackageManager.PackageInfo p in listPackageRequest.Result)
                {
                    if (p.name.Equals("com.unity.render-pipelines.universal"))
                    {
                        isUrpInstalled = true;
                    }
                    if (p.name.Equals("com.unity.shadergraph"))
                    {
                        isShaderGraphInstalled = true;
                    }
                }
            }
            EditorApplication.update -= OnRequestingPackageList;
            InitPackage();
        }

        private static void InitPackage()
        {
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup buildGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup);
            List<string> symbolList = new List<string>(symbols.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));

            bool isDirty = false;
            if (!symbolList.Contains(POSEIDON_KW))
            {
                symbolList.Add(POSEIDON_KW);
                isDirty = true;
            }

            if (isUrpInstalled && !symbolList.Contains(POSEIDON_URP_KW))
            {
                symbolList.Add(POSEIDON_URP_KW);
                isDirty = true;
            }
            else if (!isUrpInstalled && symbolList.Contains(POSEIDON_URP_KW))
            {
                symbolList.RemoveAll(s => s.Equals(POSEIDON_URP_KW));
                isDirty = true;
            }

            if (isDirty)
            {
                symbols = symbolList.ListElementsToString(";");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, symbols);
            }
        }
    }
}

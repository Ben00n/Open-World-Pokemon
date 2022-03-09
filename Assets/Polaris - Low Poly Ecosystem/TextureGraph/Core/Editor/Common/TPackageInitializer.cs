using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace Pinwheel.TextureGraph
{
    public static class TPackageInitializer
    {
        public delegate void InitCompletedHandler();
        public static event InitCompletedHandler Completed;

        public static string TEXTURE_GRAPH_KW = "TEXTURE_GRAPH";
        public static string TG_SEARCHER_KW = "TG_SEARCHER";

        private static ListRequest listPackageRequest = null;
        private static AddRequest addPackageRequest = null;

#pragma warning disable 0414
        private static bool isSearcherInstalled = false;
#pragma warning restore 0414

        [DidReloadScripts]
        public static void Init()
        {
            isSearcherInstalled = false;
            CheckThirdPartyPackages();
            CheckUnityPackagesAndInit();
        }

        private static void CheckThirdPartyPackages()
        {

        }

        private static void CheckUnityPackagesAndInit()
        {
#if !TG_SEARCHER
            addPackageRequest = Client.Add("com.unity.searcher");
            EditorApplication.update += OnRequestingPackageAdd;
#endif
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
            }
            else
            {
                foreach (UnityEditor.PackageManager.PackageInfo p in listPackageRequest.Result)
                {
                    if (p.name.Equals("com.unity.searcher"))
                    {
                        isSearcherInstalled = true;
                    }
                }
            }
            EditorApplication.update -= OnRequestingPackageList;
            InitPackage();
        }

        private static void OnRequestingPackageAdd()
        {
            if (addPackageRequest == null)
                return;
            if (!addPackageRequest.IsCompleted)
                return;
            if (addPackageRequest.Error != null)
            {
                
            }
            else
            {
                Debug.Log("TEXTURE GRAPH: Dependency package installed [com.unity.searcher]");
            }
            listPackageRequest = Client.List(true);
            EditorApplication.update += OnRequestingPackageList;
            EditorApplication.update -= OnRequestingPackageAdd;
        }

        private static void InitPackage()
        {
            BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup buildGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup);
            List<string> symbolList = new List<string>(symbols.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));

            bool isDirty = false;
            if (!symbolList.Contains(TEXTURE_GRAPH_KW))
            {
                symbolList.Add(TEXTURE_GRAPH_KW);
                isDirty = true;
            }
            isDirty = isDirty || SetKeywordActive(symbolList, TG_SEARCHER_KW, isSearcherInstalled);

            if (isDirty)
            {
                symbols = symbolList.ListElementsToString(";");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, symbols);
            }
            
            if (Completed != null)
            {
                Completed.Invoke();
            }
        }

        private static bool SetKeywordActive(List<string> kwList, string keyword, bool active)
        {
            bool isDirty = false;
            if (active && !kwList.Contains(keyword))
            {
                kwList.Add(keyword);
                isDirty = true;
            }
            else if (!active && kwList.Contains(keyword))
            {
                kwList.RemoveAll(s => s.Equals(keyword));
                isDirty = true;
            }
            return isDirty;
        }

        public static List<System.Type> GetAllLoadedTypes()
        {
            List<System.Type> loadedTypes = new List<System.Type>();
            List<string> typeName = new List<string>();
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in assembly.GetTypes())
                {
                    if (t.IsVisible && !t.IsGenericType)
                    {
                        typeName.Add(t.Name);
                        loadedTypes.Add(t);
                    }
                }
            }
            return loadedTypes;
        }
    }
}

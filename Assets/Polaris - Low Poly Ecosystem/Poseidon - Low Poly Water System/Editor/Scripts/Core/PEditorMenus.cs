using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.Poseidon
{
    public static class PEditorMenus
    {
        [MenuItem("GameObject/3D Object/Poseidon/Calm Water")]
        public static PWater CreateCalmWaterObject(MenuCommand cmd)
        {
            GameObject g = new GameObject("Calm Water");
            if (cmd != null && cmd.context != null)
            {
                GameObject root = cmd.context as GameObject;
                GameObjectUtility.SetParentAndAlign(g, root);
            }

            PWater waterComponent = g.AddComponent<PWater>();
            PWaterProfile profile = PWaterProfile.CreateInstance<PWaterProfile>();
            string fileName = "WaterProfile_" + PCommon.GetUniqueID();
            string filePath = string.Format("Assets/{0}.asset", fileName);
            AssetDatabase.CreateAsset(profile, filePath);

            profile.CopyFrom(PPoseidonSettings.Instance.CalmWaterProfile);
            waterComponent.Profile = profile;
            waterComponent.TileSize = new Vector2(100, 100);

            return waterComponent;
        }

        [MenuItem("GameObject/3D Object/Poseidon/Calm Water HQ")]
        public static PWater CreateCalmWaterHQObject(MenuCommand cmd)
        {
            GameObject g = new GameObject("Calm Water HQ");
            if (cmd != null && cmd.context != null)
            {
                GameObject root = cmd.context as GameObject;
                GameObjectUtility.SetParentAndAlign(g, root);
            }

            PWater waterComponent = g.AddComponent<PWater>();
            PWaterProfile profile = PWaterProfile.CreateInstance<PWaterProfile>();
            string fileName = "WaterProfile_" + PCommon.GetUniqueID();
            string filePath = string.Format("Assets/{0}.asset", fileName);
            AssetDatabase.CreateAsset(profile, filePath);

            profile.CopyFrom(PPoseidonSettings.Instance.CalmWaterHQProfile);
            waterComponent.Profile = profile;
            waterComponent.TileSize = new Vector2(100, 100);

            return waterComponent;
        }

        [MenuItem("Window/Poseidon/Project/Settings")]
        public static void ShowSettings()
        {
            Selection.activeObject = PPoseidonSettings.Instance;
            EditorGUIUtility.PingObject(PPoseidonSettings.Instance);
        }

        [MenuItem("Window/Poseidon/Project/Update Dependencies")]
        public static void UpdateDependencies()
        {
            PPackageInitializer.Init();
            PWaterShaderProvider.ResetShaderReferences();
        }

        [MenuItem("Window/Poseidon/Project/Version Info")]
        public static void ShowVersionInfo()
        {
            EditorUtility.DisplayDialog(
                "Version Info",
                PVersionInfo.ProductNameAndVersion,
                "OK");
        }

        [MenuItem("Window/Poseidon/Learning Resources/Online Manual")]
        public static void ShowOnlineUserGuide()
        {
            Application.OpenURL(PCommon.ONLINE_MANUAL);
        }

        [MenuItem("Window/Poseidon/Learning Resources/Youtube Channel")]
        public static void ShowYoutubeChannel()
        {
            Application.OpenURL(PCommon.YOUTUBE_CHANNEL);
        }

        [MenuItem("Window/Poseidon/Learning Resources/Facebook Page")]
        public static void ShowFacebookPage()
        {
            Application.OpenURL(PCommon.FACEBOOK_PAGE);
        }

        [MenuItem("Window/Poseidon/Community/Forum")]
        public static void ShowForum()
        {
            Application.OpenURL(PCommon.FORUM);
        }

        [MenuItem("Window/Poseidon/Community/Discord")]
        public static void ShowDiscord()
        {
            Application.OpenURL(PCommon.DISCORD);
        }

        [MenuItem("Window/Poseidon/Contact/Support")]
        public static void ShowSupportEmailEditor()
        {
            PEditorCommon.OpenEmailEditor(
                PCommon.SUPPORT_EMAIL,
                "[Poseidon] SHORT_QUESTION_HERE",
                "YOUR_QUESTION_IN_DETAIL");
        }

        [MenuItem("Window/Poseidon/Contact/Business")]
        public static void ShowBusinessEmailEditor()
        {
            PEditorCommon.OpenEmailEditor(
                PCommon.BUSINESS_EMAIL,
                "[Poseidon] SHORT_MESSAGE_HERE",
                "YOUR_MESSAGE_IN_DETAIL");
        }

        [MenuItem("Window/Poseidon/Write a Review")]
        public static void OpenStorePage()
        {
            Application.OpenURL("http://u3d.as/1CsU");
        }
    }
}

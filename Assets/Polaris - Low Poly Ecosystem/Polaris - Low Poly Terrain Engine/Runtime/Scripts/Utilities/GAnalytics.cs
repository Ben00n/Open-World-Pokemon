#if GRIFFIN
using UnityEngine;
using UnityEngine.Networking;

namespace Pinwheel.Griffin
{
    public static class GAnalytics
    {
        public const string OS_WINDOWS = "http://bit.ly/34fMuhb";
        public const string OS_MAC = "http://bit.ly/31XORUx";
        public const string OS_LINUX = "http://bit.ly/34cUYWg";

        public const string PLATFORM_PC = "http://bit.ly/333nKZq";
        public const string PLATFORM_MOBILE = "http://bit.ly/349N0gA";
        public const string PLATFORM_CONSOLE = "http://bit.ly/2NrXvVN";
        public const string PLATFORM_WEB = "http://bit.ly/36dOsQZ";
        public const string PLATFORM_OTHER = "http://bit.ly/2MZnuFm";

        public const string XR_PROJECT = "http://bit.ly/2qYUhlr";

        public const string COLOR_SPACE_GAMMA = "http://bit.ly/330OHwG";
        public const string COLOR_SPACE_LINEAR = "http://bit.ly/349O7wM";

        public const string RENDER_PIPELINE_BUILTIN = "http://bit.ly/2N091Jg";
        public const string RENDER_PIPELINE_LIGHTWEIGHT = "http://bit.ly/36hxUro";
        public const string RENDER_PIPELINE_UNIVERSAL = "http://bit.ly/34fbaX7";
        public const string RENDER_PIPELINE_OTHER = "http://bit.ly/2piMkaf";

        public const string INTEGRATION_AMPLIFY_SHADER_EDITOR = "http://bit.ly/2JBwfDw";
        public const string INTEGRATION_POSEIDON = "http://bit.ly/2WrPEMc";
        public const string INTEGRATION_CSHARP_WIZARD = "http://bit.ly/2MZsqdh";
        public const string INTEGRATION_MESH_TO_FILE = "http://bit.ly/2Nr35b1";
        public const string INTEGRATION_VEGETATION_STUDIO = "https://bit.ly/34kiPVB";
        public const string INTEGRATION_MICRO_SPLAT = "https://bit.ly/3bwXtWS";

        public const string MULTI_TERRAIN = "http://bit.ly/2q2M2Eh";
        public const string WIND_ZONE = "http://bit.ly/2JCtjWY";
        public const string CONVERT_FROM_UNITY_TERRAIN = "http://bit.ly/2WrphpN";

        public const string WIZARD_CREATE_TERRAIN = "http://bit.ly/2PurvTT";
        public const string WIZARD_SET_SHADER = "http://bit.ly/326NG5j";

        public const string SHADING_COLOR_MAP = "http://bit.ly/323pzEg";
        public const string SHADING_GRADIENT_LOOKUP = "http://bit.ly/2PB1WR0";
        public const string SHADING_SPLAT = "http://bit.ly/2C0jmi1";
        public const string SHADING_VERTEX_COLOR = "http://bit.ly/2q6Ty13";

        public const string ENABLE_INSTANCING = "http://bit.ly/2PAjM6C";
        public const string ENABLE_INTERACTIVE_GRASS = "http://bit.ly/2BWRMSD";

        public const string IMPORT_UNITY_TERRAIN_DATA = "http://bit.ly/2JApdPl";
        public const string IMPORT_POLARIS_V1_DATA = "http://bit.ly/34bKdDI";
        public const string IMPORT_RAW = "http://bit.ly/2qZK5sO";
        public const string IMPORT_TEXTURES = "http://bit.ly/2NqHYFY";

        public const string EXPORT_UNITY_TERRAIN_DATA = "http://bit.ly/2N34cyT";
        public const string EXPORT_RAW = "http://bit.ly/2Ws3XAg";
        public const string EXPORT_TEXTURES = "http://bit.ly/335KGak";

        public const string GROUP_OVERRIDE_GEOMETRY = "http://bit.ly/2N4ho6G";
        public const string GROUP_OVERRIDE_SHADING = "http://bit.ly/31VkuOs";
        public const string GROUP_OVERRIDE_RENDERING = "http://bit.ly/2NpGf3C";
        public const string GROUP_OVERRIDE_FOLIAGE = "http://bit.ly/2qZMhR4";
        public const string GROUP_REARRANGE = "http://bit.ly/2JDdW0t";
        public const string GROUP_MATCH_EDGE = "http://bit.ly/2rDM46g";

        public const string TPAINTER_ELEVATION = "http://bit.ly/2pg6dPe";
        public const string TPAINTER_HEIGHT_SAMPLING = "http://bit.ly/36ihqiH";
        public const string TPAINTER_TERRACE = "http://bit.ly/32XeeqE";
        public const string TPAINTER_REMAP = "http://bit.ly/34fXZ8t";
        public const string TPAINTER_NOISE = "http://bit.ly/2ptazlQ";
        public const string TPAINTER_SUBDIV = "http://bit.ly/2qPboWu";
        public const string TPAINTER_ALBEDO = "http://bit.ly/2otnoMz";
        public const string TPAINTER_METALLIC = "http://bit.ly/2JwqyGG";
        public const string TPAINTER_SMOOTHNESS = "http://bit.ly/2NpLkJm";
        public const string TPAINTER_SPLAT = "http://bit.ly/36aCDLo";
        public const string TPAINTER_CUSTOM = "http://bit.ly/33bdH4o";

        public const string FPAINTER_PAINT_TREE = "http://bit.ly/36dGwzb";
        public const string FPAINTER_SCALE_TREE = "http://bit.ly/2JzHJHt";
        public const string FPAINTER_PAINT_GRASS = "http://bit.ly/2MWmfqm";
        public const string FPAINTER_SCALE_GRASS = "http://bit.ly/2Pv2EiH";
        public const string FPAINTER_CUSTOM = "http://bit.ly/34dvJ6f";

        public const string OPAINTER_SPAWN = "http://bit.ly/36mPn1R";
        public const string OPAINTER_SCALE = "http://bit.ly/2BRbHCC";
        public const string OPAINTER_CUSTOM = "http://bit.ly/2PtNhHi";

        public const string SPLINE_RAMP_MAKER = "http://bit.ly/3337V50";
        public const string SPLINE_PATH_PAINTER = "http://bit.ly/2NsN7gD";
        public const string SPLINE_FOLIAGE_SPAWNER = "http://bit.ly/3307hW0";
        public const string SPLINE_FOLIAGE_REMOVER = "http://bit.ly/2WqoeGC";
        public const string SPLINE_OBJECT_SPAWNER = "http://bit.ly/2qbnFEg";
        public const string SPLINE_OBJECT_REMOVER = "http://bit.ly/2BVVxI4";

        public const string STAMPER_GEOMETRY = "http://bit.ly/2q5nOJy";
        public const string STAMPER_TEXTURE = "http://bit.ly/2JDEU8a";
        public const string STAMPER_FOLIAGE = "http://bit.ly/321JIe3";
        public const string STAMPER_OBJECT = "http://bit.ly/34ia3WC";

        public const string NAVIGATION_HELPER = "http://bit.ly/2NqLwrM";

        public const string BACKUP_CREATE = "http://bit.ly/2N2NzDf";
        public const string BACKUP_RESTORE = "http://bit.ly/2r20Ofb";

        public const string ASSET_EXPLORER_LINK_CLICK = "http://bit.ly/34iwLhr";
        public const string HELP_OPEN_WINDOW = "http://bit.ly/2pv2i0N";
        public const string BILLBOARD_SAVE = "http://bit.ly/333aaVY";

        public const string TEXTURE_CREATOR_HEIGHT_MAP = "http://bit.ly/2WqqWvM";
        public const string TEXTURE_CREATOR_HEIGHT_MAP_FROM_MESH = "http://bit.ly/2pv2YmR";
        public const string TEXTURE_CREATOR_NORMAL_MAP = "http://bit.ly/2WrJdIW";
        public const string TEXTURE_CREATOR_STEEPNESS_MAP = "http://bit.ly/2Py71cT";
        public const string TEXTURE_CREATOR_NOISE_MAP = "http://bit.ly/2JzBtQ8";
        public const string TEXTURE_CREATOR_COLOR_MAP = "http://bit.ly/2N37emP";
        public const string TEXTURE_CREATOR_BLEND_MAP = "http://bit.ly/2Ws8H92";
        public const string TEXTURE_CREATOR_FOLIAGE_DISTRIBUTION_MAP = "http://bit.ly/322zCJU";

        public const string LINK_ONLINE_MANUAL = "http://bit.ly/2NvamGK";
        public const string LINK_YOUTUBE = "http://bit.ly/2N0s2uU";
        public const string LINK_FACEBOOK = "http://bit.ly/2pjN278";
        public const string LINK_EXPLORE_ASSET = "http://bit.ly/2PFqDvs";

        public const string THERMAL_EROSION = "https://bit.ly/3ujcght";
        public const string HYDRAULIC_EROSION = "https://bit.ly/3fDFyn0";

        public static void Record(string url, bool perProject = false)
        {
#if UNITY_EDITOR
            if (!GEditorSettings.Instance.general.enableAnalytics)
                return;

            if (string.IsNullOrEmpty(url))
                return;

            bool willRecord = true;
            if (perProject && PlayerPrefs.HasKey(url))
            {
                willRecord = false;
            }

            if (!willRecord)
                return;

            if (perProject)
            {
                PlayerPrefs.SetInt(url, 1);
            }

            UnityWebRequest request = new UnityWebRequest(url);
            request.SendWebRequest();
#endif
        }
    }
}
#endif

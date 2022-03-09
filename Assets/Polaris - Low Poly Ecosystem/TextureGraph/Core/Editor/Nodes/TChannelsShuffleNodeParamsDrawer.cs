using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TChannelsShuffleNode))]
    public class TChannelsShuffleNodeParamsDrawer : TParametersDrawer
    {
        public static readonly GUIContent redSourceGUI = new GUIContent("Red Source", "Source of the output R channel");
        public static readonly GUIContent greenSourceGUI = new GUIContent("Green Source", "Source of the output G channel");
        public static readonly GUIContent blueSourceGUI = new GUIContent("Blue Source", "Source of the output B channel");
        public static readonly GUIContent alphaSourceGUI = new GUIContent("Alpha Source", "Source of the output A channel");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TChannelsShuffleNode n = target as TChannelsShuffleNode;
            n.OutputRedSource.value = (TChannelsShuffleNode.TChannelSource)EditorGUILayout.EnumPopup(redSourceGUI, n.OutputRedSource.value);
            n.OutputGreenSource.value = (TChannelsShuffleNode.TChannelSource)EditorGUILayout.EnumPopup(greenSourceGUI, n.OutputGreenSource.value);
            n.OutputBlueSource.value = (TChannelsShuffleNode.TChannelSource)EditorGUILayout.EnumPopup(blueSourceGUI, n.OutputBlueSource.value);
            n.OutputAlphaSource.value = (TChannelsShuffleNode.TChannelSource)EditorGUILayout.EnumPopup(alphaSourceGUI, n.OutputAlphaSource.value);
        }
    }
}

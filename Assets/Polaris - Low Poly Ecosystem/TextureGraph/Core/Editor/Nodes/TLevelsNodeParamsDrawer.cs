using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;
using THistogramChannel = Pinwheel.TextureGraph.TLevelsEditorDrawer.THistogramChannel;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TLevelsNode))]
    public class TLevelsNodeParamsDrawer : TParametersDrawer
    {
        private static readonly string CHANNEL_KEY = "levels-histogram-channel";
        private static readonly GUIContent channelGUI = new GUIContent("Channel", "The level to edit");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TLevelsNode n = target as TLevelsNode;

            THistogramChannel channel = (THistogramChannel)SessionState.GetInt(CHANNEL_KEY, -1);
            channel = (THistogramChannel)EditorGUILayout.EnumPopup(channelGUI, channel);
            SessionState.SetInt(CHANNEL_KEY, (int)channel);

            Texture preview = null;
            TSlot previewSlot = n.GetMainOutputSlot();
            if (previewSlot != null)
            {
                preview = Graph.GetRT(TSlotReference.Create(n.GUID, previewSlot.Id));
            }

            if (channel == THistogramChannel.Luminance)
            {
                n.LuminanceControl = TLevelsEditorDrawer.Draw(n.LuminanceControl, preview, channel, Color.white);
            }
            else if (channel == THistogramChannel.Red)
            {
                n.RedControl = TLevelsEditorDrawer.Draw(n.RedControl, preview, channel, Color.red);
            }
            else if (channel == THistogramChannel.Green)
            {
                n.GreenControl = TLevelsEditorDrawer.Draw(n.GreenControl, preview, channel, Color.green);
            }
            else if (channel == THistogramChannel.Blue)
            {
                n.BlueControl = TLevelsEditorDrawer.Draw(n.BlueControl, preview, channel, Color.blue);
            }
            else if (channel == THistogramChannel.Alpha)
            {
                n.AlphaControl = TLevelsEditorDrawer.Draw(n.AlphaControl, preview, channel, Color.gray);
            }
        }
    }
}

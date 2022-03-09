using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TBrickNode))]
    public class TBrickNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent tilingGUI = new GUIContent("Tiling", "Tile the pattern multiple times");
        private static readonly GUIContent gapSizeGUI = new GUIContent("Gap Size", "Size of the gap between blocks");
        private static readonly GUIContent innerSizeGUI = new GUIContent("Inner Size", "Size of the inner part of each block, to make the feel of extrusion");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TBrickNode n = target as TBrickNode;
            n.Tiling = TParamGUI.IntField(tilingGUI, n.Tiling);
            n.GapSize = TParamGUI.FloatSlider(gapSizeGUI, n.GapSize, 0f, 1f);
            n.InnerSize = TParamGUI.FloatSlider(innerSizeGUI, n.InnerSize, 0f, 1f);
        }
    }
}

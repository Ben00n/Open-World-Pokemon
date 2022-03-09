using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TMirrorNode))]
    public class TMirrorNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent mirrorXGUI = new GUIContent("Mirror X", "Should it mirror the image on X axis (left-right)?");
        private static readonly GUIContent flipXGUI = new GUIContent("Flip X", "Flip the mirror direction");
        private static readonly GUIContent offsetXGUI = new GUIContent("Offset X", "Define the X-axis position");
        
        private static readonly GUIContent mirrorYGUI = new GUIContent("Mirror Y", "Should it mirror the image on Y axis (bottom-top)?");
        private static readonly GUIContent flipYGUI = new GUIContent("Flip Y", "Flip the mirror direction");
        private static readonly GUIContent offsetYGUI = new GUIContent("Offset Y", "Define the Y-axis position");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TMirrorNode n = target as TMirrorNode;
            n.MirrorX = TParamGUI.Toggle(mirrorXGUI, n.MirrorX);
            if (n.MirrorX.value==true)
            {
                n.FlipX = TParamGUI.Toggle(flipXGUI, n.FlipX);
                n.OffsetX = TParamGUI.FloatSlider(offsetXGUI, n.OffsetX, 0f, 1f);
            }

            n.MirrorY = TParamGUI.Toggle(mirrorYGUI, n.MirrorY);
            if (n.MirrorY.value == true)
            {
                n.FlipY = TParamGUI.Toggle(flipYGUI, n.FlipY);
                n.OffsetY = TParamGUI.FloatSlider(offsetYGUI, n.OffsetY, 0f, 1f);
            }
        }
    }
}

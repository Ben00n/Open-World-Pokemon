using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.Poseidon
{
	public struct PSelectionGridArgs
	{
        public ICollection collection;
        public int selectedIndex;
        public Vector2 tileSize;

        public delegate void DrawHandler(Rect r, object o);
        public DrawHandler drawPreviewFunction;
        public DrawHandler drawLabelFunction;
        public DrawHandler customDrawFunction;

        public delegate object CategorizeHandler(object o);
        public CategorizeHandler categorizeFunction;

        public delegate string TooltipHandler(object o);
        public TooltipHandler tooltipFunction;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.TextureGraph
{
    public class TConst
    {
        public static readonly Vector2 NODE_PREVIEW_SIZE = new Vector2(98, 98);
        public static readonly Vector2 NODE_CREATION_POSITION_OFFSET = new Vector2(106, 131) * 0.5f;

        public static readonly string USS_STRETCH = "stretch";
        public static readonly string USS_LEFT_CONTAINER = "left-container";
        public static readonly string USS_RIGHT_CONTAINER = "right-container";
        public static readonly string USS_PREVIEW_CONTAINER = "preview-container";
        public static readonly string USS_COLUMN = "column";
        public static readonly string USS_ROW = "row";
        public static readonly string USS_SCROLL_VIEW_CONTENT = "scroll-view-content";

        public static readonly Vector2Int TEXTURE_SIZE_MIN = new Vector2Int(1, 1);
        public static readonly Vector2Int TEXTURE_SIZE_MAX = new Vector2Int(4096, 4096);
    }
}

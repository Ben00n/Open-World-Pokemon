using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TSplatterNode))]
    public class TSplatterNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent tileXGUI = new GUIContent("Tile X", "Number of tile on X axis");
        private static readonly GUIContent tileYGUI = new GUIContent("Tile Y", "Number of tile on Y axis");
        private static readonly GUIContent instancePerTileGUI = new GUIContent("Instance Per Tile", "Number of instance to draw at each tile");

        private static readonly GUIContent baseOffsetGUI = new GUIContent("Base Offset", "Offset all tiles");
        private static readonly GUIContent offsetMinXGUI = new GUIContent("Min X", "Minimum offset on X axis");
        private static readonly GUIContent offsetMaxXGUI = new GUIContent("Max X", "Maximum offset on X axis");
        private static readonly GUIContent offsetMinYGUI = new GUIContent("Min Y", "Minimum offset on Y axis");
        private static readonly GUIContent offsetMaxYGUI = new GUIContent("Max Y", "Maximum offset on Y axis");
        private static readonly GUIContent offsetMapMultiplierGUI = new GUIContent("Offset Multiplier", "This value will get multiplied by the offset map value to determine how far a tile will move");
        private static readonly GUIContent offsetRandomSeedGUI = new GUIContent("Random Seed", "Randomize the tiles position");

        private static readonly GUIContent baseRotationGUI = new GUIContent("Base Rotation", "Rotate all tiles");
        private static readonly GUIContent rotationMinGUI = new GUIContent("Min", "Minimum rotation");
        private static readonly GUIContent rotationMaxGUI = new GUIContent("Max", "Maximum rotation");
        private static readonly GUIContent rotationMapMultiplierGUI = new GUIContent("Rotation Multiplier", "This value will get multiplied by the rotation map value to determine how much a tile will rotate");
        private static readonly GUIContent rotationRandomSeedGUI = new GUIContent("Random Seed", "Randomize the tiles rotation");

        private static readonly GUIContent baseScaleGUI = new GUIContent("Base Scale", "Scale all tiles");
        private static readonly GUIContent scaleMinXGUI = new GUIContent("Min X", "Minimum scale on X axis");
        private static readonly GUIContent scaleMaxXGUI = new GUIContent("Max X", "Maximum scale on X axis");
        private static readonly GUIContent scaleMinYGUI = new GUIContent("Min Y", "Minimum scale on Y axis");
        private static readonly GUIContent scaleMaxYGUI = new GUIContent("Max Y", "Maximum scale on Y axis");
        private static readonly GUIContent scaleMapMultiplierGUI = new GUIContent("Scale Multiplier", "This value will get multiplied by the scale map value to determine how much a tile will scale");
        private static readonly GUIContent scaleRandomSeedGUI = new GUIContent("Random Seed", "Randomize the tiles scale");

        private static readonly GUIContent backgroundColorGUI = new GUIContent("Background Color", "Fill color for the background");
        private static readonly GUIContent hueGUI = new GUIContent("Hue Variation", "Add some variation to hue channel of each tile");
        private static readonly GUIContent saturationGUI = new GUIContent("Saturation Variation", "Add some variation to saturation channel of each tile");
        private static readonly GUIContent lightnessGUI = new GUIContent("Lightness Variation", "Add some variation to lightness channel of each tile");
        private static readonly GUIContent hslRandomSeedGUI = new GUIContent("Random Seed", "Randomize the HSL variation");

        private static readonly GUIContent maskThresholdGUI = new GUIContent("Threshold", "Threshold filter to apply to mask value");
        private static readonly GUIContent maskRandomSeedGUI = new GUIContent("Random Seed", "Randomize the masking");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TSplatterNode n = target as TSplatterNode;
            TEditorCommon.Header("Tiling");
            n.TileX = TParamGUI.IntField(tileXGUI, n.TileX);
            n.TileY = TParamGUI.IntField(tileYGUI, n.TileY);
            n.InstancePerTile = TParamGUI.IntField(instancePerTileGUI, n.InstancePerTile);

            TEditorCommon.Header("Offset");
            n.BaseOffset = TParamGUI.Vector2Slider(baseOffsetGUI, n.BaseOffset, -1, 1);
            n.OffsetMinX = TParamGUI.FloatField(offsetMinXGUI, n.OffsetMinX);
            n.OffsetMaxX = TParamGUI.FloatField(offsetMaxXGUI, n.OffsetMaxX);
            n.OffsetMinY = TParamGUI.FloatField(offsetMinYGUI, n.OffsetMinY);
            n.OffsetMaxY = TParamGUI.FloatField(offsetMaxYGUI, n.OffsetMaxY);
            n.OffsetMapMultiplier = TParamGUI.FloatSlider(offsetMapMultiplierGUI, n.OffsetMapMultiplier, 0f, 1f);
            n.OffsetRandomSeed = TParamGUI.IntField(offsetRandomSeedGUI, n.OffsetRandomSeed);

            TEditorCommon.Header("Rotation");
            n.BaseRotation = TParamGUI.FloatSlider(baseRotationGUI, n.BaseRotation, 0f, 360f);
            n.RotationMin = TParamGUI.FloatField(rotationMinGUI, n.RotationMin);
            n.RotationMax = TParamGUI.FloatField(rotationMaxGUI, n.RotationMax);
            n.RotationMapMultiplier = TParamGUI.FloatSlider(rotationMapMultiplierGUI, n.RotationMapMultiplier, 0f, 1f);
            n.RotationRandomSeed = TParamGUI.IntField(rotationRandomSeedGUI, n.RotationRandomSeed);

            TEditorCommon.Header("Scale");
            n.BaseScale = TParamGUI.Vector2Field(baseScaleGUI, n.BaseScale);
            n.ScaleMinX = TParamGUI.FloatField(scaleMinXGUI, n.ScaleMinX);
            n.ScaleMaxX = TParamGUI.FloatField(scaleMaxXGUI, n.ScaleMaxX);
            n.ScaleMinY = TParamGUI.FloatField(scaleMinYGUI, n.ScaleMinY);
            n.ScaleMaxY = TParamGUI.FloatField(scaleMaxYGUI, n.ScaleMaxY);
            n.ScaleMapMultiplier = TParamGUI.FloatSlider(scaleMapMultiplierGUI, n.ScaleMapMultiplier, 0f, 1f);
            n.ScaleRandomSeed = TParamGUI.IntField(scaleRandomSeedGUI, n.ScaleRandomSeed);

            TEditorCommon.Header("Color");
            n.BackgroundColor = TParamGUI.ColorField(backgroundColorGUI, n.BackgroundColor);
            n.HueVariation = TParamGUI.IntSlider(hueGUI, n.HueVariation, -180, 180);
            n.SaturationVariation = TParamGUI.IntSlider(saturationGUI, n.SaturationVariation, -100, 100);
            n.LightnessVariation = TParamGUI.IntSlider(lightnessGUI, n.LightnessVariation, -100, 100);
            n.HslRandomSeed = TParamGUI.IntField(hslRandomSeedGUI, n.HslRandomSeed);

            TEditorCommon.Header("Mask");
            n.MaskThreshold = TParamGUI.FloatSlider(maskThresholdGUI, n.MaskThreshold, 0f, 1f);
            n.MaskRandomSeed = TParamGUI.IntField(maskRandomSeedGUI, n.MaskRandomSeed);
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Ridged Noise",
        CreationMenu = "Noise/Ridged Noise",
        Icon = "TextureGraph/NodeIcons/RidgedNoise",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.bs8mirwtqhxa")]
    public class TRidgedNoiseNode : TPerlinNoiseNode
    {
        protected override int ShaderPass => 2;
    }
}

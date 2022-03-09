using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Billow Noise",
        CreationMenu = "Noise/Billow Noise",
        Icon = "TextureGraph/NodeIcons/BillowNoise",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.27mca8kofde")]
    public class TBillowNoiseNode : TPerlinNoiseNode
    {
        protected override int ShaderPass => 1;
    }
}

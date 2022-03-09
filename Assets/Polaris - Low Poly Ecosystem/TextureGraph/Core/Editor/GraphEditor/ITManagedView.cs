using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pinwheel.TextureGraph
{
    public interface ITManagedView : IDisposable
    {
        void Show();
        void Hide();
        void OnEnable();
        void OnDisable();
        void OnDestroy();
    }
}

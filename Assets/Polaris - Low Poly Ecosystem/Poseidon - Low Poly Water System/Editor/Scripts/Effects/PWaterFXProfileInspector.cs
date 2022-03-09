using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.Poseidon.FX
{
    [CustomEditor(typeof(PWaterFXProfile))]
    public class PWaterFXProfileInspector : Editor
    {
        private PWaterFXProfile instance;

        private void OnEnable()
        {
            instance = target as PWaterFXProfile;
        }

        public override void OnInspectorGUI()
        {
            PWaterFXProfileInspectorDrawer.Create(instance, null).DrawGUI();
        }
    }
}

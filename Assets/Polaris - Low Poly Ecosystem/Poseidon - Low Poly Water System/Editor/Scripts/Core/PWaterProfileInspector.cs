using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.Poseidon
{
    [CustomEditor(typeof(PWaterProfile))]
    public class PWaterProfileInspector : Editor
    {
        private PWaterProfile instance;
        private void OnEnable()
        {
            instance = target as PWaterProfile;
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Select the associated water in the scene to edit this profile.", PEditorCommon.WordWrapItalicLabel);
        }
    }
}

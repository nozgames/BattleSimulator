using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

using BattleSimulator;

namespace BattleSimulatorEditor
{
    [CustomEditor(typeof(UnitDef))]
    public class UnitDefEditor : Editor
    {
        private UnitDef _unit;

        private void OnEnable()
        {
            _unit = (UnitDef)target;
        }

        public override void OnInspectorGUI()
        {
            //EditorGUILayout.DropdownButton(new GUIContent("test"), FocusType.Keyboard);

            //EditorGUI.Popup ()

            //if(GUILayout.Button("Add Action"))
            {
                EditorGUILayout.Popup("test", 0, new[] { "test", "test2" });
            }
        }
    }
}
